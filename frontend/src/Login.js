// Login.js
import React, { useState } from 'react';
import { useNavigate } from 'react-router-dom';
import axios from './axios';
import {
  Container,
  TextField,
  Button,
  Typography,
  Box,
  Paper,
} from '@mui/material';

const Login = () => {
  const navigate = useNavigate();
  const [identifier, setIdentifier] = useState(''); // Can be email or username
  const [password, setPassword] = useState('');

  const handleLogin = async (e) => {
    e.preventDefault();

    try {
      const response = await axios.post('/Auth/login', {
        identifier,
        password,
      });

      if (response.status === 200) {
        const { token } = response.data;
        localStorage.setItem('token', token);
        navigate('/dashboard');
      }
    } catch (error) {
      console.error('Login error:', error);
      alert(error.response?.data?.message || 'Invalid credentials. Please try again.');
    }
  };

  return (
    <Container maxWidth="sm" sx={{ mt: 8 }}>
      <Paper elevation={3} sx={{ p: 4 }}>
        <Typography variant="h4" gutterBottom align="center">
          Login
        </Typography>
        <form onSubmit={handleLogin}>
          <TextField
            label="Email or Username"
            variant="outlined"
            fullWidth
            required
            value={identifier}
            onChange={(e) => setIdentifier(e.target.value)}
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
          <Box sx={{ mt: 3 }}>
            <Button type="submit" variant="contained" color="primary" fullWidth>
              Login
            </Button>
          </Box>
        </form>
      </Paper>
    </Container>
  );
};

export default Login;
