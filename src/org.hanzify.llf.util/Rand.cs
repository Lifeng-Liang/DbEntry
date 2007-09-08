
#region usings

using System;

#endregion

namespace Lephone.Util
{
	public static class Rand
	{
		private static Random r = new Random();

		public static int Next(int MaxValue)
		{
			lock (r)
			{
				return r.Next(MaxValue);
			}
		}

		public static int Next(int MinValue, int MaxValue)
		{
			lock (r)
			{
				return r.Next(MinValue, MaxValue);
			}
		}

		public static void NextBytes(byte[] buffer)
		{
			lock (r)
			{
				r.NextBytes(buffer);
			}
		}
		
		public static double NextDouble()
		{
			lock (r)
			{
				return r.NextDouble();
			}
		}
	}
}
