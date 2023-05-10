CREATE TABLE [Security].[PermissionsForUsers]
(
	[AuthorityUserId] BIGINT NOT NULL,
	[PermissionCode] VARCHAR(512) NOT NULL,
	CONSTRAINT [PK_PermissionsForUsers] PRIMARY KEY CLUSTERED ([AuthorityUserId] ASC),
	CONSTRAINT [UK_PermissionsForUsers] UNIQUE ([AuthorityUserId], [PermissionCode]),
	CONSTRAINT [FK_PermissionsForUsers_Users] FOREIGN KEY ([AuthorityUserId]) REFERENCES [Security].[AuthoritiesForUsers] ([Id]),
	INDEX [IX_PermissionsForUsers] NONCLUSTERED ([AuthorityUserId])
)