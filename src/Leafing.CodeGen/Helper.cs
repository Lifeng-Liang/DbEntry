using System;
using System.Reflection;
using Leafing.Data;

namespace Leafing.CodeGen
{
    public static class Helper
    {
        public static void EnumTypes(string fileName, Func<Type, bool> callback)
        {
            EnumTypes(fileName, false, callback);
        }

        public static void EnumTypes(string fileName, bool needStrongName, Func<Type, bool> callback)
        {
            Assembly dll = Assembly.LoadFile(fileName);
            if(needStrongName)
            {
                if (dll.FullName.EndsWith(", PublicKeyToken=null"))
                {
                    throw new DataException("The assembly should have strong name.");
                }
            }

            var ts = DbEntry.GetAllModels(dll);

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
