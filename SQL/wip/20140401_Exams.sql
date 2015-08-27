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
    QuestionCount int NOT NULL,
    IdQuestion int NOT NULL,
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
    CONSTRAINT [PK_UserExamQuestion] PRIMARY KEY CLUSTERED 
    (
      IdUserExam, IdQuestion
    ) WITH (STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF)
  )
END
GO

/* -- */