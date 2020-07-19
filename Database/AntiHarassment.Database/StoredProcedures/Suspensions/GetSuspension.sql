CREATE PROCEDURE [Core].[GetSuspension]
	@suspensionId UNIQUEIDENTIFIER
AS
BEGIN
	SELECT TOP(1) [Data] FROM [Core].[Suspension] WHERE [SuspensionId] = @suspensionId
END