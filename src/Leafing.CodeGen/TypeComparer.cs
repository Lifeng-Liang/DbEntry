using System;
using System.Collections.Generic;

namespace Leafing.CodeGen
{
    public class TypeComparer : IComparer<Type>
    {
        public int Compare(Type x, Type y)
        {
            return x.FullName.CompareTo(y.FullName);
        }
    }
}