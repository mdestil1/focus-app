import React, { useState } from 'react';
import { Container, Typography, Slider, Button, Box } from '@mui/material';
import axios from './axios';

const SurveyPage = () => {
  const [productivity, setProductivity] = useState(5); // Default value is 5

  // Handle slider value change
  const handleSliderChange = (event, newValue) => {
    setProductivity(newValue);
  };

  // Handle form submission
  const handleSubmit = async () => {
    const userId = localStorage.getItem('userId');
    if (!userId) {
      alert('You need to log in first!');
      return;
    }
    try {
      const response = await axios.post('/Auth/productivity', {
        userId: parseInt(userId),
        rating: productivity,
      });

      if (response.status === 200) {
        alert('Productivity rating recorded!');
      } else {
        alert('Error recording productivity rating');
      }
    } catch (error) {
      console.error('Error submitting productivity rating:', error);
      alert('An error occurred while recording your productivity rating.');
    }
  };

  return (
    <Container maxWidth="sm" sx={{ mt: 8 }}>
      <Typography variant="h4" gutterBottom>
        Productivity Survey
      </Typography>
      <Typography variant="h6" gutterBottom>
        On a scale of 1 to 10, how productive did you feel today?
      </Typography>

      {/* Slider Component */}
      <Box sx={{ mt: 4 }}>
        <Slider
          value={productivity}
          min={1}
          max={10}
          step={1}
          marks
          valueLabelDisplay="auto"
          onChange={handleSliderChange}
          sx={{ mb: 4 }}
        />
      </Box>

      {/* Submit Button */}
      <Button
        variant="contained"
        color="primary"
        onClick={handleSubmit}
        sx={{ mt: 2 }}
        fullWidth
      >
        Submit
      </Button>
    </Container>
  );
};

export default SurveyPage;
