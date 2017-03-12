using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ActiveListExtensions.Tests.Helpers;
using ActiveListExtensions.Utilities;
using Xunit;

namespace ActiveListExtensions.Tests.Modifiers
{
	public class ActiveSkipTests
	{
		[Fact]
		public void RandomlyInsertItems() => CollectionTestHelpers.RandomlyInsertItems(l => l.ActiveSkip(5), l => l.Skip(5), RandomGenerator.GenerateRandomInteger);

		[Fact]
		public void RandomlyRemoveItems() => CollectionTestHelpers.RandomlyRemoveItems(l => l.ActiveSkip(5), l => l.Skip(5), RandomGenerator.GenerateRandomInteger);

		[Fact]
		public void RandomlyReplaceItems() => CollectionTestHelpers.RandomlyReplaceItems(l => l.ActiveSkip(5), l => l.Skip(5), RandomGenerator.GenerateRandomInteger);

		[Fact]
		public void RandomlyMoveItems() => CollectionTestHelpers.RandomlyMoveItems(l => l.ActiveSkip(5), l => l.Skip(5), RandomGenerator.GenerateRandomInteger);

		[Fact]
		public void ResetWithRandomItems() => CollectionTestHelpers.ResetWithRandomItems(l => l.ActiveSkip(5), l => l.Skip(5), RandomGenerator.GenerateRandomInteger);
	}
}
