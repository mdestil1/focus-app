import React, { useEffect } from 'react';
import { useNavigate } from 'react-router-dom';
import { Container, Typography, Box } from '@mui/material';
import axios from './axios';

const Success = () => {
  const navigate = useNavigate();

  useEffect(() => {
    const verifyPayment = async () => {
      try {
        const response = await axios.get('/Auth/me');
        if (response.data.isPaid) {
          localStorage.setItem('isPaid', 'true');
          navigate('/dashboard');
        } else {
          navigate('/checkout');
        }
      } catch (error) {
        console.error('Error verifying payment:', error);
        navigate('/login');
      }
    };

    verifyPayment();
  }, [navigate]);

  return (
    <Container maxWidth="sm" sx={{ mt: 8 }}>
      <Box sx={{ textAlign: 'center' }}>
        <Typography variant="h4" gutterBottom>
          Payment Successful!
        </Typography>
        <Typography variant="body1">Redirecting to your dashboard...</Typography>
      </Box>
    </Container>
  );
};

export default Success;
