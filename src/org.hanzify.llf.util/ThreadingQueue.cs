
#region usings

using System;
using System.Collections;
using System.Threading;

#endregion

namespace Lephone.Util
{
	public delegate void OutputEventHandler(object o);

	public class ThreadingQueue : IDisposable
	{
		public event OutputEventHandler Output;

		private Queue ShareQueue = new Queue();
		private ArrayList Threads = new ArrayList();
		private bool Running = true;
		private AutoResetEvent HasIncoming = new AutoResetEvent(false);
		private ManualResetEvent ItsTimeToDispose = new ManualResetEvent(false);

		public ThreadingQueue(int ThreadNo)
		{
			for ( int i = 0; i < ThreadNo; i++ )
			{
				Thread t = new Thread(new ThreadStart(OutputThread));
				Threads.Add(t);
				t.Start();
			}
		}

		public void Input(object o)
		{
			if ( Running )
			{
				lock(ShareQueue)
				{
					ShareQueue.Enqueue(o);
				}
				HasIncoming.Set();
			}
		}

		private void OutputThread()
		{
			WaitHandle[] hs = new WaitHandle[] {ItsTimeToDispose, HasIncoming};
			while(true)
			{
				object o = null;
				lock(ShareQueue)
				{
					if ( ShareQueue.Count > 0 )
					{
						o = ShareQueue.Dequeue();
					}
				}
				if (o != null)
				{
					if ( Output != null )
					{
						Output(o);
					}
				}
				else
				{
					if ( WaitHandle.WaitAny(hs) == 0 ) { break; }
				}
			}
		}

		public void Dispose()
		{
			if ( Running )
			{
				Running = false;
				ItsTimeToDispose.Set();
				foreach ( Thread t in Threads )
				{
					while ( t.IsAlive )
					{
						Thread.Sleep(0);
					}
				}
			}
		}

		~ ThreadingQueue()
		{
			Dispose();
		}
	}
}
