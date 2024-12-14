using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using StudyApp.Server.Models;
using System.Diagnostics;

using Microsoft.Extensions.Configuration;
using SpotifyAPI.Web;
using SpotifyAPI.Web.Auth;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.SqlServer;
//using Microsoft.EntityFrameworkCore.Tools;
using StudyApp.Server.Data;
using Newtonsoft.Json;
using Microsoft.AspNetCore.Cors;
using Microsoft.Identity.Client;
using System.Runtime.CompilerServices;
using System.ComponentModel;

namespace StudyApp.Server.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [EnableCors("AllowLocalhost")]
    
    public class SpotifyController : Controller
    {
        private readonly IConfiguration _config;
        private readonly string _redirectUri;
        private readonly AppDBContext _context;
        private int productivity = 0;
        private List<TaskModel> tasks;
        private string userID;
        private string token;


        private readonly PixelaService _pixelaService;

        // POST api/pixelaSignUpForm
        [HttpPost("pixelaSignUpForm")]
        public async Task<IActionResult> SignUp([FromBody] PixelaSignUpModel model)
        {
            if (ModelState.IsValid)
            {
                userID = model.Username;
                token = model.Token;
                // Call the service method to create the user in Pixela
                var response = await _pixelaService.CreateUserAccount(model.Username, model.Token);
                // Process the form data
                return Ok(new { Message = "Sign-up successful", Data = model });
            }

            return BadRequest(ModelState);
        }

        public SpotifyController(IConfiguration config, AppDBContext context, PixelaService pixelaService)
        {
            _config = config;
            _redirectUri = _config["Spotify:RedirectUri"];
            _context = context;
            _pixelaService = pixelaService;
        }

        private SpotifyClient _spotify;

        /** Endpoint: Home
        [HttpGet("")]
        public IActionResult Index()
        {
            return Content($"Welcome to my Spotify App. " +
                  $"<a href='{Url.Action("Login", "Spotify")}'>Login with Spotify</a> " +
                  $"<a href='{Url.Action("StartStudySession", "Spotify")}'>Start Study Session</a> " +
                  $"<a href='{Url.Action("CreatePlaylist", "Spotify")}'>Create Study Task Playlist</a>", "text/html");
        }**/

        [HttpGet("login")]
        //Comment: Might need to do "ActionResult" instead of "IActionResult"
        public async Task<IActionResult> Login()
        {
            //var pkce = new AuthorizationCodePKCE(_config["Spotify:ClientId"], _redirectUri);
            var (verifier, challenge) = PKCEUtil.GenerateCodes();

            // Save the verifier in session for use in the callback
            HttpContext.Session.SetString("Verifier", verifier);

            var request = new LoginRequest(new Uri(_redirectUri), _config["Spotify:ClientId"], LoginRequest.ResponseType.Code)
            {
                CodeChallengeMethod = "S256",
                CodeChallenge = challenge,
                Scope = new[] { Scopes.UserReadCurrentlyPlaying, Scopes.UserReadEmail, Scopes.UserReadPrivate, Scopes.PlaylistModifyPrivate, Scopes.PlaylistModifyPublic }
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

            /*
            // Store tokens in session
            HttpContext.Session.SetString("AccessToken", tokenResponse.AccessToken);
            HttpContext.Session.SetString("RefreshToken", tokenResponse.RefreshToken);
            HttpContext.Session.SetString("ExpiresAt", DateTime.Now.AddSeconds(tokenResponse.ExpiresIn).ToString());
            */

            // Save tokens to a file
            var tokenData = new
            {
                AccessToken = tokenResponse.AccessToken,
                RefreshToken = tokenResponse.RefreshToken,
                ExpiresAt = DateTime.Now.AddSeconds(tokenResponse.ExpiresIn)
            };

            var filePath = Path.Combine(Directory.GetCurrentDirectory(), "tokens.json");
            await System.IO.File.WriteAllTextAsync(filePath, System.Text.Json.JsonSerializer.Serialize(tokenData));

            return Redirect("https://localhost:5173/index.html");
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

                return Redirect("https://localhost:5173/index.html");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error during token refresh: {ex.Message}");
                return StatusCode(500, "Internal server error during token refresh.");
            }
        }

        [HttpGet("start")]
        public async Task<IActionResult> StartSession()
        {
            var sessionStartMessage = "Study session started successfully.";
            // Perform any logic needed on the backend
            await StartStudySession();
            //return Ok(new { message = sessionStartMessage });
            //return Ok();
            // Simulate session start success (this could be a redirect, database update, etc.)
            //var sessionStartMessage = "Study session started successfully.";

            // Return a successful response
            return Ok(new { message = sessionStartMessage });
        }

        private static CancellationTokenSource _cancellationTokenSource = new CancellationTokenSource();


        [HttpPost("finish")]
        public async Task<IActionResult> FinishStudySession([FromBody] StopwatchModel request)
        {
            // Log the received data
            Console.WriteLine("Received data from frontend:");
            Console.WriteLine($"Hour: {request.Hour}, Minute: {request.Minute}, Second: {request.Second}, Count: {request.Count}");

            if (request.Tasks != null && request.Tasks.Any())
            {
                tasks = request.Tasks;
                Console.WriteLine("Tasks received:");
                foreach (var task in request.Tasks)
                {
                    Console.WriteLine($"Task Description: {task.Description}, Completed: {task.IsCompleted}");
                }
            }
            else
            {
                Console.WriteLine("No tasks received.");
            }

            
            // Process the data as required
            // ...

            return Ok(new { message = "Study Session finished!" });
        }



        // POST api/pastStudySessionsForm
        [HttpPost("surveySubmitted")]
        public async Task<IActionResult> GetProductivity([FromBody] SurveyModel model)
        {
            productivity = model.Productivity;
            return Ok(new { message = model.Productivity });
        }

        // Endpoint: Start Study Session
        [HttpGet("studysession")]
        public async Task<IActionResult> StartStudySession()
        {
            var sessionStartTime = DateTime.Now;
            DateTime sessionDateOnly = sessionStartTime.Date; // Removes the time part
            //var endStudySession = DateTime.Now;
            var studyTracks = new List<FullTrack>();

            while (!_cancellationTokenSource.Token.IsCancellationRequested)
            {
                var trackInfo = await GetCurrentTrack();
                if (trackInfo != null && (studyTracks.Count == 0 || studyTracks[^1].Id != trackInfo.Id))
                {
                    studyTracks.Add(trackInfo);
                }

                await Task.Delay(TimeSpan.FromSeconds(5), _cancellationTokenSource.Token);
            }

            // Calculate audio features, genre, and productivity score
            //var averageAudioFeatures = await CalculateAverageAudioFeatures(studyTracks);
            var genre = await DetermineMostPlayedGenre(studyTracks);
            //var productivity = GetProductivityScore();

            //Update MostProductiveGenreOverall --> fix later: need productivity argument
            //MostProductiveGenreOverall(genre, SurveyModel.) -> Finish later
            //Productivity);

            // Serialize the Tracks to a JSON string

 

            // Save session to the database
            var studySession = new StudySession
            {
                UserId = userID,
                Token = token,
                StudyDate = sessionDateOnly.ToString("yyyy-MM-dd"),
                Duration = DateTime.Now - sessionStartTime,
                TasksJson = JsonConvert.SerializeObject(tasks, Formatting.Indented),
                MusicHistoryJson = JsonConvert.SerializeObject(studyTracks.Select(t => t.Name).ToList()),
                Productivity = productivity,
                Genre = genre
            };

            _context.StudySessions.Add(studySession);
            await _context.SaveChangesAsync();

            return Redirect("https://localhost:5173/studySession.html"); 
        }

        // Helper: Get Current Track
        private async Task<FullTrack> GetCurrentTrack()
        {
            //var accessToken = HttpContext.Session.GetString("AccessToken");
            var (accessToken, refreshToken, expiresAt) = await GetTokenDataAsync();

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

        //Helper Method: Update MostProductiveGenreOverall through all study sessions
        public IActionResult MostProductiveGenreOverall(string genre, int productivity)
        {
            // Retrieve the dictionary from the session
            var serializedDict = HttpContext.Session.GetString("ProductiveGenres");

            Dictionary<string, int> genreProductivity;

            // Check if the session already contains the dictionary
            if (!string.IsNullOrEmpty(serializedDict))
            {
                // Deserialize the existing dictionary
                genreProductivity = JsonConvert.DeserializeObject<Dictionary<string, int>>(serializedDict);
            }
            else
            {
                // Initialize a new dictionary if not found in session
                genreProductivity = new Dictionary<string, int>();
            }

            // Update or add the productivity value for the genre
            if (genreProductivity.ContainsKey(genre))
            {
                genreProductivity[genre] += productivity;
            }
            else
            {
                genreProductivity[genre] = productivity;
            }

            // Serialize the updated dictionary and store it back in the session
            HttpContext.Session.SetString("ProductiveGenres", JsonConvert.SerializeObject(genreProductivity));

            return Ok($"Updated productivity for genre '{genre}' to {genreProductivity[genre]}.");
        }

        public IActionResult GetMostProductiveGenreOverall()
        {
            // Retrieve the dictionary from the session
            var serializedDict = HttpContext.Session.GetString("ProductiveGenres");

            if (!string.IsNullOrEmpty(serializedDict))
            {
                // Deserialize the dictionary
                var genreProductivity = JsonConvert.DeserializeObject<Dictionary<string, int>>(serializedDict);

                // Find the genre with the highest value
                var mostProductiveGenre = genreProductivity
                    .OrderByDescending(kvp => kvp.Value)
                    .FirstOrDefault();

                if (!string.IsNullOrEmpty(mostProductiveGenre.Key))
                {
                    return Ok(new { Genre = mostProductiveGenre.Key, Productivity = mostProductiveGenre.Value });
                }
            }

            return NotFound("No data found for productive genres.");
        }

        //Accessing Tokens
        private async Task<(string AccessToken, string RefreshToken, DateTime ExpiresAt)> GetTokenDataAsync()
        {
            var filePath = Path.Combine(Directory.GetCurrentDirectory(), "tokens.json");
            if (!System.IO.File.Exists(filePath))
                throw new FileNotFoundException("Token file not found.");

            var jsonData = await System.IO.File.ReadAllTextAsync(filePath);
            var tokenData = System.Text.Json.JsonSerializer.Deserialize<TokenData>(jsonData);

            return (tokenData.AccessToken, tokenData.RefreshToken, tokenData.ExpiresAt);
        }

    }

    // Model for receiving the stopwatch time data
    public class StopwatchModel
    {
        public int Hour { get; set; }
        public int Minute { get; set; }
        public int Second { get; set; }
        public int Count { get; set; }
        public List<TaskModel> Tasks { get; set; } // New property for tasks
    }

    public class TaskModel
    {
        public string Description { get; set; }
        public bool IsCompleted { get; set; }
    }

    public class SurveyModel
    {
        public int Productivity { get; set; }
    }

    /**
    public class FinishSessionRequest
    {
        public StopwatchModel TimeData { get; set; }
        public List<TaskModel> Tasks { get; set; }
    }

    public class TaskModel
    {
        public string Description { get; set; }
        public bool IsCompleted { get; set; }
    }**/

    // TokenData class for deserialization
    public class TokenData
    {
        public string AccessToken { get; set; }
        public string RefreshToken { get; set; }
        public DateTime ExpiresAt { get; set; }
    }

}   