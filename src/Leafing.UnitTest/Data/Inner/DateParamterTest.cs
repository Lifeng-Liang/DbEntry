using System;
using Leafing.Data.SqlEntry;
using NUnit.Framework;

namespace Leafing.UnitTest.Data.Inner
{
	[TestFixture]
	public class DateParameterTest
	{
		[Test]
		public void Test1()
		{
			DataParameter d1 = new DataParameter("@p1", 1);
			DataParameter d2 = new DataParameter("@p2", 2);
			DataParameter d3 = new DataParameter("@p3", 3);
			DataParameterCollection dpc = new DataParameterCollection(d1,d2,d3);
			Assert.AreEqual(d1, dpc[0]);
			Assert.AreEqual(d2, dpc[1]);
			Assert.AreEqual(d3, dpc[2]);
		}

        [Test, ExpectedException(typeof(ArgumentException))]
		public void Test2()
		{
			DataParameter d1 = new DataParameter("@p1", 1);
			DataParameter d2 = new DataParameter("@p2", 2);
			DataParameter d3 = new DataParameter(3);
			DataParameterCollection dpc = new DataParameterCollection(d1,d2,d3);
			Assert.AreEqual(d1, dpc[0]);
			Assert.AreEqual(d2, dpc[1]);
			Assert.AreEqual(d3, dpc[2]);
		}

        [Test, ExpectedException(typeof(ArgumentException))]
		public void Test3()
		{
			DataParameter d1 = new DataParameter(1);
			DataParameter d2 = new DataParameter("@p2", 2);
			DataParameter d3 = new DataParameter("@p3", 3);
			DataParameterCollection dpc = new DataParameterCollection(d1,d2,d3);
			Assert.AreEqual(d1, dpc[0]);
			Assert.AreEqual(d2, dpc[1]);
			Assert.AreEqual(d3, dpc[2]);
		}
	}
}
