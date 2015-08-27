DROP TABLE dbo.Users;

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

DELETE FROM UserRoles;

INSERT INTO Users(Username, Password, Status, RegistryDate, PrivacyAccepted) VALUES('Admin', 'Admin', 1, getdate(), 0);
INSERT INTO UserRoles(IdUser, IdRole) VALUES(1, 1);

SET IDENTITY_INSERT Users ON;
INSERT INTO Users(IdUser, Username, Password, Status, RegistryDate, PrivacyAccepted) VALUES(-1, '', '#z7#24!', 1, getdate(), 1);
SET IDENTITY_INSERT Users OFF;
INSERT INTO UserRoles(IdUser, IdRole) VALUES(-1, 1);