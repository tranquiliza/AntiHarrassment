CREATE PROCEDURE [Core].[InsertUpdateUser]
	@userId UNIQUEIDENTIFIER,
	@username NVARCHAR(1000),
	@email NVARCHAR(1000),
	@data NVARCHAR(MAX)
AS
BEGIN
	IF NOT EXISTS (SELECT * FROM [Core].[Users] WHERE [UserId] = @userId)
		INSERT INTO [Core].[Users]([UserId], [Username], [Email], [Data])
		VALUES(@userId, @username, @email, @data)
	ELSE
		UPDATE [Core].[Users]
		SET 
		[Username] = @username,
		[Email] = @email,
		[Data] = @data
		WHERE [UserId] = @userId
END
