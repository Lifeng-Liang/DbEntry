using System;
using System.Linq;
using NUnit.Framework;

namespace Leafing.UnitTest.util
{
	[TestFixture]
	public class FpTest
	{
		[Test]
		public void MapTest()
		{
			var list = new []{ 4, 2, 5, 7, 8, 10, 3 }.Map(x => x + 2);
			Assert.AreEqual(new []{ 6, 4, 7, 9, 10, 12, 5 }, list);

			var map = new int[]{ 4, 2, 5, 7, 8, 10, 3 }.Map(x => x + 2.1d);
			Assert.AreEqual(new Double[]{ 6.1, 4.1, 7.1, 9.1, 10.1, 12.1, 5.1 }, map);
		}

		[Test]
		public void ReduceTest()
		{
			var sum = new []{ 4, 2, 5, 7, 8, 10, 3 }.Reduce((p, n) => p + n, 0);
			Assert.AreEqual(39, sum);

			var max = new []{ 4, 2, 5, 7, 8, 10, 3 }.Reduce((p, n) => p > n ? p : n, 0);
			Assert.AreEqual(10, max);

			var min = new []{ 4, 2, 5, 7, 8, 10, 3 }.Reduce((p, n) => p < n ? p : n, 20);
			Assert.AreEqual(2, min);

			var line = new []{ "1", "4", "7", "2" }.Reduce((p, n) => p + "-" + n, ">");
			Assert.AreEqual(">-1-4-7-2", line);
		}

		[Test]
		public void FilterTest()
		{
			var list = new []{ 4, 2, 5, 7, 8, 10, 3 }.Filter(x => x > 5);
			Assert.AreEqual(new []{ 7, 8, 10 }, list);
		}

		[Test]
		public void EachTest()
		{
			var result = "";
			var list = new []{ 4, 2, 5, 7, 8, 10, 3 }.Each(x => result += x);
			Assert.AreEqual(new []{ 4, 2, 5, 7, 8, 10, 3 }, list);
			Assert.AreEqual("42578103", result);
		}

		[Test]
		public void EverySomeTest()
		{
			var flag = new []{ 4, 2, 5, 7, 8, 10, 3 }.Every(x => x > 5);
			Assert.IsFalse(flag, "Every(x => x > 5) should be false");

			flag = new []{ 4, 2, 5, 7, 8, 10, 3 }.Every(x => x < 15);
			Assert.IsTrue(flag, "Every(x => x < 15) should be true");

			flag = new []{ 4, 2, 5, 7, 8, 10, 3 }.Some(x => x > 5);
			Assert.IsTrue(flag, "Some(x => x > 5) should be true");

			flag = new []{ 4, 2, 5, 7, 8, 10, 3 }.Some(x => x < 2);
			Assert.IsFalse(flag, "Some(x => x < 2) should be false");
		}
	}
}
