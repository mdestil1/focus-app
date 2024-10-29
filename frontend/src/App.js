import React from 'react';
import { BrowserRouter as Router, Route, Routes } from 'react-router-dom';
import { createTheme, ThemeProvider } from '@mui/material/styles';
import Home from './Home';
import Signup from './Signup';
import Checkout from './Checkout';
import Success from './success';
import Cancel from './cancel';
import Dashboard from './Dashboard';
import StudyZone from './Studyzone'; 

const theme = createTheme({
  palette: {
    primary: {
      main: '#3949ab', 
    },
    secondary: {
      main: '#f50057', 
    },
    background: {
      // Light gray bg
      default: '#f5f5f5', 
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
    // global styles to mui components
    MuiButton: {
      styleOverrides: {
        root: {
          borderRadius: 8, 
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
          <Route path="/checkout" element={<Checkout />} />
          <Route path="/success" element={<Success />} />
          <Route path="/cancel" element={<Cancel />} />
          <Route path="/dashboard" element={<Dashboard />} />
          <Route path="/studyzone" element={<StudyZone />} /> 
        </Routes>
      </Router>
    </ThemeProvider>
  );
}

export default App;
