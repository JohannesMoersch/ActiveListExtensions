using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ActiveListExtensions.Tests.Helpers;
using Xunit;
using ActiveListExtensions.Utilities;
using ActiveListExtensions.Tests.Utilities;

namespace ActiveListExtensions.Tests.Modifiers
{
	public class ActiveTakeTests
	{
		[Fact]
		public void RandomlyInsertItems() => CollectionTestHelpers.RandomlyInsertItems(l => l.ActiveTake(50), l => l.Take(50), RandomGenerator.GenerateRandomInteger);

		[Fact]
		public void RandomlyRemoveItems() => CollectionTestHelpers.RandomlyRemoveItems(l => l.ActiveTake(50), l => l.Take(50), RandomGenerator.GenerateRandomInteger);

		[Fact]
		public void RandomlyReplaceItems() => CollectionTestHelpers.RandomlyReplaceItems(l => l.ActiveTake(50), l => l.Take(50), RandomGenerator.GenerateRandomInteger);

		[Fact]
		public void RandomlyMoveItems() => CollectionTestHelpers.RandomlyMoveItems(l => l.ActiveTake(50), l => l.Take(50), RandomGenerator.GenerateRandomInteger);

		[Fact]
		public void ResetWithRandomItems() => CollectionTestHelpers.ResetWithRandomItems(l => l.ActiveTake(50), l => l.Take(50), RandomGenerator.GenerateRandomInteger);

		[Fact]
		public void RandomlyChangeIndex()
		{
			RandomGenerator.ResetRandomGenerator();

			var list = new ObservableList<int>();
			foreach (var value in Enumerable.Range(0, 100))
				list.Add(list.Count, RandomGenerator.GenerateRandomInteger());

			var index = new CustomActiveValue<int>();

			var sut = list.ActiveTake(index);
			var watcher = new CollectionSynchronizationWatcher<int>(sut);
			var validator = new LinqValidator<int, int, int>(list, sut, l => l.Take(index.Value), false, null);

			foreach (var value in Enumerable.Range(0, 100))
			{
				index.Value = RandomGenerator.GenerateRandomInteger(-10, 110);

				validator.Validate();
			}
		}
	}
}
