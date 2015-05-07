Cache System
==========

Cache Sample
----------

DbEntry.Net has built-in cache support.

To use it, please follow the steps:

Set the App.config can operate the database you like.

Mark the object model as cacheable:

````c#
[Cacheable]
public class User : DbObjectModel<User>
{
    Public string Name { get; set; }
}
````

Set the CacheEnabled as true and set a SQL recorder in App.config:

````xml
<Leafing.Settings>
  <add key="SqlLogRecorder" value="@ConsoleMessage" />
  <add key="CacheEnabled" value="true" />
</Leafing.Settings>
````

Now, let¡¯s test it:

````c#
static void Main(string[] args)
{
    var d = SampleData.FindById(1);

    Console.WriteLine(d);
    Console.WriteLine();

    d.Name = "test cache";
    d.Save();

    d = SampleData.FindById(1); // read from cache
    Console.WriteLine(d);
    Console.WriteLine();

    d.Name = "( 1)liang lifeng";
    d.Save();
    Console.ReadLine();
}
````

As the result shows, the 2nd FindById function will read object from cache, so we can¡¯t find the SQL of it.

How it works
----------

When we set the CacheEnabled to true, the following things will happen:

* If we use GetObject<T>(object Key) to read a object, it will check the cache pool first. If it existed, DbEntry will create a copy of it and return it. If not, DbEntry will read it from database and insert it to cache pool.
* When we use Insert or Update to save the object, it will check the cache pool first too. If it existed, it will refresh the expired time with the object. If not, insert it to cache pool.
* When we insert a new object into cache pool, it will check the max size of the pool, if equal or greater then the max size, it will use Clear() function to reset the cache pool.
* If any exception raised from a transaction, the Clear() function will be called.

Configuration
----------

We have some parameters to define the cache system in App.config:

| Key                  | Default | Memo                        |
| -------------------- | ------- | --------------------------- |
| CacheEnabled         | false   |                             |
| CacheAnySelectedItem | false   | define as all selected items add to cache, if it is false, only the result of GetObject<T>(object Key) will add to cache |
| CacheSize            | 1000    | the max items in cache pool |
| CacheMinutes         | 5       | the minutes of expired time |

There are two more items to define the cache provider and key generator:

| Key               | Default                                                    |
| ----------------- | ---------------------------------------------------------- |
| CacheProvider     | Leafing.Data.Caching.StaticHashCacheProvider, Leafing.Data |
| CacheKeyGenerator | Leafing.Data.Caching.KeyGenerator, Leafing.Data            |

Cache Provider
----------

DbEntry.Net only provides a static hash table based cache provider, it¡¯s the default of configuration.

We can implement our cache provider by inherit from CacheProvider:

````c#
using Leafing.Data.Caching;
public class MemcachedCacheProvider : CacheProvider
{
    // implements the abstract functions
}
````

Key Generator
----------

The default key generator is based by following rules:

Use the object type's full-name plus comma plus the key as the cache key.

It works for almost all situations. But if we need more details for build the cache key, the FullKeyGenerator generate the cache key plus the assembly full name.

Or if we want other rules to generate the key, we can inherits from KeyGenerator, and override the GetKey function such as FullKeyGenerator done.

Manage Cache Directly
----------

To manage cache dirctly, please use the following code:

````c#
Leafing.Data.Caching.CacheProvider.Instance[key] = value;
Leafing.Data.Caching.CacheProvider.Instance.Remove(Key);
Leafing.Data.Caching.CacheProvider.Instance.Clear();
````
