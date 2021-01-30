CREATE PROCEDURE [dbo].[DeleteEventArea]
	@eventAreaId int
AS
BEGIN
	BEGIN TRAN
		--delete from EventSeat
		IF EXISTS (SELECT [Id] FROM [dbo].[EventSeat] WHERE [EventAreaId] = @eventAreaId)
		BEGIN
			DELETE FROM [dbo].[EventSeat] 
			WHERE [EventAreaId] = @eventAreaId
		END

		--delete from EventArea
		IF EXISTS (SELECT [Id] FROM [dbo].[EventArea] WHERE [Id] = @eventAreaId)
		BEGIN
			DELETE FROM [dbo].[EventArea] 
			WHERE [Id] = @eventAreaId
		END

		IF (@@error<>0)
			ROLLBACK
	COMMIT
END