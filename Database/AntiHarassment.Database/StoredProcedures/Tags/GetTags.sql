CREATE PROCEDURE [Core].[GetTags]
AS
BEGIN
	SELECT [Data] FROM [Core].[Tags] WHERE [Deleted] = 0
END