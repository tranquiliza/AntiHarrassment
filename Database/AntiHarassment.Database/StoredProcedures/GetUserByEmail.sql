CREATE PROCEDURE [Core].[GetUserByEmail]
	@email NVARCHAR(1000)
AS
BEGIN
	SELECT [Data] FROM [Core].[Users] WHERE [Email] = @email
END