-- Menu table
-- Students ID: 00013836, 00014725, 00014896

-- Get All
Go
Create or Alter Procedure udp_Menu_Get_All
As
Begin
    Select meal_ID, meal_title, price, size, TimeToPrepare, Image, IsForVegan, created_Date 
    From Menu
End

-- Get By ID
Go
Create or Alter Procedure udp_Menu_Get_By_Id
	@meal_ID int
As
Begin
	Select meal_ID, meal_title, price, size, TimeToPrepare, Image, IsForVegan, created_Date  
	From Menu Where meal_ID = @meal_ID
End

-- Create
Go
Create or Alter Procedure udp_Menu_Insert
    @meal_title VARCHAR(255),
    @price DECIMAL(10, 2),
    @size VARCHAR(7),
    @TimeToPrepare TIME,
    @Image VARBINARY(MAX),
    @IsForVegan BIT,
    @created_date DATETIME
As
Begin
    Insert Into Menu (meal_title, price, size, TimeToPrepare, Image, IsForVegan, created_Date)
    Values (@meal_title, @price, @size, @TimeToPrepare, @Image, @IsForVegan, @created_date)
End

-- Update
Go
Create or Alter Procedure udp_Menu_Update
    @meal_ID INT,
    @meal_title VARCHAR(255),
    @price DECIMAL(10, 2),
    @size VARCHAR(7),
    @TimeToPrepare TIME,
    @Image VARBINARY(MAX),
    @IsForVegan BIT
As
Begin
    Update Menu
    Set meal_title = @meal_title,
        price = @price,
        size = @size,
        TimeToPrepare = @TimeToPrepare,
        Image = @Image,
        IsForVegan = @IsForVegan
    Where meal_ID = @meal_ID
End

-- Delete
Go
Create or Alter Procedure udp_Menu_Delete
    @meal_ID INT
As
Begin
    Delete From Menu Where meal_ID = @meal_ID
End