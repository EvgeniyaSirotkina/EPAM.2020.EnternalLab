CREATE PROCEDURE [dbo].[CreateEvent]
	@name nvarchar(120),
	@description nvarchar(max),
	@layoutId int,
	@start datetime2,
	@end datetime2
AS

BEGIN
	BEGIN TRAN
		IF EXISTS (SELECT [Id] FROM [dbo].[Layout] WHERE [Id] = @layoutId)
			AND EXISTS (SELECT [Id] FROM [dbo].[Area] WHERE [LayoutId] = @layoutId)
			AND EXISTS (SELECT [Id] FROM [dbo].[Seat] WHERE [AreaId] IN (SELECT [Id] FROM [dbo].[Area] WHERE [LayoutId] = @layoutId))
		BEGIN
			--add event
			INSERT INTO [dbo].[Event] VALUES (@name,@description,@layoutId,@start,@end)

			DECLARE @eventId int

			--price in area & seat state
			DECLARE @eventAreaPrice decimal
			SET @eventAreaPrice = 0
			DECLARE @eventSeatState int
			SET @eventSeatState = 0

			--get new event Id 
			SELECT TOP(1) @eventId = [Id] 
			FROM [dbo].[Event] 
			WHERE [LayoutId] = @layoutId
				AND [Name] = @name
				AND [Description] = @description

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

			IF (@@error<>0)
				ROLLBACK
		END
		
	COMMIT
END