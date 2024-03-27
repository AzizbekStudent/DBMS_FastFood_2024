-- Students ID: 00013836   00014725   00014896
-- Filter Employee
CREATE INDEX Ind_Employee_ID_Filter ON Employee (employee_ID); 
CREATE INDEX Ind_Employee_FName ON Employee (FName);
CREATE INDEX Ind_Employee_LName ON Employee (LName);

-- Import from xXml and Json
CREATE INDEX Ind_Menu_Meal_ID ON Menu (meal_ID);
CREATE INDEX Ind_Menu_Meal_title ON Menu (meal_title);
CREATE INDEX Ind_Orders_Prepared_By ON Orders (Prepared_by);

/*
	In order to optimize the sql queries more than 16 indexes were created
	but only 6 of them were useful for query optimization

	First of all Indexes related to employee class
	Ind_Employee_ID_Filter
	This index was useful when user tries to filter employee records withour specifying 
	FName, LName, HireDate, Sorting column and showed high performance among other indexes

	Ind_Employee_FName and Ind_Employee_LName indexes where utilized and they optimized the query when
	filter procedure was executed with specified parameters accordingly

	Ind_Menu_Meal_ID and
	These indexes were used by procedures that import from json and xml file also for exporting purposes
	and these indexes significantly increased performance of the procedures

	Ind_Menu_Meal_title and Ind_Orders_Prepared_By
	at some point SSMS decided they are good choice and utilized them in xml import stored procedure
*/