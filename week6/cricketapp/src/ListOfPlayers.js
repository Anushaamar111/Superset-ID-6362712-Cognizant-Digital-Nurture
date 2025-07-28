import React from 'react';

const ListofPlayers = () => {
  const players = [
    { name: 'Virat', score: 90 },
    { name: 'Rohit', score: 45 },
    { name: 'Dhoni', score: 85 },
    { name: 'Sachin', score: 99 },
    { name: 'Yuvraj', score: 60 },
    { name: 'Dravid', score: 72 },
    { name: 'Ashwin', score: 55 },
    { name: 'Jadeja', score: 75 },
    { name: 'Bumrah', score: 35 },
    { name: 'Shami', score: 40 },
    { name: 'Raina', score: 65 }
  ];

  const filtered = players.filter(p => p.score < 70);

  return (
    <div>
      <h2>All Players</h2>
      <ul>
        {players.map((p, i) => (
          <li key={i}>{p.name} - {p.score}</li>
        ))}
      </ul>
      <h3>Filtered (Score < 70)</h3>
      <ul>
        {filtered.map((p, i) => (
          <li key={i}>{p.name} - {p.score}</li>
        ))}
      </ul>
    </div>
  );
};

export default ListofPlayers;
