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


-- Inserting records
Insert Into Menu (meal_title, price, size, TimeToPrepare, IsForVegan, created_Date)
Values
    ( 'Cheeseburger', 9.99, 'Large', '00:15:00', 0, '2024-02-10 08:00'),
    ( 'Margherita Pizza', 12.99, 'Medium', '00:20:00', 1, '2024-02-10 08:15'),
    ( 'Caesar Salad', 8.49, 'Small', '00:10:00', 1, '2024-02-10 08:30'),
    ( 'Chicken Alfredo', 14.99, 'Large', '00:25:00', 0, '2024-02-10 09:00'),
    ( 'Veggie Wrap', 7.99, 'Medium', '00:12:00', 1, '2024-02-10 09:30'),
    ( 'Fish and Chips', 11.49, 'Large', '00:18:00', 0, '2024-02-10 10:00'),
    ( 'Tofu Stir-Fry', 9.99, 'Small', '00:15:00', 1, '2024-02-10 10:30'),
    ( 'Spaghetti Bolognese', 10.99, 'Medium', '00:22:00', 0, '2024-02-10 11:00'),
    ( 'Grilled Chicken Sandwich', 8.99, 'Large', '00:17:00', 0, '2024-02-10 11:30'),
    ( 'Vegetable Soup', 6.49, 'Small', '00:08:00', 1, '2024-02-10 12:00');


--------------------------------------------------------

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


-- Inserting records 
Insert Into Employee (FName, LName, Telephone, Job, Age, Salary, HireDate, Image, FullTime)
Values
    ('Ali', 'Sobirov', '998-90-852-87-74', 'admin', 35, 50000.00, '2023-01-01 08:00:00', NULL, 1),
    ('Vali', 'Utkirov', '998-90-872-86-72', 'cook', 28, 40000.00, '2023-01-02 09:00:00', NULL, 1),
    ('Bobur', 'Uzoqov', '998-90-882-45-14', 'cashier', 22, 30000.00, '2023-01-03 10:00:00', NULL, 1),
    ('Vlad', 'Volkov', '998-90-892-98-29', 'cook', 30, 45000.00, '2023-01-04 11:00:00', NULL, 1),
    ('Vadim', 'Sergeyev', '998-90-952-32-26', 'cashier', 25, 32000.00, '2023-01-05 12:00:00', NULL, 1),
    ('David', 'Jones', '998-90-842-22-11', 'cook', 40, 48000.00, '2023-01-06 13:00:00', NULL, 1),
    ('Mirsayid', 'Holiqov', '998-90-452-94-19', 'cashier', 29, 31000.00, '2023-01-07 14:00:00', NULL, 1),
    ('Akmal', 'Vohidov', '998-90-822-15-24', 'admin', 45, 55000.00, '2023-01-08 15:00:00', NULL, 1),
    ('Sardor', 'Rashidov', '998-90-962-35-34', 'cashier', 27, 33000.00, '2023-01-09 16:00:00', NULL, 1),
    ('Rustam', 'Akromov', '998-90-351-55-49', 'cook', 32, 47000.00, '2023-01-10 17:00:00', NULL, 1);


----------------------------------------------------------------

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


-- Inserting records into the Ingredients table
Insert Into Ingredients (Title, Price, Amount_in_grams, Unit, IsForVegan, Image)
Values
    ('Tomato', 1.99, 100, 1, 1, NULL),
    ('Lettuce', 0.99, 50, 1, 1, NULL),
    ('Onion', 0.79, 75, 1, 1, NULL),
    ('Chicken Breast', 5.99, 200, 2, 0, NULL),
    ('Black Beans', 2.49, 150, 3, 1, NULL),
    ('Rice', 1.49, 250, 4, 1, NULL),
    ('Tofu', 3.99, 300, 5, 1, NULL),
    ('Beef', 7.99, 250, 2, 0, NULL),
    ('Bell Pepper', 1.29, 80, 1, 1, NULL),
    ('Spinach', 1.79, 120, 1, 1, NULL);


----------------------------------------------------------------------

Create Table Menu_Ingredients (
    meal_ID Int ,
    ingredient_ID Int ,

    Primary Key (meal_ID, ingredient_ID),
    Foreign Key (meal_ID) References Menu(meal_ID),
    Foreign Key (ingredient_ID) References Ingredients(ingredient_ID),
    Constraint FK_Delete_Menu_Ingredients_Meal Foreign Key (meal_ID) References Menu(meal_ID) On Delete cascade,
    Constraint FK_Delete_Menu_Ingredients_Product Foreign Key (ingredient_ID) References Ingredients(ingredient_ID) On Delete cascade
)

-- Inserting records into the MenuIngredients table
Insert Into Menu_Ingredients (meal_ID, ingredient_ID)
Values
   (1, 1),(1, 2),(1, 3),(2, 2),(2, 3),(3, 2),(3, 3),(4, 4),(4, 5),(4, 6),(5, 2),
   (5, 3),(5, 5),(6, 4),(6, 6),(7, 3),(7, 5),(7, 7),(8, 4),(8, 8),(9, 4),(9, 2),
   (9, 3),(10, 2),(10, 3),(10, 9);

------------------------------------------------------------

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


-- Inserting records into the Orders table
Insert Into Orders (OrderTime, DeliveryTime, PaymentStatus, Meal_ID, Amount, Total_Cost, Prepared_by)
Values
    ('2024-02-10 09:15:00', '2024-02-10 09:45:00', 1, 2, 1, 12.99, 3),
    ('2024-02-10 08:30:00', '2024-02-10 09:00:00', 0, 1, 2, 19.98, 2),
    ('2024-02-10 10:00:00', '2024-02-10 10:30:00', 1, 3, 3, 25.47, 1),
    ('2024-02-10 11:00:00', '2024-02-10 11:30:00', 0, 4, 2, 29.98, 5),
    ('2024-02-10 11:45:00', '2024-02-10 12:15:00', 1, 5, 1, 7.99, 4), 
    ('2024-02-10 12:30:00', '2024-02-10 13:00:00', 1, 6, 3, 34.47, 3),
    ('2024-02-10 13:30:00', '2024-02-10 14:00:00', 1, 7, 2, 19.98, 2),
    ('2024-02-10 14:15:00', '2024-02-10 14:45:00', 0, 8, 1, 10.99, 1),
    ('2024-02-10 15:00:00', '2024-02-10 15:30:00', 1, 9, 3, 26.97, 5),
    ('2024-02-10 16:00:00', '2024-02-10 16:30:00', 1, 10, 2, 12.98, 4)
