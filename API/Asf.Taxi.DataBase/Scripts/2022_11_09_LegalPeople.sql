ALTER TABLE Taxi.TaxiDrivers ADD Type TINYINT NULL;
update taxi.TaxiDrivers set Type = 1;
update taxi.TaxiDrivers set Type = 2 where len(FiscalCode) < 15;

ALTER TABLE Variation.Licensees ADD TaxiDriverPersonType TINYINT NULL;
update Variation.Licensees set TaxiDriverPersonType = 1;

ALTER TABLE Taxi.FinancialAdministrations ADD LegalPersonId BIGINT NULL;
ALTER TABLE Taxi.FinancialAdministrations ADD CONSTRAINT [FK_FinancialAdministrations_TaxiDrivers] FOREIGN KEY ([LegalPersonId]) REFERENCES [Taxi].[TaxiDrivers] ([Id]);

ALTER TABLE Taxi.FinancialAdministrations ALTER COLUMN LegalPersonId BIGINT NOT NULL;
ALTER TABLE Taxi.FinancialAdministrations DROP COLUMN [FinancialAdministrationCompany];
ALTER TABLE Taxi.FinancialAdministrations DROP COLUMN [FinancialAdministrationVatNumber];

ALTER TABLE Variation.Licensees DROP COLUMN [FinancialAdministrationCompany];
ALTER TABLE Variation.Licensees DROP COLUMN [FinancialAdministrationVatNumber];