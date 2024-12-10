using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using SpotifyTestApp.Models;
using System.Diagnostics;

using Microsoft.Extensions.Configuration;
using SpotifyAPI.Web;
using SpotifyAPI.Web.Auth;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.EntityFrameworkCore;
using SpotifyTestApp.Data;
using Newtonsoft.Json;

namespace SpotifyTestApp.Controllers
{
    [Route("")]
    public class SpotifyController : Controller
    {
        private readonly IConfiguration _config;
        private readonly string _redirectUri;
        private readonly AppDbContext _context;

        public SpotifyController(IConfiguration config, AppDbContext context)
        {
            _config = config;
            _redirectUri = _config["Spotify:RedirectUri"];
            _context = context;
        }
        // GET: api/Spotify/me
        [HttpGet("me")]
        public async Task<IActionResult> GetMe()
        {
            var userId = User.FindFirst("UserId")?.Value;

            if (string.IsNullOrEmpty(userId))
            {
                return Unauthorized(new { message = "User not found." });
            }

            // Fetch user data from the database
            var user = await _context.Users
                .Where(u => u.Id.ToString() == userId)
                .Select(u => new { u.Id, u.Username, u.Email })
                .FirstOrDefaultAsync();

            if (user == null)
            {
                return NotFound(new { message = "User not found." });
            }

            return Ok(user);
        }
        private SpotifyClient _spotify;

        // Endpoint: Home
        [HttpGet("")]
        public IActionResult Index()
        {
            return Content($"Welcome to my Spotify App. " +
                  $"<a href='{Url.Action("Login", "Spotify")}'>Login with Spotify</a> " +
                  $"<a href='{Url.Action("StartStudySession", "Spotify")}'>Start Study Session</a> " +
                  $"<a href='{Url.Action("CreatePlaylist", "Spotify")}'>Create Study Task Playlist</a>", "text/html");
        }

        [HttpGet("login")]
        //Comment: Might need to do "ActionResult" instead of "IActionResult"
        public IActionResult Login()
        {
            //var pkce = new AuthorizationCodePKCE(_config["Spotify:ClientId"], _redirectUri);
            var (verifier, challenge) = PKCEUtil.GenerateCodes();

            // Save the verifier in session for use in the callback
            HttpContext.Session.SetString("Verifier", verifier);

            var request = new LoginRequest(new Uri(_redirectUri), _config["Spotify:ClientId"], LoginRequest.ResponseType.Code)
            {
                CodeChallengeMethod = "S256",
                CodeChallenge = challenge,
                Scope = new[] { Scopes.UserReadCurrentlyPlaying, Scopes.UserReadEmail, Scopes.UserReadPrivate , Scopes.PlaylistModifyPrivate, Scopes.PlaylistModifyPublic}
            };

            return Redirect(request.ToUri().ToString());
        }

        [HttpGet("callback")]
        public async Task<IActionResult> Callback(string code)
        {
            if (string.IsNullOrEmpty(code))
                return BadRequest("Authorization code not provided.");

            var verifier = HttpContext.Session.GetString("Verifier");
            if (string.IsNullOrEmpty(verifier))
                return BadRequest("Code verifier not found in session.");

            var oauthClient = new OAuthClient();
            var tokenResponse = await oauthClient.RequestToken(new PKCETokenRequest(_config["Spotify:ClientId"], code, new Uri(_redirectUri), verifier));

            if (tokenResponse == null || string.IsNullOrEmpty(tokenResponse.AccessToken))
                return Unauthorized("Authorization failed.");

            // Store tokens in session
            HttpContext.Session.SetString("AccessToken", tokenResponse.AccessToken);
            HttpContext.Session.SetString("RefreshToken", tokenResponse.RefreshToken);
            HttpContext.Session.SetString("ExpiresAt", DateTime.Now.AddSeconds(tokenResponse.ExpiresIn).ToString());

            return RedirectToAction(nameof(Index));
        }

        // Endpoint: Refresh Token
        [HttpGet("refresh-token")]
        public async Task<IActionResult> RefreshToken()
        {
            if (!HttpContext.Session.TryGetValue("RefreshToken", out var refreshToken))
                return Unauthorized("No refresh token found.");

            try
            {
                var newResponse = await new OAuthClient().RequestToken(new PKCETokenRefreshRequest(_config["Spotify:ClientId"], HttpContext.Session.GetString("RefreshToken")));

                if (newResponse == null || string.IsNullOrEmpty(newResponse.AccessToken))
                    return Unauthorized("Failed to refresh token.");

                HttpContext.Session.SetString("AccessToken", newResponse.AccessToken);
                HttpContext.Session.SetString("ExpiresAt", DateTime.Now.AddSeconds(newResponse.ExpiresIn).ToString());

                return RedirectToAction(nameof(Index));
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error during token refresh: {ex.Message}");
                return StatusCode(500, "Internal server error during token refresh.");
            }
        }

