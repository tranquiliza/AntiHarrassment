CREATE PROCEDURE [Core].[GetSuspensions]
	@earliestDate DATETIME2(0)
AS
BEGIN
	SELECT [Data] FROM [Core].[Suspension]
	WHERE [Timestamp] > @earliestDate
END