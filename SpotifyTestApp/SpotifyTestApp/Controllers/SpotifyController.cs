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

/*
using CsvHelper;
using CsvHelper.Configuration;
using System.Globalization;
*/

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
                //Not fit for SpotifyAPI.NET: Scope = new[] { "user-read-private", "user-read-email", "user-read-currently-playing" }
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
            var averageAudioFeatures = await CalculateAverageAudioFeatures(studyTracks);
            var genre = await DetermineMostPlayedGenre(studyTracks);
            var productivity = GetProductivityScore();

            // Serialize the audio features dictionary to a JSON string
            var audioFeaturesJson = JsonConvert.SerializeObject(averageAudioFeatures);

            // Save session to the database
            var studySession = new StudySession
            {
                UserId = HttpContext.Session.GetString("UserId"), // Retrieve user ID from session
                StudyDate = DateTime.Now,
                SongAudioFeaturesJson = audioFeaturesJson,   //Store as Json string
                MusicHistory = studyTracks.Select(t => t.Name).ToList(),
                Productivity = productivity,
                Genre = genre
            };

            _context.StudySessions.Add(studySession);
            await _context.SaveChangesAsync();

            // Process tracks and update study session stats
            //ProcessTracks(studyTracks);

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

        // Helper Methods
        private async Task<Dictionary<string, double>> CalculateAverageAudioFeatures(List<FullTrack> tracks)
        {
            var audioFeatures = new Dictionary<string, double>
            {
                { "Tempo", 0 },
                { "Instrumentalness", 0 },
                { "Energy", 0 },
                { "Acousticness", 0 },
                { "Danceability", 0 },
                { "Loudness", 0 },
                { "Speechiness", 0 },
                { "Valence", 0 }
            };

            foreach (var track in tracks)
            {
                try
                {
                    var features = await _spotify.Tracks.GetAudioFeatures(track.Id);
                    audioFeatures["Tempo"] += features.Tempo;
                    audioFeatures["Instrumentalness"] += features.Instrumentalness;
                    audioFeatures["Energy"] += features.Energy;
                    audioFeatures["Acousticness"] += features.Acousticness;
                    audioFeatures["Danceability"] += features.Danceability;
                    audioFeatures["Loudness"] += features.Loudness;
                    audioFeatures["Speechiness"] += features.Speechiness;
                    audioFeatures["Valence"] += features.Valence;
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Error fetching audio features for track {track.Name}: {ex.Message}");
                    continue; // Skip this track if there was an error
                }
            }

            // Calculate averages
            if (tracks.Count > 0)
            {
                audioFeatures["Tempo"] /= tracks.Count;
                audioFeatures["Energy"] /= tracks.Count;
                audioFeatures["Acousticness"] /= tracks.Count;
                audioFeatures["Instrumentalness"] /= tracks.Count;
                audioFeatures["Danceability"] /= tracks.Count;
                audioFeatures["Loudness"] /= tracks.Count;
                audioFeatures["Speechiness"] /= tracks.Count;
                audioFeatures["Valence"] /= tracks.Count;
            }

            return audioFeatures;
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

        //Modify: Change it to get from survey
        private int GetProductivityScore()
        {
            // Example: Calculate productivity score based on some metrics (e.g., time spent studying, focus, etc.)
            return new Random().Next(1, 11); // Random for demonstration; replace with actual logic
        }


        /*
        // Helper Method: Process Tracks (Simulated) --> Using csv files
        private void ProcessTracks(List<FullTrack> tracks)
        {
            if(tracks == null || tracks.Count == 0)
            {
                Console.WriteLine("No tracks to process.");
                return;
            }

            //Counter for total number of study sessions
            var sessionId = HttpContext.Session.GetInt32("TotalStudySessions") ?? 0;
            sessionId++;
            HttpContext.Session.SetInt32("TotalStudySessions", sessionId);

            //Continue here/Modification --> Figure out how to save in db
            var csvFilePath = Path.Combine(Directory.GetCurrentDirectory(), $"studysession{sessionId}.csv");

            // Save track details to a CSV file
            SaveTracksToCsv(tracks, csvFilePath);

            // Update session-level averages
            UpdateSessionMetrics(csvFilePath);

            Console.WriteLine($"Study session {sessionId} processed and saved to {csvFilePath}");
        }
        */

        /*
        //Helper Method: Save the list of tracks to a CSV file
        private void SaveTracksToCsv(List<FullTrack> tracks, string filePath)
        {
            using (var writer = new StreamWriter(filePath))
            using (var csv = new CsvWriter(writer, new CsvConfiguration(CultureInfo.InvariantCulture)))
            {
                csv.WriteRecords(tracks.Select(t => new
                {
                    Name = t.Name,
                    Artists = string.Join(", ", t.Artists.Select(a => a.Name)),
                    DurationMs = t.DurationMs,
                    Album = t.Album.Name
                }));
            }
        }
        //Helper Method: Update study music data
        private void UpdateSessionMetrics(string csvFilePath)
        {
            var totalSongs = HttpContext.Session.GetInt32("TotalSongs") ?? 0;
            var totalMetrics = new
            {
                Acousticness = HttpContext.Session.GetDouble("AvgAcousticness") * totalSongs,
                DurationMs = HttpContext.Session.GetDouble("AvgDurationMs") * totalSongs,
                Energy = HttpContext.Session.GetDouble("AvgEnergy") * totalSongs,
                Instrumentalness = HttpContext.Session.GetDouble("AvgInstrumentalness") * totalSongs,
                Liveness = HttpContext.Session.GetDouble("AvgLiveness") * totalSongs,
                Loudness = HttpContext.Session.GetDouble("AvgLoudness") * totalSongs,
                Speechiness = HttpContext.Session.GetDouble("AvgSpeechiness") * totalSongs,
                Tempo = HttpContext.Session.GetDouble("AvgTempo") * totalSongs,
                Valence = HttpContext.Session.GetDouble("AvgValence") * totalSongs
            };

            var audioFeatures = GetAudioFeatures(csvFilePath);

            foreach (var track in audioFeatures)
            {
                totalMetrics = new
                {
                    Acousticness = totalMetrics.Acousticness + track.Acousticness,
                    DurationMs = totalMetrics.DurationMs + track.DurationMs,
                    Energy = totalMetrics.Energy + track.Energy,
                    Instrumentalness = totalMetrics.Instrumentalness + track.Instrumentalness,
                    Liveness = totalMetrics.Liveness + track.Liveness,
                    Loudness = totalMetrics.Loudness + track.Loudness,
                    Speechiness = totalMetrics.Speechiness + track.Speechiness,
                    Tempo = totalMetrics.Tempo + track.Tempo,
                    Valence = totalMetrics.Valence + track.Valence
                };
                totalSongs++;
            }

            // Update session averages
            HttpContext.Session.SetDouble("AvgAcousticness", totalMetrics.Acousticness / totalSongs);
            HttpContext.Session.SetDouble("AvgDurationMs", totalMetrics.DurationMs / totalSongs);
            HttpContext.Session.SetDouble("AvgEnergy", totalMetrics.Energy / totalSongs);
            HttpContext.Session.SetDouble("AvgInstrumentalness", totalMetrics.Instrumentalness / totalSongs);
            HttpContext.Session.SetDouble("AvgLiveness", totalMetrics.Liveness / totalSongs);
            HttpContext.Session.SetDouble("AvgLoudness", totalMetrics.Loudness / totalSongs);
            HttpContext.Session.SetDouble("AvgSpeechiness", totalMetrics.Speechiness / totalSongs);
            HttpContext.Session.SetDouble("AvgTempo", totalMetrics.Tempo / totalSongs);
            HttpContext.Session.SetDouble("AvgValence", totalMetrics.Valence / totalSongs);

            HttpContext.Session.SetInt32("TotalSongs", totalSongs);
        }

        //Adding Extension Methods to HttpContext.Session to support double
        public static class SessionExtensions
        {
            public static void SetDouble(this ISession session, string key, double value)
            {
                session.SetString(key, value.ToString(CultureInfo.InvariantCulture));
            }

            public static double GetDouble(this ISession session, string key)
            {
                var value = session.GetString(key);
                return value != null ? double.Parse(value, CultureInfo.InvariantCulture) : 0.0;
            }
        }
        */

        // Endpoint: Create StudyTask-specific playlist in Spotify
        [HttpGet("create-studyplaylist")]
        public async Task<IActionResult> CreatePlaylist(string studyTaskName)
        {
            try
            {
                // Ensure the SpotifyClient is initialized with a valid access token
                var accessToken = HttpContext.Session.GetString("AccessToken");
                if (string.IsNullOrEmpty(accessToken))
                {
                    Console.WriteLine("Access token is missing or invalid");
                    return RedirectToAction(nameof(Index));
                }

                var spotify = new SpotifyClient(accessToken);

                var userProfile = await spotify.UserProfile.Current();
                var userId = userProfile.Id;


                // Prepare the playlist creation request
                var playlistRequest = new PlaylistCreateRequest(studyTaskName)
                {
                    Description = $"Playlist created for study task: {studyTaskName}",
                    Public = false // Change to true if you want a public playlist
                };

                // Create the playlist
                var newPlaylist = await spotify.Playlists.Create(userId, playlistRequest);

                Console.WriteLine($"Playlist '{studyTaskName}' created successfully with ID: {newPlaylist.Id}");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error creating playlist: {ex.Message}");
            }

            //Continue here: Add Recommended Songs into new playlist

            return RedirectToAction(nameof(Index));
        }

        /* Modification: Change to helper method
        // Endpoint: Generate Recommendations
        [HttpGet("generate-recommendations")]
        public async Task<IActionResult> GenerateRecommendations()
        {
            //Update depend on user's inputs
            var seedGenres = new[] { "anime", "groove", "guitar" };
            var accessToken = HttpContext.Session.GetString("AccessToken");

            _spotify = new SpotifyClient(accessToken);

            var recommendations = await _spotify.Browse.GetRecommendations(new RecommendationsRequest
            {
                SeedGenres = seedGenres,
                TargetEnergy = 0.7f,
                TargetTempo = 120
            });

            return Ok(recommendations?.Tracks);
        }
        */
    }
}
