CREATE PROCEDURE [dbo].[DeleteEvent]
	@eventId int
AS
BEGIN
	BEGIN TRAN
		--delete from EventSeat
		DELETE FROM [dbo].[EventSeat]
		WHERE [Id] IN (
			SELECT es.[Id] FROM [dbo].[EventSeat] es
			INNER JOIN [dbo].[EventArea] ea
			ON es.[EventAreaId] = ea.[Id] AND ea.[EventId] = @eventId)

		--delete from EventArea
		DELETE FROM [dbo].[EventArea]
		WHERE [EventId] = @eventId
		
		--delete from Event
		DELETE FROM [dbo].[Event] 
		WHERE [Id] = @eventId

		IF (@@error<>0)
			ROLLBACK
	COMMIT
END