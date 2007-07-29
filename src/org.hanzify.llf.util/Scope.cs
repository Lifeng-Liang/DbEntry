
#region usings

using System;
using System.Collections.Generic;
using System.Threading;
using System.Diagnostics;

#endregion

namespace org.hanzify.llf.util
{
    // this class copied from MSDN :
    // http://www.microsoft.com/china/MSDN/library/netFramework/netframework/NETMattersSep.mspx?mfr=true
    public sealed class Scope<T> : IDisposable where T : class
    {
        private bool _disposed, _ownsInstance;
        private T _instance;
        private Scope<T> _parent;

        [ThreadStatic]
        private static Scope<T> _head;

        public Scope(T instance) : this(instance, true) { }

        public Scope(T instance, bool ownsInstance)
        {
            if (instance == null)
                throw new ArgumentNullException("instance");
            _instance = instance;
            _ownsInstance = ownsInstance;

            Thread.BeginThreadAffinity();
            _parent = _head;
            _head = this;
        }

        public static T Current
        {
            get { return _head != null ? _head._instance : null; }
        }

        public void Dispose()
        {
            if (!_disposed)
            {
                _disposed = true;

                Debug.Assert(this == _head, "Disposed out of order.");
                _head = _parent;
                Thread.EndThreadAffinity();

                if (_ownsInstance)
                {
                    IDisposable disposable = _instance as IDisposable;
                    if (disposable != null) disposable.Dispose();
                }
            }
        }
    }
}
