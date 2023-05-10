CREATE TABLE [Taxi].[SubShifts]
(
	[Id] BIGINT IDENTITY (1, 1) NOT NULL,
	[AuthorityId] BIGINT NOT NULL,
	[ShiftId] BIGINT NOT NULL,
	[Name] NVARCHAR(256) NOT NULL,
	[RestDay] INT NULL,
	[IsEnabled] BIT NOT NULL,
	CONSTRAINT [PK_SubShifts_Id] PRIMARY KEY Clustered ([Id] ASC),
	CONSTRAINT [FK_Subshifts_Shifts] FOREIGN KEY ([ShiftId]) REFERENCES [Taxi].[Shifts] ([Id])
)