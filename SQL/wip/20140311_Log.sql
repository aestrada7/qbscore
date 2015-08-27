/* -- */

SET ANSI_NULLS ON
SET QUOTED_IDENTIFIER ON
IF NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[QBSLog]') AND type in (N'U'))
BEGIN
CREATE TABLE dbo.QBSLog
	(
    IdLog int NOT NULL IDENTITY (1, 1),
    Name varchar(400) NULL,
    Content varchar(MAX) NULL,
    CONSTRAINT [PK_QBSLog] PRIMARY KEY CLUSTERED 
    (
      IdLog
    ) WITH (STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF)
  )
END
GO

/* -- */