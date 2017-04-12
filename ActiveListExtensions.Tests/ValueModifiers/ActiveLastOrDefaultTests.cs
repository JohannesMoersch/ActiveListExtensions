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
	public class ActiveLastOrDefaultTests
	{
		[Fact]
		public void RandomlyChangeParameter() => ValueTestHelpers.RandomlyChangeParameter((l, p) => l.ActiveLastOrDefault(p, (o, i) => o.Property > i.Property), (l, p) => l.LastOrDefault(i => i.Property > p.Property), () => new IntegerTestClass() { Property = RandomGenerator.GenerateRandomInteger(0, 100) }, () => RandomGenerator.GenerateRandomInteger(-50, 150));

		[Fact]
		public void RandomlyInsertItems() => ValueTestHelpers.RandomlyInsertItems(l => l.ActiveLastOrDefault(i => i.Property % 20 == 0), l => l.LastOrDefault(i => i.Property % 20 == 0), () => new IntegerTestClass() { Property = RandomGenerator.GenerateRandomInteger() });

		[Fact]
		public void RandomlyRemoveItems() => ValueTestHelpers.RandomlyRemoveItems(l => l.ActiveLastOrDefault(i => i.Property % 20 == 0), l => l.LastOrDefault(i => i.Property % 20 == 0), () => new IntegerTestClass() { Property = RandomGenerator.GenerateRandomInteger() });

		[Fact]
		public void RandomlyReplaceItems() => ValueTestHelpers.RandomlyReplaceItems(l => l.ActiveLastOrDefault(i => i.Property % 20 == 0), l => l.LastOrDefault(i => i.Property % 20 == 0), () => new IntegerTestClass() { Property = RandomGenerator.GenerateRandomInteger() });

		[Fact]
		public void RandomlyMoveItems() => ValueTestHelpers.RandomlyMoveItems(l => l.ActiveLastOrDefault(i => i.Property % 20 == 0), l => l.LastOrDefault(i => i.Property % 20 == 0), () => new IntegerTestClass() { Property = RandomGenerator.GenerateRandomInteger() });

		[Fact]
		public void RandomMixedOperations() => ValueTestHelpers.RandomMixedOperations(l => l.ActiveLastOrDefault(i => i.Property % 20 == 0), l => l.LastOrDefault(i => i.Property % 20 == 0), () => new IntegerTestClass() { Property = RandomGenerator.GenerateRandomInteger() });

		[Fact]
		public void ResetWithRandomItems() => ValueTestHelpers.ResetWithRandomItems(l => l.ActiveLastOrDefault(i => i.Property % 20 == 0), l => l.LastOrDefault(i => i.Property % 20 == 0), () => new IntegerTestClass() { Property = RandomGenerator.GenerateRandomInteger() });

		[Fact]
		public void RandomlyChangePropertyValues() => ValueTestHelpers.RandomlyChangePropertyValues(l => l.ActiveLastOrDefault(i => i.Property % 20 == 0), l => l.LastOrDefault(i => i.Property % 20 == 0), () => new IntegerTestClass() { Property = RandomGenerator.GenerateRandomInteger() }, o => o.Property = RandomGenerator.GenerateRandomInteger());
	}
}