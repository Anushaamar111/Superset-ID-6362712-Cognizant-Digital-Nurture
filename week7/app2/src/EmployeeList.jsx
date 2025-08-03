import React from 'react';
import EmployeeCard from './EmployeeCard';

const employees = [
  { id: 1, name: 'John Doe', position: 'Developer' },
  { id: 2, name: 'Jane Smith', position: 'Designer' },
  { id: 3, name: 'Peter Jones', position: 'Manager' },
];

function EmployeesList() {
  return (
    <div>
      {employees.map(employee => (
        <EmployeeCard key={employee.id} employee={employee} />
      ))}
    </div>
  );
}

export default EmployeesList;