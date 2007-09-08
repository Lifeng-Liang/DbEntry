
#region usings

using System;
using System.Reflection;
using Lephone.Data.SqlEntry;
using Lephone.Data.Builder.Clause;
using Lephone.Util.Logging;

#endregion

namespace Lephone.Data.Common
{
	public class ObjectInfo
	{
        internal IDbObjectHandler Handler;
        internal QueryComposer Composer;

        public string SoftDeleteColumnName;

        public Type HandleType;
		public FromClause From;
        public bool HasSystemKey;
        public bool HasAssociate;
        public bool IsAssociateObject;
		public MemberHandler[] KeyFields;
		public MemberHandler[] Fields;
        public MemberHandler[] SimpleFields;
        public MemberHandler[] RelationFields;
        public bool AllowSqlLog = true;
        public bool HasOnePremarykey;
        public FromClause ManyToManyMediFrom = null;
        public string ManyToManyMediTableName = null;
        public string ManyToManyMediColumeName1 = null;
        public string ManyToManyMediColumeName2 = null;

        public ObjectInfo(Type HandleType, string TableName, MemberHandler[] KeyFields, MemberHandler[] Fields, bool DisableSqlLog)
            : this(HandleType, new FromClause(TableName), KeyFields, Fields, DisableSqlLog)
		{
		}

        public ObjectInfo(Type HandleType, FromClause From, MemberHandler[] KeyFields, MemberHandler[] Fields, bool DisableSqlLog)
        {
            this.HandleType = HandleType;
            this.From = From;
            this.KeyFields = KeyFields;
            this.Fields = Fields;
            this.AllowSqlLog = !DisableSqlLog;

            this.HasSystemKey = ((KeyFields.Length == 1) && KeyFields[0].IsDbGenerate);

            foreach (MemberHandler f in Fields)
            {
                if (f.IsHasOne || f.IsHasMany || f.IsHasAndBelongsToMany)
                {
                    HasAssociate = true;
                    IsAssociateObject = true;
                }
                if (f.IsLazyLoad)
                {
                    IsAssociateObject = true;
                }
                if (f.IsBelongsTo || f.IsHasAndBelongsToMany) // TODO: no problem ?
                {
                    IsAssociateObject = true;
                }
            }

            HasOnePremarykey = (KeyFields != null && KeyFields.Length == 1);
        }

        public object NewObject()
        {
            return Handler.CreateInstance();
        }

        internal MemberHandler GetBelongsTo(Type t)
        {
            Type mt = t.IsAbstract ? DynamicObject.GetImplType(t) : t;
            foreach (MemberHandler mh in RelationFields)
            {
                if (mh.IsBelongsTo)
                {
                    Type st = mh.FieldType.GetGenericArguments()[0];
                    if (st.IsAbstract)
                    {
                        st = DynamicObject.GetImplType(st);
                    }
                    if (st == mt)
                    {
                        return mh;
                    }
                }
            }
            return null;
            //throw new DbEntryException("Can't find belongs to field of type {0}", t);
        }

        internal MemberHandler GetHasAndBelongsToMany(Type t)
        {
            Type mt = t.IsAbstract ? DynamicObject.GetImplType(t) : t;
            foreach (MemberHandler mh in RelationFields)
            {
                if (mh.IsHasAndBelongsToMany)
                {
                    Type st = mh.FieldType.GetGenericArguments()[0];
                    if (st.IsAbstract)
                    {
                        st = DynamicObject.GetImplType(st);
                    }
                    if (st == mt)
                    {
                        return mh;
                    }
                }
            }
            return null;
            //throw new DbEntryException("Can't find belongs to field of type {0}", t);
        }

        public object GetPrimaryKeyDefaultValue()
        {
            if (KeyFields.Length > 1)
            {
                throw new DbEntryException("GetPrimaryKeyDefaultValue don't support multi key.");
            }
            Type fkType = KeyFields[0].FieldType;
            if (fkType == typeof(int))
                return 0;
            else if (fkType == typeof(long))
                return 0L;
            else if (fkType == typeof(Guid))
                return Guid.Empty;
            else
                throw new NotSupportedException("only supported int long guid as primary key.");
        }

        public void LogSql(SqlStatement Sql)
        {
            if (AllowSqlLog)
            {
                Logger.SQL.Trace(Sql);
            }
        }
    }
}
