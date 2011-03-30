using System;
using System.Linq.Expressions;
using Lephone.Data.Model;
using Lephone.Data.Model.Linq;
using Lephone.Data.Model.QuerySyntax;
using Lephone.Data.SqlEntry.Dynamic;

namespace Lephone.Data.Definition
{
    [Serializable]
    public class DbObjectModelBase<T, TKey> : DbObjectSmartUpdate where T : DbObjectModelBase<T, TKey> where TKey : struct
    {
        protected static ModelContext ModelContext = ModelContext.GetInstance(typeof(T));

        internal override ModelContext Context
        {
            get { return ModelContext; }
        }

        protected static CK Col
        {
            get { return CK.Column; }
        }

        //protected static FieldNameGetter<T> Field
        //{
        //    get { return CK<T>.Field; }
        //}

        public static dynamic FindBy
        {
            get { return new DynamicQuery<T>(); }
        }

        public static T FindById(TKey? id)
        {
            return ModelContext.Operator.GetObject<T>(id);
        }

        public static DbObjectList<T> FindBySql(string sqlStr)
        {
            return ModelContext.Operator.ExecuteList<T>(sqlStr);
        }

        public static DbObjectList<T> FindBySql(SqlEntry.SqlStatement sql)
        {
            return ModelContext.Operator.ExecuteList<T>(sql);
        }

        public static DbObjectList<T> FindAll()
        {
            return ModelContext.From<T>().Where(Condition.Empty).Select();
        }

        public static DbObjectList<T> FindAll(OrderBy ob)
        {
            return ModelContext.From<T>().Where(Condition.Empty).OrderBy(ob).Select();
        }

        public static DbObjectList<T> FindAll(string orderBy)
        {
            return ModelContext.From<T>().Where(Condition.Empty).OrderBy(orderBy).Select();
        }

        public static DbObjectList<T> Find(Condition con)
        {
            return ModelContext.From<T>().Where(con).Select();
        }

        public static DbObjectList<T> Find(Condition con, OrderBy ob)
        {
            return ModelContext.From<T>().Where(con).OrderBy(ob).Select();
        }

        public static DbObjectList<T> Find(Condition con, string orderBy)
        {
            return ModelContext.From<T>().Where(con).OrderBy(orderBy).Select();
        }

        public static T FindOne(Condition con)
        {
            return ModelContext.Operator.GetObject<T>(con);
        }

        public static T FindOne(Condition con, OrderBy ob)
        {
            return ModelContext.Operator.GetObject<T>(con, ob);
        }

        public static T FindOne(Condition con, string orderBy)
        {
            return ModelContext.Operator.GetObject<T>(con, Data.OrderBy.Parse(orderBy));
        }

        public static IAfterWhere<T> Where(Condition con)
        {
            return new QueryContent<T>(ModelContext).Where(con);
        }

        public static DbObjectList<T> FindRecent(int count)
        {
            string id = ModelContext.Info.KeyMembers[0].Name;
            return ModelContext.From<T>().Where(Condition.Empty).OrderBy((DESC)id).Range(1, count).Select();
        }

        public static long GetCount(Condition con)
        {
            return ModelContext.From<T>().Where(con).GetCount();
        }

        public static decimal? GetMax(Condition con, string columnName)
        {
            return ModelContext.From<T>().Where(con).GetMax(columnName);
        }

        public static DateTime? GetMaxDate(Condition con, string columnName)
        {
            return ModelContext.From<T>().Where(con).GetMaxDate(columnName);
        }

        public static decimal? GetMin(Condition con, string columnName)
        {
            return ModelContext.From<T>().Where(con).GetMin(columnName);
        }

        public static DateTime? GetMinDate(Condition con, string columnName)
        {
            return ModelContext.From<T>().Where(con).GetMinDate(columnName);
        }

        public static decimal? GetSum(Condition con, string columnName)
        {
            return ModelContext.From<T>().Where(con).GetSum(columnName);
        }

        public static void DeleteAll()
        {
            DeleteAll(Condition.Empty);
        }

        public static void DeleteAll(Condition con)
        {
            ModelContext.Operator.Delete(con);
        }

        #region Linq methods

        public static LinqQueryProvider<T, T> Table
        {
            get { return new LinqQueryProvider<T, T>(null); }
        }

        public static IAfterWhere<T> Where(Expression<Func<T, bool>> condition)
        {
            return new QueryContent<T>(ModelContext).Where(condition);
        }

        public static DbObjectList<T> Find(Expression<Func<T, bool>> condition)
        {
            return ModelContext.From<T>().Where(condition).Select();
        }

        public static DbObjectList<T> Find(Expression<Func<T, bool>> condition, Expression<Func<T, object>> orderby)
        {
            return ModelContext.From<T>().Where(condition).OrderBy(orderby).Select();
        }

        public static DbObjectList<T> Find(Expression<Func<T, bool>> condition, string orderby)
        {
            return ModelContext.From<T>().Where(condition).OrderBy(orderby).Select();
        }

        public static DbObjectList<T> FindAll(Expression<Func<T, object>> orderby)
        {
            return ModelContext.From<T>().Where(Condition.Empty).OrderBy(orderby).Select();
        }

        public static T FindOne(Expression<Func<T, bool>> condition)
        {
            return ModelContext.Operator.GetObject(condition);
        }

        public static LinqOrderSyntax<T> OrderBy(Expression<Func<T, object>> orderby)
        {
            return new LinqOrderSyntax<T>(orderby, true);
        }

        public static LinqOrderSyntax<T> OrderByDescending(Expression<Func<T, object>> orderby)
        {
            return new LinqOrderSyntax<T>(orderby, false);
        }

        public static long GetCount(Expression<Func<T, bool>> condition)
        {
            return ModelContext.From<T>().Where(condition).GetCount();
        }

        public static decimal? GetMax(Expression<Func<T, bool>> condition, Expression<Func<T, object>> column)
        {
            return ModelContext.From<T>().Where(condition).GetMax(column);
        }

        public static DateTime? GetMaxDate(Expression<Func<T, bool>> condition, Expression<Func<T, object>> column)
        {
            return ModelContext.From<T>().Where(condition).GetMaxDate(column);
        }

        public static decimal? GetMin(Expression<Func<T, bool>> condition, Expression<Func<T, object>> column)
        {
            return ModelContext.From<T>().Where(condition).GetMin(column);
        }

        public static DateTime? GetMinDate(Expression<Func<T, bool>> condition, Expression<Func<T, object>> column)
        {
            return ModelContext.From<T>().Where(condition).GetMinDate(column);
        }

        public static decimal? GetSum(Expression<Func<T, bool>> condition, Expression<Func<T, object>> column)
        {
            return ModelContext.From<T>().Where(condition).GetSum(column);
        }

        public static int DeleteAll(Expression<Func<T, bool>> condition)
        {
            return ModelContext.Operator.Delete(ExpressionParser<T>.Parse(condition));
        }

        public static Condition Parse(Expression<Func<T, bool>> expr)
        {
            return ExpressionParser<T>.Parse(expr);
        }

	    #endregion
    }
}
