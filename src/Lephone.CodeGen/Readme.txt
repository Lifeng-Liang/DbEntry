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

Usage for Rails actions:
    Lephone.CodeGen ra FileName [ClassPath ActionName]

FileName:   The assembly file name who has the model classes.
ClassPath:  The class you want to generate template code
            If don't input it, the generator will show all the classes name.
ActionName: One of the following: New, Create, List, Show, Edit, Update, 
            Destroy, All

Example:
    Lephone.CodeGen ra Models.dll Models.User All

----------------------------------------------------------------------------

Usage for Rails views:
    Lephone.CodeGen rv FileName [ClassPath ViewName]

FileName:   The assembly file name who has the model classes.
ClassPath:  The class you want to generate template code
            If don't input it, the generator will show all the classes name.
ViewName:   One of the following: New, List, Show, Edit

Example:
    Lephone.CodeGen rv Models.dll Models.User New
