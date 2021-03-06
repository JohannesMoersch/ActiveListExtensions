﻿using ActiveListExtensions.Tests.Helpers;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xunit;

namespace ActiveListExtensions.Tests.Modifiers
{
	public class ActiveGroupByTests
	{
		[Fact]
		public void RandomlyChangeParameter() => CollectionTestHelpers.RandomlyChangeParameter((l, p) => l.ActiveGroupBy(p, (o, i) => o.Property * (i.Property % 5 == 0 ? 0 : 1)), (l, p) => l.GroupBy(o => o.Property * (p.Property % 5 == 0 ? 0 : 1)), () => new IntegerTestClass() { Property = RandomGenerator.GenerateRandomInteger() }, false, g => g.Key, (g1, g2) => g1.Zip(g2, (i1, i2) => Equals(i1, i2)).All(b => b));

		[Fact]
		public void RandomlyInsertItems() => CollectionTestHelpers.RandomlyInsertItems(l => l.ActiveGroupBy(o => o.Property), l => l.GroupBy(o => o.Property), () => new IntegerTestClass() { Property = RandomGenerator.GenerateRandomInteger(0, 10) }, false, g => g.Key, (g1, g2) => g1.Zip(g2, (i1, i2) => Equals(i1, i2)).All(b => b));

		[Fact]
		public void RandomlyRemoveItems() => CollectionTestHelpers.RandomlyRemoveItems(l => l.ActiveGroupBy(o => o.Property), l => l.GroupBy(o => o.Property), () => new IntegerTestClass() { Property = RandomGenerator.GenerateRandomInteger(0, 10) }, false, g => g.Key, (g1, g2) => g1.Zip(g2, (i1, i2) => Equals(i1, i2)).All(b => b));

		[Fact]
		public void RandomlyReplaceItems() => CollectionTestHelpers.RandomlyReplaceItems(l => l.ActiveGroupBy(o => o.Property), l => l.GroupBy(o => o.Property), () => new IntegerTestClass() { Property = RandomGenerator.GenerateRandomInteger(0, 10) }, false, g => g.Key, (g1, g2) => g1.Zip(g2, (i1, i2) => Equals(i1, i2)).All(b => b));

		[Fact]
		public void RandomlyMoveItems() => CollectionTestHelpers.RandomlyMoveItems(l => l.ActiveGroupBy(o => o.Property), l => l.GroupBy(o => o.Property), () => new IntegerTestClass() { Property = RandomGenerator.GenerateRandomInteger(0, 10) }, false, g => g.Key, (g1, g2) => g1.Zip(g2, (i1, i2) => Equals(i1, i2)).All(b => b));

		[Fact]
		public void ResetWithRandomItems() => CollectionTestHelpers.ResetWithRandomItems(l => l.ActiveGroupBy(o => o.Property), l => l.GroupBy(o => o.Property), () => new IntegerTestClass() { Property = RandomGenerator.GenerateRandomInteger(0, 10) }, false, g => g.Key, (g1, g2) => g1.Zip(g2, (i1, i2) => Equals(i1, i2)).All(b => b));

		[Fact]
		public void RandomlyChangePropertyValues() => CollectionTestHelpers.RandomlyChangePropertyValues(l => l.ActiveGroupBy(o => o.Property), l => l.GroupBy(o => o.Property), () => new IntegerTestClass() { Property = RandomGenerator.GenerateRandomInteger(0, 10) }, o => o.Property = RandomGenerator.GenerateRandomInteger(0, 10), false, g => g.Key, (g1, g2) => g1.Zip(g2, (i1, i2) => Equals(i1, i2)).All(b => b));
	}
}