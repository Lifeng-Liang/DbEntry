using System;
using System.Collections;
using System.Threading;

namespace Lephone.Util
{
	public delegate void OutputEventHandler(object o);

	public class ThreadingQueue : IDisposable
	{
		public event OutputEventHandler Output;

		private readonly Queue _shareQueue = new Queue();
		private readonly ArrayList _threads = new ArrayList();
		private bool _running = true;
		private readonly AutoResetEvent _hasIncoming = new AutoResetEvent(false);
		private readonly ManualResetEvent _itsTimeToDispose = new ManualResetEvent(false);

		public ThreadingQueue(int threadNo)
		{
			for ( int i = 0; i < threadNo; i++ )
			{
				var t = new Thread(OutputThread);
				_threads.Add(t);
				t.Start();
			}
		}

		public void Input(object o)
		{
			if ( _running )
			{
				lock(_shareQueue)
				{
					_shareQueue.Enqueue(o);
				}
				_hasIncoming.Set();
			}
		}

		private void OutputThread()
		{
			var hs = new WaitHandle[] {_itsTimeToDispose, _hasIncoming};
			while(true)
			{
				object o = null;
				lock(_shareQueue)
				{
					if ( _shareQueue.Count > 0 )
					{
						o = _shareQueue.Dequeue();
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
			if ( _running )
			{
				_running = false;
				_itsTimeToDispose.Set();
				foreach ( Thread t in _threads )
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
