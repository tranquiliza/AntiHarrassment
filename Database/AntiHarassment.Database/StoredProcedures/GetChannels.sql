CREATE PROCEDURE [Core].[GetChannels]
AS
BEGIN
	SELECT [ChannelName], [ShouldListen] FROM [Channel]
END