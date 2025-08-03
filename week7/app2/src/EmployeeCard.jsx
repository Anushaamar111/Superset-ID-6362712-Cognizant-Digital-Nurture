import React, { useContext } from 'react';
import ThemeContext from './ThemeContext';

function EmployeeCard({ employee }) {
  const theme = useContext(ThemeContext);

  const cardStyle = {
    background: theme === 'dark' ? '#333' : '#FFF',
    color: theme === 'dark' ? '#FFF' : '#333',
    padding: '1rem',
    margin: '1rem',
    borderRadius: '8px',
    boxShadow: '0 2px 4px rgba(0,0,0,0.1)',
  };

  const buttonStyle = {
      background: theme === 'dark' ? '#555' : '#f0f0f0',
      color: theme === 'dark' ? '#FFF' : '#333',
      border: `1px solid ${theme === 'dark' ? '#666' : '#ccc'}`
  };

  return (
    <div style={cardStyle}>
      <h3>{employee.name}</h3>
      <p>{employee.position}</p>
      <button style={buttonStyle}>View Details</button>
    </div>
  );
}

export default EmployeeCard;