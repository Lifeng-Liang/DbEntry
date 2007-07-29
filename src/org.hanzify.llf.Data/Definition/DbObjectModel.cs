
#region usings

using System;
using System.Collections.Generic;
using org.hanzify.llf.Data.Common;
using org.hanzify.llf.Data.QuerySyntax;

#endregion

namespace org.hanzify.llf.Data.Definition
{
    [Serializable]
    public class DbObjectModel<T> : DbObjectSmartUpdate
    {
        protected static CK Col
        {
            get { return CK.Column; }
        }

        public void Save()
        {
            DbEntry.Save(this);
        }

        public void Delete()
        {
            DbEntry.Delete(this);
        }

        public ValidateHandler Validate()
        {
            ValidateHandler v = new ValidateHandler();
            v.ValidateObject(this);
            return v;
        }

        public bool IsValid()
        {
            return Validate().IsValid;
        }

        public static T FindById(long Id)
        {
            return DbEntry.GetObject<T>(Id);
        }

        public static DbObjectList<T> FindBySql(string SqlStr)
        {
            return DbEntry.Context.ExecuteList<T>(SqlStr);
        }

        public static DbObjectList<T> FindBySql(SqlEntry.SqlStatement Sql)
        {
            return DbEntry.Context.ExecuteList<T>(Sql);
        }

        public static DbObjectList<T> FindAll()
        {
            return FindAll(null);
        }

        public static DbObjectList<T> FindAll(OrderBy ob)
        {
            return DbEntry.From<T>().Where(null).OrderBy(ob).Select();
        }

        public static DbObjectList<T> Find(WhereCondition con)
        {
            return Find(con, null);
        }

        public static DbObjectList<T> Find(WhereCondition con, OrderBy ob)
        {
            return DbEntry.From<T>().Where(con).OrderBy(ob).Select();
        }

        public static T FindOne(WhereCondition con)
        {
            return FindOne(con, null);
        }

        public static T FindOne(WhereCondition con, OrderBy ob)
        {
            return DbEntry.GetObject<T>(con, ob);
        }

        public static DbObjectList<T> FindRecent(int Count)
        {
            return DbEntry.From<T>().Where(null).OrderBy((DESC)"Id").Range(1, Count).Select();
        }

        public static long GetCount(WhereCondition con)
        {
            return DbEntry.From<T>().Where(con).GetCount();
        }

        public static T New(params object[] os)
        {
            return DynamicObject.NewObject<T>(os);
        }

        public static void DeleteAll()
        {
            DeleteAll(null);
        }

        public static void DeleteAll(WhereCondition con)
        {
            DbEntry.Delete<T>(con);
        }
    }
}
