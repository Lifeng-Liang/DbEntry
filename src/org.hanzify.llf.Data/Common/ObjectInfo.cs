
#region usings

using System;
using System.Reflection;
using org.hanzify.llf.Data.Builder.Clause;

#endregion

namespace org.hanzify.llf.Data.Common
{
	public class ObjectInfo
	{
        internal IDbObjectHandler Handler;

		public FromClause From;
        public bool HasSystemKey;
        public bool HasAssociate;
        public bool IsAssociateObject;
        public MemberHandler BelongsToField;
		public MemberHandler[] KeyFields;
		public MemberHandler[] Fields;
        public MemberHandler[] SimpleFields;
        public MemberHandler[] RelationFields;
        public bool DisableSqlLog = false;
        public bool HasOnePremarykey;
        public FromClause ManyToManyMediFrom = null;
        public string ManyToManyMediTableName = null;
        public string ManyToManyMediColumeName1 = null;
        public string ManyToManyMediColumeName2 = null;

        public ObjectInfo(string TableName, MemberHandler[] KeyFields, MemberHandler[] Fields, bool DisableSqlLog)
            : this(new FromClause(TableName), KeyFields, Fields, DisableSqlLog)
		{
		}

        public ObjectInfo(FromClause From, MemberHandler[] KeyFields, MemberHandler[] Fields, bool DisableSqlLog)
        {
            this.From = From;
            this.KeyFields = KeyFields;
            this.Fields = Fields;
            this.DisableSqlLog = DisableSqlLog;

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
                    if (BelongsToField != null)
                    {
                        throw new DbEntryException("An class only allow one BelongsTo field.");
                    }
                    else
                    {
                        BelongsToField = f;
                    }
                }
            }

            HasOnePremarykey = (KeyFields != null && KeyFields.Length == 1);
        }

        public object NewObject()
        {
            return Handler.CreateInstance();
        }
    }
}
