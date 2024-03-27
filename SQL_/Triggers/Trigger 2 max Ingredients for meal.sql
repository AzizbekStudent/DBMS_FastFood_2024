-- Students ID: 00013836   00014725   00014896
CREATE TRIGGER CheckMenuIngredients
ON Menu_Ingredients
AFTER INSERT, UPDATE
AS
BEGIN
    DECLARE @MealID INT
    DECLARE @IngredientCount INT

    -- Checking if any rows are inserted or updated
    IF (SELECT COUNT(*) FROM inserted) > 0
    BEGIN
        -- Loop through each Meal_ID in the inserted table
        DECLARE MealCursor CURSOR FOR
        SELECT DISTINCT Meal_ID
        FROM inserted
        
        OPEN MealCursor
        FETCH NEXT FROM MealCursor INTO @MealID
        
        -- Check ingredient count for each Meal_ID
        WHILE @@FETCH_STATUS = 0
        BEGIN
            SELECT @IngredientCount = COUNT(*)
            FROM Menu_Ingredients
            WHERE Meal_ID = @MealID
            
            -- Check if ingredient count exceeds 10
            IF @IngredientCount > 10
            BEGIN
                -- Rollback the transaction and raise an error
                RAISERROR ('A menu item cannot have more than 10 ingredients.', 16, 1)
                ROLLBACK TRANSACTION
                RETURN
            END
            
            FETCH NEXT FROM MealCursor INTO @MealID
        END
        
        CLOSE MealCursor
        DEALLOCATE MealCursor
    END
END;