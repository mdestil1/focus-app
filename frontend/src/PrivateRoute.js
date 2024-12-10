import React from 'react';
import { Navigate } from 'react-router-dom';

const PrivateRoute = ({ children }) => {
  const token = localStorage.getItem('token');
  const isPaid = localStorage.getItem('isPaid') === 'true';

  if (!token) {
    return <Navigate to="/login" replace />;
  }

  if (!isPaid) {
    return <Navigate to="/checkout" replace />;
  }

  return children;
};

export default PrivateRoute;
