Simple IoC
==========

DbEntry.Net provide a simple IoC framework for dependence creation and dependence injection.

The main goal of it is simple but useful. I don't want it to replace other high level IoC framework. Just wish it could handle 70% common scenarios.

It is in Leafing.Util.IoC.

Define objects:

````c#
[DependenceEntry]
public interface ITest
{
    string Run();
}

[Implementation]
public class TestImpl : ITest
{
    public string Run()
    {
        return "1st impl";
    }
}

[Implementation("2nd")]
public class NewTestImpl : ITest
{
    public string Run()
    {
        return "2nd impl";
    }
}
````

Use:

````c#
var t = SimpleContainer.Get<ITest>();
Assert.IsNotNull(t);
Assert.IsTrue(t is TestImpl);

var t2 = SimpleContainer.Get<ITest>("2nd");
Assert.IsNotNull(t2);
Assert.IsTrue(t2 is NewTestImpl);
````

It also works for base class mode and property injection:

````c#
[DependenceEntry, Implementation]
public class IocSame
{
    public virtual string Run()
    {
        return "same";
    }

    [Injection("2nd")]
    public ITest TestProperty { get; set; }
}

[Implementation("sub")]
public class IocSameSub : IocSame
{
    public override string Run()
    {
        return "sub class";
    }
}

public class IocSameReg : IocSame
{
    public override string Run()
    {
        return "reg class";
    }
}
````

Use:

````c#
var t3 = SimpleContainer.Get<IocSame>();
Assert.IsNotNull(t3);
Assert.AreEqual("same", t3.Run());

var t4 = SimpleContainer.Get<IocSame>("sub");
Assert.IsNotNull(t4);
Assert.IsTrue(t4 is IocSameSub);
Assert.AreEqual("sub class", t4.Run());

SimpleContainer.Register<IocSame, IocSameReg>("reg");
var t = SimpleContainer.Get<IocSame>("reg");
Assert.IsNotNull(t);
Assert.AreEqual("reg class", t.Run());
Assert.IsNotNull(t.TestProperty);
Assert.AreEqual("2nd impl", t.TestProperty.Run());
````

And it works for constrctor injection too:

````c#
[DependenceEntry, Implementation]
public class IocConstractor
{
    private readonly ITest test;

    public IocConstractor([Injection("2nd")]ITest test)
    {
        this.test = test;
    }

    public virtual string Run()
    {
        return test.Run();
    }
}
````

Use:

````c#
var item = SimpleContainer.Get<IocConstractor>();
Assert.IsNotNull(item);
Assert.AreEqual("2nd impl", item.Run());
````

Configration
----------

There is a configration item to enable or disable IoC, the default value is true:

````xml
<add key="IOC.EnableAutoLoad" value="false" />
````

And the assemblies which we defined the IoC objects should added to the assembly list:

````xml
<add key="IOC.SearchAssembly.1" value="Leafing.UnitTest" />
<add key="IOC.SearchAssembly.2" value="Leafing.Util" />
````

It will search for the key start at 1 until can not find the next key.

