go
CREATE OR ALTER PROCEDURE udp_Employee_Filter_ToXML(
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

    SELECT employee_ID as "@id"
	,FName as "Staff/FirstName"
	,LName as "Staff/LastName"
	,Telephone
	,Job
	,Age
	,Salary
	,HireDate
	,FullTime
	FROM @EmpTab 
	For xml path('Employee'), root('Employees'), elements
END

exec udp_Employee_Filter_ToXML 
