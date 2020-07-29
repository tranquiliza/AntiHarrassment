CREATE PROCEDURE [Core].[GetUniqueUsersFromSuspensions]
	@channelOfOrigin NVARCHAR(100)
AS
BEGIN
	SELECT DISTINCT([Username]) FROM [Core].[Suspension] WHERE [ChannelOfOrigin] = @channelOfOrigin
END