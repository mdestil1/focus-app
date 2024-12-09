import React from 'react';
import { useNavigate } from 'react-router-dom';
import axios from './axios'; // Use the axios instance
import {
  Container,
  Typography,
  Button,
  Card,
  CardContent,
  CardActions,
  Box,
  Paper,
} from '@mui/material';

const Checkout = () => {
  const navigate = useNavigate();
  const subscriptionPrice = '$10.00 per month';

  const handleCheckout = async () => {
    try {
      const userId = localStorage.getItem('userId');
      const response = await axios.post('/Payment/create-checkout-session', { userId: parseInt(userId) });

      if (response.status === 200) {
        const { url } = response.data;
        window.location.href = url;
      } else {
        throw new Error('Failed to create checkout session');
      }
    } catch (error) {
      console.error('Error creating checkout session:', error);
      alert('There was an error initiating the payment. Please try again later.');
    }
  };

  return (
    <Container maxWidth="sm" sx={{ mt: 8 }}>
      <Paper elevation={3} sx={{ p: 4 }}>
        <Typography variant="h4" gutterBottom align="center">
          Checkout
        </Typography>
        <Card elevation={3}>
          <CardContent>
            <Typography variant="h5" gutterBottom>
              Subscribe to Premium
            </Typography>
            <Typography variant="body1" gutterBottom>
              Unlock all features and maximize your productivity.
            </Typography>
            <Typography variant="h6" color="primary" gutterBottom>
              {subscriptionPrice}
            </Typography>
          </CardContent>
          <CardActions sx={{ justifyContent: 'center', mb: 2 }}>
            <Button variant="contained" color="primary" onClick={handleCheckout} sx={{ width: '80%' }}>
              Pay Now
            </Button>
          </CardActions>
        </Card>
        <Box sx={{ textAlign: 'center', mt: 2 }}>
          <Button variant="text" onClick={() => navigate('/')}>
            Back to Home
          </Button>
        </Box>
      </Paper>
    </Container>
  );
};

export default Checkout;
