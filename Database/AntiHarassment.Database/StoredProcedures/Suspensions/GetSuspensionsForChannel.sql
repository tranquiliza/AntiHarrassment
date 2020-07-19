CREATE PROCEDURE [Core].[GetSuspensionsForChannel]
	@channelOfOrigin NVARCHAR(100)
AS
BEGIN
	SELECT [Data] FROM [Core].[Suspension] WHERE [ChannelOfOrigin] = @channelOfOrigin
END