import React, { useState } from 'react';
import { useNavigate } from 'react-router-dom';
import {
  Container,
  TextField,
  Button,
  Typography,
  FormControlLabel,
  Checkbox,
  Box,
  Paper,
} from '@mui/material';

const Signup = () => {
  const navigate = useNavigate();
  const [username, setUsername] = useState('');
  const [password, setPassword] = useState(''); 
  const [rememberMe, setRememberMe] = useState(false);

  const handleSignup = (e) => {
    e.preventDefault();

    // Save username if remember me is checked
    if (rememberMe) {
      localStorage.setItem('username', username);
    }

    // Save user data in localStorage to use on checkout page
    localStorage.setItem('username', username);
    localStorage.setItem('password', password); // Store password if needed

    // Redirect to checkout
    navigate('/checkout');
  };

  return (
    <Container maxWidth="sm" sx={{ mt: 8 }}>
      <Paper elevation={3} sx={{ p: 4 }}>
        <Typography variant="h4" gutterBottom align="center">
          Sign Up
        </Typography>
        <form onSubmit={handleSignup}>
          <TextField
            label="Username"
            variant="outlined"
            fullWidth
            required
            value={username}
            onChange={(e) => setUsername(e.target.value)}
            sx={{ mb: 2 }}
          />
          <TextField
            label="Password" 
            type="password"   
            variant="outlined"
            fullWidth
            required
            value={password}
            onChange={(e) => setPassword(e.target.value)} 
            sx={{ mb: 2 }}
          />
          <FormControlLabel
            control={
              <Checkbox
                checked={rememberMe}
                onChange={() => setRememberMe(!rememberMe)}
                color="primary"
              />
            }
            label="Remember Me"
          />
          <Box sx={{ mt: 3 }}>
            <Button
              type="submit"
              variant="contained"
              color="primary"
              fullWidth
            >
              Continue to Checkout
            </Button>
          </Box>
        </form>
      </Paper>
    </Container>
  );
};

export default Signup;
