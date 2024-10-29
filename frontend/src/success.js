import React, { useEffect } from 'react';
import { useNavigate } from 'react-router-dom';

const Success = () => {
  const navigate = useNavigate();

  useEffect(() => {
    // Redirect to the dashboard after 2 seconds
    const timer = setTimeout(() => {
      navigate('/dashboard');
    }, 2000);
    
    return () => clearTimeout(timer);
  }, [navigate]);

  return (
    <div>
      <h2>Payment Successful!</h2>
      <p>Redirecting to your study dashboard...</p>
    </div>
  );
};

export default Success;
