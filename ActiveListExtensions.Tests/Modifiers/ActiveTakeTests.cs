using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ActiveListExtensions.Tests.Helpers;
using Xunit;

namespace ActiveListExtensions.Tests.Modifiers
{
	public class ActiveTakeTests
	{
		[Fact]
		public void RandomlyInsertItems() => CollectionTestHelpers.RandomlyInsertItems(l => l.ActiveTake(50), l => l.Take(50), RandomGenerator.GenerateRandomInteger);

		[Fact]
		public void RandomlyRemoveItems() => CollectionTestHelpers.RandomlyRemoveItems(l => l.ActiveTake(50), l => l.Take(50), RandomGenerator.GenerateRandomInteger);

		[Fact]
		public void RandomlyReplaceItems() => CollectionTestHelpers.RandomlyReplaceItems(l => l.ActiveTake(50), l => l.Take(50), RandomGenerator.GenerateRandomInteger);

		[Fact]
		public void RandomlyMoveItems() => CollectionTestHelpers.RandomlyMoveItems(l => l.ActiveTake(50), l => l.Take(50), RandomGenerator.GenerateRandomInteger);

		[Fact]
		public void ResetWithRandomItems() => CollectionTestHelpers.ResetWithRandomItems(l => l.ActiveTake(50), l => l.Take(50), RandomGenerator.GenerateRandomInteger);
	}
}
