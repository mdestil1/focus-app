import React from 'react';
import { useNavigate } from 'react-router-dom';
import {
  AppBar,
  Toolbar,
  Button,
  Container,
  Grid,
  Paper,
  Box,
  Typography,
} from '@mui/material';
import logo from './logo.png'; // Import the logo

const Home = () => {
  const navigate = useNavigate();

  return (
    <>
      {/* Navigation Bar */}
      <AppBar position="static">
        <Toolbar>
          {/* Logo as clickable image */}
          <img
            src={logo} // Use the imported logo
            alt="Logo"
            style={{
              height: '40px', // Adjust height as needed
              width: '40px',
              cursor: 'pointer',
              background: 'transparent',
              marginRight: '16px', // Add spacing between logo and buttons
            }}
            onClick={() => navigate('/')} // Navigate to the homepage
          />

          <Box sx={{ flexGrow: 1 }} />
          <Button
            color="inherit"
            onClick={() => navigate('/signup')}
          >
            Sign Up
          </Button>
          <Button
            color="inherit"
            onClick={() => navigate('/login')} // Navigate to the sign-in page
          >
            Sign In
          </Button>
        </Toolbar>
      </AppBar>

      {/* Hero Section */}
      <Box
        sx={{
          position: 'relative',
          backgroundImage: 'linear-gradient(to right, #3a7bd5, #3a6073)',
          color: '#fff',
          py: { xs: 8, sm: 12 },
          textAlign: 'center',
        }}
      >
        <Container maxWidth="sm">
          <Typography component="h1" variant="h3" gutterBottom>
            Maximize Your Productivity
          </Typography>
          <Typography variant="h5" paragraph>
            Track your study habits, stay focused, and achieve your goals with our app.
          </Typography>
          <Box sx={{ mt: 4 }}>
            <Button
              variant="contained"
              color="primary"
              onClick={() => navigate('/signup')}
              sx={{ mr: 2 }}
            >
              Sign Up
            </Button>
            <Button
              variant="contained"
              color="primary"
              onClick={() => navigate('/login')} // Navigate to the sign-in page
            >
              Sign In
            </Button>
          </Box>
        </Container>
      </Box>

      {/* Features Section */}
      <Container sx={{ py: 8 }} maxWidth="md">
        <Grid container spacing={4}>
          {/* Feature 1 */}
          <Grid item xs={12} sm={4}>
            <Paper sx={{ p: 4, textAlign: 'center' }}>
              <Typography variant="h6" gutterBottom>
                Habit Tracking
              </Typography>
              <Typography>
                Monitor your study habits and stay on top of your progress.
              </Typography>
            </Paper>
          </Grid>
          {/* Feature 2 */}
          <Grid item xs={12} sm={4}>
            <Paper sx={{ p: 4, textAlign: 'center' }}>
              <Typography variant="h6" gutterBottom>
                Focus Tools
              </Typography>
              <Typography>
                Use our focus timer and tools to eliminate distractions.
              </Typography>
            </Paper>
          </Grid>
          {/* Feature 3 */}
          <Grid item xs={12} sm={4}>
            <Paper sx={{ p: 4, textAlign: 'center' }}>
              <Typography variant="h6" gutterBottom>
                Personalized Tips
              </Typography>
              <Typography>
                Receive tips tailored to your study patterns.
              </Typography>
            </Paper>
          </Grid>
        </Grid>
      </Container>

      {/* Footer */}
      <Box sx={{ bgcolor: 'text.secondary', color: 'white', p: 6 }}>
        <Container maxWidth="lg">
          <Typography variant="h6" align="center" gutterBottom>
            StudyFocusApp
          </Typography>
          <Typography variant="body2" align="center">
            © {new Date().getFullYear()} StudyFocusApp. All rights reserved.
          </Typography>
        </Container>
      </Box>
    </>
  );
};

export default Home;
