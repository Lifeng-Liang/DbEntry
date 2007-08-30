CREATE TABLE [People] ([Id] INTEGER PRIMARY KEY AUTOINCREMENT, [Name] NVARCHAR(50) NOT NULL);
CREATE TABLE [PCs] ([Id] INTEGER PRIMARY KEY AUTOINCREMENT, [Name] NVARCHAR(50) NOT NULL, Person_Id INT NOT NULL);

INSERT INTO [People] ([Id],[Name]) VALUES (1, 'Tom');
INSERT INTO [People] ([Id],[Name]) VALUES (2, 'Jerry');
INSERT INTO [People] ([Id],[Name]) VALUES (3, 'Mike');

INSERT INTO [PCs] ([Id],[Name],[Person_Id]) VALUES (1, 'IBM', 2);
INSERT INTO [PCs] ([Id],[Name],[Person_Id]) VALUES (2, 'DELL', 3);
INSERT INTO [PCs] ([Id],[Name],[Person_Id]) VALUES (3, 'HP', 3);

--------------------------------------------------------------------------------------------------------------------

CREATE TABLE [Categories] ([Id] INTEGER PRIMARY KEY AUTOINCREMENT, [Name] NVARCHAR(50) NOT NULL);
CREATE TABLE [Books] ([Id] INTEGER PRIMARY KEY AUTOINCREMENT, [Name] NVARCHAR(50) NOT NULL, Category_Id INT NOT NULL);

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
CREATE TABLE Article_Reader (Article_Id INTEGER NOT NULL, Reader_Id INTEGER NOT NULL);

INSERT INTO Article ([Id],[Name]) VALUES (1, 'The lovely bones');
INSERT INTO Article ([Id],[Name]) VALUES (2, 'The world is float');
INSERT INTO Article ([Id],[Name]) VALUES (3, 'The load of rings');

INSERT INTO Reader ([Id],[Name]) VALUES (1, 'tom');
INSERT INTO Reader ([Id],[Name]) VALUES (2, 'jerry');
INSERT INTO Reader ([Id],[Name]) VALUES (3, 'mike');

INSERT INTO Article_Reader (Article_Id,Reader_Id) VALUES (1, 2);
INSERT INTO Article_Reader (Article_Id,Reader_Id) VALUES (1, 3);
INSERT INTO Article_Reader (Article_Id,Reader_Id) VALUES (1, 1);
INSERT INTO Article_Reader (Article_Id,Reader_Id) VALUES (2, 2);
INSERT INTO Article_Reader (Article_Id,Reader_Id) VALUES (2, 3);
INSERT INTO Article_Reader (Article_Id,Reader_Id) VALUES (3, 1);

--------------------------------------------------------------------------------------------------------------------

CREATE TABLE [File] ([Id] INTEGER PRIMARY KEY AUTOINCREMENT, [Name] NVARCHAR(50) NOT NULL, BelongsTo_Id INT NOT NULL);

INSERT INTO [File] ([Id],[Name],[BelongsTo_Id]) VALUES (1, 'Root', 0);
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

CREATE TABLE [BelongsMore] ([Id] INTEGER PRIMARY KEY AUTOINCREMENT, [Name] NVARCHAR(50) NOT NULL, Article_Id INTEGER NOT NULL, Reader_Id INTEGER NOT NULL);

INSERT INTO [BelongsMore] ([Id],[Name],[Article_Id],[Reader_Id]) VALUES (1, 'f1', 1, 2);
INSERT INTO [BelongsMore] ([Id],[Name],[Article_Id],[Reader_Id]) VALUES (2, 'f2', 2, 3);
INSERT INTO [BelongsMore] ([Id],[Name],[Article_Id],[Reader_Id]) VALUES (3, 'f3', 3, 1);
INSERT INTO [BelongsMore] ([Id],[Name],[Article_Id],[Reader_Id]) VALUES (4, 'f4', 3, 3);
