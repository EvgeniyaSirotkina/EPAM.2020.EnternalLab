CREATE PROCEDURE [dbo].[UpdateEvent]
	@eventId int,
	@name nvarchar(120),
	@description nvarchar(max),
	@layoutId int,
	@start datetime2,
	@end datetime2
AS
BEGIN
	BEGIN TRAN
		IF EXISTS (SELECT [Id] FROM [dbo].[Event] WHERE [Id] = @eventId) 
			AND EXISTS (SELECT [Id] FROM [dbo].[Layout] WHERE [Id] = @layoutId)
			AND EXISTS (SELECT [Id] FROM [dbo].[Area] WHERE [LayoutId] = @layoutId)
			AND EXISTS (SELECT [Id] FROM [dbo].[Seat] WHERE [AreaId] IN (SELECT [Id] FROM [dbo].[Area] WHERE [LayoutId] = @layoutId))
		BEGIN
			DECLARE @layoutIdFromEvent int

			--get new event Id 
			SELECT TOP(1) @layoutIdFromEvent = @layoutId
			FROM [dbo].[Event] 
			WHERE [Id] = @eventId

			IF (@layoutIdFromEvent <> @layoutId)
			BEGIN
				--delete old areas with seats
				--delete from EventSeat
				DELETE FROM [dbo].[EventSeat]
				WHERE [Id] IN (
					SELECT es.[Id] FROM [dbo].[EventSeat] es
					INNER JOIN [dbo].[EventArea] ea
					ON es.[EventAreaId] = ea.[Id] AND ea.[EventId] = @eventId)

				--delete from EventArea
				DELETE FROM [dbo].[EventArea]
				WHERE [EventId] = @eventId

				--copy from Area and Seat into EventArea and EventSeat
				--price in area & seat state
				DECLARE @eventAreaPrice decimal
				SET @eventAreaPrice = 0
				DECLARE @eventSeatState int
				SET @eventSeatState = 0

				--copy from Area and Seat into EventArea and EventSeat
				INSERT INTO [dbo].[EventArea] ([EventId],[Description],[CoordX],[CoordY],[Price])
				SELECT @eventId, a.[Description], a.[CoordX], a.[CoordY], @eventAreaPrice
				FROM [dbo].[Area] a
				INNER JOIN [dbo].[Seat] s
				ON s.[AreaId] = a.[Id]
				WHERE a.[LayoutId] = @layoutId
				GROUP BY a.[Description], a.[CoordX], a.[CoordY]

				INSERT INTO [dbo].[EventSeat] ([EventAreaId],[Row],[Number],[State])
				SELECT ea.[Id], s.[Row], s.[Number], @eventSeatState
				FROM [dbo].[EventArea] ea, [dbo].[Area] a
				INNER JOIN [dbo].[Seat] s
				ON s.[AreaId] = a.[Id]
				WHERE a.[Description] = ea.[Description]
				ORDER BY s.[Row], s.[Number]
			END

			UPDATE [dbo].[Event] 
			SET [Name] = @name,
				[Description] = @description,
				[LayoutId] = @layoutId,
				[EventStart] = @start,
				[EventEnd] = @end
			WHERE [Id] = @eventId

			IF (@@error<>0)
				ROLLBACK
		END
	COMMIT
END
