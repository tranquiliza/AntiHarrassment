CREATE PROCEDURE [Core].[GetSuspendedUsersForChannel]
	@channelOfOrigin NVARCHAR(100)
AS
BEGIN
	SELECT DISTINCT([Username]) FROM [Core].[Suspension] 
	WHERE JSON_VALUE([Data], '$.Audited') = 'true' 
	AND JSON_VALUE([Data], '$.InvalidSuspension') = 'false'
	AND [ChannelOfOrigin] = @channelOfOrigin
END