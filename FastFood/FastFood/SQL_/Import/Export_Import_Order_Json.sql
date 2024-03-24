-----------------------------------------------
--------------Exporting to Json----------------
-----------------------------------------------
go
Create or Alter procedure Export_Order_To_Json
as
Begin

-- Without this statement my procedure is infinitely called
-- I don't know What is causing this
	IF @@ROWCOUNT > 0
		BEGIN
			PRINT 'Procedure already executed. Exiting'; 
			RETURN; 
		END

	select o.order_ID
	,o.OrderTime
	,o.DeliveryTime
	,o.PaymentStatus
	,m.meal_ID as "Meal.meal_ID"
	,m.meal_title as "Meal.meal_title"
	,m.price as "Meal.price"
	,m.size as "Meal.size"
	,m.TimeToPrepare as "Meal.TimeToPrepare"
	,m.IsForVegan as "Meal.IsForVegan"
	,m.created_Date as "Meal.created_Date"
	,o.Amount
	,o.Total_Cost
	,e.employee_ID as "Employee.employee_ID"
	,e.FName as "Employee.FName"
	,e.LName as "Employee.LName"
	,e.Telephone as "Employee.Telephone"
	,e.Job as "Employee.Job"
	,e.Age as "Employee.Age"
	,e.Salary as "Employee.Salary"
	,e.HireDate as "Employee.HireDate"
	,e.FullTime as "Employee.FullTime"
from Orders o left join Menu m on o.Meal_ID = m.meal_ID
	left join Employee e on o.Prepared_by = e.employee_ID

	order by o.order_ID asc
	for json path, root('Orders'), INCLUDE_NULL_VALUES
End

-- Testing
exec Export_Order_To_Json


-----------------------------------------------
--------------Importing to Json----------------
-----------------------------------------------

-- First inserting into Menu

Go
Create OR alter procedure udp_populate_menu_from_import
(
    @json_m nvarchar(max)
)
AS
BEGIN
IF @@ROWCOUNT > 1
		BEGIN
			PRINT 'Procedure already executed. Exiting  menu'; 
			RETURN; 
		END
-- Table for stroing IDs
	DECLARE @MenuMapTable TABLE (id int identity, old_id INT )
	Declare @menuNewRec table (id int identity, new_ids int)

	-- Storing old id
	insert into @MenuMapTable (old_id)
	select MealID 
	FROM OPENJSON(@json_m, N'$.Orders')
	WITH (
		MealID INT '$.Meal.meal_ID')
	
	-- Somehow it is creating null values
	delete from @MenuMapTable where old_id is null

    -- stroing new id
	INSERT INTO Menu (meal_title, price, size, TimeToPrepare, IsForVegan, created_Date)
	OUTPUT inserted.meal_ID into @menuNewRec (new_ids)  
	SELECT MealTitle, Price, Size, TimeToPrepare, IsForVegan, CreatedDate
	FROM OPENJSON(@json_m, N'$.Orders')
	WITH (
		MealTitle VARCHAR(255) '$.Meal.meal_title',
		Price DECIMAL(10, 2) '$.Meal.price',
		Size VARCHAR(7) '$.Meal.size',
		TimeToPrepare TIME '$.Meal.TimeToPrepare',
		IsForVegan BIT '$.Meal.IsForVegan',
		CreatedDate DATETIME '$.Meal.created_Date'
	)

	-- Returning mapped id
	SELECT e.old_id as "Menu old id", m.new_ids 
	FROM @MenuMapTable e
	left JOIN @menuNewRec m on e.id = m.id
END

-- Next inserting into Employee
Go
CREATE OR ALTER PROCEDURE udp_populate_employee_from_import
(
    @json_e nvarchar(max)
)
AS
BEGIN
IF @@ROWCOUNT > 1
		BEGIN
			PRINT 'Procedure already executed. Exiting employee'; 
			RETURN; 
		END
-- Table for stroing IDs
	DECLARE @EmpMapTable TABLE (id int identity, old_id INT )
	Declare @empNewRec table (id int identity, new_ids int)
	
	-- Storing old id
	insert into @EmpMapTable (old_id)
	select employee_ID 
	FROM OPENJSON(@json_e, N'$.Orders')
	WITH (
		employee_ID INT '$.Employee.employee_ID')

	-- somehow system is adding one additonal null record
	delete from @EmpMapTable where old_id is null
  
    -- storing new id
	INSERT INTO Employee (FName, LName, Telephone, Job, Age, Salary, HireDate, FullTime)
	OUTPUT inserted.employee_ID into @empNewRec (new_ids)  
	SELECT FName, LName, Telephone, Job, Age, Salary, HireDate, FullTime  
	FROM OPENJSON(@json_e, N'$.Orders')
	WITH (
		FName VARCHAR(255) '$.Employee.FName',
		LName VARCHAR(255) '$.Employee.LName',
		Telephone VARCHAR(20) '$.Employee.Telephone',
		Job VARCHAR(255) '$.Employee.Job',
		Age INT '$.Employee.Age',
		Salary DECIMAL(10, 2) '$.Employee.Salary',
		HireDate DATETIME '$.Employee.HireDate',
		FullTime BIT '$.Employee.FullTime'
	)

	-- Returning mapped id
	SELECT e.old_id as Employee, m.new_ids 
	FROM @EmpMapTable e
	Left JOIN @empNewRec m on e.id = m.id
