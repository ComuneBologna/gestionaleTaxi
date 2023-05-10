CREATE TABLE [Taxi].[Templates]
(
	[Id] BIGINT IDENTITY (1, 1) NOT NULL,
    [AuthorityId] BIGINT NOT NULL, 
	[Description] VARCHAR(512) NOT NULL,
	[FileName] VARCHAR(256) NOT NULL,
	[MimeType] VARCHAR(256) NULL,
    [FileId] VARCHAR(256) NOT NULL, 
    [LastUpdate] DATETIME NOT NULL,
    [Deleted] BIT NOT NULL,
    CONSTRAINT [PK_Templates] PRIMARY KEY Clustered ([Id] ASC)
)
