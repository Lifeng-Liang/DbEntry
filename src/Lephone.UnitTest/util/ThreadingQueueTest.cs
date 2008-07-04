using System.Collections;
using System.Threading;
using Lephone.Util;
using NUnit.Framework;

namespace Lephone.UnitTest.util
{
	[TestFixture]
	public class ThreadingQueueTest
	{
		private ArrayList al;

		private ArrayList Run()
		{
			const int iStart = 20;
			const int iEnd = 9999;
			const int iSleep = 1000;

			al = new ArrayList();
			using ( ThreadingQueue qt = new ThreadingQueue(6) )
			{
				qt.Output += new OutputEventHandler(qt_Output);
				for ( int i = iStart; i <= iEnd; i++ )
				{
					lock(al)
					{
						al.Add("1 Input:" + i.ToString());
					}
					qt.Input(i);
					if ( (i % iSleep) == 0 )
					{
						Thread.Sleep(0);
					}
				}
			}

			ArrayList Exp = new ArrayList();
			for ( int i = iStart; i <= iEnd; i++ )
			{
				Exp.Add("1 Input:" + i.ToString());
			}
			for ( int i = iStart; i <= iEnd; i++ )
			{
				Exp.Add("2 Output:" + i.ToString());
			}
			return Exp;
		}

        [Test, ExpectedException(typeof(AssertionException))]
		public void TestThreadingQueueNotSort()
		{
			ArrayList Exp = Run();
			Assert.AreEqual(Exp.ToArray(typeof(string)), al.ToArray(typeof(string)));
		}

		[Test]
		public void TestThreadingQueue()
		{
			ArrayList Exp = Run();
			al.Sort();
			Exp.Sort();
			Assert.AreEqual(Exp.ToArray(typeof(string)), al.ToArray(typeof(string)));
		}

		private void qt_Output(object o)
		{
			lock(al)
			{
				al.Add("2 Output:" + o.ToString());
			}
		}
	}
}