END


-- Final procedure
go
Create or Alter procedure udp_Order_Menu_Employee_Import_Json
(
    @json nvarchar(max)
)
as
Begin
	IF @@ROWCOUNT > 1
		BEGIN
			PRINT 'Procedure already executed. Exiting  main'; 
			RETURN
		END
	-- Variables to save new id and old id
	declare @MealResult table (old_id int, new_id int)
	declare @Emp_Result table (old_id int, new_id int)

	-- Meal
	insert into @MealResult (old_id, new_id)
	exec udp_populate_menu_from_import @json_m = @json

	-- Prepared by
	insert into @Emp_Result (old_id, new_id)
	exec udp_populate_employee_from_import @json_e = @json

	-- Orders table 
	insert into Orders (OrderTime, DeliveryTime, PaymentStatus, Meal_ID, Amount, Total_Cost, Prepared_by)
	output inserted.*
	select OrderTime, DeliveryTime, PaymentStatus, m.new_id, Amount, TotalCost, e.new_id 
	from openjson(@json, N'$.Orders')
	with (
		OrderID INT '$.order_ID',
		OrderTime DATETIME '$.OrderTime',
		DeliveryTime DATETIME '$.DeliveryTime',
		PaymentStatus BIT '$.PaymentStatus',
		MealID INT '$.Meal.meal_ID',
		Amount INT '$.Amount',
		TotalCost DECIMAL(10, 2) '$.Total_Cost',
		EmployeeID INT '$.Employee.employee_ID'
	)
	 left Join @MealResult m on m.old_id = MealID 
	 left Join @Emp_Result e on e.old_id = EmployeeID 
End


