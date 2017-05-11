using ActiveListExtensions.Tests.Helpers;
using ActiveListExtensions.Utilities;
using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xunit;

namespace ActiveListExtensions.Tests.Modifiers
{
	public class ActiveTranslateResetNotificationsTests
	{
		[Fact]
		public void EqualInitially()
		{
			var value = new ActiveValue<IEnumerable<int>>(new[] { 1, 2, 3 });

			var sut = value.ToActiveList().ActiveTranslateResetNotifications();

			Assert.True(value.Value.SequenceEqual(sut));
		}

		[Fact]
		public void InsertAtStart()
		{
			var value = new ActiveValue<IEnumerable<int>>(new[] { 1, 2, 3 });

			var sut = value.ToActiveList().ActiveTranslateResetNotifications();

			value.Value = new[] { 0, 1, 2, 3 };

			Assert.True(value.Value.SequenceEqual(sut));
		}

		[Fact]
		public void InsertInMiddle()
		{
			var value = new ActiveValue<IEnumerable<int>>(new[] { 1, 2, 4, 5 });

			var sut = value.ToActiveList().ActiveTranslateResetNotifications();

			value.Value = new[] { 1, 2, 3, 4, 5 };

			Assert.True(value.Value.SequenceEqual(sut));
		}

		[Fact]
		public void InsertAtEnd()
		{
			var value = new ActiveValue<IEnumerable<int>>(new[] { 1, 2, 3 });

			var sut = value.ToActiveList().ActiveTranslateResetNotifications();

			value.Value = new[] { 1, 2, 3, 4 };

			Assert.True(value.Value.SequenceEqual(sut));
		}

		[Fact]
		public void AddMultiple()
		{
			var value = new ActiveValue<IEnumerable<int>>(new[] { 1, 2, 4, 6, 7, 8 });

			var sut = value.ToActiveList().ActiveTranslateResetNotifications();

			value.Value = new[] { 0, 1, 2, 3, 4, 5, 6, 7, 8, 9 };

			Assert.True(value.Value.SequenceEqual(sut));
		}

		[Fact]
		public void AddWithDuplicates()
		{
			var value = new ActiveValue<IEnumerable<int>>(new[] { 1, 1, 2 });

			var sut = value.ToActiveList().ActiveTranslateResetNotifications();

			value.Value = new[] { 0, 0, 1, 1, 1, 2, 2 };

			Assert.True(value.Value.SequenceEqual(sut));
		}

		[Fact]
		public void RemoveFromStart()
		{
			var value = new ActiveValue<IEnumerable<int>>(new[] { 1, 2, 3 });

			var sut = value.ToActiveList().ActiveTranslateResetNotifications();

			value.Value = new[] { 2, 3 };

			Assert.True(value.Value.SequenceEqual(sut));
		}

		[Fact]
		public void RemoveFromMiddle()
		{
			var value = new ActiveValue<IEnumerable<int>>(new[] { 1, 2, 3 });

			var sut = value.ToActiveList().ActiveTranslateResetNotifications();

			value.Value = new[] { 1, 3 };

			Assert.True(value.Value.SequenceEqual(sut));
		}

		[Fact]
		public void RemoveFromEnd()
		{
			var value = new ActiveValue<IEnumerable<int>>(new[] { 1, 2, 3 });

			var sut = value.ToActiveList().ActiveTranslateResetNotifications();

			value.Value = new[] { 1, 2 };

			Assert.True(value.Value.SequenceEqual(sut));
		}

		[Fact]
		public void RemoveMultiple()
		{
			var value = new ActiveValue<IEnumerable<int>>(new[] { 0, 1, 2, 3, 4, 5, 6, 7, 8, 9 });

			var sut = value.ToActiveList().ActiveTranslateResetNotifications();

			value.Value = new[] { 1, 2, 4, 6, 7, 8 };

			Assert.True(value.Value.SequenceEqual(sut));
		}

		[Fact]
		public void RemoveWithDuplicates()
		{
			var value = new ActiveValue<IEnumerable<int>>(new[] { 0, 0, 1, 1, 1, 2, 2 });

			var sut = value.ToActiveList().ActiveTranslateResetNotifications();

			value.Value = new[] { 1, 1, 2 };

			Assert.True(value.Value.SequenceEqual(sut));
		}

		[Fact]
		public void Scramble()
		{
			var value = new ActiveValue<IEnumerable<int>>(new[] { 0, 1, 2, 3, 4, 5, 6, 7, 8, 9 });

			var sut = value.ToActiveList().ActiveTranslateResetNotifications();

			value.Value = new[] { 1, 4, 8, 5, 2, 9, 0, 3, 7, 6 };

			Assert.True(value.Value.SequenceEqual(sut));
		}

		[Fact]
		public void RandomReplaceOperations()
		{
			RandomGenerator.ResetRandomGenerator();

			var value = new ActiveValue<IEnumerable<int>>(new int[0]);

			var sut = value.ToActiveList().ActiveTranslateResetNotifications();

			for (int i = 0; i < 500; ++i)
			{
				var count = RandomGenerator.GenerateRandomInteger(0, 50);

				value.Value = RandomGenerator.GenerateRandomIntegerList(count, 0, 50);

				Assert.True(value.Value.SequenceEqual(sut));
			}
		}

		[Fact]
		public void RandomChangeOperations()
		{
			RandomGenerator.ResetRandomGenerator();

			var value = new ActiveValue<IEnumerable<object>>(Enumerable.Range(0, 50).Select(i => new object()));

			var sut = value.ToActiveList().ActiveTranslateResetNotifications();

			int addCount = 0;
			int removeCount = 0;
			sut.CollectionChanged += (s, e) =>
			{
				if (e.Action == NotifyCollectionChangedAction.Add)
					++addCount;
				if (e.Action == NotifyCollectionChangedAction.Remove)
					++removeCount;
			};

			for (int i = 0; i < 100; ++i)
			{
				var list = new ObservableList<object>();
				foreach (var item in sut)
					list.Add(list.Count, item);

				int targetAddCount = 0;
				int targetRemoveCount = 0;
				foreach (var index in Enumerable.Range(0, 10))
				{
					if (list.Count > 0)
					{
						++targetRemoveCount;
						list.Remove(RandomGenerator.GenerateRandomInteger(0, list.Count));
					}
					break;
				}
				foreach (var index in Enumerable.Range(0, 15))
				{
					++targetAddCount;
					list.Add(RandomGenerator.GenerateRandomInteger(0, list.Count), new object());
					break;
				}
				foreach (var index in Enumerable.Range(0, 10))
					list.Move(RandomGenerator.GenerateRandomInteger(0, list.Count), RandomGenerator.GenerateRandomInteger(0, list.Count));

				value.Value = list;

				Assert.Equal(addCount, targetAddCount);
				Assert.Equal(removeCount, targetRemoveCount);
				Assert.True(value.Value.SequenceEqual(sut));

				addCount = 0;
				removeCount = 0;
			}
		}
	}
}
