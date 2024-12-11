# React + Vite

This template provides a minimal setup to get React working in Vite with HMR and some ESLint rules.

Currently, two official plugins are available:

- [@vitejs/plugin-react](https://github.com/vitejs/vite-plugin-react/blob/main/packages/plugin-react/README.md) uses [Babel](https://babeljs.io/) for Fast Refresh
- [@vitejs/plugin-react-swc](https://github.com/vitejs/vite-plugin-react-swc) uses [SWC](https://swc.rs/) for Fast Refresh



How To Use Our App:

Once you’ve downloaded the files and opened the StudyApp.sln file in Visual Studios please press the green “Start” button.

That takes you to our Dashboard where the journey to productivity can begin!

Steps to a Study Session:
1. On the Dashboard please click the “Sign into your Spotify Account” button
2. That will redirect you to the Spotify login page 
3. If you don’t want to log in with your own Spotify account you can use this one:
    sydm42162@gmail.com
	testingcs392
4. Once logged in, please click “Accept” the terms and you’ll be redirected to our Dashboard
5. On the Dashboard please click the “Sign up for a Pixela Account” button
6. You can view your newly created account at: https://pixe.la/@{username}
7. Once done with those tasks you can click on the button that says “Study Session”
8. You can enter a multitude of tasks on this page using the “Add Task” button and the tasks will later be saved to a database under the study session you completed (saving to a database isn’t set up but you can still add tasks)
9. Once ready, click the “Start” button to start a study session
    Above the buttons is a stopwatch that tells you how long you’ve been studying
    “Stop” temporarily pauses the stopwatch but you can just click start to start it up again
    “Reset” resets the stopwatch, so don’t hit it by accident or all your hard work will be lost >O<
    While you’re studying the songs that you play on Spotify will be listed in the terminal, along with the percentage of how long you listened to the song
10. Once you’re done studying, hit the “Finish” button
11. You’ll be redirected to our survey, where we ask you to submit how productive you think you were on a scale of 1-10 (if you try to enter a number that’s not in that range it will let you – we didn’t get to implement error handling)
12. Once you’ve submitted the survey you can feel free to navigate to another area of our app (the information from the survey does get sent to the backend we just didn’t have a database to store it in)

