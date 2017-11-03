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
	public class AsActiveValueSelectListTests
	{
		[Fact]
		public void MutatedListIsCorrectWithoutAnyAppliedChanges()
		{
			var source = ActiveList.Create(new[] { 1, 2, 3 });

			var sut = source.AsActiveValue().ActiveSelect(list => list.Select(i => i * 2));

			Assert.True(source.SequenceEqual(sut.Value.Select(i => i / 2)));
		}

		[Fact]
		public void RandomlyInsertItems() => ValueTestHelpers.RandomlyInsertItems(l => l.AsActiveValue().ActiveSelect(i => i.Select(o => o.Property).ToArray()), l => l.Select(o => o.Property).ToArray(), () => new IntegerTestClass() { Property = RandomGenerator.GenerateRandomInteger() }, (l1, l2) => l1.SequenceEqual(l2));

		[Fact]
		public void RandomlyRemoveItems() => ValueTestHelpers.RandomlyRemoveItems(l => l.AsActiveValue().ActiveSelect(i => i.Select(o => o.Property).ToArray()), l => l.Select(o => o.Property).ToArray(), () => new IntegerTestClass() { Property = RandomGenerator.GenerateRandomInteger() }, (l1, l2) => l1.SequenceEqual(l2));

		[Fact]
		public void RandomlyReplaceItems() => ValueTestHelpers.RandomlyReplaceItems(l => l.AsActiveValue().ActiveSelect(i => i.Select(o => o.Property).ToArray()), l => l.Select(o => o.Property).ToArray(), () => new IntegerTestClass() { Property = RandomGenerator.GenerateRandomInteger() }, (l1, l2) => l1.SequenceEqual(l2));

		[Fact]
		public void RandomlyMoveItems() => ValueTestHelpers.RandomlyMoveItems(l => l.AsActiveValue().ActiveSelect(i => i.Select(o => o.Property).ToArray()), l => l.Select(o => o.Property).ToArray(), () => new IntegerTestClass() { Property = RandomGenerator.GenerateRandomInteger() }, (l1, l2) => l1.SequenceEqual(l2));

		[Fact]
		public void ResetWithRandomItems() => ValueTestHelpers.ResetWithRandomItems(l => l.AsActiveValue().ActiveSelect(i => i.Select(o => o.Property).ToArray()), l => l.Select(o => o.Property).ToArray(), () => new IntegerTestClass() { Property = RandomGenerator.GenerateRandomInteger() }, (l1, l2) => l1.SequenceEqual(l2));
	}
}
