-- Students ID: 00013836   00014725   00014896

-- Creating tables for logging 
-- Log table for Menu table
CREATE TABLE Log_Menu (
    Log_ID INT IDENTITY(1,1) PRIMARY KEY,
    OperationType VARCHAR(10),
    meal_ID INT,
    meal_title VARCHAR(255),
    price DECIMAL(10, 2),
    size VARCHAR(7),
    TimeToPrepare TIME,
    IsForVegan BIT,
    created_Date DATETIME,
    LogDateTime DATETIME DEFAULT GETDATE()
)

-- Log table for Employee table
CREATE TABLE Log_Employee (
    Log_ID INT IDENTITY(1,1) PRIMARY KEY,
    OperationType VARCHAR(10),
    employee_ID INT,
    FName VARCHAR(255),
    LName VARCHAR(255),
    Telephone VARCHAR(20),
    Job VARCHAR(255),
    Age INT,
    Salary DECIMAL(10, 2),
    HireDate DATETIME,
    FullTime BIT,
    LogDateTime DATETIME DEFAULT GETDATE()
)

-- Log table for Ingredients table
CREATE TABLE Log_Ingredients (
    Log_ID INT IDENTITY(1,1) PRIMARY KEY,
    OperationType VARCHAR(10),
    ingredient_ID INT,
    Title VARCHAR(255),
    Price DECIMAL(10, 2),
    Amount_in_grams INT,
    Unit INT,
    IsForVegan BIT,
    LogDateTime DATETIME DEFAULT GETDATE()
)

-- Log table for Menu_Ingredients table
CREATE TABLE Log_Menu_Ingredients (
    Log_ID INT IDENTITY(1,1) PRIMARY KEY,
    OperationType VARCHAR(10),
    meal_ID INT,
    ingredient_ID INT,
    LogDateTime DATETIME DEFAULT GETDATE()
)

CREATE TABLE Log_Orders (
    Log_ID INT IDENTITY(1,1) PRIMARY KEY,
    OperationType VARCHAR(10),
    order_ID INT,
    OrderTime DATETIME,
    DeliveryTime DATETIME,
    PaymentStatus BIT,
    Meal_ID INT NULL,
    Amount INT,
    Total_Cost DECIMAL(10, 2),
    Prepared_by INT NULL,
    LogDateTime DATETIME DEFAULT GETDATE()
)

-- Creating Triggers for logging the changes made on particular table
-- Menu logging
go
CREATE or ALTER TRIGGER trg_Menu_Log
ON Menu
AFTER INSERT, UPDATE, DELETE
AS
BEGIN
    SET NOCOUNT ON;

    -- Changes for insert
    IF EXISTS (SELECT * FROM inserted) 
    BEGIN
        INSERT INTO Log_Menu (OperationType, meal_ID, meal_title, price, size, TimeToPrepare, IsForVegan, created_Date)
        SELECT 'INSERT', meal_ID, meal_title, price, size, TimeToPrepare, IsForVegan, created_Date
        FROM inserted;
    END;

    -- Changes for delete
    IF EXISTS (SELECT * FROM deleted) 
    BEGIN
        INSERT INTO Log_Menu (OperationType, meal_ID, meal_title, price, size, TimeToPrepare, IsForVegan, created_Date)
        SELECT 'DELETE', meal_ID, meal_title, price, size, TimeToPrepare, IsForVegan, created_Date
        FROM deleted;
    END;

    -- Change for update
    IF EXISTS (SELECT * FROM inserted i INNER JOIN deleted d ON i.meal_ID = d.meal_ID) 
    BEGIN
        INSERT INTO Log_Menu (OperationType, meal_ID, meal_title, price, size, TimeToPrepare, IsForVegan, created_Date)
        SELECT 'UPDATE', i.meal_ID, i.meal_title, i.price, i.size, i.TimeToPrepare, i.IsForVegan, i.created_Date
        FROM inserted i
        INNER JOIN deleted d ON i.meal_ID = d.meal_ID;
    END;
END;

go
-- Employee logging
CREATE OR ALTER TRIGGER trg_Employee_Log
ON Employee
AFTER INSERT, UPDATE, DELETE
AS
BEGIN
    SET NOCOUNT ON;

    -- Changes for insert
    IF EXISTS (SELECT * FROM inserted) 
    BEGIN
        INSERT INTO Log_Employee (OperationType, employee_ID, FName, LName, Telephone, Job, Age, Salary, HireDate, FullTime)
        SELECT 'INSERT', employee_ID, FName, LName, Telephone, Job, Age, Salary, HireDate, FullTime
        FROM inserted;
    END;

    -- Changes for delete
    IF EXISTS (SELECT * FROM deleted) 
    BEGIN
        INSERT INTO Log_Employee (OperationType, employee_ID, FName, LName, Telephone, Job, Age, Salary, HireDate, FullTime)
        SELECT 'DELETE', employee_ID, FName, LName, Telephone, Job, Age, Salary, HireDate, FullTime
        FROM deleted;
    END;

    -- Changes for update
    IF EXISTS (SELECT * FROM inserted i INNER JOIN deleted d ON i.employee_ID = d.employee_ID) 
    BEGIN
        INSERT INTO Log_Employee (OperationType, employee_ID, FName, LName, Telephone, Job, Age, Salary, HireDate, FullTime)
        SELECT 'UPDATE', i.employee_ID, i.FName, i.LName, i.Telephone, i.Job, i.Age, i.Salary, i.HireDate, i.FullTime
        FROM inserted i
        INNER JOIN deleted d ON i.employee_ID = d.employee_ID;
    END;
