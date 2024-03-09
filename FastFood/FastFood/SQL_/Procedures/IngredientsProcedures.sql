-- Ingredient table
-- Students ID: 00013836, 00014725, 00014896

-- Get All
Go
Create or Alter Procedure udp_Ingredients_Get_All
As
Begin
	Select ingredient_ID, Title, Price, Amount_in_grams, Unit, IsForVegan, Image  
	From Ingredients
End

-- Get By ID
Go
Create or Alter Procedure udp_Ingredients_Get_By_Id
    @ingredientID Int
As
Begin
    Select ingredient_ID, Title, Price, Amount_in_grams, Unit, IsForVegan, Image 
	From Ingredients Where ingredient_ID = @ingredientID
End

-- Create
Go
Create or Alter Procedure udp_Ingredients_Insert
    @Title Varchar(255),
    @Price Decimal(10, 2),
    @Amount_in_grams Int,
    @Unit Int,
    @IsForVegan BIT,
    @Image VarBinary(MAX)
As
Begin
    Insert Into Ingredients (Title, Price, Amount_in_grams, Unit, IsForVegan, Image)
    Values (@Title, @Price, @Amount_in_grams, @Unit, @IsForVegan, @Image)
End

-- Update
Go
Create or Alter Procedure udp_Ingredients_Update
    @ingredientID Int,
    @Title Varchar(255),
    @Price Decimal(10, 2),
    @Amount_in_grams Int,
    @Unit Int,
    @IsForVegan BIT,
    @Image VarBinary(MAX)
As
Begin
    Update Ingredients
    Set Title = @Title,
        Price = @Price,
        Amount_in_grams = @Amount_in_grams,
        Unit = @Unit,
        IsForVegan = @IsForVegan,
        Image = @Image
    Where ingredient_ID = @ingredientID
End

-- Delete
Go
Create or Alter Procedure udp_Ingredients_Delete
    @ingredientID Int
As
Begin
    Delete From Ingredients Where ingredient_ID = @ingredientID
End