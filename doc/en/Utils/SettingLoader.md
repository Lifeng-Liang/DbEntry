Setting loader
==========

Load setting from App.config/Web.config is common task for many application. DbEntry provide a simple method to load the key/value style setting from configuration file.

The following shows load settings from the custom key/value section ``DuwamishConfiguration``:

````c#
public static class DuwamishConfiguration
{
    [ShowString("Duwamish.Web.EnablePageCache")]
    public static readonly bool EnablePageCache= true;
 
    [ShowString("Duwamish.Web.PageCacheExpiresInSeconds")]
    public static readonly int PageCacheExpiresInSeconds = 3600;
 
    [ShowString("Duwamish.Web.EnableSsl")]
    public static readonly bool EnableSsl = false;
 
    static DuwamishConfiguration()
    {
        ConfigHelper ch = new ConfigHelper("DuwamishConfiguration");
        ch.InitClass(typeof(DuwamishConfiguration));
    }
}
````

The settings all static and readonly, so we won't change the value by mistake. The ``ConfigHelper`` will set the value if it has a value in the configuration file. And the ``ShowString`` is defined the key in configuration section. If does not define this attribute, it will use the field name as the key. And the value of field in the code is the default value if it does not find in the configuration file.

In DbEntry, it has two pre-defined section in the ``ConfigHelper``: ``DefaultSettings`` and ``AppSettings``.

``DefaultSettings`` pointed to the ``Leafing.Settings`` section.

``AppSettings`` pointed to the ``appSettings`` section.

To use appSettings section is like the following:

````c#
public static class MySetting
{
    public static readonly bool UseText         = false;
    public static readonly string DefaultText   = "";

    static MySetting()
    {
        typeof(MySetting).Initialize();
    }
}
````
