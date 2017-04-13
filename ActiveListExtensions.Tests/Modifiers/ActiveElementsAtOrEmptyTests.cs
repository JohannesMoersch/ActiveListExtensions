using ActiveListExtensions.Tests.Helpers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xunit;

namespace ActiveListExtensions.Tests.Modifiers
{
	public class ActiveElementsAtOrEmptyTests
	{
		[Fact]
		public void RandomlyInsertItems() => CollectionTestHelpers.RandomlyInsertItemsIntoTwoCollections((l1, l2) => l1.ActiveElementsAtOrEmpty(l2), (l1, l2) => ElementsAtOrEmpty(l1, l2), () => RandomGenerator.GenerateRandomInteger(), () => RandomGenerator.GenerateRandomInteger(-20, 120));

		[Fact]
		public void RandomlyRemoveItems() => CollectionTestHelpers.RandomlyRemoveItemsFromTwoCollections((l1, l2) => l1.ActiveElementsAtOrEmpty(l2), (l1, l2) => ElementsAtOrEmpty(l1, l2), () => RandomGenerator.GenerateRandomInteger(), () => RandomGenerator.GenerateRandomInteger(-20, 120));

		[Fact]
		public void RandomlyReplaceItems() => CollectionTestHelpers.RandomlyReplaceItemsInTwoCollections((l1, l2) => l1.ActiveElementsAtOrEmpty(l2), (l1, l2) => ElementsAtOrEmpty(l1, l2), () => RandomGenerator.GenerateRandomInteger(), () => RandomGenerator.GenerateRandomInteger(-20, 120));

		[Fact]
		public void RandomlyMoveItems() => CollectionTestHelpers.RandomlyMoveItemsWithinTwoCollections((l1, l2) => l1.ActiveElementsAtOrEmpty(l2), (l1, l2) => ElementsAtOrEmpty(l1, l2), () => RandomGenerator.GenerateRandomInteger(), () => RandomGenerator.GenerateRandomInteger(-20, 120));

		[Fact]
		public void ResetWithRandomItems() => CollectionTestHelpers.ResetTwoCollectionsWithRandomItems((l1, l2) => l1.ActiveElementsAtOrEmpty(l2), (l1, l2) => ElementsAtOrEmpty(l1, l2), () => RandomGenerator.GenerateRandomInteger(), () => RandomGenerator.GenerateRandomInteger(-20, 120));

		private IEnumerable<int> ElementsAtOrEmpty(IReadOnlyList<int> listOne, IReadOnlyList<int> listTwo)
		{
			var indexes = new HashSet<int>(listTwo);
			foreach (var item in listOne.Where((value, index) => indexes.Contains(index)))
				yield return item;
		}
	}
}
