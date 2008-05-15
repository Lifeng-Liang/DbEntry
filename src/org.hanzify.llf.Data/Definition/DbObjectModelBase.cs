using System;
using Lephone.Data.Common;

namespace Lephone.Data.Definition
{
    [Serializable]
    public class DbObjectModelBase<T, TKey> : DbObjectSmartUpdate where T : DbObjectModelBase<T, TKey>
    {
        protected static CK Col
        {
            get { return CK.Column; }
        }

        protected static FieldNameGetter<T> Field
        {
            get { return CK<T>.Field; }
        }

        public static T FindById(TKey Id)
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
            return DbEntry.From<T>().Where(WhereCondition.EmptyCondition).Select();
        }

        public static DbObjectList<T> FindAll(OrderBy ob)
        {
            return DbEntry.From<T>().Where(WhereCondition.EmptyCondition).OrderBy(ob).Select();
        }

        public static DbObjectList<T> FindAll(string orderBy)
        {
            return DbEntry.From<T>().Where(WhereCondition.EmptyCondition).OrderBy(orderBy).Select();
        }

        public static DbObjectList<T> Find(WhereCondition con)
        {
            return DbEntry.From<T>().Where(con).Select();
        }

        public static DbObjectList<T> Find(WhereCondition con, OrderBy ob)
        {
            return DbEntry.From<T>().Where(con).OrderBy(ob).Select();
        }

        public static DbObjectList<T> Find(WhereCondition con, string orderBy)
        {
            return DbEntry.From<T>().Where(con).OrderBy(orderBy).Select();
        }

        public static T FindOne(WhereCondition con)
        {
            return DbEntry.GetObject<T>(con);
        }

        public static T FindOne(WhereCondition con, OrderBy ob)
        {
            return DbEntry.GetObject<T>(con, ob);
        }

        public static T FindOne(WhereCondition con, string orderBy)
        {
            return DbEntry.GetObject<T>(con, OrderBy.Parse(orderBy));
        }

        public static DbObjectList<T> FindRecent(int Count)
        {
            string Id = ObjectInfo.GetKeyField(typeof(T)).Name;
            return DbEntry.From<T>().Where(WhereCondition.EmptyCondition).OrderBy((DESC)Id).Range(1, Count).Select();
        }

        public static long GetCount(WhereCondition con)
        {
            return DbEntry.From<T>().Where(con).GetCount();
        }

        public static T New()
        {
            return DynamicObject.NewObject<T>();
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
