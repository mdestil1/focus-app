#Study Web App Demo

## Frontend:

React/Material UI (MUI)  

## Backend:

  .NET Core (C#)/ Stripe.NET (Stripe API integration)

Running Instructions:

CD frontend

npm install

npm start

New Terminal

CD Backend

dotnet run


## Spotify API: 

This Python code allows users to interact with Spotify API through Spotify account login and granting permission to access users' private data. After successful login, the Spotify server will give the program {access_token, refresh_token, and expires_in}, which allows the program to access users' data for a specific amount of time. The program will compile the user's current playing tracks into a Python dictionary and return the value.



## PixelaScript (Code to connect with Pixela's API):

This is C# code that allows you to connect to Pixela's API and create a user (with a token), a graph under that user's account, and pixels to be added to a graph found on the user's account. There are 5 methods: CreateUserAccount, CreateGraph, CreatePixel, GetGraphSVG, and Main. 

  **CreateUserAccount** takes in a username (a string) and a token (also a string) and creates a user account on Pixela with that username and token. The profile for that specific user can be viewed at https://pixe.la/@{username you chose}. 

  **CreateGraph** takes in the username and token that was passed to CreateUserAccount and also takes in a uID, uName, uUnit, and uColor (all of which are strings) and these are the parameters for the graph that's being created. uID is the graph ID, uName is the name of the graph, uUnit is the units the graph uses, and uColor is the color of the graph's pixels. 

  **CreatePixel** takes in the same username and token, in addition to the graphID and uQuantity (a string) which is the quantity the pixel is going to hold/record. 

  **GetGraphSVG** takes the username and graphID, in addition to date (a string) which specifies what date (year-month-day format) you want the graph to start from. For example, if you inputted 20241029 the graph would start from the 29th and end at the current date. It then takes the information you've entered and retrieves the graph you specified (using graphID), inputs the data from that into a new graph that starts from the date you specified, and then saves the graph information as an SVG file in the Documents folder on your local machine. To open the SVG file you navigate to the Documents folder on your laptop/computer and double click on it. It'll then open up the image of the graph in a web browser. 

  In the Main method I have a line of code to test each one of the four other methods. The lines should be run one at a time and in order because some tasks will be completed before the other is finished and that'll cause an error. 

When testing, you need to make a username that isn't already taken, and if that isn't done first the others will report a failure to complete the task. Also the token you chose needs to be at least 8 characters long. 
  
To run the program uncomment the line of code in Main that you're testing and click the Start button at the top. 

*Reference Materials:* https://stackoverflow.com/questions/75516705/pixela-api-fails-with-username-doesnt-exist-or-token-is-wrong (to figure out how to format the HTTP endpoint to connect to the Pixela API), Microsoft C# Reference Docs
