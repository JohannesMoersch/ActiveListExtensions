using ActiveListExtensions.Tests.Helpers;
using ActiveListExtensions.Utilities;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xunit;

namespace ActiveListExtensions.Tests.Modifiers
{
	public class ActiveOrderByTests
	{
		[Fact]
		public void RandomlyInsertItemsAscending() => CollectionTestHelpers.RandomlyInsertItems(l => l.ActiveOrderBy(o => o.Property), l => l.OrderBy(o => o.Property), () => new IntegerTestClass() { Property = RandomGenerator.GenerateRandomInteger() });

		[Fact]
		public void RandomlyRemoveItemsAscending() => CollectionTestHelpers.RandomlyRemoveItems(l => l.ActiveOrderBy(o => o.Property), l => l.OrderBy(o => o.Property), () => new IntegerTestClass() { Property = RandomGenerator.GenerateRandomInteger() });

		[Fact]
		public void RandomlyReplaceItemsAscending() => CollectionTestHelpers.RandomlyReplaceItems(l => l.ActiveOrderBy(o => o.Property), l => l.OrderBy(o => o.Property), () => new IntegerTestClass() { Property = RandomGenerator.GenerateRandomInteger() });

		[Fact]
		public void RandomlyMoveItemsAscending() => CollectionTestHelpers.RandomlyMoveItems(l => l.ActiveOrderBy(o => o.Property), l => l.OrderBy(o => o.Property), () => new IntegerTestClass() { Property = RandomGenerator.GenerateRandomInteger() });

		[Fact]
		public void ResetWithRandomItemsAscending() => CollectionTestHelpers.ResetWithRandomItems(l => l.ActiveOrderBy(o => o.Property), l => l.OrderBy(o => o.Property), () => new IntegerTestClass() { Property = RandomGenerator.GenerateRandomInteger() });

		[Fact]
		public void RandomlyChangePropertyValuesAscending() => CollectionTestHelpers.RandomlyChangePropertyValues(l => l.ActiveOrderBy(o => o.Property), l => l.OrderBy(o => o.Property), () => new IntegerTestClass() { Property = RandomGenerator.GenerateRandomInteger() }, o => o.Property = RandomGenerator.GenerateRandomInteger());

		[Fact]
		public void RandomlyInsertItemsDescending() => CollectionTestHelpers.RandomlyInsertItems(l => l.ActiveOrderBy(o => o.Property, ListSortDirection.Descending), l => l.OrderByDescending(o => o.Property), () => new IntegerTestClass() { Property = RandomGenerator.GenerateRandomInteger() });

		[Fact]
		public void RandomlyRemoveItemsDescending() => CollectionTestHelpers.RandomlyRemoveItems(l => l.ActiveOrderBy(o => o.Property, ListSortDirection.Descending), l => l.OrderByDescending(o => o.Property), () => new IntegerTestClass() { Property = RandomGenerator.GenerateRandomInteger() });

		[Fact]
		public void RandomlyReplaceItemsDescending() => CollectionTestHelpers.RandomlyReplaceItems(l => l.ActiveOrderBy(o => o.Property, ListSortDirection.Descending), l => l.OrderByDescending(o => o.Property), () => new IntegerTestClass() { Property = RandomGenerator.GenerateRandomInteger() });

		[Fact]
		public void RandomlyMoveItemsDescending() => CollectionTestHelpers.RandomlyMoveItems(l => l.ActiveOrderBy(o => o.Property, ListSortDirection.Descending), l => l.OrderByDescending(o => o.Property), () => new IntegerTestClass() { Property = RandomGenerator.GenerateRandomInteger() });

		[Fact]
		public void ResetWithRandomItemsDescending() => CollectionTestHelpers.ResetWithRandomItems(l => l.ActiveOrderBy(o => o.Property, ListSortDirection.Descending), l => l.OrderByDescending(o => o.Property), () => new IntegerTestClass() { Property = RandomGenerator.GenerateRandomInteger() });

		[Fact]
		public void RandomlyChangePropertyValuesDescending() => CollectionTestHelpers.RandomlyChangePropertyValues(l => l.ActiveOrderBy(o => o.Property, ListSortDirection.Descending), l => l.OrderByDescending(o => o.Property), () => new IntegerTestClass() { Property = RandomGenerator.GenerateRandomInteger() }, o => o.Property = RandomGenerator.GenerateRandomInteger());

		[Fact]
		public void RandomlyChangeDirection()
		{
			RandomGenerator.ResetRandomGenerator();

			var list = new ObservableList<int>();
			foreach (var value in Enumerable.Range(0, 100))
				list.Add(list.Count, RandomGenerator.GenerateRandomInteger());

			var index = new ActiveValue<ListSortDirection>();

			var sut = list.ActiveOrderBy(i => i, index);
			var watcher = new CollectionSynchronizationWatcher<int>(sut);
			var validator = new LinqValidator<int, int, int>(list, sut, l => index.Value == ListSortDirection.Ascending ? l.OrderBy(i => i) : l.OrderByDescending(i => i), false, null);

			foreach (var value in Enumerable.Range(0, 10))
			{
				index.Value = RandomGenerator.GenerateRandomInteger(0, 2) == 0 ? ListSortDirection.Ascending : ListSortDirection.Descending;

				validator.Validate();
			}
		}
	}
}
