GO
CREATE FUNCTION fn_CalculateAnnualSalary (@EmployeeID INT)
RETURNS DECIMAL(18, 2)
AS
BEGIN
    DECLARE @AnnualSalary DECIMAL(18, 2);

    SELECT @AnnualSalary = Salary * 12
    FROM Employees
    WHERE EmployeeID = @EmployeeID;

    RETURN @AnnualSalary;
END;
GO

SELECT dbo.fn_CalculateAnnualSalary(1) AS AnnualSalary;
