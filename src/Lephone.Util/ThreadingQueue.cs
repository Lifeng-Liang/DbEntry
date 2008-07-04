using System;
using System.Collections;
using System.Threading;

namespace Lephone.Util
{
	public delegate void OutputEventHandler(object o);

	public class ThreadingQueue : IDisposable
	{
		public event OutputEventHandler Output;

		private readonly Queue ShareQueue = new Queue();
		private readonly ArrayList Threads = new ArrayList();
		private bool Running = true;
		private readonly AutoResetEvent HasIncoming = new AutoResetEvent(false);
		private readonly ManualResetEvent ItsTimeToDispose = new ManualResetEvent(false);

		public ThreadingQueue(int ThreadNo)
		{
			for ( int i = 0; i < ThreadNo; i++ )
			{
				Thread t = new Thread(OutputThread);
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
