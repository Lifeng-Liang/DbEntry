using System;
using System.Collections.Generic;
using Lephone.Core;
using Lephone.Data.Definition;

namespace Lephone.Data.Common
{
    public class ObjectInfo : ObjectInfoBase
    {
        public Type[] CreateTables;
        public bool Cacheable;

        internal protected ObjectInfo(Type t)
            : base(t)
        {
            CheckType(t);
            // get create tables
            if (From.PartOf != null)
            {
                CreateTables = new[] { From.PartOf };
            }
            else if (From.JoinClauseList != null)
            {
                var tables = new Dictionary<Type, int>();
                foreach (var joinClause in From.JoinClauseList)
                {
                    if (joinClause.Type1 != null)
                    {
                        tables[joinClause.Type1] = 1;
                    }
                    if (joinClause.Type2 != null)
                    {
                        tables[joinClause.Type2] = 1;
                    }
                }
                if (tables.Count > 0)
                {
                    CreateTables = new List<Type>(tables.Keys).ToArray();
                }
            }

            if (ClassHelper.HasAttribute<CacheableAttribute>(t, false))
            {
                Cacheable = true;
            }
        }

        private static void CheckType(Type t)
        {
            if (t.IsNotPublic)
            {
                throw new ModelException(t, "The model class should be public.");
            }
            var c = ClassHelper.GetArgumentlessConstructor(t);
            if (c == null)
            {
                throw new ModelException(t, "The model need a public/protected(DbObjectModel) argumentless constructor");
            }
        }

        internal static MemberHandler GetKeyField(Type type)
        {
            // TODO: change the way to use cached ObjectInfo first
            var oi = new ObjectInfoBase(type);
            if (oi.KeyFields.Length > 0)
            {
                return oi.KeyFields[0];
            }
            return null;
        }
    }
}
