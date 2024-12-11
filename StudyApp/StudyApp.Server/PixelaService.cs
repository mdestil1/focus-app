using System;
using System.Net.Http;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Text.Json;
using System.IO;



namespace StudyApp.Server
{
    public class PixelaService
    {

        private readonly HttpClient _httpClient;

        public PixelaService(HttpClient httpClient)
        {
            _httpClient = httpClient;
            _httpClient.BaseAddress = new Uri("https://pixe.la");
        }

        public async Task<string> CreateUserAccount(string username, string token)
        {
            // API Endpoint
            string pixelaSignUpEndpoint = "/v1/users";

            var userParameters = new
            {
                token = token,
                username = username,
                agreeTermsOfService = "yes",
                notMinor = "yes"
            };

            // serializes the userParameters object into Json
            string userParamsJson = JsonSerializer.Serialize(userParameters);

            // creates an HTTP content stream for reading 
            var content = new StringContent(userParamsJson, Encoding.UTF8, "application/json");

            // post request to the Pixela API to create a new user with the specified username and token
            HttpResponseMessage response = await _httpClient.PostAsync(pixelaSignUpEndpoint, content);

            // prints out "{"message":"Success. Let's visit https://pixe.la/@{username} , it is your profile page!","isSuccess":true}" if a user was successfully created
            // else it prints out a HTTP response status ode letting you know what error has occurred
            //Console.WriteLine(await response.Content.ReadAsStringAsync());

            //response.EnsureSuccessStatusCode();

            return await response.Content.ReadAsStringAsync();
        }
    }
}
