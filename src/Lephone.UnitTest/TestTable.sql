CREATE TABLE [People] ([Id] INTEGER PRIMARY KEY AUTOINCREMENT, [Name] NVARCHAR(50) NOT NULL);
CREATE TABLE [PCs] ([Id] INTEGER PRIMARY KEY AUTOINCREMENT, [Name] NVARCHAR(50) NOT NULL, [Person_Id] BIGINT NULL,
	FOREIGN KEY([Person_Id]) REFERENCES [People] ([Id]));

INSERT INTO [People] ([Id],[Name]) VALUES (1, 'Tom');
INSERT INTO [People] ([Id],[Name]) VALUES (2, 'Jerry');
INSERT INTO [People] ([Id],[Name]) VALUES (3, 'Mike');

INSERT INTO [PCs] ([Id],[Name],[Person_Id]) VALUES (1, 'IBM', 2);
INSERT INTO [PCs] ([Id],[Name],[Person_Id]) VALUES (2, 'DELL', 3);
INSERT INTO [PCs] ([Id],[Name],[Person_Id]) VALUES (3, 'HP', 3);

--------------------------------------------------------------------------------------------------------------------

CREATE TABLE [Categories] ([Id] INTEGER PRIMARY KEY AUTOINCREMENT, [Name] NVARCHAR(50) NOT NULL);
CREATE TABLE [Books] ([Id] INTEGER PRIMARY KEY AUTOINCREMENT, [Name] NVARCHAR(50) NOT NULL, [Category_Id] BIGINT NULL,
	FOREIGN KEY([Category_Id]) REFERENCES [Categories] ([Id]));

INSERT INTO [Categories] ([Id],[Name]) VALUES (1, 'Tech');
INSERT INTO [Categories] ([Id],[Name]) VALUES (2, 'Game');
INSERT INTO [Categories] ([Id],[Name]) VALUES (3, 'Tour');

INSERT INTO [Books] ([Id],[Name],[Category_Id]) VALUES (1, 'Diablo', 2);
INSERT INTO [Books] ([Id],[Name],[Category_Id]) VALUES (2, 'Beijing', 3);
INSERT INTO [Books] ([Id],[Name],[Category_Id]) VALUES (3, 'Shanghai', 3);
INSERT INTO [Books] ([Id],[Name],[Category_Id]) VALUES (4, 'Pal95', 2);
INSERT INTO [Books] ([Id],[Name],[Category_Id]) VALUES (5, 'Wow', 2);

--------------------------------------------------------------------------------------------------------------------

CREATE TABLE Article ([Id] INTEGER PRIMARY KEY AUTOINCREMENT, [Name] NVARCHAR(50) NOT NULL);
CREATE TABLE Reader ([Id] INTEGER PRIMARY KEY AUTOINCREMENT, [Name] NVARCHAR(50) NOT NULL);
CREATE TABLE R_Article_Reader (Article_Id BIGINT NOT NULL, Reader_Id BIGINT NOT NULL,
	FOREIGN KEY(Reader_Id) REFERENCES Reader ([Id]),
	FOREIGN KEY(Article_Id) REFERENCES Article ([Id]));

INSERT INTO Article ([Id],[Name]) VALUES (1, 'The lovely bones');
INSERT INTO Article ([Id],[Name]) VALUES (2, 'The world is float');
INSERT INTO Article ([Id],[Name]) VALUES (3, 'The load of rings');

INSERT INTO Reader ([Id],[Name]) VALUES (1, 'tom');
INSERT INTO Reader ([Id],[Name]) VALUES (2, 'jerry');
INSERT INTO Reader ([Id],[Name]) VALUES (3, 'mike');

INSERT INTO R_Article_Reader (Article_Id,Reader_Id) VALUES (1, 2);
INSERT INTO R_Article_Reader (Article_Id,Reader_Id) VALUES (1, 3);
INSERT INTO R_Article_Reader (Article_Id,Reader_Id) VALUES (1, 1);
INSERT INTO R_Article_Reader (Article_Id,Reader_Id) VALUES (2, 2);
INSERT INTO R_Article_Reader (Article_Id,Reader_Id) VALUES (2, 3);
INSERT INTO R_Article_Reader (Article_Id,Reader_Id) VALUES (3, 1);

