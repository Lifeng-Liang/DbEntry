----------------------------------------------------------------------------

Code Generator For DbEntry.Net
http://www.codeplex.com/DbEntry

----------------------------------------------------------------------------

Usage for Asp.Net:
    Lephone.CodeGen a FileName [ClassPath]

FileName:   The assembly file name who has the model classes.
ClassPath:  The class you want to generate template code
            If don't input it, the generator will show all the classes name.

Example:
    Lephone.CodeGen a Models.dll Models.User

----------------------------------------------------------------------------

Usage for Mvc actions:
    Lephone.CodeGen ra FileName [ClassPath ActionName]

FileName:   The assembly file name who has the model classes.
ClassPath:  The class you want to generate template code
            If don't input it, the generator will show all the classes name.
ActionName: One of the following: New, Create, List, Show, Edit, Update, 
            Destroy, All

Example:
    Lephone.CodeGen ra Models.dll Models.User All

----------------------------------------------------------------------------

Usage for Mvc views:
    Lephone.CodeGen rv FileName [ClassPath ViewName [MasterPage]]

FileName:   The assembly file name who has the model classes.
ClassPath:  The class you want to generate template code
            If don't input it, the generator will show all the classes name.
ViewName:   One of the following: New, List, Show, Edit
MasterPage: Master page link like "~/Master.master", if this argument exist,
            the output code will be master page mode.

Example:
    Lephone.CodeGen rv Models.dll Models.User New ~/Master.master

----------------------------------------------------------------------------

Usage for models from database:
    Lephone.CodeGen m [TableName]

TableName:  Table name in database, and it will be the model class name too.
            If the name is "*", it will generater all models by tables.
            If don't input it, the generater will show all the tables.

Example:
    Lephone.CodeGen m User

----------------------------------------------------------------------------

Usage for generate assembly dll:
    Lephone.CodeGen dll FileName

FileName:   The assembly file name who has the model classes.

Example:
    Lephone.CodeGen dll Models.dll
