using System;
using System.Net.Http;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Text.Json;
using System.IO;

namespace PixelaScript
{
    internal class Program
    {
        // Creates an HTTPClient to be used for submitting HTTP requests
        private static readonly HttpClient client = new HttpClient();

        // Creates a new Pixela user given a username and token
        public static async Task CreateUserAccount(string username, string token)
        {
            // API Endpoint
            string pixelaHTTPEndpoint = "https://pixe.la/v1/users";

            // parameters that are going to be passed to the API
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
            HttpResponseMessage response = await client.PostAsync(pixelaHTTPEndpoint, content);

            // prints out "{"message":"Success. Let's visit https://pixe.la/@{username} , it is your profile page!","isSuccess":true}" if a user was successfully created
            // else it prints out a HTTP response status ode letting you know what error has occurred
            Console.WriteLine(await response.Content.ReadAsStringAsync());
        }

        // Creates a new graph -- with user specified id, name, unit, and color -- in the specified user's account using their token
        public static async Task CreateGraph(string username, string token, string uID, string uName, string uUnit, string uColor)
        {
            // API Endpoint
            string graphHTTPEndpoint = $"https://pixe.la/v1/users/{username}/graphs";

            // parameters that are going to be passed to the API
            var graphParameters = new
            {
                id = uID,
                name = uName,
                unit = uUnit,
                type = "int",
                color = uColor
            };

            // adds a request header to the client with the key "X-USER-TOKEN" and the token
            client.DefaultRequestHeaders.Add("X-USER-TOKEN", token);

            // serializes the userParameters object into Json
            string graphParametersJson = JsonSerializer.Serialize(graphParameters);

            // creates an HTTP content stream for reading
            var content = new StringContent(graphParametersJson, Encoding.UTF8, "application/json");

            // post request to the Pixela API to create a new graph in the specified username's account with the specified id, name, unit, and color
            HttpResponseMessage graphResponse = await client.PostAsync(graphHTTPEndpoint, content);

            // prints out "{"message":"Success.","isSuccess":true}" if a user was successfully created
            // else it prints out a HTTP response status ode letting you know what error has occurred
            Console.WriteLine(await graphResponse.Content.ReadAsStringAsync());
        }

        // Creates a pixel with a specified value (using uQuantity) in the specified graph (using graphID) in the specified user's account
        public static async Task CreatePixel(string username, string token, string graphID, string uQuantity)
        {
            // API Endpoint
            string pixelHTTPEndpoint = $"https://pixe.la/v1/users/{username}/graphs/{graphID}";

            // retrieves the current date in year month day format to be given as a parameter to the API
            DateTime currentDate = DateTime.Now;
            string todaysDate = currentDate.ToString("yyyyMMdd");

            // parameters that are going to be passed to the API
            var pixelParameters = new
            {
                date = todaysDate,
                quantity = uQuantity
            };

            // adds a request header to the client with the key "X-USER-TOKEN" and the token
            client.DefaultRequestHeaders.Add("X-USER-TOKEN", token);

            // serializes the userParameters object into Json
            string pixelParametersJson = JsonSerializer.Serialize(pixelParameters);

            // creates an HTTP content stream for reading
            var content = new StringContent(pixelParametersJson, Encoding.UTF8, "application/json");

            // post request to the Pixela API to create a new pixel in the specified graph
            HttpResponseMessage pixelResponse = await client.PostAsync(pixelHTTPEndpoint, content);

            // prints out "{"message":"Success.","isSuccess":true}" if a user was successfully created
            // else it prints out a HTTP response status ode letting you know what error has occurred
            Console.WriteLine(await pixelResponse.Content.ReadAsStringAsync());
        }

        // Gets the graph in SVG format from the API and saves it as an SVG file on your local machine in the Documents folder
        public static async Task GetGraphSVG(string username, string graphID, string date)
        {
            // API Endpoint
            string graphSVGHTTPEndpoint = $"https://pixe.la/v1/users/{username}/graphs/{graphID}?date={date}";

            // gets us the information for the SVG file
            HttpResponseMessage graphSVGResponse = await client.GetAsync(graphSVGHTTPEndpoint);

            // set a variable to the Documents path
            string docPath = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments);

            // write the specified text asynchronously to a new svg file named "testing{graphID}.svg"
            // when used in our project we wouldn't have this test we'd just imbed the svg file in the HTML as an image
            using (StreamWriter outputFile = new StreamWriter(Path.Combine(docPath, $"testing{graphID}.svg")))
            {
                await outputFile.WriteAsync(await graphSVGResponse.Content.ReadAsStringAsync());
            }

        }

        static async Task Main()
        {
            //await CreateUserAccount("test49", "piexelatest");
            //await CreateGraph("test49", "piexelatest", "graph3", "day1", "hours", "sora");
            //await CreatePixel("test49", "piexelatest", "graph3", "5");
            await GetGraphSVG("test49", "graph3", "20241029");
            Console.ReadLine();

        }

    }

}
