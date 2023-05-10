﻿CREATE TABLE [Taxi].[Licensees]
(
	[Id] BIGINT IDENTITY (1, 1) NOT NULL,
	[Number] NVARCHAR(64) NULL,
	[ReleaseDate] DATETIME NOT NULL,
	[EndDate] DATETIME NOT NULL,
	[AuthorityId] BIGINT NOT NULL,
	[TaxiDriverAssociationId] BIGINT NULL,
	[ShiftId] BIGINT NULL,
	[SubShiftId] BIGINT NULL,
	[ExpireActivityCause] NVARCHAR(MAX) NULL,
	[Type] TINYINT NOT NULL,
	[Status] TINYINT NOT NULL,
	[Note] VARCHAR(MAX) NULL,
	[Acronym] VARCHAR(64) NULL,
	[SysStartTime] DATETIME NULL,
	[LicenseesIssuingOfficeId] BIGINT NULL,
	[GarageAddress] VARCHAR(MAX) NULL,
	[IsFinancialAdministration] BIT NULL,
	[IsFamilyCollaboration] BIT NULL,
	[FolderId] UNIQUEIDENTIFIER NULL,
	CONSTRAINT [PK_Licensees] PRIMARY KEY Clustered ([Id] ASC),
	CONSTRAINT [FK_Licensee_TaxiDriverAssociations] FOREIGN KEY ([TaxiDriverAssociationId]) REFERENCES [Taxi].[TaxiDriverAssociations] ([Id]),
	CONSTRAINT [FK_Licensee_SubShifts] FOREIGN KEY ([SubShiftId]) REFERENCES [Taxi].[SubShifts] ([Id]),
	CONSTRAINT [FK_Licensee_Shifts] FOREIGN KEY ([ShiftId]) REFERENCES [Taxi].[Shifts] ([Id]),
	CONSTRAINT [FK_Licensees_LicenseesIssuingOffices] FOREIGN KEY ([LicenseesIssuingOfficeId]) REFERENCES [Taxi].[LicenseesIssuingOffices] ([Id]),
)