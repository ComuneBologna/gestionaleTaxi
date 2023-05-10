CREATE TABLE [Security].[AuthoritiesForUsers]
(
	[Id] BIGINT IDENTITY(1,1) NOT NULL,
	[SmartPAUserId] UNIQUEIDENTIFIER NOT NULL,
	[AuthorityId] BIGINT NOT NULL,
	[IsEnabled] BIT NOT NULL,
	[IsDefault] BIT NOT NULL,
	[DriverId] BIGINT NULL,
	[Discriminator] NVARCHAR(50) NULL,
	CONSTRAINT [PK_AuthoritiesForUsers] PRIMARY KEY CLUSTERED ([Id] ASC),
	CONSTRAINT [UK_AuthoritiesForUsers] UNIQUE ([AuthorityId], [SmartPAUserId]),
	CONSTRAINT [FK_AuthoritiesForUsers_Users] FOREIGN KEY([SmartPAUserId]) REFERENCES [Security].[Users] ([SmartPAUserId])
)