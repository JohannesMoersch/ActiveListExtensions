using ActiveListExtensions.Tests.Helpers;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xunit;

namespace ActiveListExtensions.Tests.Modifiers
{
	public class ActiveOrderByDescendingTests
	{
		[Fact]
		public void RandomlyInsertItems() => CollectionTestHelpers.RandomlyInsertItems(l => l.ActiveOrderByDescending(o => o.Property), l => l.OrderByDescending(o => o.Property), () => new ActiveOrderByTestClass() { Property = RandomGenerator.GenerateRandomInteger() });

		[Fact]
		public void RandomlyRemoveItems() => CollectionTestHelpers.RandomlyRemoveItems(l => l.ActiveOrderByDescending(o => o.Property), l => l.OrderByDescending(o => o.Property), () => new ActiveOrderByTestClass() { Property = RandomGenerator.GenerateRandomInteger() });

		[Fact]
		public void RandomlyReplaceItems() => CollectionTestHelpers.RandomlyReplaceItems(l => l.ActiveOrderByDescending(o => o.Property), l => l.OrderByDescending(o => o.Property), () => new ActiveOrderByTestClass() { Property = RandomGenerator.GenerateRandomInteger() });

		[Fact]
		public void RandomlyMoveItems() => CollectionTestHelpers.RandomlyMoveItems(l => l.ActiveOrderByDescending(o => o.Property), l => l.OrderByDescending(o => o.Property), () => new ActiveOrderByTestClass() { Property = RandomGenerator.GenerateRandomInteger() });

		[Fact]
		public void ResetWithRandomItems() => CollectionTestHelpers.ResetWithRandomItems(l => l.ActiveOrderByDescending(o => o.Property), l => l.OrderByDescending(o => o.Property), () => new ActiveOrderByTestClass() { Property = RandomGenerator.GenerateRandomInteger() });

		[Fact]
		public void RandomlyChangePropertyValues() => CollectionTestHelpers.RandomlyChangePropertyValues(l => l.ActiveOrderByDescending(o => o.Property), l => l.OrderByDescending(o => o.Property), () => new ActiveOrderByTestClass() { Property = RandomGenerator.GenerateRandomInteger() }, o => o.Property = RandomGenerator.GenerateRandomInteger());
	}
}
