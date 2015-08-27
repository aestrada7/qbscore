/* -- */

DELETE FROM Modules;

SET IDENTITY_INSERT Modules ON;
INSERT INTO Modules(IdModule, IdModuleParent, Name) VALUES(100, 0, '#USER_ADMINISTRATION#');
INSERT INTO Modules(IdModule, IdModuleParent, Name) VALUES(101, 100, '#ADD_USER#');
INSERT INTO Modules(IdModule, IdModuleParent, Name) VALUES(102, 100, '#EDIT_USER#');
INSERT INTO Modules(IdModule, IdModuleParent, Name) VALUES(103, 100, '#DELETE_USER#');
INSERT INTO Modules(IdModule, IdModuleParent, Name) VALUES(104, 100, '#EDIT_PERMISSIONS#');
INSERT INTO Modules(IdModule, IdModuleParent, Name) VALUES(105, 100, '#EDIT_PASSWORDS#');
INSERT INTO Modules(IdModule, IdModuleParent, Name) VALUES(106, 100, '#EDIT_SELF_PASSWORD#');
INSERT INTO Modules(IdModule, IdModuleParent, Name) VALUES(107, 100, '#EDIT_SELF_INFO#');
INSERT INTO Modules(IdModule, IdModuleParent, Name) VALUES(108, 100, '#EXPORT_USERS#');
INSERT INTO Modules(IdModule, IdModuleParent, Name) VALUES(109, 100, '#IMPORT_USERS#');
INSERT INTO Modules(IdModule, IdModuleParent, Name) VALUES(200, 0, '#DATAFIELDS_ADMINISTRATION#');
INSERT INTO Modules(IdModule, IdModuleParent, Name) VALUES(201, 200, '#ADD_FIELDS#');
INSERT INTO Modules(IdModule, IdModuleParent, Name) VALUES(202, 200, '#EDIT_FIELDS#');
INSERT INTO Modules(IdModule, IdModuleParent, Name) VALUES(203, 200, '#DELETE_FIELDS#');
INSERT INTO Modules(IdModule, IdModuleParent, Name) VALUES(300, 0, '#LOG_ACCESS#');
INSERT INTO Modules(IdModule, IdModuleParent, Name) VALUES(400, 0, '#CATALOGS#');
INSERT INTO Modules(IdModule, IdModuleParent, Name) VALUES(401, 400, '#ROLE_CATALOG#');
INSERT INTO Modules(IdModule, IdModuleParent, Name) VALUES(402, 401, '#ROLE_CATALOG_ADD#');
INSERT INTO Modules(IdModule, IdModuleParent, Name) VALUES(403, 401, '#ROLE_CATALOG_EDIT#');
INSERT INTO Modules(IdModule, IdModuleParent, Name) VALUES(404, 401, '#ROLE_CATALOG_DELETE#');
INSERT INTO Modules(IdModule, IdModuleParent, Name) VALUES(500, 0, '#EVALUATION#');
INSERT INTO Modules(IdModule, IdModuleParent, Name) VALUES(501, 500, '#EVAL_QUESTION_CATALOG#');
INSERT INTO Modules(IdModule, IdModuleParent, Name) VALUES(502, 501, '#EVAL_QUESTION_CATALOG_ADD#');
INSERT INTO Modules(IdModule, IdModuleParent, Name) VALUES(503, 501, '#EVAL_QUESTION_CATALOG_EDIT#');
INSERT INTO Modules(IdModule, IdModuleParent, Name) VALUES(504, 501, '#EVAL_QUESTION_CATALOG_DELETE#');
INSERT INTO Modules(IdModule, IdModuleParent, Name) VALUES(505, 500, '#EVAL_THEME_CATALOG#');
INSERT INTO Modules(IdModule, IdModuleParent, Name) VALUES(506, 505, '#EVAL_THEME_CATALOG_ADD#');
INSERT INTO Modules(IdModule, IdModuleParent, Name) VALUES(507, 505, '#EVAL_THEME_CATALOG_EDIT#');
INSERT INTO Modules(IdModule, IdModuleParent, Name) VALUES(508, 505, '#EVAL_THEME_CATALOG_DELETE#');
INSERT INTO Modules(IdModule, IdModuleParent, Name) VALUES(509, 500, '#EVAL_EXAM_CATALOG#');
INSERT INTO Modules(IdModule, IdModuleParent, Name) VALUES(510, 509, '#EVAL_EXAM_CATALOG_ADD#');
INSERT INTO Modules(IdModule, IdModuleParent, Name) VALUES(511, 509, '#EVAL_EXAM_CATALOG_EDIT#');
INSERT INTO Modules(IdModule, IdModuleParent, Name) VALUES(512, 509, '#EVAL_EXAM_CATALOG_DELETE#');
INSERT INTO Modules(IdModule, IdModuleParent, Name) VALUES(513, 500, '#EVAL_SOLVE_USER#');
INSERT INTO Modules(IdModule, IdModuleParent, Name) VALUES(514, 500, '#EVAL_IMPORT_EXAMS#');
INSERT INTO Modules(IdModule, IdModuleParent, Name) VALUES(515, 500, '#EVAL_IMPORT_RESULTS#');
INSERT INTO Modules(IdModule, IdModuleParent, Name) VALUES(516, 500, '#EVAL_REPORTS#');
INSERT INTO Modules(IdModule, IdModuleParent, Name) VALUES(517, 500, '#EVAL_ASSIGN#');
SET IDENTITY_INSERT Modules OFF;

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

DELETE FROM UserRoles;

INSERT INTO Users(Username, Name, Password, Status, RegistryDate, PrivacyAccepted) VALUES('Admin', 'Admin', 'Admin', 1, getdate(), 0);
INSERT INTO UserRoles(IdUser, IdRole) VALUES(1, 1);

SET IDENTITY_INSERT Users ON;
INSERT INTO Users(IdUser, Username, Password, Status, RegistryDate, PrivacyAccepted) VALUES(-1, '', '#z7#24!', 1, getdate(), 1);
SET IDENTITY_INSERT Users OFF;
INSERT INTO UserRoles(IdUser, IdRole) VALUES(-1, 1);

/* -- */

DELETE FROM SystemConfig;

INSERT INTO SystemConfig VALUES(1, 'QBScore');
INSERT INTO SystemConfig VALUES(2, '0.9');
INSERT INTO SystemConfig VALUES(3, '0');
INSERT INTO SystemConfig VALUES(4, '0');
INSERT INTO SystemConfig VALUES(5, '0');
INSERT INTO SystemConfig VALUES(6, '0');
INSERT INTO SystemConfig VALUES(7, '20140803');
INSERT INTO SystemConfig VALUES(8, getdate());
INSERT INTO SystemConfig VALUES(9, dateadd(yyyy, 1, getdate()));

/* -- */