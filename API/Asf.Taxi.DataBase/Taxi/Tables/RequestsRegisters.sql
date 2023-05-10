CREATE TABLE [Taxi].[RequestsRegisters]
(
	[Id] BIGINT NOT NULL IDENTITY (1,1), 
    [AuthorityId] BIGINT NOT NULL,
    [LicenseeId] BIGINT NOT NULL,
    [TemplateId] BIGINT NOT NULL,
    [DMSDocumentId] VARCHAR(128) NOT NULL,
    [LastUpdate] DATETIME NOT NULL,
    [ExecutiveDigitalSignStatus] TINYINT NOT NULL, 
    [DigitalSignResult] VARCHAR(MAX) NULL,
    [AuthorUserId] uniqueidentifier NOT NULL,
    CONSTRAINT [Pk_RequestsRegisters] PRIMARY KEY CLUSTERED ([Id] ASC),
	CONSTRAINT [FK_RequestsRegisters_Licensees] FOREIGN KEY ([LicenseeId]) REFERENCES [Taxi].[Licensees] ([Id]),
	CONSTRAINT [FK_RequestsRegisters_Templates] FOREIGN KEY ([TemplateId]) REFERENCES [Taxi].[Templates] ([Id])
)