﻿CREATE TABLE [dbo].[Layout]
(
	[Id] INT NOT NULL PRIMARY KEY IDENTITY,
	[VenueId] INT NOT NULL,
	[Description] NVARCHAR(120) NOT NULL,
)