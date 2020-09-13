CREATE PROCEDURE [Core].[GetAllUsers]
AS
BEGIN
	SELECT [Data] FROM [Core].[Users]
END