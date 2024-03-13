-- Orders table
-- Students ID: 00013836, 00014725, 00014896

-- Get all
Go
Create or Alter Procedure udp_Order_Get_All
AS
Begin
	Select order_ID, OrderTime, DeliveryTime, PaymentStatus, Meal_ID, Amount, Total_Cost, Prepared_by 
	From Orders
    order by order_ID desc
End


-- Get By ID
go
Create or Alter Procedure udp_Order_Get_By_Id
    @OrderID Int
As
Begin
    Select order_ID, OrderTime, DeliveryTime, PaymentStatus, Meal_ID, Amount, Total_Cost, Prepared_by  
	From Orders Where order_ID = @OrderID
End

-- Create
go
Create or Alter Procedure udp_Order_Create
    @OrderTime DateTime,
    @DeliveryTime DateTime,
    @PaymentStatus BIT,
    @MealID Int,
    @Amount Int,
    @TotalCost Decimal(10, 2),
    @PreparedBy Int
As
Begin
    Insert Into Orders (OrderTime, DeliveryTime, PaymentStatus, Meal_ID, Amount, Total_Cost, Prepared_by)
    Values (@OrderTime, @DeliveryTime, @PaymentStatus, @MealID, @Amount, @TotalCost, @PreparedBy)
End

-- Update
go
Create or Alter Procedure udp_Order_Update
    @OrderID Int,
    @OrderTime DateTime,
    @DeliveryTime DateTime,
    @PaymentStatus BIT,
    @MealID Int,
    @Amount Int,
    @TotalCost Decimal(10, 2),
    @PreparedBy Int
As
Begin
    If Exists (Select 1 From Orders Where order_ID = @OrderID)
    Begin
        Update Orders
        Set OrderTime = @OrderTime,
            DeliveryTime = @DeliveryTime,
            PaymentStatus = @PaymentStatus,
            Meal_ID = @MealID,
            Amount = @Amount,
            Total_Cost = @TotalCost,
            Prepared_by = @PreparedBy
        Where order_ID = @OrderID
    End
    Else
    Begin
        Raiserror('Order with ID = @OrderID does not exist.', 16, 1)
    End
End

-- Delete
go
Create or Alter Procedure udp_Order_Delete
    @OrderID Int
As
Begin
    Delete From Orders Where order_ID = @OrderID
End 
