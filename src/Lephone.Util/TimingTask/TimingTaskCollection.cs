using System.Collections;

namespace Lephone.Util.TimingTask
{
	public class TimingTaskCollection : CollectionBase
	{
		public TimingTaskCollection()
		{
		}

		public TimingTaskCollection(params TimingTask[] tts)
		{
			Add(tts);
		}

		public void Add(params TimingTask[] tts)
		{
			foreach ( TimingTask tt in tts )
			{
				List.Add(tt);
			}
		}

		public TimingTask this[int index]
		{
			get
			{
				return (TimingTask)List[index];
			}
			set
			{
				List[index] = value;
			}
		}
	}
}

