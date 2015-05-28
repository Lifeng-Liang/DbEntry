----------------------------------------------------------------------------

Code Generator For DbEntry.Net
http://dbentry.codeplex.com

----------------------------------------------------------------------------

Usage for Asp.Net:
    Leafing.CodeGen a FileName [ClassPath]

FileName:   The assembly file name who has the model classes.
ClassPath:  The class you want to generate template code
            If don't input it, the generator will show all the classes name.

Example:
    Leafing.CodeGen a Models.dll Models.User

----------------------------------------------------------------------------

Usage for Mvc actions:
    Leafing.CodeGen ra FileName [ClassPath ActionName]

FileName:   The assembly file name who has the model classes.
ClassPath:  The class you want to generate template code
            If don't input it, the generator will show all the classes name.
ActionName: One of the following: New, Create, List, Show, Edit, Update, 
            Destroy, All

Example:
    Leafing.CodeGen ra Models.dll Models.User All

----------------------------------------------------------------------------

Usage for Mvc views:
    Leafing.CodeGen rv FileName [ClassPath ViewName [MasterPage]]

FileName:   The assembly file name who has the model classes.
ClassPath:  The class you want to generate template code
            If don't input it, the generator will show all the classes name.
ViewName:   One of the following: New, List, Show, Edit
MasterPage: Master page link like "~/Master.master", if this argument exist,
            the output code will be master page mode.

Example:
    Leafing.CodeGen rv Models.dll Models.User New ~/Master.master

----------------------------------------------------------------------------

Usage for models from database:
    Leafing.CodeGen m [TableName]

TableName:  Table name in database, and it will be the model class name too.
            If the name is "*", it will generater all models by tables.
            If don't input it, the generater will show all the tables.

Example:
    Leafing.CodeGen m User

----------------------------------------------------------------------------

Usage for get assembly fullname:
	Leafing.CodeGen fn assemblyFileName

assemblyFileName:	assembly file name such as dll or exe file name.

Example:
	Leafing.CodeGen fn MyBiz.dll
