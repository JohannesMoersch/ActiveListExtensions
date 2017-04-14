using ActiveListExtensions.Tests.Helpers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xunit;

namespace ActiveListExtensions.Tests.Modifiers
{
	public class ActiveOfTypeTests
	{
		[Fact]
		public void RandomlyInsertItemsWhenFilteringForClassType() => CollectionTestHelpers.RandomlyInsertItems(l => l.ActiveOfType<IntegerTestClass>(), l => l.OfType<IntegerTestClass>(), GenerateRandomObject);

		[Fact]
		public void RandomlyRemoveItemsWhenFilteringForClassType() => CollectionTestHelpers.RandomlyRemoveItems(l => l.ActiveOfType<IntegerTestClass>(), l => l.OfType<IntegerTestClass>(), GenerateRandomObject);

		[Fact]
		public void RandomlyReplaceItemsWhenFilteringForClassType() => CollectionTestHelpers.RandomlyReplaceItems(l => l.ActiveOfType<IntegerTestClass>(), l => l.OfType<IntegerTestClass>(), GenerateRandomObject);

		[Fact]
		public void RandomlyMoveItemsWhenFilteringForClassType() => CollectionTestHelpers.RandomlyMoveItems(l => l.ActiveOfType<IntegerTestClass>(), l => l.OfType<IntegerTestClass>(), GenerateRandomObject);

		[Fact]
		public void ResetWithRandomItemsWhenFilteringForClassType() => CollectionTestHelpers.ResetWithRandomItems(l => l.ActiveOfType<IntegerTestClass>(), l => l.OfType<IntegerTestClass>(), GenerateRandomObject);

		[Fact]
		public void RandomlyInsertItemsWhenFilteringForValueType() => CollectionTestHelpers.RandomlyInsertItems(l => l.ActiveOfType<int>(), l => l.OfType<int>(), GenerateRandomObject);

		[Fact]
		public void RandomlyRemoveItemsWhenFilteringForValueType() => CollectionTestHelpers.RandomlyRemoveItems(l => l.ActiveOfType<int>(), l => l.OfType<int>(), GenerateRandomObject);

		[Fact]
		public void RandomlyReplaceItemsWhenFilteringForValueType() => CollectionTestHelpers.RandomlyReplaceItems(l => l.ActiveOfType<int>(), l => l.OfType<int>(), GenerateRandomObject);

		[Fact]
		public void RandomlyMoveItemsWhenFilteringForValueType() => CollectionTestHelpers.RandomlyMoveItems(l => l.ActiveOfType<int>(), l => l.OfType<int>(), GenerateRandomObject);

		[Fact]
		public void ResetWithRandomItemsWhenFilteringForValueType() => CollectionTestHelpers.ResetWithRandomItems(l => l.ActiveOfType<int>(), l => l.OfType<int>(), GenerateRandomObject);

		private object GenerateRandomObject()
		{
			switch (RandomGenerator.GenerateRandomInteger(0, 4))
			{
				case 0:
					return new IntegerTestClass();
				case 1:
					return new object();
				case 2:
					return RandomGenerator.GenerateRandomInteger();
			}
			return null;
		}
	}
}
