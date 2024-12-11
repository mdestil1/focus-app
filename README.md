# Study Web App Demo
### The Goal: 
A focus/study app that tracks your productivity and the music associated with your different levels of productivity. The app will show you your productivity over time, in addition to showing you how productive you are while listening to different genres of music, tracking your progress and rewarding you as you become more and more productive. The reward system will be something similar to a gatcha game, where the goal is to collect all of the characters, but ours will be related to music. Whatever music genre you listen to while studying will be what theme the character/reward you recieve will be based on, however if it's not the music genre that you're most productive during the reward won't be as good. For example, if you're most productive when listening to classical music however you choose to listen to pop music the reward for studying will more often be something smaller or of less value. However if you listen to classical music while studying the chance of recieving a bigger reward is higher. 

# How To Use Our App:

Once you’ve downloaded the files and opened the StudyApp.sln file in Visual Studios please press the green “Start” button.

That takes you to our Dashboard where the journey to productivity can begin!

Steps to a Study Session:
1. On the Dashboard please click the “Sign into your Spotify Account” button
     That will redirect you to the Spotify login page 
     If you don’t want to log in with your own Spotify account you can use this one:
       sydm42162@gmail.com
	     testingcs392
2. Once logged in, please click “Accept” the terms and you’ll be redirected to our Dashboard
3. On the Dashboard please click the “Sign up for a Pixela Account” button
      You can view your newly created account at: https://pixe.la/@{username}
4. Once done with those tasks you can click on the button that says “Study Session”
      You can enter a multitude of tasks on this page using the “Add Task” button and the tasks will later be saved to a database 
      under the study session you completed (saving to a database isn’t set up but you can still add tasks)
5. Once ready, click the “Start” button to start a study session
      Above the buttons is a stopwatch that tells you how long you’ve been studying
      “Stop” temporarily pauses the stopwatch but you can just click start to start it up again
      “Reset” resets the stopwatch, so don’t hit it by accident or all your hard work will be lost >O<
      While you’re studying the songs that you play on Spotify will be listed in the terminal, along with the percentage of how 
      long you listened to the song
6. Once you’re done studying, hit the “Finish” button
7. You’ll be redirected to our survey, where we ask you to submit how productive you think you were on a scale of 1-10 (if you 
   try to enter a number that’s not in that range it will let you – we didn’t get to implement error handling)
8. Once you’ve submitted the survey you can feel free to navigate to another area of our app (the information from the survey does get sent to the backend we just didn’t have a database to store it in)





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

This Python code allows users to interact with Spotify API through Spotify account login and granting permission to access users' private data. After successful login, the Spotify server will give the program {access_token, refresh_token, and expires_in}, which allows the program to access users' data for a specific amount of time. The program will compile the user's current playing tracks into a Python dictionary and create a .scv file. In the future, it would ideally be in C# and use a SQL database.

Instructions on running Study Web App Demo:

  0. Reference Material: https://www.youtube.com/watch?v=olY_2MW4Eik&list=PL1TBkFFBtagorhLzvm5dCA1cOqJKxnWNz&index=2
  1. Install the libraries
  2. Run the program. Select "http://127.0.0.1:5000".
  3. Login with a Spotify Account. Free Spotify Account: spotifyapi392@gmail.com, CSSpotifyAPI392
  4. Play music tracks on Spotify for roughly 1 minute.

## PixelaScript (Code to connect with Pixela's API):

This is C# code that allows you to connect to Pixela's API and create a user (with a token), a graph under that user's account, and pixels to be added to a graph found on the user's account. There are 5 methods: CreateUserAccount, CreateGraph, CreatePixel, GetGraphSVG, and Main. 

  **CreateUserAccount**(username, token) takes in a username (a string) and a token (also a string) and creates a user account on Pixela with that username and token. The profile for that specific user can be viewed at https://pixe.la/@{username you chose}. 

  **CreateGraph**(username, token, graphID, name, unit, color) takes in the username and token that was passed to CreateUserAccount and also takes in a uID, uName, uUnit, and uColor (all of which are strings) and these are the parameters for the graph that's being created. uID is the graph ID, uName is the name of the graph, uUnit is the units the graph uses, and uColor is the color of the graph's pixels. 

  **CreatePixel**(username, token, graphID, quantity) takes in the same username and token, in addition to the graphID and uQuantity (a string) which is the quantity the pixel is going to hold/record. 

  **GetGraphSVG**(username, graphID, date) takes the username and graphID, in addition to date (a string) which specifies what date (year-month-day format) you want the graph to start from. For example, if you inputted 20241029 the graph would start from the 29th and end at the current date. It then takes the information you've entered and retrieves the graph you specified (using graphID), inputs the data from that into a new graph that starts from the date you specified, and then saves the graph information as an SVG file in the Documents folder on your local machine. To open the SVG file you navigate to the Documents folder on your laptop/computer and double click on it. It'll then open up the image of the graph in a web browser. In our final app instead of taking the graph data we recieve and downloading it as an SVG file we can just put it directly into the HTML within an <img> tag.

  In the Main method I have a line of code to test each one of the four other methods. The lines should be run one at a time and in order because some tasks will be completed before the other is finished and that'll cause an error. 

When testing, you need to make a username that isn't already taken, and if that isn't done first the others will report a failure to complete the task. Also the token you chose needs to be at least 8 characters long. 
  
*To run the program:* Download the PixelaScript folder and open it in Microsoft Visual Studio. Once open, uncomment the line of code in Main that you're testing and click the Start button at the top. 

*Reference Materials:* https://stackoverflow.com/questions/75516705/pixela-api-fails-with-username-doesnt-exist-or-token-is-wrong (to figure out how to format the HTTP endpoint to connect to the Pixela API), Microsoft C# Reference Docs
