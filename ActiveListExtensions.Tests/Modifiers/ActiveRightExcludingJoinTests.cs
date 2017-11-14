using ActiveListExtensions.Tests.Helpers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xunit;

namespace ActiveListExtensions.Tests.Modifiers
{
	public class ActiveRightExcludingJoinTests
	{
		[Fact]
		public void RandomlyChangeParameter() => CollectionTestHelpers.RandomlyChangeParameterInTwoCollections((l1, l2, p) => l1.ActiveRightExcludingJoin(l2, p, (l, i) => (l != null ? l.Property : 0) + i.Property, (r, i) => r.Property + i.Property, (l, r, i) => (l != null ? l.Property : 0) + r.Property + i.Property), (l1, l2, p) => l1.RightExcludingJoin(l2, i => i.Property + p.Property, i => i.Property + p.Property, (i1, i2) => i1.Property + i2.Property + p.Property), () => new IntegerTestClass() { Property = RandomGenerator.GenerateRandomInteger(0, 10) }, () => new IntegerTestClass() { Property = RandomGenerator.GenerateRandomInteger(0, 10) });

		[Fact]
		public void RandomlyInsertItems() => CollectionTestHelpers.RandomlyInsertItemsIntoTwoCollections((l1, l2) => l1.ActiveRightExcludingJoin(l2, l => l.Property, r => r.Property, (l, r) => (l != null ? l.Property : 0) + r.Property), (l1, l2) => l1.RightExcludingJoin(l2, l => l.Property, r => r.Property, (l, r) => (l != null ? l.Property : 0) + r.Property), () => new IntegerTestClass() { Property = RandomGenerator.GenerateRandomInteger(0, 10) }, () => new IntegerTestClass() { Property = RandomGenerator.GenerateRandomInteger(0, 10) });

		[Fact]
		public void RandomlyRemoveItems() => CollectionTestHelpers.RandomlyRemoveItemsFromTwoCollections((l1, l2) => l1.ActiveRightExcludingJoin(l2, l => l.Property, r => r.Property, (l, r) => (l != null ? l.Property : 0) + r.Property), (l1, l2) => l1.RightExcludingJoin(l2, l => l.Property, r => r.Property, (l, r) => (l != null ? l.Property : 0) + r.Property), () => new IntegerTestClass() { Property = RandomGenerator.GenerateRandomInteger(0, 10) }, () => new IntegerTestClass() { Property = RandomGenerator.GenerateRandomInteger(0, 10) });

		[Fact]
		public void RandomlyReplaceItems() => CollectionTestHelpers.RandomlyReplaceItemsInTwoCollections((l1, l2) => l1.ActiveRightExcludingJoin(l2, l => l.Property, r => r.Property, (l, r) => (l != null ? l.Property : 0) + r.Property), (l1, l2) => l1.RightExcludingJoin(l2, l => l.Property, r => r.Property, (l, r) => (l != null ? l.Property : 0) + r.Property), () => new IntegerTestClass() { Property = RandomGenerator.GenerateRandomInteger(0, 10) }, () => new IntegerTestClass() { Property = RandomGenerator.GenerateRandomInteger(0, 10) });

		[Fact]
		public void RandomlyMoveItems() => CollectionTestHelpers.RandomlyMoveItemsWithinTwoCollections((l1, l2) => l1.ActiveRightExcludingJoin(l2, l => l.Property, r => r.Property, (l, r) => (l != null ? l.Property : 0) + r.Property), (l1, l2) => l1.RightExcludingJoin(l2, l => l.Property, r => r.Property, (l, r) => (l != null ? l.Property : 0) + r.Property), () => new IntegerTestClass() { Property = RandomGenerator.GenerateRandomInteger(0, 10) }, () => new IntegerTestClass() { Property = RandomGenerator.GenerateRandomInteger(0, 10) });

		[Fact]
		public void ResetWithRandomItems() => CollectionTestHelpers.ResetTwoCollectionsWithRandomItems((l1, l2) => l1.ActiveRightExcludingJoin(l2, l => l.Property, r => r.Property, (l, r) => (l != null ? l.Property : 0) + r.Property), (l1, l2) => l1.RightExcludingJoin(l2, l => l.Property, r => r.Property, (l, r) => (l != null ? l.Property : 0) + r.Property), () => new IntegerTestClass() { Property = RandomGenerator.GenerateRandomInteger(0, 10) }, () => new IntegerTestClass() { Property = RandomGenerator.GenerateRandomInteger(0, 10) });

		[Fact]
		public void RandomlyChangePropertyValues() => CollectionTestHelpers.RandomlyChangePropertyValuesInTwoCollections((l1, l2) => l1.ActiveRightExcludingJoin(l2, l => l.Property, r => r.Property, (l, r) => (l != null ? l.Property : 0) + r.Property), (l1, l2) => l1.RightExcludingJoin(l2, l => l.Property, r => r.Property, (l, r) => (l != null ? l.Property : 0) + r.Property), () => new IntegerTestClass() { Property = RandomGenerator.GenerateRandomInteger() }, () => new IntegerTestClass() { Property = RandomGenerator.GenerateRandomInteger() }, o => o.Property = RandomGenerator.GenerateRandomInteger(), o => o.Property = RandomGenerator.GenerateRandomInteger());
	}
}
