-- Menu_Ingredients table
-- Students ID: 00013836, 00014725, 00014896

-- Get All
Go
Create or Alter Procedure udp_Menu_Ingredients_Get_All
As
Begin
	Select meal_ID, ingredient_ID
    From Menu_Ingredients
    order by meal_ID
End

-- Get By Id
go
Create or Alter Procedure udp_Menu_Ingredients_Get_By_Id
    @mealID Int
As
Begin
    Select  meal_ID, ingredient_ID
    From Menu_Ingredients
    Where meal_ID = @mealID
End

-- Create
Go
Create or Alter Procedure udp_Menu_Ingredients_Insert
    @mealID Int,
    @ingredientID Int
As
Begin
    Insert Into Menu_Ingredients (meal_ID, ingredient_ID)
    Values (@mealID, @ingredientID)
End

-- Update
Go
Create or Alter Procedure udp_Menu_Ingredients_Update
    @mealID INT,
    @oldIngredientID INT,
    @newIngredientID INT
As
Begin
    Update Menu_Ingredients
    Set ingredient_ID = @newIngredientID
    Where meal_ID = @mealID AND ingredient_ID = @oldIngredientID;
End

-- Delete
go
Create or Alter Procedure udp_Menu_Ingredients_Delete
   @mealID INT,
    @ingredientID INT
As
Begin
    Delete From Menu_Ingredients
    Where meal_ID = @mealID And ingredient_ID = @ingredientID;
End 
