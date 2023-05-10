CREATE TABLE [Taxi].[TaxiDriverAssociations]
(
	[Id] BIGINT IDENTITY (1, 1) NOT NULL,
	[AuthorityId] BIGINT NOT NULL,
	[Name] NVARCHAR(MAX),
	[FiscalCode] NVARCHAR(64),
	[Email] NVARCHAR(256),
	[TelephoneNumber] VARCHAR(16),
	[IsDeleted] BIT NOT NULL,
	CONSTRAINT [PK_TaxiDriverAssociations] PRIMARY KEY Clustered ([Id] ASC)
)