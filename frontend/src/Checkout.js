import React from 'react';
import { useNavigate } from 'react-router-dom';
import {
  Container,
  Typography,
  Button,
  Card,
  CardContent,
  CardActions,
  Box,
} from '@mui/material';

const Checkout = () => {
  const navigate = useNavigate();
  const subscriptionPrice = '$10.00 per month';

  const handleCheckout = async () => {
    try {
      // Send a post request to backend to create a Stripe checkout session
      const response = await fetch('http://localhost:5123/create-checkout-session', {
        method: 'POST',
        headers: { 'Content-Type': 'application/json' },
      });

      // Check if the response is successful
      if (!response.ok) {
        throw new Error(`HTTP error! status: ${response.status}`);
      }

      // Parse the JSON response to get the checkout URL
      const { url } = await response.json();

      // Redirect user to the Stripe checkout page
      window.location.href = url;
    } catch (error) {
      // Handle any errors that occur during the checkout process
      console.error('Error creating checkout session:', error);
      alert('There was an error initiating the payment. Please try again later.');
    }
  };

  return (
    <Container maxWidth="sm" sx={{ mt: 8 }}>
      {/* Card containing checkout information */}
      <Card elevation={3}>
        <CardContent>
          <Typography variant="h4" gutterBottom>
            Checkout
          </Typography>
          <Typography variant="body1" gutterBottom>
            Subscribe to unlock all features and maximize your productivity.
          </Typography>
          <Typography variant="h5" color="primary" gutterBottom>
            {subscriptionPrice}
          </Typography>
        </CardContent>
        <CardActions sx={{ justifyContent: 'center', mb: 2 }}>
          {/* Pay Now button which triggers the handleCheckout function */}
          <Button
            variant="contained"
            color="primary"
            onClick={handleCheckout}
            sx={{ width: '80%' }}
          >
            Pay Now
          </Button>
        </CardActions>
      </Card>
      {/* Back to Home button */}
      <Box sx={{ textAlign: 'center', mt: 2 }}>
        <Button variant="text" onClick={() => navigate('/')}>
          Back to Home
        </Button>
      </Box>
    </Container>
  );
};

export default Checkout;
