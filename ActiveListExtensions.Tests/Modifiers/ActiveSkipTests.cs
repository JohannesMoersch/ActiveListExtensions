using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ActiveListExtensions.Tests.Helpers;
using ActiveListExtensions.Tests.Utilities;
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

		[Fact]
		public void RandomlyChangeIndex()
		{
			RandomGenerator.ResetRandomGenerator();

			var list = new ObservableList<int>();
			foreach (var value in Enumerable.Range(0, 100))
				list.Add(list.Count, RandomGenerator.GenerateRandomInteger());

			var index = new ActiveValue<int>();

			var sut = list.ActiveSkip(index);
			var watcher = new CollectionSynchronizationWatcher<int>(sut);
			var validator = new LinqValidator<int, int, int>(list, sut, l => l.Skip(index.Value), false, null);

			foreach (var value in Enumerable.Range(0, 100))
			{
				index.Value = RandomGenerator.GenerateRandomInteger(-10, 110);

				validator.Validate();
			}
		}

		[Fact]
		public void SkipLessThanCountAndIncreaseSkip()
		{
			int raisedCount = 0;
			var skipCount = ActiveValue.Create(2);
			var list = ActiveList.Create(new[] { 1, 2, 3 });
			var sut = list.ActiveSkip(skipCount);

			sut.CollectionChanged += (s, e) => raisedCount += e.Action == NotifyCollectionChangedAction.Remove ? 1 : 0;

			skipCount.Value = 3;

			Assert.Equal(1, raisedCount);
		}

		[Fact]
		public void SkipLessThanCountAndDecreaseSkip()
		{
			int raisedCount = 0;
			var skipCount = ActiveValue.Create(2);
			var list = ActiveList.Create(new[] { 1, 2, 3 });
			var sut = list.ActiveSkip(skipCount);

			sut.CollectionChanged += (s, e) => raisedCount += e.Action == NotifyCollectionChangedAction.Add ? 1 : 0;

			skipCount.Value = 1;

			Assert.Equal(1, raisedCount);
		}

		[Fact]
		public void SkipCountAndIncreaseSkip()
		{
			int raisedCount = 0;
			var skipCount = ActiveValue.Create(3);
			var list = ActiveList.Create(new[] { 1, 2, 3 });
			var sut = list.ActiveSkip(skipCount);

			sut.CollectionChanged += (s, e) => ++raisedCount;

			skipCount.Value = 4;

			Assert.Equal(0, raisedCount);
		}

		[Fact]
		public void SkipCountAndDecreaseSkip()
		{
			int raisedCount = 0;
			var skipCount = ActiveValue.Create(3);
			var list = ActiveList.Create(new[] { 1, 2, 3 });
			var sut = list.ActiveSkip(skipCount);

			sut.CollectionChanged += (s, e) => raisedCount += e.Action == NotifyCollectionChangedAction.Add ? 1 : 0;

			skipCount.Value = 2;

			Assert.Equal(1, raisedCount);
		}

		[Fact]
		public void SkipMoreThanCountAndIncreaseSkip()
		{
			int raisedCount = 0;
			var skipCount = ActiveValue.Create(4);
			var list = ActiveList.Create(new[] { 1, 2, 3 });
			var sut = list.ActiveSkip(skipCount);

			sut.CollectionChanged += (s, e) => ++raisedCount;

			skipCount.Value = 5;

			Assert.Equal(0, raisedCount);
		}

		[Fact]
		public void SkipMoreThanCountAndDecreaseSkip()
		{
			int raisedCount = 0;
			var skipCount = ActiveValue.Create(4);
			var list = ActiveList.Create(new[] { 1, 2, 3 });
			var sut = list.ActiveSkip(skipCount);

			sut.CollectionChanged += (s, e) => ++raisedCount;

			skipCount.Value = 3;

			Assert.Equal(0, raisedCount);
		}
	}
}
