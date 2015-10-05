Helpers
==========

DbEntry provide some helpers to let us coding easily.

``ClassHelper`` provide some functions to create instance or call private method easily.

````c#
ClassHelper.CreateInstance<User>("tom");
ClassHelper.SetValue<User>("m_Id", 10);
ClassHelper.CallFunction<User>("InnerSetAge", 36);
````

``CommonHelper`` provide some functions to make the common task ease.

````c#
CommonHelper.IfCatchException(true, delegate()
{
    int n = 0;
    n = 5 / n;
});
CommonHelper.TryEnumerate(obj, delegate(subobj)
{
    Process(subobj);
});
````

``Rand`` provide some functions to get the thread safe random.

````c#
Rand.NextDouble();
````

>``RemotingHelper`` provide some functions to set the remoting application easy. The code of use it could be find in the samples of the released package. In recent version, this class moved to samples - orm - remoting - common.

``ResourceHelper`` provide some functions to get the embedded resource file easily.

````c#
string s = ResourceHelper.ReadToEnd(TypeOf(Program), "res.profile.txt");
````

``SystemHelper`` provide some functions to get the information from system such as ExeFileName, BaseDirectory, calling function name etc.

````c#
string configFileName = SystemHelper.BaseDirectory + "config.xml";
````

``ThreadingQueue`` provide a queue with thread safe multi input and output by every thread. For usage of it, please see the unit tests.

``TimeSpanCounter`` provide a simple time difference counter:

````c#
TimeSpanCounter tc = new TimeSpanCounter();
doSomethingForCount();
Console.WriteLine(tc);
````

``UrlBuilder`` provide a simple and safe builder for url, it will encode the arguments of the request. _(It belongs to Leafing.Web now.)_

````c#
UrlBuilder ub = new UrlBuilder("http://llf.hanzify.org/search.asp");
ub.Add("Where", "title");
ub.Add("keyword", "DbEntry");
string url = ub.ToString();
````
