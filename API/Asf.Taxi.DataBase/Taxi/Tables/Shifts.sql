CREATE TABLE [Taxi].[Shifts]
(
	[Id] BIGINT IDENTITY (1, 1) NOT NULL,
	[AuthorityId] BIGINT NOT NULL,
	[Name] VARCHAR(256) NOT NULL,
	[DurationInHour] TINYINT NOT NULL,
	[IsHandicapMode] BIT NOT NULL,
	[HandicapBeforeInHour] TINYINT NULL,
	[HandicapAfterInHour] TINYINT NULL,
	[BreakInHours] TINYINT NOT NULL,
	[BreakThresholdActivationInHour] TINYINT NOT NULL,
	[RestDayFrequencyInDays] TINYINT NOT NULL,
	[IsEnabled] BIT NOT NULL,
	CONSTRAINT [PK_Shifts_Id] PRIMARY KEY Clustered ([Id] ASC)
)