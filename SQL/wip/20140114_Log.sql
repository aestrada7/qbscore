/* -- */

SET ANSI_NULLS ON
SET QUOTED_IDENTIFIER ON
IF NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[SystemLog]') AND type in (N'U'))
BEGIN
CREATE TABLE dbo.SystemLog
	(
    IdLog int NOT NULL IDENTITY (1, 1),
    IdUser int NOT NULL,
    Kind int NOT NULL,
    IdModule int NULL,
    IdRelated int NULL,
    Description varchar(MAX) NULL,
    TimeStamp DateTime NOT NULL,
    CONSTRAINT [PK_SystemLog] PRIMARY KEY CLUSTERED 
    (
      IdLog
    ) WITH (STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF)
  )
END
GO

/* -- */