CREATE TABLE [Taxi].[CalendarShifts]
(
	[Id] BIGINT IDENTITY (1, 1) NOT NULL,
    [Date] DATE NOT NULL,
    [IsRestDay] BIT NOT NULL DEFAULT 0,
    [IsHoliday] BIT NOT NULL DEFAULT 0,
    [IsSwitch] BIT NOT NULL DEFAULT 0,
    [IsSickness] BIT NOT NULL DEFAULT 0,
    [IsVehicleStop] BIT NOT NULL DEFAULT 0,
	[IsAllarmAccepted] BIT NOT NULL DEFAULT 0,
    [LicenseeId] BIGINT NOT NULL,
    [DateSwitched] DATE NULL,
    [AuthorityId] BIGINT NOT NULL,
    CONSTRAINT [PK_CalendarId] PRIMARY KEY Clustered ([Id] ASC),
    CONSTRAINT [FK_Calendar_Licensees] FOREIGN KEY ([LicenseeId]) REFERENCES [Taxi].[Licensees] ([Id]),
)
