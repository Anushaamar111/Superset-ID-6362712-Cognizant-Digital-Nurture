-- Create DB only if it doesn't exist
IF DB_ID('HRDemo') IS NULL
    CREATE DATABASE HRDemo;
GO

USE HRDemo;
GO

-- Drop and recreate Employees table (optional if you want clean setup)
IF OBJECT_ID('Employees', 'U') IS NOT NULL
    DROP TABLE Employees;
GO

CREATE TABLE Employees (
    EmployeeID INT IDENTITY(1,1) PRIMARY KEY,
    FirstName VARCHAR(50),
    LastName VARCHAR(50),
    DepartmentID INT,
    Salary DECIMAL(10,2),
    JoinDate DATE
);
GO

-- Drop and recreate sp_InsertEmployee
IF OBJECT_ID('sp_InsertEmployee', 'P') IS NOT NULL
    DROP PROCEDURE sp_InsertEmployee;
GO

CREATE PROCEDURE sp_InsertEmployee
 @FirstName VARCHAR(50),
 @LastName VARCHAR(50),
 @DepartmentID INT,
 @Salary DECIMAL(10,2),
 @JoinDate DATE
AS
BEGIN
 INSERT INTO Employees (FirstName, LastName, DepartmentID, Salary, JoinDate)
 VALUES (@FirstName, @LastName, @DepartmentID, @Salary, @JoinDate);
END;
GO

-- Drop and recreate sp_GetEmployeeCountByDepartment
IF OBJECT_ID('sp_GetEmployeeCountByDepartment', 'P') IS NOT NULL
    DROP PROCEDURE sp_GetEmployeeCountByDepartment;
GO

CREATE PROCEDURE sp_GetEmployeeCountByDepartment
 @DepartmentID INT
AS
BEGIN
    SELECT COUNT(*) AS EmployeeCount
    FROM Employees
    WHERE DepartmentID = @DepartmentID;
END;
GO

-- Drop and recreate sp_GetEmployeesByDepartment
IF OBJECT_ID('sp_GetEmployeesByDepartment', 'P') IS NOT NULL
    DROP PROCEDURE sp_GetEmployeesByDepartment;
GO

CREATE PROCEDURE sp_GetEmployeesByDepartment
 @DepartmentID INT
AS
BEGIN
    SELECT *
    FROM Employees
    WHERE DepartmentID = @DepartmentID;
END;
GO

-- ✅ Now test the procedures

-- Insert an employee
EXEC sp_InsertEmployee 
 @FirstName = 'Akansha',
 @LastName = 'Singh',
 @DepartmentID = 101,
 @Salary = 55000.00,
 @JoinDate = '2023-08-01';
GO

-- Get employee count in Department 101
EXEC sp_GetEmployeeCountByDepartment @DepartmentID = 101;
GO

-- Get all employee records
SELECT * FROM Employees;
GO
