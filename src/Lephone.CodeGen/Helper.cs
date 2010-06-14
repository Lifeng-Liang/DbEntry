using System;
using System.Reflection;
using Lephone.Data;
using Lephone.Core;

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
                if (dll.FullName == null || dll.FullName.EndsWith(", PublicKeyToken=null"))
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
