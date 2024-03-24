-----------------------------------------------
--------------Exporting to Xml----------------
-----------------------------------------------
go
Create or Alter procedure Export_Order_To_Xml
as
Begin

-- Without this statement my procedure is infinitely called
-- I don't know What is causing this
	IF @@ROWCOUNT > 0
		BEGIN
			PRINT 'Procedure already executed. Exiting'; 
			RETURN; 
		END

	select 
        o.order_ID as "@order_ID",
        o.OrderTime,
        o.DeliveryTime,
        o.PaymentStatus,
        m.meal_ID as "Meal/@meal_ID",
        m.meal_title as "Meal/meal_title",
        m.price as "Meal/price",
        m.size as "Meal/size",
        m.TimeToPrepare as "Meal/TimeToPrepare",
        m.IsForVegan as "Meal/IsForVegan",
        m.created_Date as "Meal/created_Date",
        o.Amount,
        o.Total_Cost,
        e.employee_ID as "Employee/@employee_ID",
        e.FName as "Employee/FName",
        e.LName as "Employee/LName",
        e.Telephone as "Employee/Telephone",
        e.Job as "Employee/Job",
        e.Age as "Employee/Age",
        e.Salary as "Employee/Salary",
        e.HireDate as "Employee/HireDate",
        e.FullTime as "Employee/FullTime"
    from 
        Orders o 
    left join 
        Menu m on o.Meal_ID = m.meal_ID
    left join 
        Employee e on o.Prepared_by = e.employee_ID
    order by 
        o.order_ID asc
    for xml path('order'), root('Orders'), elements
End

-- Testing
exec Export_Order_To_Xml

-----------------------------------------------
--------------Importing to Xml----------------
-----------------------------------------------
Go
Create or Alter procedure udp_populate_employee_with_xml
(
	@xml_e nvarchar(max)
)
as
Begin
-- preventing from looping
	IF @@ROWCOUNT > 2
		BEGIN
			PRINT 'Procedure already executed. Exiting employee'; 
			RETURN; 
		END
   
	Declare @DocHandle_e int
	EXEC sp_xml_preparedocument @DocHandle_e out, @xml_e
	-- Declaring tables where old and new id will be stroed
	Declare @EmpMapTable table (id int identity, old_id int)
	Declare @EmpNewRecord table (id int identity, new_id int)

	-- populating with old id
	insert into @EmpMapTable (old_id)
	select employee_ID
	from openxml(@DocHandle_e, '/Orders/order', 1)
	WITH (
		employee_ID int 'Employee/@employee_ID'
	)
	-- preventing from null value
	delete from @EmpMapTable where old_id is null

	-- populating with ne id
	insert into Employee (FName, LName, Telephone, Job, Age, Salary, HireDate, FullTime)
    output inserted.employee_ID  into @EmpNewRecord (new_id)
	select  FName, LName, Telephone, Job, Age, Salary, HireDate, FullTime
    from openxml(@DocHandle_e, '/Orders/order', 1)
        WITH (
            FName NVARCHAR(255) 'Employee/FName',
            LName NVARCHAR(255) 'Employee/LName',
            Telephone NVARCHAR(20) 'Employee/Telephone',
            Job NVARCHAR(255) 'Employee/Job',
            Age INT 'Employee/Age',
            Salary DECIMAL(10, 2) 'Employee/Salary',
            HireDate DATETIME 'Employee/HireDate',
            FullTime BIT 'Employee/FullTime'
        )
	-- removing if there null value
	delete from @EmpNewRecord where new_id is null

	-- returning joined table
	Select e.old_id, n.new_id
	from @EmpMapTable e
	Join @EmpNewRecord n on e.id = n.id

	EXEC sp_xml_removedocument @DocHandle_e;
End

