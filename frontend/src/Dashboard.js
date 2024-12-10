import React, { useEffect, useState } from 'react';
import axios from './axios';
import {
  AppBar,
  Toolbar,
  Typography,
  Container,
  Grid,
  Paper,
  Button,
  Box,
} from '@mui/material';
import { Link } from 'react-router-dom';

const Dashboard = () => {
  const [userData, setUserData] = useState(null);

  useEffect(() => {
    const fetchUserData = async () => {
      try {
        const response = await axios.get('/Auth/me');
        setUserData(response.data);
      } catch (error) {
        console.error('Error fetching user data:', error);
        alert('Error loading dashboard. Please try again.');
      }
    };

    fetchUserData();
  }, []);

  if (!userData) {
    return (
      <Container maxWidth="md" sx={{ mt: 8 }}>
        <Typography variant="h5">Loading Dashboard...</Typography>
      </Container>
    );
  }

  return (
    <Box sx={{ flexGrow: 1 }}>
      {/* AppBar */}
      <AppBar position="static">
        <Toolbar>
          <Typography variant="h6" component="div" sx={{ flexGrow: 1 }}>
            Study Dashboard
          </Typography>
          <Button color="inherit" component={Link} to="/survey">
            Survey
          </Button>
        </Toolbar>
      </AppBar>

      {/* Main Content */}
      <Container maxWidth="lg" sx={{ mt: 4 }}>
        <Typography variant="h4" gutterBottom>
          Welcome, {userData.username}!
        </Typography>
        <Grid container spacing={3}>
          {/* Example Section: Today's Tasks */}
          <Grid item xs={12} md={4}>
            <Paper sx={{ p: 2 }}>
              <Typography variant="h6" gutterBottom>
                Today's Tasks
              </Typography>
              {/* Implement your tasks list here */}
              <ul>
                <li>Review lecture notes</li>
                <li>Complete assignment</li>
                <li>Practice problems</li>
              </ul>
              <Button variant="contained" color="primary" fullWidth sx={{ mt: 2 }}>
                Add New Task
              </Button>
            </Paper>
          </Grid>

          {/* Example Section: Study Timer */}
          <Grid item xs={12} md={4}>
            <Paper sx={{ p: 2, textAlign: 'center' }}>
              <Typography variant="h6" gutterBottom>
                Study Timer
              </Typography>
              <Typography variant="h3" color="primary" sx={{ my: 2 }}>
                00:00:00
              </Typography>
              <Button variant="contained" color="secondary" sx={{ mr: 1 }}>
                Start
              </Button>
              <Button variant="outlined" color="secondary" sx={{ ml: 1 }}>
                Reset
              </Button>
            </Paper>
          </Grid>

          {/* Example Section: Productivity Stats */}
          <Grid item xs={12} md={4}>
            <Paper sx={{ p: 2 }}>
              <Typography variant="h6" gutterBottom>
                Productivity Stats
              </Typography>
              {/* Implement your charts or stats here */}
              <Typography>Chart Placeholder</Typography>
            </Paper>
          </Grid>
        </Grid>

        {/* Additional Sections */}
        <Grid container spacing={3} sx={{ mt: 2 }}>
          <Grid item xs={12}>
            <Paper sx={{ p: 2 }}>
              <Typography variant="h6" gutterBottom>
                Personalized Study Tips
              </Typography>
              <Typography>
                Based on your recent activity, we recommend focusing on time
                management and minimizing distractions during study sessions.
              </Typography>
            </Paper>
          </Grid>
        </Grid>
      </Container>
    </Box>
  );
};

export default Dashboard;