END;

GO
-- Ingredients logging
CREATE OR ALTER TRIGGER trg_Ingredients_Log
ON Ingredients
AFTER INSERT, UPDATE, DELETE
AS
BEGIN
    SET NOCOUNT ON;

    -- Changes for insert
    IF EXISTS (SELECT * FROM inserted) 
    BEGIN
        INSERT INTO Log_Ingredients (OperationType, ingredient_ID, Title, Price, Amount_in_grams, Unit, IsForVegan)
        SELECT 'INSERT', ingredient_ID, Title, Price, Amount_in_grams, Unit, IsForVegan
        FROM inserted;
    END;

    -- Changes for delete
    IF EXISTS (SELECT * FROM deleted) 
    BEGIN
        INSERT INTO Log_Ingredients (OperationType, ingredient_ID, Title, Price, Amount_in_grams, Unit, IsForVegan)
        SELECT 'DELETE', ingredient_ID, Title, Price, Amount_in_grams, Unit, IsForVegan
        FROM deleted;
    END;

    -- Changes for update
    IF EXISTS (SELECT * FROM inserted i INNER JOIN deleted d ON i.ingredient_ID = d.ingredient_ID) 
    BEGIN
        INSERT INTO Log_Ingredients (OperationType, ingredient_ID, Title, Price, Amount_in_grams, Unit, IsForVegan)
        SELECT 'UPDATE', i.ingredient_ID, i.Title, i.Price, i.Amount_in_grams, i.Unit, i.IsForVegan
        FROM inserted i
        INNER JOIN deleted d ON i.ingredient_ID = d.ingredient_ID;
    END;
END;

GO
-- Menu-Ingredients logging
CREATE OR ALTER TRIGGER trg_Menu_Ingredients_Log
ON Menu_Ingredients
AFTER INSERT, UPDATE, DELETE
AS
BEGIN
    SET NOCOUNT ON;

    -- Changes for insert
    IF EXISTS (SELECT * FROM inserted) 
    BEGIN
        INSERT INTO Log_Menu_Ingredients (OperationType, meal_ID, ingredient_ID)
        SELECT 'INSERT', meal_ID, ingredient_ID
        FROM inserted;
    END;

    -- Changes for delete
    IF EXISTS (SELECT * FROM deleted) 
    BEGIN
        INSERT INTO Log_Menu_Ingredients (OperationType, meal_ID, ingredient_ID)
        SELECT 'DELETE', meal_ID, ingredient_ID
        FROM deleted;
    END;

    -- Changes for update
    IF EXISTS (SELECT * FROM inserted i INNER JOIN deleted d ON i.meal_ID = d.meal_ID AND i.ingredient_ID = d.ingredient_ID) 
    BEGIN
        INSERT INTO Log_Menu_Ingredients (OperationType, meal_ID, ingredient_ID)
        SELECT 'UPDATE', i.meal_ID, i.ingredient_ID
        FROM inserted i
        INNER JOIN deleted d ON i.meal_ID = d.meal_ID AND i.ingredient_ID = d.ingredient_ID;
    END;
END;

GO
-- Logging orders
CREATE OR ALTER TRIGGER trg_Orders_Log
ON Orders
AFTER INSERT, UPDATE, DELETE
AS
BEGIN
    SET NOCOUNT ON;

    -- Changes for insert
    IF EXISTS (SELECT * FROM inserted) 
    BEGIN
        INSERT INTO Log_Orders (OperationType, order_ID, OrderTime, DeliveryTime, PaymentStatus, Meal_ID, Amount, Total_Cost, Prepared_by)
        SELECT 'INSERT', order_ID, OrderTime, DeliveryTime, PaymentStatus, Meal_ID, Amount, Total_Cost, Prepared_by
        FROM inserted;
    END;

    -- Changes for delete
    IF EXISTS (SELECT * FROM deleted) 
    BEGIN
        INSERT INTO Log_Orders (OperationType, order_ID, OrderTime, DeliveryTime, PaymentStatus, Meal_ID, Amount, Total_Cost, Prepared_by)
        SELECT 'DELETE', order_ID, OrderTime, DeliveryTime, PaymentStatus, Meal_ID, Amount, Total_Cost, Prepared_by
        FROM deleted;
    END;

    -- Changes for update
    IF EXISTS (SELECT * FROM inserted i INNER JOIN deleted d ON i.order_ID = d.order_ID) 
    BEGIN
        INSERT INTO Log_Orders (OperationType, order_ID, OrderTime, DeliveryTime, PaymentStatus, Meal_ID, Amount, Total_Cost, Prepared_by)
        SELECT 'UPDATE', i.order_ID, i.OrderTime, i.DeliveryTime, i.PaymentStatus, i.Meal_ID, i.Amount, i.Total_Cost, i.Prepared_by
        FROM inserted i
        INNER JOIN deleted d ON i.order_ID = d.order_ID;
    END;
END;

