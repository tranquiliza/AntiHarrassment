CREATE PROCEDURE [Core].[GetUniqueChattersForSystem]
AS
BEGIN
	SELECT DISTINCT([Username]) FROM [Core].[ChatMessage]
END