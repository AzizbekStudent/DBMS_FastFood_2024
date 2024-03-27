-- Students ID: 00013836   00014725   00014896
use DBMS_FastFood
Go

-- Creating login
Create login db_CW_FastFood_Login with Password = 'DBMS_FastFood', Default_Database = DBMS_FastFood

-- Drop login db_CW_FastFood_Login

-- Creating users
-- User for application usual activities
-- Name will be  (Fast_Sardor)
go
Create user Fast_Sardor for login db_CW_FastFood_Login
-- Deleting User
-- Drop user Fast_Sardor
Revoke Alter, Control, Take Ownership on Menu to Fast_Sardor
Revoke Alter, Control, Take Ownership on Employee to Fast_Sardor
Revoke Alter, Control, Take Ownership on Ingredients to Fast_Sardor
Revoke Alter, Control, Take Ownership on Menu_Ingredients to Fast_Sardor
Revoke Alter, Control, Take Ownership on Orders to Fast_Sardor
Deny Alter, Control, Take Ownership On Schema::dbo To Fast_Sardor;


Grant select, insert, update, delete on Menu to Fast_Sardor
Grant select, insert, update, delete on Employee to Fast_Sardor
Grant select, insert, update, delete on Ingredients to Fast_Sardor
Grant select, insert, update, delete on Menu_Ingredients to Fast_Sardor
Grant select, insert, update, delete on Orders to Fast_Sardor

------------------------------------------------------------------------------------------
-- User for data import
-- Name will be (Import_Sardor)
Create login db_CW_FastFood_Import with Password = 'DBMS_FastFood', Default_Database = DBMS_FastFood
Create user Import_Sardor for login db_CW_FastFood_Import
-- Deleting User
-- Drop user Import_Sardor

-- Lets first revoke all permissions then assign only necessary
Revoke select, insert, update, delete, Alter, Control, Take Ownership on Menu to Import_Sardor
Revoke select, insert, update, delete, Alter, Control, Take Ownership on Employee to Import_Sardor
Revoke select, insert, update, delete, Alter, Control, Take Ownership on Ingredients to Import_Sardor
Revoke select, insert, update, delete, Alter, Control, Take Ownership on Menu_Ingredients to Import_Sardor
Revoke select, insert, update, delete, Alter, Control, Take Ownership on Orders to Import_Sardor
Deny Alter, Control, Take Ownership On Schema::dbo To Import_Sardor;

-- Assigning only necessary permissions
Grant select, insert on Menu to Import_Sardor
Grant select, insert on Employee to Import_Sardor
Grant select, insert on Ingredients to Import_Sardor
Grant select, insert on Menu_Ingredients to Import_Sardor
Grant select, insert on Orders to Import_Sardor

------------------------------------------------------------------------------------------
-- User for data export
-- Name will be (Export_Sardor)
Create login db_CW_FastFood_Export with Password = 'DBMS_FastFood', Default_Database = DBMS_FastFood
Create user Export_Sardor for login db_CW_FastFood_Export
-- Deleting User
-- Drop user Export_Sardor

-- Lets first revoke all permissions then assign only necessary
Revoke select, insert, update, delete, Alter, Control, Take Ownership on Menu to Export_Sardor
Revoke select, insert, update, delete, Alter, Control, Take Ownership on Employee to Export_Sardor
Revoke select, insert, update, delete, Alter, Control, Take Ownership on Ingredients to Export_Sardor
Revoke select, insert, update, delete, Alter, Control, Take Ownership on Menu_Ingredients to Export_Sardor
Revoke select, insert, update, delete, Alter, Control, Take Ownership on Orders to Export_Sardor
Deny Alter, Control, Take Ownership On Schema::dbo To Export_Sardor;

-- Assigning only necessary permissions
Grant select on Menu to Export_Sardor
Grant select on Employee to Export_Sardor
Grant select on Ingredients to Export_Sardor
Grant select on Menu_Ingredients to Export_Sardor
Grant select on Orders to Export_Sardor
