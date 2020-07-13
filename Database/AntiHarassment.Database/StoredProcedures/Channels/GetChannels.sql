CREATE PROCEDURE [Core].[GetChannels]
AS
BEGIN
	SELECT [Data] FROM [Core].[Channel]
END