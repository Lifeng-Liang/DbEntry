DB Enums
==========

DbEntry also provides an enum type read from database.

The table name is ``LeafingEnum``. It could created by auto create table feature.

And it also designed to work together with .net enums.

It only read when we use it, and won¡¯t read it again.

The following shows how to use it. And this is a sample in the released package:

````c#
public enum DotNetEnum
{
    Debug,
    Release,
    [ShowString("In Design")] InDesign
}

public class Enums
{
    public static readonly DbEnum Headship = new DbEnum(0);
    public static readonly DbEnum Container = new DbEnum(1);
    public static readonly DbEnum Level = new DbEnum(2);
    public static readonly DbEnum Status = new DbEnum(typeof(DotNetEnum));
}

class Program
{
    [STAThread]
    static void Main(string[] args)
    {
        Console.WriteLine("Enum Headship:");
        Show( Enums.Headship );

        Console.WriteLine("Enum Container:");
        Show( Enums.Container );

        Console.WriteLine("Enum Level:");
        Show( Enums.Level );

        Console.WriteLine("Enum Mode:");
        Show( Enums.Status );

        Console.WriteLine( "Enum Container String:\n{0} = {1}\n", 1, Enums.Container[1] );

        Console.ReadLine();
    }

    public static void Show(DbEnum de)
    {
        string[] ss = de.GetNames();
        foreach ( string s in ss )
        {
            Console.WriteLine( "{0} = {1}", s, de[s] );
        }
        Console.WriteLine();
    }
}
````
