using ActiveListExtensions.Tests.Helpers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xunit;

namespace ActiveListExtensions.Tests.ValueModifiers
{
	public class ActiveSumTests
	{
		[Fact]
		public void RandomlyChangeParameter() => ValueTestHelpers.RandomlyChangeParameter((l, p) => l.ActiveSum(p, (o, i) => o.Property * i.Property), (l, p) => l.Sum(i => i.Property * p.Property), () => new IntegerTestClass() { Property = RandomGenerator.GenerateRandomInteger(0, 100) }, () => RandomGenerator.GenerateRandomInteger(-10, 10));

		[Fact]
		public void RandomlyInsertItems() => ValueTestHelpers.RandomlyInsertItems(l => l.ActiveSum(i => i.Property), l => l.Sum(i => i.Property), () => new IntegerTestClass() { Property = RandomGenerator.GenerateRandomInteger() });

		[Fact]
		public void RandomlyRemoveItems() => ValueTestHelpers.RandomlyRemoveItems(l => l.ActiveSum(i => i.Property), l => l.Sum(i => i.Property), () => new IntegerTestClass() { Property = RandomGenerator.GenerateRandomInteger() });

		[Fact]
		public void RandomlyReplaceItems() => ValueTestHelpers.RandomlyReplaceItems(l => l.ActiveSum(i => i.Property), l => l.Sum(i => i.Property), () => new IntegerTestClass() { Property = RandomGenerator.GenerateRandomInteger() });

		[Fact]
		public void RandomlyMoveItems() => ValueTestHelpers.RandomlyMoveItems(l => l.ActiveSum(i => i.Property), l => l.Sum(i => i.Property), () => new IntegerTestClass() { Property = RandomGenerator.GenerateRandomInteger() });

		[Fact]
		public void RandomMixedOperations() => ValueTestHelpers.RandomMixedOperations(l => l.ActiveSum(i => i.Property), l => l.Sum(i => i.Property), () => new IntegerTestClass() { Property = RandomGenerator.GenerateRandomInteger() });

		[Fact]
		public void ResetWithRandomItems() => ValueTestHelpers.ResetWithRandomItems(l => l.ActiveSum(i => i.Property), l => l.Sum(i => i.Property), () => new IntegerTestClass() { Property = RandomGenerator.GenerateRandomInteger() });

		[Fact]
		public void RandomlyChangePropertyValues() => ValueTestHelpers.RandomlyChangePropertyValues(l => l.ActiveSum(i => i.Property), l => l.Sum(i => i.Property), () => new IntegerTestClass() { Property = RandomGenerator.GenerateRandomInteger() }, o => o.Property = RandomGenerator.GenerateRandomInteger());
	}
}
