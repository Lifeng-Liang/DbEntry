using System;

namespace Lephone.Core
{
	public static class Rand
	{
		private static readonly Random rand = new Random();

		public static int Next(int maxValue)
		{
			lock (rand)
			{
				return rand.Next(maxValue);
			}
		}

		public static int Next(int minValue, int maxValue)
		{
			lock (rand)
			{
				return rand.Next(minValue, maxValue);
			}
		}

		public static void NextBytes(byte[] buffer)
		{
			lock (rand)
			{
				rand.NextBytes(buffer);
			}
		}
		
		public static double NextDouble()
		{
			lock (rand)
			{
				return rand.NextDouble();
			}
		}
	}
}
