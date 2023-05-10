ALTER TABLE [Taxi].[RequestsRegisters] ADD [AuthorUserId] UNIQUEIDENTIFIER NULL

-- valorizzare le colonne con userId

ALTER TABLE [Taxi].[RequestsRegisters] ALTER COLUMN [AuthorUserId] UNIQUEIDENTIFIER NOT NULL