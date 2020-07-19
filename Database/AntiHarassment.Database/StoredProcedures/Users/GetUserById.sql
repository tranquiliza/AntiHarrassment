﻿CREATE PROCEDURE [Core].[GetUserById]
	@userId UNIQUEIDENTIFIER
AS
BEGIN
	SELECT TOP(1) [Data] FROM [Core].[Users] WHERE [UserId] = @userId
END