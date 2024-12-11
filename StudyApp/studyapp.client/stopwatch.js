let startBtn = document.getElementById('start');
let stopBtn = document.getElementById('stop');
let finishBtn = document.getElementById('finish');
let resetBtn = document.getElementById('reset');

let hour = 0o0;
let minute = 0o0;
let second = 0o0;
let count = 0o0;

startBtn.addEventListener('click', function () {
    timer = true;
    stopWatch();
    // Start the stopwatch after the backend call
    fetch('https://localhost:7104/api/Spotify/start', {
        method: 'GET',
        headers: { 'Content-Type': 'application/json' }
    })
        .then(response => {
            if (!response.ok) {
                throw new Error('Failed to start session');
            }
            return response.json();
        })
        .then(data => {
            console.log(data.message);
            alert(data.message);
        })
        .catch(error => {
            console.error('Error:', error);
            alert('An error occurred while starting the session.');
        });
});



stopBtn.addEventListener('click', function () {
    timer = false;
});

finishBtn.addEventListener('click', function () {
    // Capture the current stopwatch time
    let timeData = {
        hour: hour,
        minute: minute,
        second: second,
        count: count,
        tasks: []  // Initialize the tasks array
    };

    // Capture the tasks (task descriptions and completion status)
    const taskElements = document.querySelectorAll('.task-container'); // Get all task containers
    taskElements.forEach(taskElement => {
        const taskDescription = taskElement.querySelector('label').textContent; // Get task description
        const taskCompleted = taskElement.querySelector('input[type="checkbox"]').checked; // Get task completion status

        // Add task data to the array
        timeData.tasks.push({
            description: taskDescription,
            isCompleted: taskCompleted
        });
    });

    // Log the data being sent to the backend
    console.log('Sending data to backend:', JSON.stringify(timeData));

    // Send the data to the server using a POST request
    fetch('https://localhost:7104/api/Spotify/finish', {
        method: 'POST',
        headers: {
            'Content-Type': 'application/json'
        },
        body: JSON.stringify(timeData) // Send timeData including tasks
    })
        .then(response => response.json())
        .then(data => {
            console.log('Session finished:', data);
            alert(data.message);  // Show the success message
            window.location.href = "https://localhost:5173/survey.html"; // Redirect the user
        })
        .catch(error => {
            console.error('Error finishing session:', error);
            alert('An error occurred while finishing the session. Check console for details.');
        });
});






resetBtn.addEventListener('click', function () {
    timer = false;
    hour = 0;
    minute = 0;
    second = 0;
    count = 0;
    document.getElementById('hr').innerHTML = "00";
    document.getElementById('min').innerHTML = "00";
    document.getElementById('sec').innerHTML = "00";
    document.getElementById('count').innerHTML = "00";
});

function stopWatch() {
    if (timer) {
        count++;

        if (count === 100) {
            second++;
            count = 0;
        }

        if (second === 60) {
            minute++;
            second = 0;
        }

        if (minute === 60) {
            hour++;
            minute = 0;
            second = 0;
        }

        // Update the displayed time
        let hrString = hour < 10 ? "0" + hour : hour;
        let minString = minute < 10 ? "0" + minute : minute;
        let secString = second < 10 ? "0" + second : second;
        let countString = count < 10 ? "0" + count : count;

        document.getElementById('hr').innerHTML = hrString;
        document.getElementById('min').innerHTML = minString;
        document.getElementById('sec').innerHTML = secString;
        document.getElementById('count').innerHTML = countString;

        // Re-run the function after 10 milliseconds
        setTimeout(stopWatch, 10);
    }
}

