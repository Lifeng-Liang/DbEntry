using System;
using System.Collections.Generic;
using System.Reflection;
using Leafing.Core;

namespace Leafing.Data.Model.Handler.Generator
{
    public abstract class AssemblyHandler
    {
        public class DynamicAssemblyHandler : AssemblyHandler
        {
            public override Type GetDbObjectHandler(Type sourceType, ObjectInfo oi)
            {
				var gen = new ModelHandlerGenerator (oi);
				return gen.Generate ();
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
					return Assembly.Load(MemoryAssembly.DefaultAssemblyName + ", Version=0.0.0.0, Culture=neutral, PublicKeyToken=735f278977bae975");
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
                return base.CreateDbObjectHandler(srcType, oi);
            }

            public override Type GetDbObjectHandler(Type sourceType, ObjectInfo oi)
            {
                foreach (var type in _staticAssembly.GetTypes())
                {
					var attr = ClassHelper.GetAttribute<InstanceHandlerAttribute>(type, false);
                    if (attr != null)
                    {
                        if (attr.Type == sourceType)
                        {
                            return type;
                        }
                    }
                }
                throw new DataException("Can not find ObjectHandler for: {0}", sourceType.FullName);
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

        public virtual IDbObjectHandler CreateDbObjectHandler(Type srcType, ObjectInfo oi)
        {
            Type t = GetDbObjectHandler(srcType, oi);
            var o = (EmitObjectHandlerBase)ClassHelper.CreateInstance(t);
            o.Init(oi);
            return o;
        }

        public abstract Type GetDbObjectHandler(Type sourceType, ObjectInfo oi);
    }
}
