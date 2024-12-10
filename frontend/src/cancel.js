import React from 'react';
import { useNavigate } from 'react-router-dom';
import { Container, Typography, Button, Box } from '@mui/material';

const Cancel = () => {
  const navigate = useNavigate();

  return (
    <Container maxWidth="sm" sx={{ mt: 8 }}>
      <Box sx={{ textAlign: 'center' }}>
        <Typography variant="h4" gutterBottom>
          Payment Canceled
        </Typography>
        <Typography variant="body1" gutterBottom>
          Your payment was not completed. If this was an error, please try again.
        </Typography>
        <Button variant="contained" color="primary" onClick={() => navigate('/checkout')} sx={{ mt: 2 }}>
          Try Again
        </Button>
      </Box>
    </Container>
  );
};

export default Cancel;
