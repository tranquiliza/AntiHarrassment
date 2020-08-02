CREATE PROCEDURE [Core].[GetAuditedSuspensionsForChannel]
	@channelOfOrigin NVARCHAR(100),
	@earliestDate DATETIME2(0)
AS
BEGIN
	SELECT [Data] FROM [Core].[Suspension] 
	WHERE [ChannelOfOrigin] = @channelOfOrigin
	AND [Timestamp] > @earliestDate
	AND JSON_VALUE([Data], '$.Audited') = 'true' 
	AND JSON_VALUE([Data], '$.InvalidSuspension') = 'false'
END