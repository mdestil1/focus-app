import React from 'react';
import { Container, Typography, Box } from '@mui/material';

const StudyZone = () => {
  return (
    <Container maxWidth="md" sx={{ mt: 8 }}>
      <Box sx={{ textAlign: 'center' }}>
        <Typography variant="h4" gutterBottom>
          Welcome to the Study Zone
        </Typography>
        <Typography variant="body1">
          This is where you can focus and enhance your productivity.
          <br />
          We'll integrate Spotify login and data collection here soon.
        </Typography>
      </Box>
    </Container>
  );
};

export default StudyZone;
