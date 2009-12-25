using System;
using Lephone.Util;

namespace Lephone.UnitTest.util
{
    public class MockMiscProvider : MiscProvider
    {
        private DateTime _now = DateTime.MinValue;

        public MockMiscProvider() {}

        public MockMiscProvider(DateTime dt)
        {
            SetNow(dt);
        }

        public override DateTime Now
        {
            get { return _now; }
        }

        public void SetNow(DateTime dt)
        {
            _now = dt;
        }

        public void Add(TimeSpan ts)
        {
            _now = _now.Add(ts);
        }

        private Guid _guid = Guid.Empty;

        public override Guid NewGuid()
        {
            return _guid;
        }

        public void SetGuid(Guid id)
        {
            _guid = id;
        }
    }
}