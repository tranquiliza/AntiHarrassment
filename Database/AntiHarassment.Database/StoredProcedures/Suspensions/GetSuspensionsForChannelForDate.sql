CREATE PROCEDURE [Core].[GetSuspensionsForChannelForDate]
	@channelOfOrigin NVARCHAR(100),
	@startDate DATETIME2,
	@endDate DATETIME2
AS
BEGIN
	SELECT [Data] FROM [Core].[Suspension] 
	WHERE 
		[ChannelOfOrigin] = @channelOfOrigin 
		AND [Timestamp] > @startDate
		AND [Timestamp] < @endDate
END