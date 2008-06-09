using System;
using Lephone.Data.SqlEntry;
using NUnit.Framework;

namespace Lephone.UnitTest.Data.Inner
{
	[TestFixture]
	public class DateParamterTest
	{
		[Test]
		public void Test1()
		{
			DataParamter d1 = new DataParamter("@p1", 1);
			DataParamter d2 = new DataParamter("@p2", 2);
			DataParamter d3 = new DataParamter("@p3", 3);
			DataParamterCollection dpc = new DataParamterCollection(d1,d2,d3);
			Assert.AreEqual(d1, dpc[0]);
			Assert.AreEqual(d2, dpc[1]);
			Assert.AreEqual(d3, dpc[2]);
		}

        [Test, ExpectedException(typeof(ArgumentException))]
		public void Test2()
		{
			DataParamter d1 = new DataParamter("@p1", 1);
			DataParamter d2 = new DataParamter("@p2", 2);
			DataParamter d3 = new DataParamter(3);
			DataParamterCollection dpc = new DataParamterCollection(d1,d2,d3);
			Assert.AreEqual(d1, dpc[0]);
			Assert.AreEqual(d2, dpc[1]);
			Assert.AreEqual(d3, dpc[2]);
		}

        [Test, ExpectedException(typeof(ArgumentException))]
		public void Test3()
		{
			DataParamter d1 = new DataParamter(1);
			DataParamter d2 = new DataParamter("@p2", 2);
			DataParamter d3 = new DataParamter("@p3", 3);
			DataParamterCollection dpc = new DataParamterCollection(d1,d2,d3);
			Assert.AreEqual(d1, dpc[0]);
			Assert.AreEqual(d2, dpc[1]);
			Assert.AreEqual(d3, dpc[2]);
		}
	}
}
