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

DELETE FROM Roles;
DELETE FROM RoleModules;

SET IDENTITY_INSERT Roles ON;
INSERT INTO Roles(IdRole, Role) VALUES(1, '#ADMINISTRATOR#');
INSERT INTO Roles(IdRole, Role) VALUES(2, '#USER#');
SET IDENTITY_INSERT Roles OFF;

INSERT INTO RoleModules(IdRole, IdModule, GrantPermission, RevokePermission) VALUES(1, 100, 1, 0);
INSERT INTO RoleModules(IdRole, IdModule, GrantPermission, RevokePermission) VALUES(1, 200, 1, 0);
INSERT INTO RoleModules(IdRole, IdModule, GrantPermission, RevokePermission) VALUES(1, 300, 1, 0);
INSERT INTO RoleModules(IdRole, IdModule, GrantPermission, RevokePermission) VALUES(1, 400, 1, 0);
INSERT INTO RoleModules(IdRole, IdModule, GrantPermission, RevokePermission) VALUES(1, 500, 1, 0);

INSERT INTO RoleModules(IdRole, IdModule, GrantPermission, RevokePermission) VALUES(2, 100, 0, 1);
INSERT INTO RoleModules(IdRole, IdModule, GrantPermission, RevokePermission) VALUES(2, 106, 1, 0);
INSERT INTO RoleModules(IdRole, IdModule, GrantPermission, RevokePermission) VALUES(2, 107, 1, 0);
INSERT INTO RoleModules(IdRole, IdModule, GrantPermission, RevokePermission) VALUES(2, 200, 0, 1);
INSERT INTO RoleModules(IdRole, IdModule, GrantPermission, RevokePermission) VALUES(2, 300, 0, 1);
INSERT INTO RoleModules(IdRole, IdModule, GrantPermission, RevokePermission) VALUES(2, 400, 0, 1);
INSERT INTO RoleModules(IdRole, IdModule, GrantPermission, RevokePermission) VALUES(2, 500, 0, 1);

/* -- */