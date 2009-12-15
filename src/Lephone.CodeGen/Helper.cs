using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Reflection;
using Lephone.Data;
using Lephone.Util;

namespace Lephone.CodeGen
{
    public static class Helper
    {
        public static void EnumTypes(string fileName, CallbackReturnHandler<Type, bool> callback)
        {
            EnumTypes(fileName, false, callback);
        }

        public static void EnumTypes(string fileName, bool needStrongName, CallbackReturnHandler<Type, bool> callback)
        {
            Assembly dll = Assembly.LoadFile(fileName);
            if(needStrongName)
            {
                Debug.Assert(dll.FullName != null);
                if (dll.FullName.EndsWith(", PublicKeyToken=null"))
                {
                    throw new DataException("The assembly should have strong name.");
                }
            }
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
