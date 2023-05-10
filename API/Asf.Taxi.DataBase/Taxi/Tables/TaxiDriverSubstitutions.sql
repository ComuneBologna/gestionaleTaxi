CREATE TABLE [Taxi].[TaxiDriverSubstitutions]
(
	[Id] BIGINT IDENTITY (1, 1) NOT NULL,
	[AuthorityId] BIGINT NOT NULL,
	[LicenseeId] BIGINT NOT NULL,
	[DriverToId] BIGINT NOT NULL,
	[StartDate] DATETIME NOT NULL,
	[EndDate] DATETIME NOT NULL,
	[Note] NVARCHAR(MAX) NULL,
	[Status] TINYINT NOT NULL, 
    CONSTRAINT [PK_TaxiDriverSubstitutions] PRIMARY KEY Clustered ([Id] ASC),
	CONSTRAINT [FK_TaxiDriverSubstitutions_Driver_To] FOREIGN KEY ([DriverToId]) REFERENCES [Taxi].[TaxiDrivers] ([Id])
)