-- Employee table
-- Students ID: 00013836, 00014725, 00014896

-- Get all
Go
Create or Alter Procedure udp_GetAllEmployee
As
Begin
	Select employee_ID, FName, LName, Telephone, Job, Age, Salary, HireDate, Image, FullTime
	From Employee
End

-- Get By ID
Go
Create or Alter Procedure udp_Employee_Get_ByID
    @employeeID int
As
Begin
    Select employee_ID, FName, LName, Telephone, Job, Age, Salary, HireDate, Image, FullTime
    From Employee
    Where employee_ID = @employeeID
End

-- Create
Go
Create or Alter Procedure udp_Employee_Insert
    @FName varchar(255),
    @LName varchar(255),
    @Telephone varchar(20),
    @Job varchar(255),
    @Age Int,
    @Salary Decimal(10, 2),
    @HireDate DateTime,
    @Image VarBinary(MAX),
    @FullTime BIT
As
Begin
    If @Job Not In ('admin', 'cook', 'cashier')
    Begin 
        Raiserror('Invalid job type. Job type might consist of these: admin, cook, and cashier.', 15, 1)
        Return
    End

    Insert Into Employee (FName, LName, Telephone, Job, Age, Salary, HireDate, Image, FullTime)
    Values (@FName, @LName, @Telephone, @Job, @Age, @Salary, @HireDate, @Image, @FullTime)
End

-- Update
go
Create or Alter Procedure udp_Employee_Update
    @employeeID Int,
    @FName varchar(255),
    @LName varchar(255),
    @Telephone varchar(20),
    @Job varchar(255),
    @Age Int,
    @Salary Decimal(10, 2),
    @HireDate DateTime,
    @Image VarBinary(MAX),
    @FullTime BIT
As
Begin
    If @Job Not In ('admin', 'cook', 'cashier')
    Begin 
        Raiserror('Invalid job type. Job type might consist of these: admin, cook, and cashier.', 15, 1)
        Return
    End
    Update Employee
    Set FName = @FName,
        LName = @LName,
        Telephone = @Telephone,
        Job = @Job,
        Age = @Age,
        Salary = @Salary,
        HireDate = @HireDate,
        Image = @Image,
        FullTime = @FullTime
    Where employee_ID = @employeeID
End

-- Delete
go
Create or Alter Procedure udp_Employee_Delete
    @employeeID Int
As
Begin
    Delete From Employee Where employee_ID = @employeeID
End