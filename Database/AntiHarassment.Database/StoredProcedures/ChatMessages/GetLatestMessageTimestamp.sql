CREATE PROCEDURE [Core].[GetLatestMessageTimestamp]
AS
BEGIN
	SELECT TOP(1) [Timestamp] FROM [Core].[ChatMessage] ORDER BY [Timestamp] DESC
END