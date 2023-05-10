CREATE TABLE [Protocol].[ProcessCodes]
(
	[Id] BIGINT IDENTITY (1, 1) NOT NULL,
	[AuthorityId] BIGINT NOT NULL,
	[Code] VARCHAR(64) NOT NULL,
	[Description] VARCHAR(1024) NOT NULL,
	[Territorial] BIT NOT NULL,
	[StartDate] DATETIME NOT NULL,
	[EndDate] DATETIME NULL,
	[FullTextSearch] AS ([Code] + ' ' + [Description])
    CONSTRAINT [PK_ProcessCodes] PRIMARY KEY Clustered ([Id] ASC)
)