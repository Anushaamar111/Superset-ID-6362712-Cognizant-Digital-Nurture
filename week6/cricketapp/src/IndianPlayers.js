import React from 'react';

const IndianPlayers = () => {
  const T20players = ['Virat', 'Rohit', 'Pant', 'Hardik'];
  const RanjiPlayers = ['Shreyas', 'Siraj', 'Gill'];

  const allPlayers = [...T20players, ...RanjiPlayers];

  const oddPlayers = allPlayers.filter((_, i) => i % 2 !== 0);
  const evenPlayers = allPlayers.filter((_, i) => i % 2 === 0);

  return (
    <div>
      <h2>All Players: {allPlayers.join(', ')}</h2>
      <p><strong>Odd Team Players:</strong> {oddPlayers.join(', ')}</p>
      <p><strong>Even Team Players:</strong> {evenPlayers.join(', ')}</p>
    </div>
  );
};

export default IndianPlayers;
    