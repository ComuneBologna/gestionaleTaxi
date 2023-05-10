CREATE TABLE [Taxi].[Vehicles]
(
	[Id] BIGINT IDENTITY (1, 1) NOT NULL,
	[Model] NVARCHAR(256) NOT NULL,
	[LicensePlate] VARCHAR(16) NOT NULL,
	[RegistrationDate] DATETIME NOT NULL,
	[InUseSince] DATETIME NOT NULL,
	[AuthorityId] BIGINT NOT NULL,
	[LicenseeId] BIGINT NOT NULL,
	[CarFuelType] TINYINT NOT NULL,
	[SysStartTime] DATETIME NULL,
	CONSTRAINT [PK_VehicleId] PRIMARY KEY Clustered ([Id] ASC),
	CONSTRAINT [FK_Vehicle_Licensees] FOREIGN KEY ([LicenseeId]) REFERENCES [Taxi].[Licensees] ([Id])
)