CREATE TABLE [Taxi].[LicenseesIssuingOffices]
(
	[Id] BIGINT IDENTITY (1,1) NOT NULL, 
    [AuthorityId] BIGINT NOT NULL, 
    [Description] VARCHAR(128) NOT NULL, 
    [Deleted] BIT NOT NULL, 
    CONSTRAINT [PK_LicenseesIssuingOffices] PRIMARY KEY Clustered ([Id] ASC)
)
