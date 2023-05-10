CREATE TABLE [Taxi].[FinancialAdministrations]
(
	[Id] BIGINT NOT NULL IDENTITY (1,1),
	[AuthorityId] BIGINT NOT NULL,
    [LicenseeId] BIGINT NOT NULL,
    [LegalPersonId] BIGINT NOT NULL,
    [Deleted] BIT NOT NULL,
    CONSTRAINT [PK_FinancialAdministrations] PRIMARY KEY Clustered ([Id] ASC), 
    CONSTRAINT [FK_FinancialAdministrations_Licensees] FOREIGN KEY ([LicenseeId]) REFERENCES [Taxi].[Licensees] ([Id]),
    CONSTRAINT [FK_FinancialAdministrations_TaxiDrivers] FOREIGN KEY ([LegalPersonId]) REFERENCES [Taxi].[TaxiDrivers] ([Id])
)
