CREATE PROCEDURE [dbo].[DeleteEventSeat]
	@eventSeatId int
AS
BEGIN
	BEGIN TRAN
		--delete from EventSeat
		IF EXISTS (SELECT [Id] FROM [dbo].[EventSeat] WHERE [Id] = @eventSeatId)
		BEGIN
			DELETE FROM [dbo].[EventSeat] 
			WHERE [Id] = @eventSeatId
		END

		IF (@@error<>0)
			ROLLBACK
	COMMIT
END