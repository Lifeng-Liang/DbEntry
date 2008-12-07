using System;
using System.Collections.Generic;
using System.Reflection;
using Lephone.Util;

namespace Lephone.CodeGen
{
    public static class Helper
    {
        public static void EnumTypes(string fileName, CallbackHandler<Type, bool> callback)
        {
            Assembly dll = Assembly.LoadFile(fileName);
            Type idot = Type.GetType("Lephone.Data.Definition.IDbObject, Lephone.Data", true);
            var ts = new List<Type>();
            foreach (Type t in dll.GetExportedTypes())
            {
                var lt = new List<Type>(t.GetInterfaces());
                if (lt.Contains(idot))
                {
                    ts.Add(t);
                }
            }
            ts.Sort(new TypeComparer());
            foreach (Type t in ts)
            {
                if (!callback(t))
                {
                    break;
                }
            }
        }
    }
}
