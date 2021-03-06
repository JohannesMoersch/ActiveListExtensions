﻿using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ActiveListExtensions.Tests.Helpers;
using Xunit;

namespace ActiveListExtensions.Tests.Modifiers
{
	public class ActiveUnionTests
	{
		[Fact]
		public void RandomlyChangeParameter() => CollectionTestHelpers.RandomlyChangeParameterInTwoCollections((l1, l2, p) => l1.ActiveUnion(l2, p, (o, i) => o + i.Property), (l1, l2, p) => l1.Union(l2, new KeyEqualityComparer<int>(o => o + p.Property, null)), () => RandomGenerator.GenerateRandomInteger(0, 10), () => RandomGenerator.GenerateRandomInteger(0, 10), true);

		[Fact]
		public void RandomlyInsertItems() => CollectionTestHelpers.RandomlyInsertItemsIntoTwoCollections((l1, l2) => l1.ActiveUnion(l2), (l1, l2) => l1.Union(l2), () => RandomGenerator.GenerateRandomInteger(0, 10), () => RandomGenerator.GenerateRandomInteger(0, 10), true);

		[Fact]
		public void RandomlyRemoveItems() => CollectionTestHelpers.RandomlyRemoveItemsFromTwoCollections((l1, l2) => l1.ActiveUnion(l2), (l1, l2) => l1.Union(l2), () => RandomGenerator.GenerateRandomInteger(0, 10), () => RandomGenerator.GenerateRandomInteger(0, 10), true);

		[Fact]
		public void RandomlyReplaceItems() => CollectionTestHelpers.RandomlyReplaceItemsInTwoCollections((l1, l2) => l1.ActiveUnion(l2), (l1, l2) => l1.Union(l2), () => RandomGenerator.GenerateRandomInteger(0, 10), () => RandomGenerator.GenerateRandomInteger(0, 10), true);

		[Fact]
		public void RandomlyMoveItems() => CollectionTestHelpers.RandomlyMoveItemsWithinTwoCollections((l1, l2) => l1.ActiveUnion(l2), (l1, l2) => l1.Union(l2), () => RandomGenerator.GenerateRandomInteger(0, 10), () => RandomGenerator.GenerateRandomInteger(0, 10), true);

		[Fact]
		public void ResetWithRandomItems() => CollectionTestHelpers.ResetTwoCollectionsWithRandomItems((l1, l2) => l1.ActiveUnion(l2), (l1, l2) => l1.Union(l2), () => RandomGenerator.GenerateRandomInteger(0, 10), () => RandomGenerator.GenerateRandomInteger(0, 10), true);

		[Fact]
		public void RandomlyChangePropertyValues() => CollectionTestHelpers.RandomlyChangePropertyValuesInTwoCollections((l1, l2) => l1.ActiveUnion(l2, o => o.Property), (l1, l2) => l1.Union(l2, new KeyEqualityComparer<IntegerTestClass>(o => o.Property, null)), () => new IntegerTestClass() { Property = RandomGenerator.GenerateRandomInteger() }, () => new IntegerTestClass() { Property = RandomGenerator.GenerateRandomInteger() }, o => o.Property = RandomGenerator.GenerateRandomInteger(), o => o.Property = RandomGenerator.GenerateRandomInteger(), true, o => o.Property);
	}
}
