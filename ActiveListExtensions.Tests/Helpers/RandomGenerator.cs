using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace ActiveListExtensions.Tests.Helpers
{
	public static class RandomGenerator
	{
		public static void ResetRandomGenerator() => _random.Value = new Random(8);

		private static ThreadLocal<Random> _random = new ThreadLocal<Random>();

		public static int GenerateRandomInteger() => _random.Value.Next(0, 1000);

		public static int GenerateRandomInteger(int min, int max) => _random.Value.Next(min, max);

		public static IReadOnlyList<int> GenerateRandomIntegerList(int count = 10) => Enumerable.Range(0, count).Select(_ => _random.Value.Next(0, 1000)).ToArray();
	}
}
