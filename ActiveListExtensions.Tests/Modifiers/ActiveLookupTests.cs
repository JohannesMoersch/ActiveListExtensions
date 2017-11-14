using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xunit;

namespace ActiveListExtensions.Tests.Modifiers
{
	public class ActiveLookupTests
	{
		[Fact]
		public void ExistingGroupThrowsChangeNotifications()
		{
			var list = ActiveList.Create(Enumerable.Range(0, 5));

			var sut = list.ToActiveLookup(i => i / 10);

			var group = sut[0];

			bool changeOccured = false;
			group.CollectionChanged += (s, e) => changeOccured = true;

			list.Add(2, 15);

			Assert.False(changeOccured);

			list.Add(1, 7);

			Assert.True(changeOccured);
		}

		[Fact]
		public void ExistingGroupStaysUpToDate()
		{
			var list = ActiveList.Create(Enumerable.Range(0, 5));

			var sut = list.ToActiveLookup(i => i / 10);

			Assert.True(list.SequenceEqual(sut[0]));

			list.Add(2, 15);
			list.Add(1, 7);

			Assert.True(new[] { 0, 7, 1, 2, 3, 4 }.SequenceEqual(sut[0]));
		}

		[Fact]
		public void ExistingGroupStaysUpToDateAfterReset()
		{
			var list = ActiveList.Create(Enumerable.Range(0, 5));

			var sut = list.ToActiveLookup(i => i / 10);

			Assert.True(list.SequenceEqual(sut[0]));

			list.Reset(new[] { 1, 12, 7, 3, 18 });

			Assert.True(new[] { 1, 7, 3 }.SequenceEqual(sut[0]));
		}

		[Fact]
		public void SelectByKeyGroupStaysUpToDateAfterReset()
		{
			var list = ActiveList.Create(Enumerable.Range(0, 5));

			var sut = list.ToActiveLookup(i => i / 10).ActiveSelectByKey(0);

			Assert.True(list.SequenceEqual(sut));

			list.Reset(new[] { 1, 12, 7, 3, 18 });

			Assert.True(new[] { 1, 7, 3 }.SequenceEqual(sut));
		}

		[Fact]
		public void NewGroupThrowsChangeNotifications()
		{
			var list = ActiveList.Create(Enumerable.Range(0, 5));

			var sut = list.ToActiveLookup(i => i / 10);

			var group = sut[1];

			bool changeOccured = false;
			group.CollectionChanged += (s, e) => changeOccured = true;

			list.Add(1, 7);

			Assert.False(changeOccured);

			list.Add(2, 15);

			Assert.True(changeOccured);
		}

		[Fact]
		public void NewGroupStaysUpToDate()
		{
			var list = ActiveList.Create(Enumerable.Range(0, 5));

			var sut = list.ToActiveLookup(i => i / 10);

			Assert.True(new int[0].SequenceEqual(sut[1]));

			list.Add(1, 7);
			list.Add(2, 15);

			Assert.True(new[] { 15 }.SequenceEqual(sut[1]));
		}

		[Fact]
		public void OrderingInsideGroupMatchesSourceOrdering()
		{
			var list = ActiveList.Create(Enumerable.Range(0, 5));

			var sut = list.ToActiveLookup(i => i / 10);

			Assert.True(new int[] { 0, 1, 2, 3, 4 }.SequenceEqual(sut[0]));

			list.Add(2, 7);
			list.Add(1, 15);
			list.Add(0, 9);

			Assert.True(new int[] { 9, 0, 1, 7, 2, 3, 4 }.SequenceEqual(sut[0]));
		}

		[Fact]
		public void GroupOrderingMatchesSourceOrdering()
		{

			var list = ActiveList.Create(new[] { 1, 2, 5, 3 });

			var sut = list.ToActiveLookup(i => i / 2);
			
			Assert.True(new int[] { 0, 1, 2 }.SequenceEqual(sut.Select(g => g.Key)));

			list.Remove(1);

			Assert.True(new int[] { 0, 2, 1 }.SequenceEqual(sut.Select(g => g.Key)));

			list.Add(4);

			Assert.True(new int[] { 0, 2, 1 }.SequenceEqual(sut.Select(g => g.Key)));

			list.Move(1, 0);

			Assert.True(new int[] { 2, 0, 1 }.SequenceEqual(sut.Select(g => g.Key)));

			list.Add(0, 8);

			Assert.True(new int[] { 4, 2, 0, 1 }.SequenceEqual(sut.Select(g => g.Key)));
		}
	}
}