Go
Create or Alter procedure udp_populate_menu_with_xml
(
	@xml_m nvarchar(max)
)
as
Begin
	IF @@ROWCOUNT > 2
		BEGIN
			PRINT 'Procedure already executed. Exiting menu'; 
			RETURN; 
		END
	Declare @DocHandle_m int
	EXEC sp_xml_preparedocument @DocHandle_m out, @xml_m
	-- Declaring tables where old and new id will be stroed
	Declare @MenuMapTable table (id int identity, old_id int)
	Declare @MenuNewRecord table (id int identity, new_id int)

	-- populating with old id
	insert into @MenuMapTable (old_id)
	select meal_ID
	from openxml(@DocHandle_m, '/Orders/order', 1)
	WITH (
		meal_ID int 'Meal/@meal_ID'
	)
	-- preventing from null value
	delete from @MenuMapTable where old_id is null

	-- populating with ne id
	insert into Menu (meal_title, price, size, TimeToPrepare, IsForVegan, created_Date)
    output inserted.meal_ID  into @MenuNewRecord (new_id)
	select  meal_title, price, size, TimeToPrepare, IsForVegan, created_Date
    from openxml(@DocHandle_m, '/Orders/order', 1)
        WITH (
            meal_title nvarchar(255) 'Meal/meal_title',
			price decimal(10,2) 'Meal/prcie',
			size nvarchar(7) 'Meal/size',
			TimeToPrepare time 'Meal/TimeToPrepare',
			IsForVegan bit 'Meal/IsForVegan',
			created_Date datetime 'Meal/created_Date'
        )
	-- removing if there null value
	delete from @MenuNewRecord where new_id is null

	-- returning joined table
	Select m.old_id, n.new_id
	from @MenuMapTable m
	Join @MenuNewRecord n on m.id = n.id

	EXEC sp_xml_removedocument @DocHandle_m;
End


Go
Create or Alter procedure udp_Order_Menu_Employee_Import_XML
(
	@xml nvarchar(max)
)
as
Begin
--preventing from looping
	IF @@ROWCOUNT > 1
		BEGIN
			PRINT 'Procedure already executed. Exiting menu'; 
			RETURN; 
		END
	
	Declare @DocHandle_o int
	EXEC sp_xml_preparedocument @DocHandle_o out, @xml

	declare @MealResult table (old_id int, new_id int)
	declare @Emp_Result table (old_id int, new_id int)

	-- Meal
	insert into @MealResult (old_id, new_id)
	exec udp_populate_menu_with_xml @xml_m = @xml

	-- Prepared by
	insert into @Emp_Result (old_id, new_id)
	exec udp_populate_employee_with_xml @xml_e = @xml

	-- Insert Order
	insert into Orders(OrderTime, DeliveryTime, PaymentStatus, Meal_ID, Amount, Total_Cost, Prepared_by)
    output inserted.*
    select OrderTime, DeliveryTime, PaymentStatus, m.new_id, Amount, Total_Cost, e.new_id
    from openxml(@DocHandle_o, '/Orders/order',  1)
    with(
    order_ID int '@order_ID', 
    OrderTime Datetime 'OrderTime',
    DeliveryTime Datetime 'DeliveryTime',
    PaymentStatus bit 'PaymentStatus',
    meal_ID int '(Meal/@meal_ID)',
    Amount int 'Amount',
    Total_Cost decimal(10,2) 'Total_Cost',
    employee_ID int '(Employee/@employee_ID)'
    )
	left Join @MealResult m on m.old_id = meal_ID 
	left Join @Emp_Result e on e.old_id = employee_ID 

	EXEC sp_xml_removedocument @DocHandle_o;
End


go
-- Testing
Declare @xml_t nvarchar(max) = '<Orders><order order_ID="506">
    <OrderTime>2024-03-22T15:59:25.413</OrderTime>
    <DeliveryTime>2024-03-22T16:50:25.413</DeliveryTime>
    <PaymentStatus>0</PaymentStatus>
    <Meal meal_ID="837">
      <meal_title>Grilled Chicken Sandwich</meal_title>
      <price>8.99</price>
      <size>Large</size>
      <TimeToPrepare>00:17:00</TimeToPrepare>
      <IsForVegan>0</IsForVegan>
      <created_Date>2024-02-10T11:30:00</created_Date>
    </Meal>
    <Amount>3</Amount>
    <Total_Cost>26.97</Total_Cost>
    <Employee employee_ID="834">
      <FName>Vlad</FName>
      <LName>Volkov</LName>
      <Telephone>998-90-892-98-29</Telephone>
      <Job>cook</Job>
      <Age>30</Age>
      <Salary>45000.00</Salary>
      <HireDate>2023-01-04T11:00:00</HireDate>
      <FullTime>1</FullTime>
    </Employee>
  </order>
  </Orders>'

exec udp_Order_Menu_Employee_Import_XML @xml = @xml_t