CREATE PROCEDURE [dbo].[UpdateEventArea]
	@eventAreaId int,
	@eventId int,
	@description nvarchar(200),
	@coordX int,
	@coordY int,
	@price decimal

AS
BEGIN
	BEGIN TRAN
		IF EXISTS (SELECT [Id] FROM [dbo].[EventArea] WHERE [Id] = @eventAreaId)
		BEGIN
			UPDATE [dbo].[EventArea] 
			SET [EventId] = @eventId,
				[Description] = @description,
				[CoordX] = @coordX,
				[CoordY] = @coordY,
				[Price] = @price
			WHERE [Id] = @eventAreaId

			IF (@@error<>0)
				ROLLBACK
		END
	COMMIT
END