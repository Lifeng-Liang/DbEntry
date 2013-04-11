using System;
using System.Linq;
using System.Linq.Expressions;
using Leafing.Data.Builder;
using Leafing.Data.Definition;
using Leafing.Data.Model.Linq;
using Leafing.Data.SqlEntry;

namespace Leafing.Data.Model.QuerySyntax
{
    public class AlterContent<T> : IAddColumn<T>, IAlterable<T> where T : class, IDbObject
    {
        private readonly ModelContext _ctx;
        private readonly AlterTableStatementBuilder _builder;

        public AlterContent(ModelContext ctx)
        {
            _ctx = ctx;
            _builder = new AlterTableStatementBuilder(ctx.Info.From);
        }

        public IAlterable<T> AddColumn(Expression<Func<T, object>> expr)
        {
            var n = expr.GetColumnName();
            var mem = _ctx.Info.Members.FirstOrDefault(p => p.Name == n);
            _builder.AddColumn = new ColumnInfo(mem);
            return this;
        }

        public IAlter<T> Default(object o)
        {
            _builder.DefaultValue = o;
            return this;
        }

        public void AlterTable()
        {
            var sql = _builder.ToSqlStatement(_ctx);
            _ctx.Provider.ExecuteNonQuery(sql);
        }
    }
}
