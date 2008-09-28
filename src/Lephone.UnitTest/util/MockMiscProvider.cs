using System;
using Lephone.Util;

namespace Lephone.UnitTest.util
{
    public class MockMiscProvider : MiscProvider
    {
        private DateTime _Now = DateTime.MinValue;

        public MockMiscProvider() {}

        public MockMiscProvider(DateTime dt)
        {
            SetNow(dt);
        }

        public override DateTime Now
        {
            get { return _Now; }
        }

        public void SetNow(DateTime dt)
        {
            _Now = dt;
        }

        public void Add(TimeSpan ts)
        {
            _Now = _Now.Add(ts);
        }

        private Guid guid = Guid.Empty;

        public override Guid NewGuid()
        {
            return guid;
        }

        public void SetGuid(Guid id)
        {
            guid = id;
        }
    }
}