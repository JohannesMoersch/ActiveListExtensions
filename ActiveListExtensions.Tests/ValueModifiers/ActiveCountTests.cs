using ActiveListExtensions.Tests.Helpers;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xunit;

namespace ActiveListExtensions.Tests.ValueModifiers
{
	public class ActiveCountTests
	{
		[Fact]
		public void RandomlyChangeParameter() => ValueTestHelpers.RandomlyChangeParameter((l, p) => l.ActiveCount(p, (o, i) => o.Property > i.Property), (l, p) => l.Count(i => i.Property > p.Property), () => new IntegerTestClass() { Property = RandomGenerator.GenerateRandomInteger(0, 100) }, () => RandomGenerator.GenerateRandomInteger(-50, 150));

		[Fact]
		public void RandomlyInsertItems() => ValueTestHelpers.RandomlyInsertItems(l => l.ActiveCount(), l => l.Count(), () => new IntegerTestClass() { Property = RandomGenerator.GenerateRandomInteger() });

		[Fact]
		public void RandomlyRemoveItems() => ValueTestHelpers.RandomlyRemoveItems(l => l.ActiveCount(), l => l.Count(), () => new IntegerTestClass() { Property = RandomGenerator.GenerateRandomInteger() });

		[Fact]
		public void RandomlyReplaceItems() => ValueTestHelpers.RandomlyReplaceItems(l => l.ActiveCount(), l => l.Count(), () => new IntegerTestClass() { Property = RandomGenerator.GenerateRandomInteger() });

		[Fact]
		public void RandomlyMoveItems() => ValueTestHelpers.RandomlyMoveItems(l => l.ActiveCount(), l => l.Count(), () => new IntegerTestClass() { Property = RandomGenerator.GenerateRandomInteger() });

		[Fact]
		public void RandomMixedOperations() => ValueTestHelpers.RandomMixedOperations(l => l.ActiveCount(), l => l.Count(), () => new IntegerTestClass() { Property = RandomGenerator.GenerateRandomInteger() });

		[Fact]
		public void ResetWithRandomItems() => ValueTestHelpers.ResetWithRandomItems(l => l.ActiveCount(), l => l.Count(), () => new IntegerTestClass() { Property = RandomGenerator.GenerateRandomInteger() });

		[Fact]
		public void RandomlyInsertItemsWithPredicate() => ValueTestHelpers.RandomlyInsertItems(l => l.ActiveCount(i => i.Property > 50), l => l.Count(i => i.Property > 50), () => new IntegerTestClass() { Property = RandomGenerator.GenerateRandomInteger() });

		[Fact]
		public void RandomlyRemoveItemsWithPredicate() => ValueTestHelpers.RandomlyRemoveItems(l => l.ActiveCount(i => i.Property > 50), l => l.Count(i => i.Property > 50), () => new IntegerTestClass() { Property = RandomGenerator.GenerateRandomInteger() });

		[Fact]
		public void RandomlyReplaceItemsWithPredicate() => ValueTestHelpers.RandomlyReplaceItems(l => l.ActiveCount(i => i.Property > 50), l => l.Count(i => i.Property > 50), () => new IntegerTestClass() { Property = RandomGenerator.GenerateRandomInteger() });

		[Fact]
		public void RandomlyMoveItemsWithPredicate() => ValueTestHelpers.RandomlyMoveItems(l => l.ActiveCount(i => i.Property > 50), l => l.Count(i => i.Property > 50), () => new IntegerTestClass() { Property = RandomGenerator.GenerateRandomInteger() });

		[Fact]
		public void RandomMixedOperationsWithPredicate() => ValueTestHelpers.RandomMixedOperations(l => l.ActiveCount(i => i.Property > 50), l => l.Count(i => i.Property > 50), () => new IntegerTestClass() { Property = RandomGenerator.GenerateRandomInteger() });

		[Fact]
		public void ResetWithRandomItemsWithPredicate() => ValueTestHelpers.ResetWithRandomItems(l => l.ActiveCount(i => i.Property > 50), l => l.Count(i => i.Property > 50), () => new IntegerTestClass() { Property = RandomGenerator.GenerateRandomInteger() });

		[Fact]
		public void RandomlyChangePropertyValues() => ValueTestHelpers.RandomlyChangePropertyValues(l => l.ActiveCount(i => i.Property > 50), l => l.Count(i => i.Property > 50), () => new IntegerTestClass() { Property = RandomGenerator.GenerateRandomInteger() }, o => o.Property = RandomGenerator.GenerateRandomInteger());
	}
}
