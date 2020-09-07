CREATE PROCEDURE [Core].[GetDatesForUnauditedSuspensionsForChannel]
	@channelOfOrigin NVARCHAR(100)
AS
BEGIN
	SELECT DISTINCT(CONVERT(DATE, [Timestamp])) as DateWithUnauditedSuspensions
		FROM [Suspension]
		WHERE JSON_VALUE([Data], '$.Audited') = 'false'
		AND ChannelOfOrigin = @channelOfOrigin
END