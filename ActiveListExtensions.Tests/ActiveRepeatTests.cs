using ActiveListExtensions.Tests.Helpers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xunit;

namespace ActiveListExtensions.Tests
{
	public class ActiveRepeatTests
	{
		[Fact]
		public void RandomlyChangeStart()
		{
			RandomGenerator.ResetRandomGenerator();

			var value = new ActiveValue<int>(0);
			var count = new ActiveValue<int>(10);

			var sut = ActiveEnumerable.ActiveRepeat(value, count);
			var watcher = new CollectionSynchronizationWatcher<int>(sut);
			var validator = new LinqValidator<int, int, int>(() => Enumerable.Repeat(value.Value, count.Value).ToArray(), sut, l => l, false, i => i);

			foreach (var num in Enumerable.Range(0, 500))
			{
				value.Value = RandomGenerator.GenerateRandomInteger(-50, 50);
				validator.Validate();
			}
		}

		[Fact]
		public void RandomlyChangeCount()
		{
			RandomGenerator.ResetRandomGenerator();

			var value = new ActiveValue<int>(0);
			var count = new ActiveValue<int>(10);

			var sut = ActiveEnumerable.ActiveRepeat(value, count);
			var watcher = new CollectionSynchronizationWatcher<int>(sut);
			var validator = new LinqValidator<int, int, int>(() => Enumerable.Repeat(value.Value, count.Value).ToArray(), sut, l => l, false, i => i);

			foreach (var num in Enumerable.Range(0, 500))
			{
				count.Value = RandomGenerator.GenerateRandomInteger(0, 50);
				validator.Validate();
			}
		}
	}
}
