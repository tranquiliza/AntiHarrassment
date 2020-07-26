﻿CREATE PROCEDURE [Core].[GetTag]
	@tagId UNIQUEIDENTIFIER
AS
BEGIN
	SELECT TOP(1) [Data] FROM [Core].[Tags] WHERE [TagId] = @tagId AND [Deleted] = 0
END
