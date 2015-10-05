Object Validation
==========

DbEntry provides the validation feature for object model based on the information of itself.

For example, if a field do not mark by ``AllowNull`` attribute and not a nullable type object, and the value of it is null, the validate function will return false and the field name.

Another example, if a string field mark by ``Length`` with 8, and the real length of it is 10, the validate function will return false and the field name.

The validate function check all members of the object, if all passed, it will return true, if any check failed, it will return false.

The validate function belongs to ``ValidataHandler`` and named ``ValidateObject``, it returns bool to tell us the basic result. The ``ValidateHandler`` has the property ``ErrorMessages``. It's a list of all failed message with fields name. We can show it to the end user if we want. The ``ValidateHandler`` also has a field named ``IsValid``, it can gives use the bool result if we miss the return value of ``ValidateObject``.

The arguments of constructor will let us define some acts of the validate function. ``EmptyAsNull`` means the empty string will regarded as null, if the value is get from user input, it will not be null, so this argument makes us can check this situation. ``NotNullFieldMinSize`` makes us can check the min length of the not null string field, normally it will set to 1. ``IncludeClassName`` means the error message of field name will include class name. ``InvalidFieldText`` makes us can set the invalid text by ourselves.

The ``AllowNull`` will check all types of the field. And Length, StringColumn will check string type field only. StringColumn include isUnicode and Reguar checks all.

The attributes not only works for validate, but also works for create table.

The following code is a simple sample of object definition:

````c#
public class vtest : IDbObject
{
    [Length(5), StringColumn(IsUnicode=false)]
    public string Name;

    [AllowNull, Length(8)]
    public string Address;

    [StringColumn(Regular=CommonRegular.EmailRegular)]
    public string Email;

    public vtest() { }

    public vtest(string Name, string Address, string Email)
    {
        this.Name = Name;
        this.Address = Address;
        this.Email = Email;
    }
}
````

And the usage of ``ValidateHandler``:

````c#
ValidateHandler vh = new ValidateHandler(true, 1);
vh.ValidateObject(new vtest("12345", null, "a@b.c")); // true
vh.ValidateObject(new vtest("123456", null, "a@b.c")); // false
vh.ValidateObject(new vtest("", null, "a@b.c")); // false
vh.ValidateObject(new vtest("1", null, "a@b.c")); // true
vh.ValidateObject(new vtest("1", null, "a@b@c")); // false
````