        // Endpoint: Start Study Session
        [HttpGet("studysession")]
        public async Task<IActionResult> StartStudySession()
        {
            var sessionStartTime = DateTime.Now;
            var endStudySession = DateTime.Now.AddMinutes(1); // 1-minute session
            var studyTracks = new List<FullTrack>();

            while (DateTime.Now < endStudySession)
            {
                var trackInfo = await GetCurrentTrack();
                if (trackInfo != null && (studyTracks.Count == 0 || studyTracks[^1].Id != trackInfo.Id))
                {
                    studyTracks.Add(trackInfo);
                }
                await Task.Delay(TimeSpan.FromSeconds(5)); // 5 seconds delay + Safeguards against thread-blocking
            }

            // Calculate audio features, genre, and productivity score
            var genre = await DetermineMostPlayedGenre(studyTracks);
            

            

            // Save session to the database
            var studySession = new StudySession
            {
                UserId = HttpContext.Session.GetString("UserId"), // Retrieve user ID from session
                StudyDate = sessionStartTime,
                //SongAudioFeaturesJson = audioFeaturesJson,   //Store as Json string
                MusicHistory = studyTracks.Select(t => t.Name).ToList(),
                Genre = genre
            };

            //_context.StudySessions.Add(studySession);
            //await _context.SaveChangesAsync();

            return RedirectToAction(nameof(Index));
        }

        // Helper: Get Current Track
        private async Task<FullTrack> GetCurrentTrack()
        {
            var accessToken = HttpContext.Session.GetString("AccessToken");
            if (string.IsNullOrEmpty(accessToken))
            {
                Console.WriteLine("Access token is missing or invalid");
                return null;
            }

            try
            {
                _spotify = new SpotifyClient(accessToken);

                var currentlyPlaying = await _spotify.Player.GetCurrentlyPlaying(new PlayerCurrentlyPlayingRequest());

                //Check if no song is playing --> return null
                if (currentlyPlaying?.Item == null || !currentlyPlaying.IsPlaying)
                {
                    Console.WriteLine("No track is currently playing.");
                    return null;
                }

                var track = currentlyPlaying.Item as FullTrack;
                if (track == null)
                {
                    Console.WriteLine("The currently playing item is not a track.");
                    return null;
                }

                //Check if the user listen to 50% of the song
                if (currentlyPlaying.ProgressMs.HasValue && track.DurationMs > 0)
                {
                    double progressPercentage = (double)currentlyPlaying.ProgressMs.Value / track.DurationMs * 100;
                    if (progressPercentage > 50)
                    {
                        Console.WriteLine($"Currently Playing: {track.Name} by {string.Join(", ", track.Artists.Select(a => a.Name))} (>50% of the song was played)");
                        return track;
                    }
                    else
                    {
                        Console.WriteLine($"Skipping '{track.Name}' - Only {progressPercentage:0.##}% played.");
                        return null;
                    }
                }

                // If no progress data is available
                Console.WriteLine("Track progress data is unavailable.");
                return null;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error retrieving the currently playing track: {ex.Message}");
                return null;
            }
        }
        public async Task<string> DetermineMostPlayedGenre(List<FullTrack> tracks)
        {
            var genreCounts = new Dictionary<string, int>();

            // Iterate through each track and gather the genres of the artists
            foreach (var track in tracks)
            {
                try
                {
                    // Iterate through each artist for the track and get their genres
                    foreach (var artist in track.Artists)
                    {
                        // Fetch the artist's information (including genres)
                        var artistDetails = await _spotify.Artists.Get(artist.Id);

                        // Add each genre to the dictionary with a count
                        foreach (var genre in artistDetails.Genres)
                        {
                            if (genreCounts.ContainsKey(genre))
                            {
                                genreCounts[genre]++;
                            }
                            else
                            {
                                genreCounts.Add(genre, 1);
                            }
                        }
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Error retrieving genres for track {track.Name}: {ex.Message}");
                    continue;
                }
            }

            // If no genres were found, return null or a default genre
            if (genreCounts.Count == 0)
            {
                return "No genres found";
            }

            // Find the genre with the highest count
            var mostPlayedGenre = genreCounts.OrderByDescending(g => g.Value).FirstOrDefault();

            return mostPlayedGenre.Key; // Return the genre with the highest play count
        }

        /*
        //Modify: Change it to get from survey
        private int GetProductivityScore()
        {
            // Example: Calculate productivity score based on some metrics (e.g., time spent studying, focus, etc.)
            return new Random().Next(1, 11); // Random for demonstration; replace with actual logic
        }
        */
    }
}