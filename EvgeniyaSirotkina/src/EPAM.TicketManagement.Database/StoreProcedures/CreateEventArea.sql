CREATE PROCEDURE [dbo].[CreateEventArea]
	@eventId int,
	@description nvarchar(200),
	@coordX int,
	@coordY int,
	@price decimal
AS

BEGIN
	BEGIN TRAN
		IF NOT EXISTS (SELECT [Id] FROM [dbo].[EventArea]
						WHERE [EventId] = @eventId
						AND [Description] = @description)
		BEGIN
			INSERT INTO [dbo].[EventArea] VALUES (@eventId,@description,@coordX,@coordY,@price)
		END

		IF (@@error<>0)
			ROLLBACK
	COMMIT
END