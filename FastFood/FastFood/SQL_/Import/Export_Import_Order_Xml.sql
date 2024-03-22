-----------------------------------------------
--------------Exporting to Json----------------
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
Create or Alter procedure udp_Order_Menu_Employee_Import_XML(
@xml nvarchar(max)
)
as
Begin

    IF LEN(@xml) = 0
    BEGIN
        RETURN;
    END

    Declare @DocHandle int

    exec sp_xml_preparedocument @DocHandle out, @xml

    insert into Menu (meal_title, price, size, TimeToPrepare, IsForVegan, created_Date)
    select meal_title, price, size, TimeToPrepare, IsForVegan, created_Date
    from openxml(@DocHandle, '/Orders/order/Meal', 1)
    with(
    meal_title nvarchar(255) 'meal_title',
    price decimal(10,2) 'prcie',
    size nvarchar(7) 'size',
    TimeToPrepare time 'TimeToPrepare',
    IsForVegan bit 'IsForVegan',
    created_Date datetime 'created_Date'
    )

    insert into Employee (FName, LName, Telephone, Job, Age, Salary, HireDate, FullTime)
    select  FName, LName, Telephone, Job, Age, Salary, HireDate, FullTime
    from openxml(@DocHandle, '/Orders/order/Employee', 1)
        WITH (
            FName NVARCHAR(255) 'FName',
            LName NVARCHAR(255) 'LName',
            Telephone NVARCHAR(20) 'Telephone',
            Job NVARCHAR(255) 'Job',
            Age INT 'Age',
            Salary DECIMAL(10, 2) 'Salary',
            HireDate DATETIME 'HireDate',
            FullTime BIT 'FullTime'
        )

    insert into Orders(OrderTime, DeliveryTime, PaymentStatus, Meal_ID, Amount, Total_Cost, Prepared_by)
    output inserted.*
    select OrderTime, DeliveryTime, PaymentStatus, meal_ID, Amount, Total_Cost, employee_ID
    from openxml(@DocHandle, '/Orders/order',  1)
    with(
    order_ID int '@order_ID', 
    OrderTime Datetime 'OrderTime',
    DeliveryTime Datetime 'DeliveryTime',
    PaymentStatus bit 'PaymentStatus',
    meal_ID int '(Meal/@meal_ID)[1]',
    Amount int 'Amount',
    Total_Cost decimal(10,2) 'Total_Cost',
    employee_ID int '(Employee/@employee_ID)[1]'
    )

    EXEC sp_xml_removedocument @DocHandle;
End


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
