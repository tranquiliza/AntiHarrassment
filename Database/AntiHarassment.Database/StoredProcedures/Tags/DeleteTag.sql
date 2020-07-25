CREATE PROCEDURE [Core].[DeleteTag]
	@tagID UNIQUEIDENTIFIER
AS
BEGIN
	UPDATE [Core].[Tags] SET [Deleted] = 1 WHERE [TagId] = @tagID
END