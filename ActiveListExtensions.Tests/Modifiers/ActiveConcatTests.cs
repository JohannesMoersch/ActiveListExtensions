using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ActiveListExtensions.Tests.Helpers;
using Xunit;

namespace ActiveListExtensions.Tests.Modifiers
{
	public class ActiveConcatTests
	{
		[Fact]
		public void RandomlyInsertItems() => CollectionTestHelpers.RandomlyInsertItemsIntoTwoCollections((l1, l2) => l1.ActiveConcat(l2), (l1, l2) => l1.Concat(l2), RandomGenerator.GenerateRandomInteger, RandomGenerator.GenerateRandomInteger);

		[Fact]
		public void RandomlyRemoveItems() => CollectionTestHelpers.RandomlyRemoveItemsFromTwoCollections((l1, l2) => l1.ActiveConcat(l2), (l1, l2) => l1.Concat(l2), RandomGenerator.GenerateRandomInteger, RandomGenerator.GenerateRandomInteger);

		[Fact]
		public void RandomlyReplaceItems() => CollectionTestHelpers.RandomlyReplaceItemsInTwoCollections((l1, l2) => l1.ActiveConcat(l2), (l1, l2) => l1.Concat(l2), RandomGenerator.GenerateRandomInteger, RandomGenerator.GenerateRandomInteger);

		[Fact]
		public void RandomlyMoveItems() => CollectionTestHelpers.RandomlyMoveItemsWithinTwoCollections((l1, l2) => l1.ActiveConcat(l2), (l1, l2) => l1.Concat(l2), RandomGenerator.GenerateRandomInteger, RandomGenerator.GenerateRandomInteger);

		[Fact]
		public void ResetWithRandomItems() => CollectionTestHelpers.ResetTwoCollectionsWithRandomItems((l1, l2) => l1.ActiveConcat(l2), (l1, l2) => l1.Concat(l2), RandomGenerator.GenerateRandomInteger, RandomGenerator.GenerateRandomInteger);
	}
}
