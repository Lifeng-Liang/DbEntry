using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using Leafing.Data.Model.Linq;
using Leafing.Data.Model.QuerySyntax;
using Leafing.Data.SqlEntry;
using Leafing.Data.SqlEntry.Dynamic;

namespace Leafing.Data.Definition
{
    [Serializable]
    public class DbObjectModel<T> : DbObjectModel<T, long> where T : DbObjectModel<T, long>, new()
    {
    }

    [Serializable]
    public class DbObjectModel<T, TKey> : DbObjectSmartUpdate where T : DbObjectModel<T, TKey>, new() where TKey : struct
    {
        [DbKey]
        public TKey Id { get; set; }

        protected static ModelContext ModelContext = ModelContext.GetInstance(typeof(T));

        internal override ModelContext Context
        {
            get { return ModelContext; }
        }

		public DbObjectModel ()
		{
			Context.Handler.CtorInit(this);
		}

        protected static CK Col
        {
            get { return CK.Column; }
        }

        public static dynamic FindBy
        {
            get { return new DynamicQuery<T>(); }
        }

        public static T FindById(TKey? id)
        {
            return ModelContext.Operator.GetObject<T>(id);
        }

        public static List<T> FindBySql(string sqlStr)
        {
            return ModelContext.Operator.ExecuteList<T>(sqlStr);
        }

        public static List<T> FindBySql(SqlStatement sql)
        {
            return ModelContext.Operator.ExecuteList<T>(sql);
        }

        public static List<T> Find(Condition con)
        {
            return ModelContext.From<T>().Where(con).Select();
        }

        public static List<T> Find(Condition con, OrderBy ob)
        {
            return ModelContext.From<T>().Where(con).OrderBy(ob).Select();
        }

        public static List<T> Find(Condition con, string orderBy)
        {
            return ModelContext.From<T>().Where(con).OrderBy(orderBy).Select();
        }

        public static List<T> Find(ConditionBuilder<T> con)
        {
            return ModelContext.From<T>().Where(con).Select();
        }

        public static List<T> Find(ConditionBuilder<T> con, OrderBy ob)
        {
            return ModelContext.From<T>().Where(con).OrderBy(ob).Select();
        }

        public static List<T> Find(ConditionBuilder<T> con, string orderBy)
        {
            return ModelContext.From<T>().Where(con).OrderBy(orderBy).Select();
        }

        public static T FindOne(Condition condition)
        {
            return FindOne(condition, new OrderBy("Id"));
        }

        public static T FindOne(Condition condition, OrderBy ob)
        {
            var ls = Where(condition).OrderBy(ob).Range(1, 1).Select();
            return ls.Count > 0 ? ls[0] : null;
        }

        public static T FindOne(ConditionBuilder<T> condition)
        {
            return FindOne(condition, new OrderBy("Id"));
        }

        public static T FindOne(ConditionBuilder<T> condition, OrderBy ob)
        {
            var ls = Where(condition).OrderBy(ob).Range(1, 1).Select();
            return ls.Count > 0 ? ls[0] : null;
        }

        public static IAfterWhere<T> Where(Condition con)
        {
            return new QueryContent<T>(ModelContext).Where(con);
        }

        public static IAfterWhere<T> Where(ConditionBuilder<T> con)
        {
            return new QueryContent<T>(ModelContext).Where(con.ToCondition());
        }

        public static List<T> FindRecent(int count)
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

        public static long GetCount(ConditionBuilder<T> con)
        {
            return ModelContext.From<T>().Where(con).GetCount();
        }

        public static decimal? GetMax(ConditionBuilder<T> con, string columnName)
        {
            return ModelContext.From<T>().Where(con).GetMax(columnName);
        }

        public static DateTime? GetMaxDate(ConditionBuilder<T> con, string columnName)
        {
            return ModelContext.From<T>().Where(con).GetMaxDate(columnName);
        }

        public static decimal? GetMin(ConditionBuilder<T> con, string columnName)
        {
            return ModelContext.From<T>().Where(con).GetMin(columnName);
        }

        public static DateTime? GetMinDate(ConditionBuilder<T> con, string columnName)
        {
            return ModelContext.From<T>().Where(con).GetMinDate(columnName);
        }

        public static decimal? GetSum(ConditionBuilder<T> con, string columnName)
        {
            return ModelContext.From<T>().Where(con).GetSum(columnName);
        }

        public static void DeleteBy(Condition condition)
        {
            ModelContext.Operator.DeleteBy(condition);
        }

