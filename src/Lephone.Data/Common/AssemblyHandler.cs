using System;
using System.Collections.Generic;
using System.Reflection;
using Lephone.Util;

namespace Lephone.Data.Common
{
    public abstract class AssemblyHandler
    {
        public class DynamicAssemblyHandler : AssemblyHandler
        {
            public override Type GetDbObjectHandler(Type sourceType, ObjectInfo oi)
            {
                return DynamicObjectBuilder.Instance.GetDbObjectHandler(sourceType, oi);
            }

            public override Type GetImplementedType(Type sourceType)
            {
                return DynamicObjectBuilder.Instance.GenerateType(sourceType);
            }
        }

        public class StaticAssemblyHandler : AssemblyHandler
        {
            private readonly Assembly _staticAssembly;

            public static bool Useable()
            {
                if (GetAssmbly() == null)
                {
                    return false;
                }
                return true;
            }

            private static Assembly GetAssmbly()
            {
                try
                {
                    return Assembly.Load("DbEntry_MemoryAssembly, Version=0.0.0.0, Culture=neutral, PublicKeyToken=735f278977bae975");
                }
                catch (Exception)
                {
                    return null;
                }
            }

            public StaticAssemblyHandler()
            {
                _staticAssembly = GetAssmbly();
            }

            public override IDbObjectHandler CreateDbObjectHandler(Type srcType, ObjectInfo oi)
            {
                if (srcType.IsGenericType)
                {
                    return new ReflectionDbObjectHandler(srcType, oi);
                }
                return base.CreateDbObjectHandler(srcType, oi);
            }

            public override Type GetDbObjectHandler(Type sourceType, ObjectInfo oi)
            {
                var attr = ClassHelper.GetAttribute<ModelHandlerAttribute>(sourceType, false);
                if(attr != null)
                {
                    return attr.Type;
                }
                throw new DataException("Can not find ObjectHandler for: {0}", sourceType.FullName);
            }

            public override Type GetImplementedType(Type sourceType)
            {
                foreach (var type in _staticAssembly.GetTypes())
                {
                    if (type.BaseType == sourceType)
                    {
                        return type;
                    }
                }
                throw new DataException("Can not find implemented type for: {0}", sourceType.FullName);
            }
        }

        public static AssemblyHandler Instance;

        static AssemblyHandler()
        {
            if(StaticAssemblyHandler.Useable())
            {
                Instance = new StaticAssemblyHandler();
            }
            else
            {
                Instance = new DynamicAssemblyHandler();
            }
        }

        private static readonly Dictionary<Type, Type> Jar = new Dictionary<Type, Type>();

        public virtual IDbObjectHandler CreateDbObjectHandler(Type srcType, ObjectInfo oi)
        {
            Type t = GetDbObjectHandler(srcType, oi);
            var o = (EmitObjectHandlerBase)ClassHelper.CreateInstance(t);
            o.Init(oi);
            return o;
        }

        public abstract Type GetDbObjectHandler(Type sourceType, ObjectInfo oi);


        public virtual Type GetImplType(Type sourceType)
        {
            if (Jar.ContainsKey(sourceType))
            {
                return Jar[sourceType];
            }
            lock (Jar)
            {
                if (Jar.ContainsKey(sourceType))
                {
                    return Jar[sourceType];
                }
                Type t = GetImplementedType(sourceType);
                Jar[sourceType] = t;
                return t;
            }
        }

        public abstract Type GetImplementedType(Type sourceType);
    }
}
