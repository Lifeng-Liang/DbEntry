using System;
using System.Collections.Generic;
using Leafing.Core.Ioc;
using Leafing.Web.Mvc.Core;

namespace Leafing.UnitTest.Mocks
{
    [Implementation(2)]
    public class MockHttpContextHandler : HttpContextHandler
    {
        public static Dictionary<string, string> Request = new Dictionary<string, string>();
        public static string MockApplicationPath = "http://dbentry.codeplex.com/";
        public static Uri MockUrlReferrer = new Uri("http://llf.hanzify.org");
        public static string MockRawUrl = "http://dbentry.codeplex.com/";
        public static string MockAppRelativeCurrentExecutionFilePath = "~/test/add";
        public static string LastWriteMessage;

        public override string this[string name]
        {
            get
            {
                if (Request.ContainsKey(name))
                {
                    return Request[name];
                }
                return null;
            }
        }

        public override string ApplicationPath
        {
            get { return MockApplicationPath; }
        }

        public override Uri UrlReferrer
        {
            get { return MockUrlReferrer; }
        }

        public override string RawUrl
        {
            get
            {
                return MockRawUrl;
            }
        }

        public override string[] GetAllKeys()
        {
            var list = new List<string>(Request.Keys);
            return list.ToArray();
        }

        public override string AppRelativeCurrentExecutionFilePath
        {
            get { return MockAppRelativeCurrentExecutionFilePath; }
        }

        public override void Write(string s)
        {
            LastWriteMessage = s;
        }
    }
}
