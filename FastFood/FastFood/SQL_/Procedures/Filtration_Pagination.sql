﻿CREATE OR ALTER PROCEDURE udp_Filter_Employee
    @FName nvarchar(255),
    @LName nvarchar(255),
    @HireDate DateTime,
    @SortField nvarchar(200) = 'employee_ID',
    @SortAsc bit = 1,
    @PageNumber int = 1,
    @PageSize int = 3
AS
BEGIN
    DECLARE @SortDirection nvarchar(20) = 'ASC';
    IF @SortAsc = 0
        SET @SortDirection = 'DESC';

    DECLARE @ParameterDefinition nvarchar(2000) = '
        @FName nvarchar(255),
        @LName nvarchar(255),
        @HireDate datetime,
        @PageNumber int,
        @PageSize int';

    DECLARE @SqlStatement nvarchar(max) =
        'SELECT employee_ID, FName, LName, Telephone, Job, Age, Salary, HireDate, Image, FullTime,
            COUNT(*) OVER () AS TotalRows
        FROM Employee
        WHERE (@FName IS NULL OR FName LIKE CONCAT(@FName, ''%''))
            AND (@LName IS NULL OR LName LIKE CONCAT(@LName, ''%''))
            AND (@HireDate IS NULL OR HireDate >= @HireDate)
        ORDER BY ' + QUOTENAME(@SortField) + ' ' + @SortDirection + '
        OFFSET (@PageNumber - 1) * @PageSize ROWS
        FETCH NEXT @PageSize ROWS ONLY';

    EXEC sp_executesql @SqlStatement, @ParameterDefinition,
        @FName = @FName,
        @LName = @LName,
        @HireDate = @HireDate,
        @PageNumber = @PageNumber,
        @PageSize = @PageSize
END;
