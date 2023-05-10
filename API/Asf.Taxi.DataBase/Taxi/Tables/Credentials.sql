﻿CREATE TABLE [Taxi].[Credentials]
(
	[Id] BIGINT IDENTITY (1, 1) NOT NULL,
	[AuthorityId] BIGINT NOT NULL,
	[UserId] UNIQUEIDENTIFIER NOT NULL,
	[Username] VARCHAR(256) NOT NULL,
	[Password] VARCHAR(512) NOT NULL,
	[FirstName] VARCHAR(128) NOT NULL,
	[LastName] VARCHAR(128) NOT NULL,
    CONSTRAINT [PK_Credentials] PRIMARY KEY Clustered ([Id] ASC)
)