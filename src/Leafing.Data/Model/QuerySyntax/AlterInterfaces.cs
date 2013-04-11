using System;
using System.Linq.Expressions;
using Leafing.Data.Definition;

namespace Leafing.Data.Model.QuerySyntax
{
    public interface IAddColumn<T> where T : class, IDbObject
    {
        IAlterable<T> AddColumn(Expression<Func<T, object>> expr);
    }

    public interface IAlterable<T> : IAlter<T> where T : class, IDbObject
    {
        IAlter<T> Default(object o);
    }

    public interface IAlter<T> where T : class, IDbObject
    {
        void AlterTable();
    }
}