        public static void DeleteBy(ConditionBuilder<T> condition)
        {
            ModelContext.Operator.DeleteBy(condition.ToCondition());
        }

        public static int UpdateBy(Condition condition, object obj)
        {
            return ModelContext.Operator.UpdateBy(condition, obj);
        }

        public static int UpdateBy(ConditionBuilder<T> condition, object obj)
        {
            return ModelContext.Operator.UpdateBy(condition.ToCondition(), obj);
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

        public static List<T> Find(Expression<Func<T, bool>> condition)
        {
            return ModelContext.From<T>().Where(condition).Select();
        }

        public static List<T> Find(Expression<Func<T, bool>> condition, Expression<Func<T, object>> orderby)
        {
            return ModelContext.From<T>().Where(condition).OrderBy(orderby).Select();
        }

        public static List<T> Find(ConditionBuilder<T> condition, Expression<Func<T, object>> orderby)
        {
            return ModelContext.From<T>().Where(condition).OrderBy(orderby).Select();
        }

        public static List<T> Find(Expression<Func<T, bool>> condition, string orderby)
        {
            return ModelContext.From<T>().Where(condition).OrderBy(orderby).Select();
        }

        public static T FindOne(Expression<Func<T, bool>> condition)
        {
            return FindOne(condition, p => p.Id);
        }

        public static T FindOne(Expression<Func<T, bool>> condition, Expression<Func<T, object>> column)
        {
            var ls = Where(condition).OrderBy(column).Range(1, 1).Select();
            return ls.Count > 0 ? ls[0] : null;
        }

        public static T FindOne(ConditionBuilder<T> condition, Expression<Func<T, object>> column)
        {
            var ls = Where(condition).OrderBy(column).Range(1, 1).Select();
            return ls.Count > 0 ? ls[0] : null;
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

        public static decimal? GetMax(ConditionBuilder<T> condition, Expression<Func<T, object>> column)
        {
            return ModelContext.From<T>().Where(condition).GetMax(column);
        }

        public static DateTime? GetMaxDate(ConditionBuilder<T> condition, Expression<Func<T, object>> column)
        {
            return ModelContext.From<T>().Where(condition).GetMaxDate(column);
        }

        public static decimal? GetMin(ConditionBuilder<T> condition, Expression<Func<T, object>> column)
        {
            return ModelContext.From<T>().Where(condition).GetMin(column);
        }

        public static DateTime? GetMinDate(ConditionBuilder<T> condition, Expression<Func<T, object>> column)
        {
            return ModelContext.From<T>().Where(condition).GetMinDate(column);
        }

        public static decimal? GetSum(ConditionBuilder<T> condition, Expression<Func<T, object>> column)
        {
            return ModelContext.From<T>().Where(condition).GetSum(column);
        }

        public static decimal? GetMax(Condition condition, Expression<Func<T, object>> column)
        {
            return ModelContext.From<T>().Where(condition).GetMax(column);
        }

        public static DateTime? GetMaxDate(Condition condition, Expression<Func<T, object>> column)
        {
            return ModelContext.From<T>().Where(condition).GetMaxDate(column);
        }

        public static decimal? GetMin(Condition condition, Expression<Func<T, object>> column)
        {
            return ModelContext.From<T>().Where(condition).GetMin(column);
        }

        public static DateTime? GetMinDate(Condition condition, Expression<Func<T, object>> column)
        {
            return ModelContext.From<T>().Where(condition).GetMinDate(column);
        }

        public static decimal? GetSum(Condition condition, Expression<Func<T, object>> column)
        {
            return ModelContext.From<T>().Where(condition).GetSum(column);
        }

        public static int DeleteBy(Expression<Func<T, bool>> condition)
        {
            return ModelContext.Operator.DeleteBy(ExpressionParser<T>.Parse(condition));
        }

        public static int UpdateBy(Expression<Func<T, bool>> condition, object obj)
        {
            return ModelContext.Operator.UpdateBy(condition, obj);
        }

        public static Condition Parse(Expression<Func<T, bool>> expr)
        {
            return ExpressionParser<T>.Parse(expr);
        }

        public static void AddColumn(Expression<Func<T, object>> expr)
        {
            AddColumn(expr, null);
        }

        public static void AddColumn(Expression<Func<T, object>> expr, object o)
        {
            var n = expr.GetColumnName();
            ModelContext.AddColumn(n, o);
        }

        public static void DropColumn(string columnName)
        {
            ModelContext.DropColumn(columnName);
        }

        #endregion
    }
}
