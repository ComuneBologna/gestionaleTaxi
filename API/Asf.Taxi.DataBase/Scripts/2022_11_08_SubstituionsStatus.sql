ALTER TABLE Taxi.TaxiDriverSubstitutions ADD Status TINYINT NULL;
UPDATE Taxi.TaxiDriverSubstitutions SET Status = 1;
ALTER TABLE Taxi.TaxiDriverSubstitutions ALTER COLUMN Status TINYINT NOT NULL;