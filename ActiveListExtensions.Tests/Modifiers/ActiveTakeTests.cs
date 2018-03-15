using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ActiveListExtensions.Tests.Helpers;
using Xunit;
using ActiveListExtensions.Utilities;
using ActiveListExtensions.Tests.Utilities;
using System.Collections.Specialized;

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

		[Fact]
		public void RandomlyChangeIndex()
		{
			RandomGenerator.ResetRandomGenerator();

			var list = new ObservableList<int>();
			foreach (var value in Enumerable.Range(0, 100))
				list.Add(list.Count, RandomGenerator.GenerateRandomInteger());

			var index = new ActiveValue<int>();

			var sut = list.ActiveTake(index);
			var watcher = new CollectionSynchronizationWatcher<int>(sut);
			var validator = new LinqValidator<int, int, int>(list, sut, l => l.Take(index.Value), false, null);

			foreach (var value in Enumerable.Range(0, 100))
			{
				index.Value = RandomGenerator.GenerateRandomInteger(-10, 110);

				validator.Validate();
			}
		}

		[Fact]
		public void TakeLessThanCountAndIncreaseTake()
		{
			int raisedCount = 0;
			var takeCount = ActiveValue.Create(2);
			var list = ActiveList.Create(new[] { 1, 2, 3 });
			var sut = list.ActiveTake(takeCount);

			sut.CollectionChanged += (s, e) => raisedCount += e.Action == NotifyCollectionChangedAction.Add ? 1 : 0;

			takeCount.Value = 3;

			Assert.Equal(1, raisedCount);
		}

		[Fact]
		public void TakeLessThanCountAndDecreaseTake()
		{
			int raisedCount = 0;
			var takeCount = ActiveValue.Create(2);
			var list = ActiveList.Create(new[] { 1, 2, 3 });
			var sut = list.ActiveTake(takeCount);

			sut.CollectionChanged += (s, e) => raisedCount += e.Action == NotifyCollectionChangedAction.Remove ? 1 : 0;

			takeCount.Value = 1;

			Assert.Equal(1, raisedCount);
		}

		[Fact]
		public void TakeCountAndIncreaseTake()
		{
			int raisedCount = 0;
			var takeCount = ActiveValue.Create(3);
			var list = ActiveList.Create(new[] { 1, 2, 3 });
			var sut = list.ActiveTake(takeCount);

			sut.CollectionChanged += (s, e) => ++raisedCount;

			takeCount.Value = 4;

			Assert.Equal(0, raisedCount);
		}

		[Fact]
		public void TakeCountAndDecreaseTake()
		{
			int raisedCount = 0;
			var takeCount = ActiveValue.Create(3);
			var list = ActiveList.Create(new[] { 1, 2, 3 });
			var sut = list.ActiveTake(takeCount);

			sut.CollectionChanged += (s, e) => raisedCount += e.Action == NotifyCollectionChangedAction.Remove ? 1 : 0;

			takeCount.Value = 2;

			Assert.Equal(1, raisedCount);
		}

		[Fact]
		public void TakeMoreThanCountAndIncreaseTake()
		{
			int raisedCount = 0;
			var takeCount = ActiveValue.Create(4);
			var list = ActiveList.Create(new[] { 1, 2, 3 });
			var sut = list.ActiveTake(takeCount);

			sut.CollectionChanged += (s, e) => ++raisedCount;

			takeCount.Value = 5;

			Assert.Equal(0, raisedCount);
		}

		[Fact]
		public void TakeMoreThanCountAndDecreaseTake()
		{
			int raisedCount = 0;
			var takeCount = ActiveValue.Create(4);
			var list = ActiveList.Create(new[] { 1, 2, 3 });
			var sut = list.ActiveTake(takeCount);

			sut.CollectionChanged += (s, e) => ++raisedCount;

			takeCount.Value = 3;

			Assert.Equal(0, raisedCount);
		}
	}
}
