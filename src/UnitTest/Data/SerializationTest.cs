
#region usings

using System;
using System.Collections.Generic;
using System.Text;

using NUnit.Framework;

using org.hanzify.llf.Data;
using org.hanzify.llf.Data.Common;
using org.hanzify.llf.util.Coding;
using org.hanzify.llf.util;

using org.hanzify.llf.UnitTest.Data.Objects;

#endregion

namespace org.hanzify.llf.UnitTest.Data
{
    [TestFixture]
    public class SerializationTest
    {
        [SetUp]
        public void Setup()
        {
            ClassHelper.SetValue<MemoryTypeBuilder>("FieldPrifix", "_");
        }

        [Test]
        public void TestSerializedBase()
        {
            SerializedBase o = DynamicObject.NewObject<SerializedBase>("TOM");
            Assert.AreEqual("TOM", o.Name);
            SerializationCoding c = new SerializationCoding();
            SerializedBase o1 = (SerializedBase)c.Decode(c.Encode(o));
            Assert.AreEqual("TOM", o1.Name);
        }

        [Test]
        public void TestSerializedClassForSerialize()
        {
            SerializedClass o = new SerializedClass();
            o.Name = "TOM";
            SerializationCoding c = new SerializationCoding();
            c.Encode(o);
        }

        [Test]
        public void TestSerializedClass()
        {
            // SerializedClass
            SerializedClass o = new SerializedClass();
            o.Name = "TOM";
            SerializationCoding c = new SerializationCoding();
            byte[] bs = c.Encode(o);
            // trans to SerializedBase
            SerializedBase o1 = (SerializedBase)c.Decode(bs);
            Assert.AreEqual("TOM", o1.Name);
        }
    }
}