--------------------------------------------------------------------------------------------------------------------

CREATE TABLE [File] ([Id] INTEGER PRIMARY KEY AUTOINCREMENT, [Name] NVARCHAR(50) NOT NULL, BelongsTo_Id BIGINT NULL,
	FOREIGN KEY(BelongsTo_Id) REFERENCES [File] ([Id]));

INSERT INTO [File] ([Id],[Name],[BelongsTo_Id]) VALUES (1, 'Root', NULL);
INSERT INTO [File] ([Id],[Name],[BelongsTo_Id]) VALUES (2, 'Windows', 1);
INSERT INTO [File] ([Id],[Name],[BelongsTo_Id]) VALUES (3, 'Program Files', 1);
INSERT INTO [File] ([Id],[Name],[BelongsTo_Id]) VALUES (4, 'Tools', 1);
INSERT INTO [File] ([Id],[Name],[BelongsTo_Id]) VALUES (5, 'Config.sys', 1);
INSERT INTO [File] ([Id],[Name],[BelongsTo_Id]) VALUES (6, 'Command.com', 1);
INSERT INTO [File] ([Id],[Name],[BelongsTo_Id]) VALUES (7, 'regedit.exe', 2);
INSERT INTO [File] ([Id],[Name],[BelongsTo_Id]) VALUES (8, 'notepad.exe', 2);
INSERT INTO [File] ([Id],[Name],[BelongsTo_Id]) VALUES (9, 'System32', 2);
INSERT INTO [File] ([Id],[Name],[BelongsTo_Id]) VALUES (10, 'regsvr32.exe', 9);
INSERT INTO [File] ([Id],[Name],[BelongsTo_Id]) VALUES (11, 'Office', 3);
INSERT INTO [File] ([Id],[Name],[BelongsTo_Id]) VALUES (12, 'Word.exe', 11);
INSERT INTO [File] ([Id],[Name],[BelongsTo_Id]) VALUES (13, 'Outlook.exe', 11);
INSERT INTO [File] ([Id],[Name],[BelongsTo_Id]) VALUES (14, 'Excel.exe', 11);
INSERT INTO [File] ([Id],[Name],[BelongsTo_Id]) VALUES (15, 'LocPlus', 4);
INSERT INTO [File] ([Id],[Name],[BelongsTo_Id]) VALUES (16, 'cConv', 4);
INSERT INTO [File] ([Id],[Name],[BelongsTo_Id]) VALUES (17, 'LocPlus.exe', 15);
INSERT INTO [File] ([Id],[Name],[BelongsTo_Id]) VALUES (18, 'LocPlus.ini', 15);
INSERT INTO [File] ([Id],[Name],[BelongsTo_Id]) VALUES (19, 'cConv.exe', 16);
INSERT INTO [File] ([Id],[Name],[BelongsTo_Id]) VALUES (20, 'cConv.ini', 16);

--------------------------------------------------------------------------------------------------------------------

CREATE TABLE [NullTest] ([Id] INTEGER PRIMARY KEY AUTOINCREMENT, [Name] NVARCHAR(50) NULL, [MyInt] INT NULL, [MyBool] BOOL NULL);

INSERT INTO [NullTest] ([Id],[Name],[MyInt],[MyBool]) VALUES (1, 'tom', null, 1);
INSERT INTO [NullTest] ([Id],[Name],[MyInt],[MyBool]) VALUES (2, null, 1, 0);
INSERT INTO [NullTest] ([Id],[Name],[MyInt],[MyBool]) VALUES (3, null, null, null);
INSERT INTO [NullTest] ([Id],[Name],[MyInt],[MyBool]) VALUES (4, 'tom', 1, 1);

--------------------------------------------------------------------------------------------------------------------

CREATE TABLE ArticleMore ([Id] INTEGER PRIMARY KEY AUTOINCREMENT, [Name] NVARCHAR(50) NOT NULL);
CREATE TABLE ReaderMore ([Id] INTEGER PRIMARY KEY AUTOINCREMENT, [Name] NVARCHAR(50) NOT NULL);

