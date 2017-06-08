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
	public class ActiveMutateListTests
	{
		[Fact]
		public void MutatedListIsCorrectWithoutAnyAppliedChanges()
		{
			var source = ActiveList.Create<int>(new[] { 1, 2, 3 });

			var sut = source.ActiveMutate(list => list.Select(i => i * 2));

			Assert.True(source.SequenceEqual(sut.Value.Select(i => i / 2)));
		}

		[Fact]
		public void RandomlyInsertItems() => ValueTestHelpers.RandomlyInsertItems(l => l.ToActiveValue(i => i.Select(o => o.Property)), l => l.Select(o => o.Property), () => new IntegerTestClass() { Property = RandomGenerator.GenerateRandomInteger() }, (l1, l2) => l1.SequenceEqual(l2));

		[Fact]
		public void RandomlyRemoveItems() => ValueTestHelpers.RandomlyRemoveItems(l => l.ToActiveValue(i => i.Select(o => o.Property)), l => l.Select(o => o.Property), () => new IntegerTestClass() { Property = RandomGenerator.GenerateRandomInteger() }, (l1, l2) => l1.SequenceEqual(l2));

		[Fact]
		public void RandomlyReplaceItems() => ValueTestHelpers.RandomlyReplaceItems(l => l.ToActiveValue(i => i.Select(o => o.Property)), l => l.Select(o => o.Property), () => new IntegerTestClass() { Property = RandomGenerator.GenerateRandomInteger() }, (l1, l2) => l1.SequenceEqual(l2));

		[Fact]
		public void RandomlyMoveItems() => ValueTestHelpers.RandomlyMoveItems(l => l.ToActiveValue(i => i.Select(o => o.Property)), l => l.Select(o => o.Property), () => new IntegerTestClass() { Property = RandomGenerator.GenerateRandomInteger() }, (l1, l2) => l1.SequenceEqual(l2));

		[Fact]
		public void ResetWithRandomItems() => ValueTestHelpers.ResetWithRandomItems(l => l.ToActiveValue(i => i.Select(o => o.Property)), l => l.Select(o => o.Property), () => new IntegerTestClass() { Property = RandomGenerator.GenerateRandomInteger() }, (l1, l2) => l1.SequenceEqual(l2));
	}
}
