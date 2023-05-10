CREATE TABLE [Taxi].[Recipients]
(
	[Id] BIGINT IDENTITY (1, 1) NOT NULL,
	[AuthorityId] BIGINT NOT NULL,
	[LicenseeId] BIGINT NOT NULL,
	[Mail] NVARCHAR(256) NOT NULL,
	[Order] TINYINT NULL, 
    CONSTRAINT [PK_Recipients] PRIMARY KEY Clustered ([Id] ASC),
	CONSTRAINT [FK_Recipients_Licensees] FOREIGN KEY (LicenseeId) REFERENCES [Taxi].[Licensees](Id)
)