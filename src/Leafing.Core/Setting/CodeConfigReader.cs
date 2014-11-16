using Leafing.Core.Ioc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Leafing.Core.Setting
{
    [Implementation("Code")]
    public class CodeConfigReader : ConfigReader
    {
        public override System.Collections.Specialized.NameValueCollection GetSection(string sectionName)
        {
            return base.GetSection(sectionName);
        }
    }
}
