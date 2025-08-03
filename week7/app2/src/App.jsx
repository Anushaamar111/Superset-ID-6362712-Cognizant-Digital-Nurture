import React, { useState } from 'react';
import './App.css';
import EmployeesList from './EmployeeList.jsx';
import ThemeContext from './ThemeContext.jsx';

function App() {
  const [theme, setTheme] = useState('light');

  const toggleTheme = () => {
    setTheme(prevTheme => (prevTheme === 'light' ? 'dark' : 'light'));
  };

  return (
    <ThemeContext.Provider value={theme}>
      <div className={`App ${theme}`}>
        <button onClick={toggleTheme}>Toggle Theme</button>
        <EmployeesList />
      </div>
    </ThemeContext.Provider>
  );
}

export default App;