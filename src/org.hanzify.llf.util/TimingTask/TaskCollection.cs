
#region usings

using System;
using System.Data;
using System.Collections;

#endregion

namespace org.hanzify.llf.util.TimingTask
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

