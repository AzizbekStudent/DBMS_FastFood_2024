-- Students ID: 00013836   00014725   00014896
Set Language English
Go

-------------------------------------------------
---------------  Creating Tables  ---------------
-------------------------------------------------

-- Students ID
-- 00013836		00014725	00014896
-- Case study related to FastFood

Create Table Menu (
    meal_ID Int Identity(1,1),
    meal_title Varchar(255),
    price Decimal(10, 2),
    size Varchar(7),
    TimeToPrepare Time,
    Image VarBinary(MAX),
    IsForVegan BIT,
    created_Date DateTime DEFAULT GETDATE()

	Constraint pk_meal_id Primary key (meal_ID),
	Constraint chk_size_type Check (size in ('Large', 'Medium', 'Small'))
)

----------------------------------------------------------------------------------

Create Table Employee (
    employee_ID Int Identity(1,1), 
    FName Varchar(255),
    LName Varchar(255),
    Telephone Varchar(20),
    Job Varchar(255),
    Age Int,
    Salary Decimal(10, 2),
    HireDate DateTime,
    Image VarBinary(MAX),
    FullTime BIT
	Constraint pk_employee_id Primary key (employee_ID),
	Constraint chk_Job_type Check (Job in ('admin', 'cook', 'cashier'))
)

----------------------------------------------------------------------------------

Create Table Ingredients (
    ingredient_ID Int Identity (1,1),
    Title Varchar(255),
    Price Decimal(10, 2),
    Amount_in_grams Int,
    Unit Int,
    IsForVegan BIT,
    Image VarBinary(MAX)

	Constraint pk_ingredient_id Primary key (ingredient_ID)
)

----------------------------------------------------------------------------------

Create Table Menu_Ingredients (
    meal_ID Int ,
    ingredient_ID Int ,

    Primary Key (meal_ID, ingredient_ID),
    Foreign Key (meal_ID) References Menu(meal_ID),
    Foreign Key (ingredient_ID) References Ingredients(ingredient_ID),
    Constraint FK_Delete_Menu_Ingredients_Meal Foreign Key (meal_ID) References Menu(meal_ID) On Delete cascade,
    Constraint FK_Delete_Menu_Ingredients_Product Foreign Key (ingredient_ID) References Ingredients(ingredient_ID) On Delete cascade
)

----------------------------------------------------------------------------------

Create Table Orders (
    order_ID Int Identity(1,1),
    OrderTime DateTime,
    DeliveryTime DateTime,
    PaymentStatus BIT,
    Meal_ID Int null,
    Amount Int,
    Total_Cost Decimal(10, 2),
    Prepared_by Int null,

	Constraint pk_order_id Primary key (order_ID),
    Foreign Key (Meal_ID) References Menu(meal_ID),
    Foreign Key (Prepared_by) References Employee(employee_ID),
    Constraint FK_Delete_Order_Menu Foreign Key (Meal_ID) References Menu(meal_ID) On Delete set null,
    Constraint FK_Delete_Order_Employee Foreign Key (Prepared_by) References Employee(employee_ID) On Delete set null
)

----------------------------------------------------------------------------------