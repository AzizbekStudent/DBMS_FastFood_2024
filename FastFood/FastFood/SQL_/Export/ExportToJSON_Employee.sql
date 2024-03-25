-- Students ID: 00013836   00014725   00014896
go
CREATE OR ALTER PROCEDURE udp_Employee_Filter_ToJSON(
    @FName nvarchar(255) = null,
    @Lname nvarchar(255) = null,
    @HireDate datetime = null
)
AS
BEGIN

-- Without this statement my procedure is infinitely called
-- I don't know What is causing this
	IF @@ROWCOUNT > 0
		BEGIN
			PRINT 'Procedure already executed. Exiting'; 
			RETURN; 
		END

    DECLARE @EmpTab AS TABLE (
        employee_ID int, 
        FName nvarchar(255),
        LName nvarchar(255), 
        Telephone nvarchar(20), 
        Job nvarchar(255),
        Age int,
        Salary decimal(10,2),
        HireDate datetime,
        FullTime bit
    );

    INSERT INTO @EmpTab
    SELECT employee_ID, FName, LName, Telephone, Job, Age, Salary, HireDate, FullTime
    FROM Employee
    WHERE (@FName IS NULL OR FName LIKE CONCAT(@FName, '%'))
        AND (@LName IS NULL OR LName LIKE CONCAT(@LName, '%'))
        AND (@HireDate IS NULL OR HireDate >= @HireDate)
    ORDER BY employee_ID;

    -- A bit modified the output
    -- Now it has Name {FirstName: _ ; LastName: _ ;}
    SELECT employee_ID
	,FName as "Name.FirstName"
	,LName as "Name.LastName"
	,Telephone
	,Job
	,Age
	,Salary
	,HireDate
	,FullTime
	FROM @EmpTab 
	FOR JSON PATH, ROOT('Employees'), INCLUDE_NULL_VALUES;
END

exec udp_Employee_Filter_ToJSON