About GAC
==========

GAC is the short name of global assembly cache. If an assembly in GAC, we can get it from everywhere by its full name. 

As we know, .net use JIT _(just in time)_ compiler to compile the MSIL to native code, the GAC assemblies are pre-compiled, so it has a little faster loading speed than the assemblies not in GAC.

And, assembly in GAC has more permission to assess system resources, sometimes we need more permission we can stored the assembly to GAC too. For example, when we using SharePoint, if our web part need to read/write file permission…

Recently, I installed the new version provider of SQLite, it not install it into GAC anymore. Yes, it can be copied to the exe file folder and running well. But if we don’t want copy it at every project created, we can store it into GAC.

The assembly which want to store into GAC must have a strong name.

To store an assembly into GAC, we can navigate to ``c:\windows\assembly`` folder by windows explore, and drag the assembly to this folder. Attention we MUST drag it into this folder NOT copy it. After that, we already stored the assembly into GAC.

Using command line to install an assembly into GAC like following:

````
gacutil -i test.dll
````

