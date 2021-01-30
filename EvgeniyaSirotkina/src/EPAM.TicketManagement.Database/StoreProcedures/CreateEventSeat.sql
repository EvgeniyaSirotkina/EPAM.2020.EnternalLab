CREATE PROCEDURE [dbo].[CreateEventSeat]
	@eventAreaId int,
	@row int,
	@number int,
	@state int
AS

BEGIN
	BEGIN TRAN
		IF NOT EXISTS (SELECT [Id] FROM [dbo].[EventSeat]
						WHERE [EventAreaId] = @eventAreaId
						AND [Row] = @row
						AND [Number] = @number)
		BEGIN
			INSERT INTO [dbo].[EventSeat] VALUES (@eventAreaId,@row,@number,@state)
		END

		IF (@@error<>0)
			ROLLBACK
	COMMIT
END