go
---------- Sample Data ----------
---------------------------------
declare @jsonList nvarchar(max) = '{"Orders":[{"order_ID":1,"OrderTime":"2024-02-10T09:15:00","DeliveryTime":"2024-02-10T09:45:00","PaymentStatus":true,"Meal":{"meal_ID":2,"meal_title":"Margherita Pizza","price":12.99,"size":"Medium","TimeToPrepare":"00:20:00","IsForVegan":true,"created_Date":"2024-02-10T08:15:00"},"Amount":1,"Total_Cost":12.99,"Employee":{"employee_ID":3,"FName":"Bobur","LName":"Uzoqov","Telephone":"998-90-882-45-14","Job":"cashier","Age":22,"Salary":30000.00,"HireDate":"2023-01-03T10:00:00","FullTime":true}},{"order_ID":2,"OrderTime":"2024-02-10T08:30:00","DeliveryTime":"2024-02-10T09:00:00","PaymentStatus":false,"Meal":{"meal_ID":1,"meal_title":"Cheeseburger","price":9.99,"size":"Large","TimeToPrepare":"00:15:00","IsForVegan":false,"created_Date":"2024-02-10T08:00:00"},"Amount":2,"Total_Cost":19.98,"Employee":{"employee_ID":2,"FName":"Vali","LName":"Utkirov","Telephone":"998-90-872-86-72","Job":"cook","Age":28,"Salary":40000.00,"HireDate":"2023-01-02T09:00:00","FullTime":true}},{"order_ID":3,"OrderTime":"2024-02-10T10:00:00","DeliveryTime":"2024-02-10T10:30:00","PaymentStatus":true,"Meal":{"meal_ID":3,"meal_title":"Caesar Salad","price":8.49,"size":"Small","TimeToPrepare":"00:10:00","IsForVegan":true,"created_Date":"2024-02-10T08:30:00"},"Amount":3,"Total_Cost":25.47,"Employee":{"employee_ID":1,"FName":"Ali","LName":"Sobirov","Telephone":"998-90-852-87-74","Job":"admin","Age":35,"Salary":50000.00,"HireDate":"2023-01-01T08:00:00","FullTime":true}},{"order_ID":4,"OrderTime":"2024-02-10T11:00:00","DeliveryTime":"2024-02-10T11:30:00","PaymentStatus":false,"Meal":{"meal_ID":4,"meal_title":"Chicken Alfredo","price":14.99,"size":"Large","TimeToPrepare":"00:25:00","IsForVegan":false,"created_Date":"2024-02-10T09:00:00"},"Amount":2,"Total_Cost":29.98,"Employee":{"employee_ID":5,"FName":"Vadim","LName":"Sergeyev","Telephone":"998-90-952-32-26","Job":"cashier","Age":25,"Salary":32000.00,"HireDate":"2023-01-05T12:00:00","FullTime":true}},{"order_ID":5,"OrderTime":"2024-02-10T11:45:00","DeliveryTime":"2024-02-10T12:15:00","PaymentStatus":true,"Meal":{"meal_ID":5,"meal_title":"Veggie Wrap","price":7.99,"size":"Medium","TimeToPrepare":"00:12:00","IsForVegan":true,"created_Date":"2024-02-10T09:30:00"},"Amount":1,"Total_Cost":7.99,"Employee":{"employee_ID":4,"FName":"Vlad","LName":"Volkov","Telephone":"998-90-892-98-29","Job":"cook","Age":30,"Salary":45000.00,"HireDate":"2023-01-04T11:00:00","FullTime":true}},{"order_ID":6,"OrderTime":"2024-02-10T12:30:00","DeliveryTime":"2024-02-10T13:00:00","PaymentStatus":true,"Meal":{"meal_ID":6,"meal_title":"Fish and Chips","price":11.49,"size":"Large","TimeToPrepare":"00:18:00","IsForVegan":false,"created_Date":"2024-02-10T10:00:00"},"Amount":3,"Total_Cost":34.47,"Employee":{"employee_ID":3,"FName":"Bobur","LName":"Uzoqov","Telephone":"998-90-882-45-14","Job":"cashier","Age":22,"Salary":30000.00,"HireDate":"2023-01-03T10:00:00","FullTime":true}},{"order_ID":7,"OrderTime":"2024-02-10T13:30:00","DeliveryTime":"2024-02-10T14:00:00","PaymentStatus":true,"Meal":{"meal_ID":7,"meal_title":"Tofu Stir-Fry","price":9.99,"size":"Small","TimeToPrepare":"00:15:00","IsForVegan":true,"created_Date":"2024-02-10T10:30:00"},"Amount":2,"Total_Cost":19.98,"Employee":{"employee_ID":2,"FName":"Vali","LName":"Utkirov","Telephone":"998-90-872-86-72","Job":"cook","Age":28,"Salary":40000.00,"HireDate":"2023-01-02T09:00:00","FullTime":true}},{"order_ID":8,"OrderTime":"2024-02-10T14:15:00","DeliveryTime":"2024-02-10T14:45:00","PaymentStatus":false,"Meal":{"meal_ID":8,"meal_title":"Spaghetti Bolognese","price":10.99,"size":"Medium","TimeToPrepare":"00:22:00","IsForVegan":false,"created_Date":"2024-02-10T11:00:00"},"Amount":1,"Total_Cost":10.99,"Employee":{"employee_ID":1,"FName":"Ali","LName":"Sobirov","Telephone":"998-90-852-87-74","Job":"admin","Age":35,"Salary":50000.00,"HireDate":"2023-01-01T08:00:00","FullTime":true}},{"order_ID":9,"OrderTime":"2024-02-10T15:00:00","DeliveryTime":"2024-02-10T15:30:00","PaymentStatus":true,"Meal":{"meal_ID":9,"meal_title":"Grilled Chicken Sandwich","price":8.99,"size":"Large","TimeToPrepare":"00:17:00","IsForVegan":false,"created_Date":"2024-02-10T11:30:00"},"Amount":3,"Total_Cost":26.97,"Employee":{"employee_ID":5,"FName":"Vadim","LName":"Sergeyev","Telephone":"998-90-952-32-26","Job":"cashier","Age":25,"Salary":32000.00,"HireDate":"2023-01-05T12:00:00","FullTime":true}},{"order_ID":10,"OrderTime":"2024-02-10T16:00:00","DeliveryTime":"2024-02-10T16:30:00","PaymentStatus":true,"Meal":{"meal_ID":10,"meal_title":"Vegetable Soup","price":6.49,"size":"Small","TimeToPrepare":"00:08:00","IsForVegan":true,"created_Date":"2024-02-10T12:00:00"},"Amount":2,"Total_Cost":12.98,"Employee":{"employee_ID":4,"FName":"Vlad","LName":"Volkov","Telephone":"998-90-892-98-29","Job":"cook","Age":30,"Salary":45000.00,"HireDate":"2023-01-04T11:00:00","FullTime":true}},{"order_ID":11,"OrderTime":"2024-03-20T21:27:21.103","DeliveryTime":"2024-03-20T22:27:21.103","PaymentStatus":true,"Meal":{"meal_ID":2,"meal_title":"Margherita Pizza","price":12.99,"size":"Medium","TimeToPrepare":"00:20:00","IsForVegan":true,"created_Date":"2024-02-10T08:15:00"},"Amount":3,"Total_Cost":38.97,"Employee":{"employee_ID":null,"FName":null,"LName":null,"Telephone":null,"Job":null,"Age":null,"Salary":null,"HireDate":null,"FullTime":null}}]}'
---------------------------------
exec udp_populate_employee_from_import @json_e = @jsonList
exec udp_populate_menu_from_import @json_m = @jsonList
exec udp_Order_Menu_Employee_Import_Json @json = @jsonList

