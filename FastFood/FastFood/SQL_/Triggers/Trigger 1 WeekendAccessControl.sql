﻿-- Create trigger to disallow operations on Menu table during weekends
create or alter TRIGGER WeekendAccessControl_Menu
ON Menu
FOR INSERT, UPDATE, DELETE
AS
BEGIN
    -- Check if it's Saturday (Day 7) or Sunday (Day 1)
    IF DATEPART(dw, GETDATE()) IN (1, 7)
    BEGIN
        RAISERROR('Operations on the Menu table are not allowed during weekends.', 16, 1)
        ROLLBACK
    END
END;