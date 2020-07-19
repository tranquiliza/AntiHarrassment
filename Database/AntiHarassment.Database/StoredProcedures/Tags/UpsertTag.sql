CREATE PROCEDURE [Core].[UpsertTag]
	@tagId UNIQUEIDENTIFIER,
	@tagName NVARCHAR(50),
	@data NVARCHAR(MAX)
AS
BEGIN
	IF NOT EXISTS(SELECT * FROM [Core].[Tags] WHERE [TagId] = @tagId)
		INSERT INTO [Core].[Tags]([TagId], [TagName], [Data])
		VALUES (@tagId, @tagName, @data)
	ELSE
		UPDATE [Core].[Tags]
		SET 
		[TagName] = @tagName,
		[Data] = @data
		WHERE [TagId] = @tagId
END