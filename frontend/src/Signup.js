import React, { useState } from 'react';
import { useNavigate } from 'react-router-dom';
import axios from './axios'; // Adjust the path as necessary
import {
  Container,
  TextField,
  Button,
  Typography,
  Box,
  Paper,
} from '@mui/material';

const Signup = () => {
  const navigate = useNavigate();
  const [formData, setFormData] = useState({
    firstName: '',
    lastName: '',
    username: '',
    email: '',
    password: '',
  });

  const { firstName, lastName, username, email, password } = formData;

  const handleChange = (e) => {
    setFormData({ ...formData, [e.target.name]: e.target.value });
  };

  const handleSignup = async (e) => {
    e.preventDefault();

    try {
      const response = await axios.post('/Auth/signup', {
        firstName,
        lastName,
        username,
        email,
        password,
      });

      if (response.status === 201) {
        const { token } = response.data;
        localStorage.setItem('token', token);
        navigate('/dashboard');
      }
    } catch (error) {
      console.error('Signup error:', error);
      alert(error.response?.data?.message || 'Error signing up. Please try again.');
    }
  };

  return (
    <Container maxWidth="sm" sx={{ mt: 8 }}>
      <Paper elevation={3} sx={{ p: 4 }}>
        <Typography variant="h4" gutterBottom align="center">
          Sign Up
        </Typography>
        <form onSubmit={handleSignup}>
          <TextField
            label="First Name"
            name="firstName"
            variant="outlined"
            fullWidth
            required
            value={firstName}
            onChange={handleChange}
            sx={{ mb: 2 }}
          />
          <TextField
            label="Last Name"
            name="lastName"
            variant="outlined"
            fullWidth
            required
            value={lastName}
            onChange={handleChange}
            sx={{ mb: 2 }}
          />
          <TextField
            label="Username"
            name="username"
            variant="outlined"
            fullWidth
            required
            value={username}
            onChange={handleChange}
            sx={{ mb: 2 }}
          />
          <TextField
            label="Email"
            name="email"
            type="email"
            variant="outlined"
            fullWidth
            required
            value={email}
            onChange={handleChange}
            sx={{ mb: 2 }}
          />
          <TextField
            label="Password"
            name="password"
            type="password"
            variant="outlined"
            fullWidth
            required
            value={password}
            onChange={handleChange}
            sx={{ mb: 2 }}
          />
          <Box sx={{ mt: 3 }}>
            <Button type="submit" variant="contained" color="primary" fullWidth>
              Sign Up
            </Button>
          </Box>
        </form>
      </Paper>
    </Container>
  );
};

export default Signup;
