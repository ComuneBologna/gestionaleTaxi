CREATE TABLE [Taxi].[LicenseesTaxiDrivers]
(
	[AuthorityId] BIGINT NOT NULL,
	[LicenseeId] BIGINT NOT NULL,
	[TaxiDriverId] BIGINT NOT NULL,
	[LicenseeType] TINYINT NOT NULL,
	[LicenseeStatus] TINYINT NOT NULL,
    [IsFinancialAdministration] BIT NOT NULL,
	[TaxiDriverType] TINYINT NOT NULL,
	CONSTRAINT [UK_LicenseesTaxiDrivers_AuthorityId_LicenseeId_TaxiDriverId] UNIQUE ([AuthorityId], [LicenseeId],[TaxiDriverId], [LicenseeType], [TaxiDriverType]),
	CONSTRAINT [FK_LicenseesTaxiDrivers_Licensees] FOREIGN KEY ([LicenseeId]) REFERENCES [Taxi].[Licensees] ([Id]),
	CONSTRAINT [FK_LicenseesTaxiDrivers_TaxiDrivers] FOREIGN KEY ([TaxiDriverId]) REFERENCES [Taxi].[TaxiDrivers] ([Id])
)
