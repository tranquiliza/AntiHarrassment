CREATE PROCEDURE [Core].[GetSuspensionsForUser]
	@username NVARCHAR(100)
AS
BEGIN
	SELECT [Data] FROM [Core].[Suspension] WHERE [Username] = @username
END