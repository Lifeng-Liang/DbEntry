ActsAsTree
==========

Sometimes we need the object works like tree. For example: the directory structure.

It is a table has column to point itself. And the class has children and parent property point itself to tour the linked nodes.

If we have a table like following:

| Id  | Name          | BelongsTo_Id |
| --- | ------------- | ------------ |
| 1   | Root          | 0            |
| 2   | Windows       | 1            |
| 3   | Program Files | 1            |
| 4   | Tools         | 1            |
| 5   | Config.sys    | 1            |
| 6   | Command.com   | 1            |
| 7   | regedit.exe   | 2            |
| 8   | notepad.exe   | 2            |
| 9   | System32      | 2            |
| 10  | regsvr32.exe  | 9            |
| 11  | Office        | 3            |
| 12  | Word.exe      | 11           |
| 13  | Outlook.exe   | 11           |
| 14  | Excel.exe     | 11           |
| 15  | LocPlus       | 4            |
| 16  | cConv         | 4            |
| 17  | LocPlus.exe   | 15           |
| 18  | LocPlus.ini   | 15           |
| 19  | cConv.exe     | 16           |
| 20  | cConv.ini     | 16           |

We can define the class like following :

````c#
public class File : DbObjectModelAsTree<File>
{
    public string Name { get; set; }
}
````

It implements by ``HasMany`` and ``BelongsTo`` in DbEntry. It's same as:

````c#
public class File : DbObjectModel<File>
{
    [OrderBy("Id")]
    public HasMany<File> Children { get; private set; }

    [DbColumn("BelongsTo_Id")]
    public BelongsTo<File> _Parent { get; private set; }

    [Exclude]
    public File Parent {
    	get { return _Parent.Value; }
    	set { _Parent.Value = value; }
    }

    public string Name { get; set; }
}
````

When we use ``ActsAsTree`` class, we don't have chance to change the order by and parent id column name. If we want define the order-by and parent id column name by ourselves, we need use ``HasMany`` and ``BelongsTo`` like above.

Now, we can read it now:

````c#
File f = File.FindById(1);
Assert.AreEqual("Root", f.Name);

Assert.IsNull(f.Parent);
Assert.AreEqual(5, f.Children.Count);
Assert.AreEqual("Windows", f.Children[0].Name);
Assert.AreEqual("Program Files", f.Children[1].Name);
Assert.AreEqual("Tools", f.Children[2].Name);
Assert.AreEqual("Config.sys", f.Children[3].Name);
Assert.AreEqual("Command.com", f.Children[4].Name);
````

And we can write it too:

````c#
File f = File.FindById(16);
File nf = new File{Name = "gbk.tbl"};
f.Children.Add(nf);
f.Save();
````

Have fun!
