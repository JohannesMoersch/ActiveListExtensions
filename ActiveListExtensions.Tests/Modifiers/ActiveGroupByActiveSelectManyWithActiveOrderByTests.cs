using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ActiveListExtensions.Tests.Helpers;
using Xunit;

namespace ActiveListExtensions.Tests.Modifiers
{
	public class ActiveGroupByActiveSelectManyWithActiveOrderByTests
	{
		[Fact]
		public void RandomlyInsertItems() => CollectionTestHelpers.RandomlyInsertItems(ActiveGroupByThenActiveSelectManyWithActiveOrderBy, GroupByThenSelectManyWithOrderBy, () => (RandomGenerator.GenerateRandomInteger(), RandomGenerator.GenerateRandomInteger()));

		[Fact]
		public void RandomlyRemoveItems() => CollectionTestHelpers.RandomlyRemoveItems(ActiveGroupByThenActiveSelectManyWithActiveOrderBy, GroupByThenSelectManyWithOrderBy, () => (RandomGenerator.GenerateRandomInteger(), RandomGenerator.GenerateRandomInteger()));

		[Fact]
		public void RandomlyReplaceItems() => CollectionTestHelpers.RandomlyReplaceItems(ActiveGroupByThenActiveSelectManyWithActiveOrderBy, GroupByThenSelectManyWithOrderBy, () => (RandomGenerator.GenerateRandomInteger(), RandomGenerator.GenerateRandomInteger()));

		[Fact]
		public void RandomlyMoveItems() => CollectionTestHelpers.RandomlyMoveItems(ActiveGroupByThenActiveSelectManyWithActiveOrderBy, GroupByThenSelectManyWithOrderBy, () => (RandomGenerator.GenerateRandomInteger(), RandomGenerator.GenerateRandomInteger()));

		[Fact]
		public void ResetWithRandomItems() => CollectionTestHelpers.ResetWithRandomItems(ActiveGroupByThenActiveSelectManyWithActiveOrderBy, GroupByThenSelectManyWithOrderBy, () => (RandomGenerator.GenerateRandomInteger(), RandomGenerator.GenerateRandomInteger()));

		private IActiveList<(int key, int value)> ActiveGroupByThenActiveSelectManyWithActiveOrderBy(IActiveList<(int key, int value)> list)
			=> list
				.ActiveGroupBy(item => item.key)
				.ActiveSelectMany(group => group.ActiveOrderBy(item => item.value, ListSortDirection.Ascending));

		private IEnumerable<(int key, int value)> GroupByThenSelectManyWithOrderBy(IReadOnlyList<(int key, int value)> list)
			=> list
				.GroupBy(item => item.key)
				.SelectMany(group => group.OrderBy(item => item.value));
	}
}
