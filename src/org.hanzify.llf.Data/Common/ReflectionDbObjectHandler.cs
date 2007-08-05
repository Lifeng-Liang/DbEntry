
using System;
using System.Collections.Generic;
using System.Text;
using System.Reflection;

namespace org.hanzify.llf.Data.Common
{
    public class ReflectionDbObjectHandler : IDbObjectHandler
    {
        private static readonly object[] os = new object[] { };

        private ConstructorInfo Creator;

        public ReflectionDbObjectHandler(Type srcType)
        {
            ConstructorInfo ci = srcType.GetConstructor(new Type[] { });
            Creator = ci;
        }

        public object CreateInstance()
        {
            return Creator.Invoke(os);
        }
    }
}
