CREATE PROCEDURE [Core].[InsertUpdateUser]
	@userId UNIQUEIDENTIFIER,
	@twitchUsername NVARCHAR(1000),
	@email NVARCHAR(1000) NULL,
	@data NVARCHAR(MAX)
AS
BEGIN
	IF NOT EXISTS (SELECT * FROM [Core].[Users] WHERE [UserId] = @userId)
		INSERT INTO [Core].[Users]([UserId], [TwitchUsername], [Email], [Data])
		VALUES(@userId, @twitchUsername, @email, @data)
	ELSE
		UPDATE [Core].[Users]
		SET 
		[TwitchUsername] = @twitchUsername,
		[Email] = @email,
		[Data] = @data
		WHERE [UserId] = @userId
END
