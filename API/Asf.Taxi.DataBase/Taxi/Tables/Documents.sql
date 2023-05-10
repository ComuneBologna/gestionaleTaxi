CREATE TABLE [Taxi].[Documents]
(
	[Id] BIGINT IDENTITY (1, 1) NOT NULL,
    [AuthorityId] BIGINT NOT NULL,
    [Type] TINYINT NOT NULL, 
    [Number] VARCHAR(64) NOT NULL, 
	[ReleasedBy] NVARCHAR(256) NOT NULL, 
	[ValidityDate] DATETIME NOT NULL, 
    [TaxiDriverId] BIGINT NOT NULL, 
    CONSTRAINT [PK_Documents] PRIMARY KEY Clustered ([Id] ASC),
    constraint [FK_Documents_TaxiDrivers] FOREIGN KEY ([TaxiDriverId]) REFERENCES [Taxi].[TaxiDrivers] ([Id])
)
