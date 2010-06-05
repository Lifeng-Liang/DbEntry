using System;
using System.Linq;
using System.Linq.Expressions;
using Lephone.Data.Common;
using Lephone.Data.Definition;
using Lephone.Data.Linq;

namespace DebugLib
{
    public class User : DbObjectModel<User>
    {
        public string Name { get; set; }

        [BelongsTo]
        public Books Book { get; set; }
    }

    public class Books : DbObjectModel<Books>
    {
        public string Name { get; set; }

        [HasOne]
        public User User { get; set; }
    }

    public static class SipoExtends
    {
        public static TKey GetForeignKey<T, TKey>(this DbObjectModel<T, TKey> obj, Expression<Func<T, object>> expr) where T : DbObjectModel<T, TKey>
        {
            var oi = ObjectInfo.GetInstance(typeof(T));
            string name = ExpressionParser<T>.GetColumnName(expr.GetMemberExpression().Member.Name);
            return (TKey)((IBelongsTo)oi.RelationFields.Single(p => p.Name == name).GetValue(obj)).ForeignKey;
        }

        public static TKey GetForeignKey<T, TKey>(this DbObjectModel<T, TKey> obj, string name) where T : DbObjectModel<T, TKey>
        {
            var oi = ObjectInfo.GetInstance(typeof(T));
            return (TKey)((IBelongsTo)oi.RelationFields.Single(p => p.Name == name).GetValue(obj)).ForeignKey;
        }
    }
}
