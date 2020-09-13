CREATE PROCEDURE [Core].[GetUnconfirmedSourcesSuspensions]
AS
BEGIN
	SELECT [Data] FROM [Core].[Suspension]
	WHERE [UnconfirmedSource] = 1
END