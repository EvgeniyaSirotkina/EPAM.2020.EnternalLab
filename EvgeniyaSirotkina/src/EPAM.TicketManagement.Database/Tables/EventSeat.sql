﻿CREATE TABLE [dbo].[EventSeat]
(
	[Id] INT NOT NULL PRIMARY KEY IDENTITY,
	[EventAreaId] INT NOT NULL,
	[Row] INT NOT NULL,
	[Number] INT NOT NULL,
	[State] INT NOT NULL
)