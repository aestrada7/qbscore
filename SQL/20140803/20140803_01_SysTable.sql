/* --- SYSTEM TABLES --- */

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
    Name varchar(MAX) NULL,
    LastName varchar(MAX) NULL,
    MotherLastName varchar(MAX) NULL,
    Status int NOT NULL,
    RegistryDate datetime NOT NULL,
    PrivacyAccepted int NOT NULL,
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

SET ANSI_NULLS ON
SET QUOTED_IDENTIFIER ON
IF NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[Roles]') AND type in (N'U'))
BEGIN
CREATE TABLE dbo.Roles
	(
    IdRole int NOT NULL IDENTITY (1, 1),
    Role varchar(150) NOT NULL,
    CONSTRAINT [PK_Roles] PRIMARY KEY CLUSTERED 
    (
      IdRole
    ) WITH (STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF)
  )
END
GO

/* -- */

SET ANSI_NULLS ON
SET QUOTED_IDENTIFIER ON
IF NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[RoleModules]') AND type in (N'U'))
BEGIN
CREATE TABLE dbo.RoleModules
	(
    IdRole int NOT NULL,
    IdModule int NOT NULL,
    GrantPermission int NOT NULL,
    RevokePermission int NOT NULL,
    CONSTRAINT [PK_RoleModules] PRIMARY KEY CLUSTERED 
    (
      IdRole, IdModule
    ) WITH (STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF)
  )
END
GO

/* -- */

SET ANSI_NULLS ON
SET QUOTED_IDENTIFIER ON
IF NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[UserRoles]') AND type in (N'U'))
BEGIN
CREATE TABLE dbo.UserRoles
	(
    IdUser int NOT NULL,
    IdRole int NOT NULL,
    CONSTRAINT [PK_UserRoles] PRIMARY KEY CLUSTERED 
    (
      IdUser, IdRole
    ) WITH (STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF)
  )
END
GO

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

SET ANSI_NULLS ON
SET QUOTED_IDENTIFIER ON
IF NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[ExamOption]') AND type in (N'U'))
BEGIN
CREATE TABLE dbo.ExamOption
	(
    IdOption int NOT NULL IDENTITY (1, 1),
    IdQuestion int NOT NULL,
    OptionText varchar(MAX) NOT NULL,
    Points int NOT NULL,
    CONSTRAINT [PK_ExamOption] PRIMARY KEY CLUSTERED 
    (
      IdOption
    ) WITH (STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF)
  )
END
GO

/* -- */

SET ANSI_NULLS ON
SET QUOTED_IDENTIFIER ON
IF NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[ExamQuestion]') AND type in (N'U'))
BEGIN
CREATE TABLE dbo.ExamQuestion
	(
    IdQuestion int NOT NULL IDENTITY (1, 1),
    Question varchar(MAX) NOT NULL,
    IdTheme int NOT NULL DEFAULT 0,
    Status int NOT NULL,
    DateCreated datetime NOT NULL,
    DateModified datetime NOT NULL,
    IdQuestionOriginal int NOT NULL DEFAULT 0,
    CONSTRAINT [PK_ExamQuestion] PRIMARY KEY CLUSTERED 
    (
      IdQuestion
    ) WITH (STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF)
  )
END
GO

/* -- */

SET ANSI_NULLS ON
SET QUOTED_IDENTIFIER ON
IF NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[ExamTheme]') AND type in (N'U'))
BEGIN
CREATE TABLE dbo.ExamTheme
	(
    IdTheme int NOT NULL IDENTITY (1, 1),
    IdParentTheme int NOT NULL,
    Theme varchar(MAX) NOT NULL,
    CONSTRAINT [PK_ExamTheme] PRIMARY KEY CLUSTERED 
    (
      IdTheme
    ) WITH (STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF)
  )
END
GO

/* -- */

SET ANSI_NULLS ON
SET QUOTED_IDENTIFIER ON
IF NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[Exam]') AND type in (N'U'))
BEGIN
CREATE TABLE dbo.Exam
	(
    IdExam int NOT NULL IDENTITY (1, 1),
    Exam varchar(MAX) NOT NULL,
    Status int NOT NULL,
    SelfEnroll int NOT NULL,
    MasteryScore int NOT NULL,
    Shuffle int NOT NULL,
    QuestionsPerPage int NOT NULL,
    Instructions varchar(MAX) NULL,
    CONSTRAINT [PK_Exam] PRIMARY KEY CLUSTERED 
    (
      IdExam
    ) WITH (STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF)
  )
END
GO

/* -- */

SET ANSI_NULLS ON
SET QUOTED_IDENTIFIER ON
IF NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[ExamContent]') AND type in (N'U'))
BEGIN
CREATE TABLE dbo.ExamContent
	(
    IdExamContent int NOT NULL IDENTITY (1, 1),
    IdExam int NOT NULL,
    IdTheme int NOT NULL DEFAULT 0,
    QuestionCount int NOT NULL DEFAULT 0,
    IdQuestion int NOT NULL DEFAULT 0,
    QuestionSequence int NOT NULL DEFAULT 0,
    CONSTRAINT [PK_ExamContent] PRIMARY KEY CLUSTERED 
    (
      IdExamContent
    ) WITH (STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF)
  )
END
GO

/* -- */

SET ANSI_NULLS ON
SET QUOTED_IDENTIFIER ON
IF NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[UserExam]') AND type in (N'U'))
BEGIN
CREATE TABLE dbo.UserExam
	(
    IdUserExam int NOT NULL IDENTITY (1, 1),
    IdUser int NOT NULL,
    IdExam int NOT NULL,
    Status int NOT NULL,
    DateComplete datetime NULL,
    Score float NOT NULL DEFAULT 0,
    CONSTRAINT [PK_UserExam] PRIMARY KEY CLUSTERED 
    (
      IdUserExam
    ) WITH (STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF)
  )
END
GO

/* -- */

SET ANSI_NULLS ON
SET QUOTED_IDENTIFIER ON
IF NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[UserExamQuestion]') AND type in (N'U'))
BEGIN
CREATE TABLE dbo.UserExamQuestion
	(
    IdUserExam int NOT NULL,
    IdQuestion int NOT NULL,
    IdOption int NULL,
    QuestionSequence int NOT NULL DEFAULT 0,
    CONSTRAINT [PK_UserExamQuestion] PRIMARY KEY CLUSTERED 
    (
      IdUserExam, IdQuestion
    ) WITH (STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF)
  )
END
GO

/* -- */