CREATE TABLE [Protocol].[Emails]
(
	[Id] BIGINT IDENTITY(1,1) NOT NULL,
	[AuthorityId] BIGINT NOT NULL,
    [Email] VARCHAR(512) NOT NULL, 
    [Description] VARCHAR(1024) NULL,
    [Active] BIT NOT NULL,
    CONSTRAINT [PK_Emails] PRIMARY KEY CLUSTERED ([Id] ASC)
)
