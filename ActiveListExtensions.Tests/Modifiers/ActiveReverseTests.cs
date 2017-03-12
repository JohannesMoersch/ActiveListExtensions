using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ActiveListExtensions.Tests.Helpers;
using Xunit;

namespace ActiveListExtensions.Tests.Modifiers
{
	public class ActiveReverseTests
	{
		[Fact]
		public void RandomlyInsertItems() => CollectionTestHelpers.RandomlyInsertItems(l => l.ActiveReverse(), l => l.Reverse(), RandomGenerator.GenerateRandomInteger);

		[Fact]
		public void RandomlyRemoveItems() => CollectionTestHelpers.RandomlyRemoveItems(l => l.ActiveReverse(), l => l.Reverse(), RandomGenerator.GenerateRandomInteger);

		[Fact]
		public void RandomlyReplaceItems() => CollectionTestHelpers.RandomlyReplaceItems(l => l.ActiveReverse(), l => l.Reverse(), RandomGenerator.GenerateRandomInteger);

		[Fact]
		public void RandomlyMoveItems() => CollectionTestHelpers.RandomlyMoveItems(l => l.ActiveReverse(), l => l.Reverse(), RandomGenerator.GenerateRandomInteger);

		[Fact]
		public void ResetWithRandomItems() => CollectionTestHelpers.ResetWithRandomItems(l => l.ActiveReverse(), l => l.Reverse(), RandomGenerator.GenerateRandomInteger);
	}
}