INSERT INTO ArticleMore ([Id],[Name]) VALUES (1, 'The lovely bones');
INSERT INTO ArticleMore ([Id],[Name]) VALUES (2, 'The world is float');
INSERT INTO ArticleMore ([Id],[Name]) VALUES (3, 'The load of rings');

INSERT INTO ReaderMore ([Id],[Name]) VALUES (1, 'tom');
INSERT INTO ReaderMore ([Id],[Name]) VALUES (2, 'jerry');
INSERT INTO ReaderMore ([Id],[Name]) VALUES (3, 'mike');

CREATE TABLE [BelongsMore] ([Id] INTEGER PRIMARY KEY AUTOINCREMENT, [Name] NVARCHAR(50) NOT NULL, Article_Id BIGINT NULL, Reader_Id BIGINT NULL,
	FOREIGN KEY(Reader_Id) REFERENCES ReaderMore ([Id]),
	FOREIGN KEY(Article_Id) REFERENCES ArticleMore ([Id]));

INSERT INTO [BelongsMore] ([Id],[Name],[Article_Id],[Reader_Id]) VALUES (1, 'f1', 1, 2);
INSERT INTO [BelongsMore] ([Id],[Name],[Article_Id],[Reader_Id]) VALUES (2, 'f2', 2, 3);
INSERT INTO [BelongsMore] ([Id],[Name],[Article_Id],[Reader_Id]) VALUES (3, 'f3', 3, 1);
INSERT INTO [BelongsMore] ([Id],[Name],[Article_Id],[Reader_Id]) VALUES (4, 'f4', 3, 3);

--------------------------------------------------------------------------------------------------------------------

CREATE TABLE [SoftDelete] ([Id] INTEGER PRIMARY KEY AUTOINCREMENT, [Name] NVARCHAR(50) NOT NULL, IsDeleted BOOL NOT NULL);

INSERT INTO [SoftDelete] ([Id],[Name],[IsDeleted]) VALUES (1, 'tom', 0);
INSERT INTO [SoftDelete] ([Id],[Name],[IsDeleted]) VALUES (2, 'jerry', 0);
INSERT INTO [SoftDelete] ([Id],[Name],[IsDeleted]) VALUES (3, 'mike', 0);
INSERT INTO [SoftDelete] ([Id],[Name],[IsDeleted]) VALUES (4, 'nothing', 1);

--------------------------------------------------------------------------------------------------------------------

CREATE TABLE [DateAndTime] ([Id] INTEGER PRIMARY KEY AUTOINCREMENT, [dtValue] datetime NOT NULL, [dValue] date NOT NULL, [tValue] time NOT NULL, [dtnValue] datetime NULL, [dnValue] date NULL, [tnValue] time NULL);

INSERT INTO [DateAndTime] ([Id],[dtValue],[dValue],[tValue],[dtnValue],[dnValue],[tnValue]) VALUES (1, datetime('1092941466','unixepoch'), date('1092941466','unixepoch'), time('1092941466','unixepoch'), null, null,null);
INSERT INTO [DateAndTime] ([Id],[dtValue],[dValue],[tValue],[dtnValue],[dnValue],[tnValue]) VALUES (2, datetime('1092941466','unixepoch'), date('1092941466','unixepoch'), time('1092941466','unixepoch'), datetime('1092941466','unixepoch'), date('1092941466','unixepoch'), time('1092941466','unixepoch'));

--------------------------------------------------------------------------------------------------------------------

CREATE TABLE [LockVersionTest] ([Id] INTEGER PRIMARY KEY AUTOINCREMENT, [Name] NVARCHAR(50) NOT NULL, LockVersion INT NOT NULL);

INSERT INTO [LockVersionTest] ([Id],[Name],[LockVersion]) VALUES (1, 'test', 1);

--------------------------------------------------------------------------------------------------------------------

CREATE TABLE [DCS_USERS] (
	[Id] INTEGER PRIMARY KEY AUTOINCREMENT ,
	[USER_NAME] ntext NOT NULL 
);

INSERT INTO [DCS_USERS] ([Id], [USER_NAME]) VALUES (1, 'SYSTEM');

