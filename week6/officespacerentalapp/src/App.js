import React from 'react';

function App() {
  const heading = "Office Space Rental";

  const officeList = [
    { name: "Tech Hub", rent: 55000, address: "Bangalore" },
    { name: "Innovation Tower", rent: 75000, address: "Hyderabad" },
    { name: "Startup Bay", rent: 45000, address: "Pune" },
    { name: "Corporate Heights", rent: 80000, address: "Mumbai" }
  ];

  const imageUrl = "https://via.placeholder.com/300x200.png?text=Office+Space";

  return (
    <div style={{ padding: '20px', fontFamily: 'Arial' }}>
      <h1>{heading}</h1>
      <img src={imageUrl} alt="Office" style={{ width: '300px', height: '200px' }} />
      <ul>
        {officeList.map((office, index) => (
          <li key={index}>
            <p><strong>Name:</strong> {office.name}</p>
            <p><strong>Address:</strong> {office.address}</p>
            <p style={{ color: office.rent < 60000 ? 'red' : 'green' }}>
              <strong>Rent:</strong> ₹{office.rent}
            </p>
            <hr />
          </li>
        ))}
      </ul>
    </div>
  );
}

export default App;
