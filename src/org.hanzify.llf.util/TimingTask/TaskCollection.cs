using System.Collections;

namespace Lephone.Util.TimingTask
{
	public class TaskCollection : CollectionBase
	{
		public TaskCollection()
		{
		}

		public TaskCollection(ITask tp)
		{
			Add(tp);
		}

		public void Add(ITask tp)
		{
			List.Add(tp);
		}

		public ITask this[int index]
		{
			get
			{
				return (ITask)List[index];
			}
			set
			{
				List[index] = value;
			}
		}
	}
}

