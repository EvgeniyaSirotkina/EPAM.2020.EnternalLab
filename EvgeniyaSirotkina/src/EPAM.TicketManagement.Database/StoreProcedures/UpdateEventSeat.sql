CREATE PROCEDURE [dbo].[UpdateEventSeat]
	@eventSeatId int,
	@eventAreaId int,
	@row int,
	@number int,
	@state int

AS
BEGIN
	BEGIN TRAN
		IF EXISTS (SELECT [Id] FROM [dbo].[EventSeat] WHERE [Id] = @eventSeatId)
		BEGIN
			UPDATE [dbo].[EventSeat] 
			SET [EventAreaId] = @eventAreaId,
				[Row] = @row,
				[Number] = @number,
				[State] = @state
			WHERE [Id] = @eventSeatId

			IF (@@error<>0)
				ROLLBACK
		END
	COMMIT
END