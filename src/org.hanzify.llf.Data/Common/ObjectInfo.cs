
#region usings

using System;
using System.Collections.Generic;
using System.Reflection;
using Lephone.Data.SqlEntry;
using Lephone.Data.Builder.Clause;
using Lephone.Util.Logging;
using Lephone.Util;

#endregion

namespace Lephone.Data.Common
{
	public class ObjectInfo
	{
        internal IDbObjectHandler Handler;
        internal QueryComposer Composer;

        public string SoftDeleteColumnName;

        public Dictionary<string, List<ASC>> Indexes = new Dictionary<string, List<ASC>>();
        public Dictionary<string, List<MemberHandler>> UniqueIndexes = new Dictionary<string, List<MemberHandler>>();

        public Type BaseType;
        public Type HandleType;
		public FromClause From;
        public string DeleteToTableName;
        public bool HasSystemKey;
        public bool HasAssociate;
        public bool IsAssociateObject;
		public MemberHandler[] KeyFields;
		public MemberHandler[] Fields;
        public MemberHandler[] SimpleFields;
        public MemberHandler[] RelationFields;
        public bool AllowSqlLog = true;
        public bool HasOnePremarykey;
        public Dictionary<Type, ManyToManyMediTable> ManyToManys = new Dictionary<Type,ManyToManyMediTable>();

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

            this.HasSystemKey = ((KeyFields.Length == 1) && (KeyFields[0].IsDbGenerate || KeyFields[0].FieldType == typeof(Guid)));

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
                throw new DataException("GetPrimaryKeyDefaultValue don't support multi key.");
            }
            return CommonHelper.GetEmptyValue(KeyFields[0].FieldType, false, "only supported int long guid as primary key.");
        }

        public void LogSql(SqlStatement Sql)
        {
            if (AllowSqlLog)
            {
                Logger.SQL.Trace(Sql);
            }
        }

        public bool IsNewObject(object obj)
        {
            return KeyFields[0].UnsavedValue.Equals(Handler.GetKeyValue(obj));
        }
    }
}
