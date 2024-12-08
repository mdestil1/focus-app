import React from 'react';
import { BrowserRouter as Router, Route, Routes } from 'react-router-dom';
import { createTheme, ThemeProvider } from '@mui/material/styles';
import Home from './Home';
import Signup from './Signup';
import Login from './Login'; // New Login page
import Checkout from './Checkout';
import Success from './Success';
import Cancel from './Cancel';
import Dashboard from './Dashboard';
import StudyZone from './StudyZone';
import SurveyPage from './SurveyPage'; // Import the SurveyPage component

const theme = createTheme({
  palette: {
    primary: {
      main: '#3949ab', // Blue color
    },
    secondary: {
      main: '#f50057', // Red color
    },
    background: {
      default: '#f5f5f5', // Light gray background
    },
  },
  typography: {
    fontFamily: 'Roboto, Arial',
    h4: {
      fontWeight: 600,
    },
    h6: {
      fontWeight: 500,
    },
    button: {
      textTransform: 'none',
    },
  },
  components: {
    MuiButton: {
      styleOverrides: {
        root: {
          borderRadius: 8, // Rounded buttons
        },
      },
    },
  },
});

function App() {
  return (
    <ThemeProvider theme={theme}>
      <Router>
        <Routes>
          <Route path="/" element={<Home />} />
          <Route path="/signup" element={<Signup />} />
          <Route path="/login" element={<Login />} /> {/* New Login route */}
          <Route path="/checkout" element={<Checkout />} />
          <Route path="/success" element={<Success />} />
          <Route path="/cancel" element={<Cancel />} />
          <Route path="/dashboard" element={<Dashboard />} />
          <Route path="/studyzone" element={<StudyZone />} />
          <Route path="/survey" element={<SurveyPage />} /> {/* Survey Page route */}
        </Routes>
      </Router>
    </ThemeProvider>
  );
}

export default App;
