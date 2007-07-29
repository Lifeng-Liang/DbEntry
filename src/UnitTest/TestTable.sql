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
