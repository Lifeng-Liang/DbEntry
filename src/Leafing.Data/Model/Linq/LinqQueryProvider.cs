using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using Leafing.Data.Definition;
using Leafing.Data.Model.QuerySyntax;

namespace Leafing.Data.Model.Linq
{
    public class LinqQueryProvider<T, TResult> : IOrderedQueryable<TResult>, IQueryProvider where T : class, IDbObject
    {
        private readonly Expression _expression;

        public LinqQueryProvider(Expression expression)
        {
            this._expression = expression;
        }

        #region IEnumerable<T> Members

        public IEnumerator<TResult> GetEnumerator()
        {
            var lep = new LinqExpressionParser<T>(this._expression);
            var query = (QueryContent<T>)DbEntry.From<T>().Where(lep.Condition).OrderBy(lep.Orderby);
            var list = query.Select<TResult>();
            return ((IEnumerable<TResult>)list).GetEnumerator();
        }

        #endregion

        #region IEnumerable Members

        System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
        {
            // never used ???
            return GetEnumerator();
        }

        #endregion

        #region IQueryable Members

        public Type ElementType
        {
            get { return typeof(T); }
        }

        public Expression Expression
        {
            get
            {
                return _expression ?? Expression.Constant(this);
            }
        }

        public IQueryProvider Provider
        {
            get { return this; }
        }

        #endregion

        #region IQueryProvider Members

        public IQueryable<TElement> CreateQuery<TElement>(Expression expr)
        {
            return new LinqQueryProvider<T, TElement>(expr);
        }

        public IQueryable CreateQuery(Expression expr)
        {
            return new LinqQueryProvider<T, TResult>(expr);
        }

        public TElement Execute<TElement>(Expression expr)
        {
            // never used ???
            return (TElement)CreateQuery(expr).GetEnumerator();
        }

        public object Execute(Expression expr)
        {
            // never used ???
            return CreateQuery(expr).GetEnumerator();
        }

        #endregion
    }
}