CREATE TABLE [REF_ORG_UNIT] (
	[Id] INTEGER PRIMARY KEY AUTOINCREMENT 
);

INSERT INTO [REF_ORG_UNIT] ([Id]) VALUES (1);

CREATE TABLE [DCS_PERSONS] (
	[Id] INTEGER PRIMARY KEY AUTOINCREMENT ,
	[NAME_LAST] ntext NOT NULL 
);

INSERT INTO [DCS_PERSONS] ([Id], [NAME_LAST]) VALUES (1, 'Mustermann');

CREATE TABLE [HRM_EMPLOYEES] (
	[Id] INTEGER PRIMARY KEY AUTOINCREMENT ,
	[DCS_PERSONS_Id] bigint NULL ,
	FOREIGN KEY([DCS_PERSONS_Id]) REFERENCES [DCS_PERSONS] ([Id])
);

INSERT INTO [HRM_EMPLOYEES] ([Id], [DCS_PERSONS_Id]) VALUES (1, 1);

CREATE TABLE [REL_EMP_JOB_ROLE] (
	[Id] INTEGER PRIMARY KEY AUTOINCREMENT ,
	[UC] bigint NOT NULL ,
	[AF] bool NOT NULL ,
	[START_DATE] datetime NULL ,
	[HRM_EMPLOYEES_Id] bigint NULL ,
	[HRM_JOB_ROLES_Id] bigint NULL ,
	FOREIGN KEY([HRM_EMPLOYEES_Id]) REFERENCES [HRM_EMPLOYEES] ([Id]),
	FOREIGN KEY([HRM_JOB_ROLES_Id]) REFERENCES [HRM_JOB_ROLES] ([Id])
);

CREATE TABLE [REL_JOB_ROLE_ORG_UNIT] (
	[Id] INTEGER PRIMARY KEY AUTOINCREMENT ,
	[UC] bigint NOT NULL ,
	[AF] bool NOT NULL ,
	[RELATION_TYPE] int NOT NULL ,
	[REF_ORG_UNIT_Id] bigint NULL ,
	[HRM_JOB_ROLES_Id] bigint NULL ,
	FOREIGN KEY([REF_ORG_UNIT_Id]) REFERENCES [REF_ORG_UNIT] ([Id]),
	FOREIGN KEY([HRM_JOB_ROLES_Id]) REFERENCES [HRM_JOB_ROLES] ([Id])
);

CREATE TABLE [HRM_JOB_ROLES] (
	[Id] INTEGER PRIMARY KEY AUTOINCREMENT ,
	[UC] bigint NOT NULL ,
	[CODE] ntext NOT NULL ,
	[ROLE_NAME] ntext NOT NULL ,
	[DESCRIPTION] ntext NOT NULL 
);

CREATE TABLE [Co_User1] (
    [Id] INTEGER PRIMARY KEY AUTOINCREMENT ,
    [Name] NTEXT NOT NULL ,
    [LocationPhone] NTEXT NOT NULL ,
    [LocationAddress] NTEXT NOT NULL ,
    [LocationNumber] INT NOT NULL ,
    [LocationWow] INT NULL 
);

CREATE TABLE [Co_User] (
    [Id] INTEGER PRIMARY KEY AUTOINCREMENT ,
    [Name] NTEXT NOT NULL ,
    [LocationPhone] NTEXT NOT NULL ,
    [LocationAddress] NTEXT NULL ,
    [MyNumber] INT NOT NULL ,
    [LocationWow] INT NULL 
);

CREATE TABLE [Lock_Book] (
    [Id] INTEGER PRIMARY KEY AUTOINCREMENT ,
    [Name] NTEXT NOT NULL ,
    [LockVersion] INT NOT NULL 
);

CREATE TABLE [Required_Model] ([Id] INTEGER PRIMARY KEY AUTOINCREMENT, [Name] NVARCHAR(50) NOT NULL, [Age] INT NOT NULL);
CREATE TABLE [Required_Two] ([Id] INTEGER PRIMARY KEY AUTOINCREMENT, [Name] NVARCHAR(50) NOT NULL, [Age] INT NOT NULL, [Person_Id] BIGINT NOT NULL);
