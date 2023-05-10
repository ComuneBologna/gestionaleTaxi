--CREATE TABLE [Security].[Users]
--(
--	[UserId] UNIQUEIDENTIFIER NOT NULL,
--	[AuthorityId] BIGINT NOT NULL,
--	[DriverId] BIGINT NULL,
--	[RoleCode] nvarchar(64) NOT NULL,
--	CONSTRAINT [PK_Users] PRIMARY KEY CLUSTERED ([UserId] ASC, [AuthorityId] ASC),
--	INDEX [uidx_AuthorityId_UserId] UNIQUE NONCLUSTERED ([AuthorityId] ASC, [UserId] ASC)
--)
CREATE TABLE [Security].[Users]
(
	[SmartPAUserId] UNIQUEIDENTIFIER NOT NULL,
	[TenantId] UNIQUEIDENTIFIER NOT NULL,
	[FirstName] VARCHAR(128)  NOT NULL,
	[LastName] VARCHAR(128)  NOT NULL,
	[Email] VARCHAR(256) NOT NULL,
	[PhoneNumber] VARCHAR(16) NULL,
	[FiscalCode] VARCHAR(16) NOT NULL,
	[AvatarPath] VARCHAR(1024) NULL,
	[FullTextSearch] AS ([FirstName] + ' ' + [LastName] + ' ' + [Email]),
	CONSTRAINT [PK_Users] PRIMARY KEY CLUSTERED ([SmartPAUserId] ASC),
	INDEX [IX_Users] NONCLUSTERED ([SmartPAUserId])
)