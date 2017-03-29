using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ActiveListExtensions.Tests.Helpers;
using ActiveListExtensions.Tests.Utilities;
using ActiveListExtensions.Utilities;
using Xunit;

namespace ActiveListExtensions.Tests.Modifiers
{
	public class ActiveSkipTests
	{
		[Fact]
		public void RandomlyInsertItems() => CollectionTestHelpers.RandomlyInsertItems(l => l.ActiveSkip(5), l => l.Skip(5), RandomGenerator.GenerateRandomInteger);

		[Fact]
		public void RandomlyRemoveItems() => CollectionTestHelpers.RandomlyRemoveItems(l => l.ActiveSkip(5), l => l.Skip(5), RandomGenerator.GenerateRandomInteger);

		[Fact]
		public void RandomlyReplaceItems() => CollectionTestHelpers.RandomlyReplaceItems(l => l.ActiveSkip(5), l => l.Skip(5), RandomGenerator.GenerateRandomInteger);

		[Fact]
		public void RandomlyMoveItems() => CollectionTestHelpers.RandomlyMoveItems(l => l.ActiveSkip(5), l => l.Skip(5), RandomGenerator.GenerateRandomInteger);

		[Fact]
		public void ResetWithRandomItems() => CollectionTestHelpers.ResetWithRandomItems(l => l.ActiveSkip(5), l => l.Skip(5), RandomGenerator.GenerateRandomInteger);

		[Fact]
		public void RandomlyChangeIndex()
		{
			RandomGenerator.ResetRandomGenerator();

			var list = new ObservableList<int>();
			foreach (var value in Enumerable.Range(0, 100))
				list.Add(list.Count, RandomGenerator.GenerateRandomInteger());

			var index = new CustomActiveValue<int>();

			var sut = list.ActiveSkip(index);
			var watcher = new CollectionSynchronizationWatcher<int>(sut);
			var validator = new LinqValidator<int, int, int>(list, sut, l => l.Skip(index.Value), false, null);

			foreach (var value in Enumerable.Range(0, 100))
			{
				index.Value = RandomGenerator.GenerateRandomInteger(-10, 110);

				validator.Validate();
			}
		}
	}
}
