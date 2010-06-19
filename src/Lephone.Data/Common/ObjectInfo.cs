using System;
using System.Collections.Generic;
using Lephone.Data.Builder.Clause;
using Lephone.Core;
using Lephone.Data.Definition;

namespace Lephone.Data.Common
{
    public partial class ObjectInfo : ObjectInfoBase
    {
        public static readonly ObjectInfoFactory Factory = new ObjectInfoFactory();

        public static ObjectInfo GetInstance(Type type)
        {
            return Factory.GetInstance(type);
        }

        internal QueryComposer Composer;
        public Type[] CreateTables;
        public bool Cacheable;

        internal ObjectInfo(Type handleType, FromClause from, MemberHandler[] keyFields, MemberHandler[] fields, bool disableSqlLog)
        {
            Init(handleType, from, keyFields, fields, disableSqlLog);
        }

        internal protected ObjectInfo(Type t)
            : base(t)
        {
            // binding QueryComposer
            if (!string.IsNullOrEmpty(SoftDeleteColumnName))
            {
                Composer = new SoftDeleteQueryComposer(this, SoftDeleteColumnName);
            }
            else if (!string.IsNullOrEmpty(DeleteToTableName))
            {
                Composer = new DeleteToQueryComposer(this);
            }
            else if (LockVersion != null)
            {
                Composer = new OptimisticLockingQueryComposer(this);
            }
            else
            {
                Composer = new QueryComposer(this);
            }
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
            //InitContext();
        }
    }
}
