CREATE PROCEDURE [Core].[GetUniqueUsersFromSuspensionsForSystem]
AS
BEGIN
	SELECT DISTINCT([Username]) FROM [Core].[Suspension]
END