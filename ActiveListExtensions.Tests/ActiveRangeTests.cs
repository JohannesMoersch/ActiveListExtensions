using ActiveListExtensions.Tests.Helpers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xunit;

namespace ActiveListExtensions.Tests
{
	public class ActiveRangeTests
	{
		[Fact]
		public void RandomlyChangeStart()
		{
			RandomGenerator.ResetRandomGenerator();

			var start = new ActiveValue<int>(0);
			var count = new ActiveValue<int>(10);

			var sut = ActiveEnumerable.ActiveRange(start, count);
			var watcher = new CollectionSynchronizationWatcher<int>(sut);
			var validator = new LinqValidator<int, int, int>(() => Enumerable.Range(start.Value, count.Value).ToArray(), sut, l => l, false, i => i);

			foreach (var value in Enumerable.Range(0, 500))
			{
				start.Value = RandomGenerator.GenerateRandomInteger(-50, 50);
				validator.Validate();
			}
		}

		[Fact]
		public void RandomlyChangeCount()
		{
			RandomGenerator.ResetRandomGenerator();

			var start = new ActiveValue<int>(0);
			var count = new ActiveValue<int>(10);

			var sut = ActiveEnumerable.ActiveRange(start, count);
			var watcher = new CollectionSynchronizationWatcher<int>(sut);
			var validator = new LinqValidator<int, int, int>(() => { try { return Enumerable.Range(start.Value, count.Value).ToArray(); } catch { return new int[0]; } }, sut, l => l, false, i => i);

			foreach (var value in Enumerable.Range(0, 500))
			{
				count.Value = RandomGenerator.GenerateRandomInteger(-5, 50);
				validator.Validate();
			}
		}
	}
}
