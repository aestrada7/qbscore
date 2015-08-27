SET ANSI_NULLS ON
SET QUOTED_IDENTIFIER ON
IF NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[Users]') AND type in (N'U'))
BEGIN
CREATE TABLE dbo.Users
	(
    IdUser int NOT NULL IDENTITY (1, 1),
    IdExternal varchar(50) NULL,
    Username varchar(MAX) NOT NULL,
    Password varchar(MAX) NOT NULL,
    CONSTRAINT [PK_Users] PRIMARY KEY CLUSTERED 
    (
      IdUser
    ) WITH (STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF)
  )
END
GO

/* -- */

SET ANSI_NULLS ON
SET QUOTED_IDENTIFIER ON
IF NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[UserData]') AND type in (N'U'))
BEGIN
CREATE TABLE dbo.UserData
	(
    IdUser int NOT NULL,
    IdData int NOT NULL,
    Value varchar(MAX) NULL,
    CONSTRAINT [PK_UserData] PRIMARY KEY CLUSTERED 
    (
      IdUser, IdData
    ) WITH (STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF)
  )
END
GO

/* -- */

SET ANSI_NULLS ON
SET QUOTED_IDENTIFIER ON
IF NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[DataDesc]') AND type in (N'U'))
BEGIN
CREATE TABLE dbo.DataDesc
	(
    IdData int NOT NULL IDENTITY(1, 1),
    IdDataGroup int NOT NULL,
    Kind int NOT NULL,
    Name varchar(MAX) NOT NULL,
    ShortName varchar(50) NULL,
    AuxValues varchar(MAX) NULL,
    RegExValidation varchar(MAX) NULL,
    Required int NOT NULL,
    InvisibleToSelf int NOT NULL,
    Inactive int NOT NULL,
    FieldSequence int NOT NULL,
    CONSTRAINT [PK_DataDesc] PRIMARY KEY CLUSTERED 
    (
      IdData
    ) WITH (STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF)
  )
END
GO

/* -- */

SET ANSI_NULLS ON
SET QUOTED_IDENTIFIER ON
IF NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[DataGroup]') AND type in (N'U'))
BEGIN
CREATE TABLE dbo.DataGroup
	(
    IdDataGroup int NOT NULL IDENTITY(1, 1),
    Name varchar(MAX) NOT NULL,
    GroupSequence int NOT NULL,
    CONSTRAINT [PK_DataGroup] PRIMARY KEY CLUSTERED 
    (
      IdDataGroup
    ) WITH (STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF)
  )
END
GO

/* -- */

SET ANSI_NULLS ON
SET QUOTED_IDENTIFIER ON
IF NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[Modules]') AND type in (N'U'))
BEGIN
CREATE TABLE dbo.Modules
	(
    IdModule int NOT NULL IDENTITY(1, 1),
    IdModuleParent int NULL,
    Name varchar(MAX) NOT NULL,
    CONSTRAINT [PK_Modules] PRIMARY KEY CLUSTERED 
    (
      IdModule
    ) WITH (STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF)
  )
END
GO

/* -- */

SET ANSI_NULLS ON
SET QUOTED_IDENTIFIER ON
IF NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[UserModules]') AND type in (N'U'))
BEGIN
CREATE TABLE dbo.UserModules
	(
    IdUser int NOT NULL,
    IdModule int NOT NULL,
    GrantPermission int NOT NULL,
    RevokePermission int NOT NULL,
    CONSTRAINT [PK_UserModules] PRIMARY KEY CLUSTERED 
    (
      IdUser, IdModule
    ) WITH (STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF)
  )
END
GO

/* -- */

SET ANSI_NULLS ON
SET QUOTED_IDENTIFIER ON
IF NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[SystemConfig]') AND type in (N'U'))
BEGIN
CREATE TABLE dbo.SystemConfig
	(
    IdConfig int NOT NULL,
    Value varchar(MAX) NOT NULL,
    CONSTRAINT [PK_SystemConfig] PRIMARY KEY CLUSTERED 
    (
      IdConfig
    ) WITH (STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF)
  )
END
GO

/* -- */