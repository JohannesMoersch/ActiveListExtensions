using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ActiveListExtensions.Tests.Helpers;
using ActiveListExtensions.Utilities;
using Xunit;

namespace ActiveListExtensions.Tests.Utilities
{
	public class ObservableListTests
	{
		[Fact]
		public void RandomlyInsertItems()
		{
			RandomGenerator.ResetRandomGenerator();

			var sut = new ObservableList<int>();
			var watcher = new CollectionSynchronizationWatcher<int>(sut);

			foreach (var value in Enumerable.Range(0, 100))
				sut.Add(RandomGenerator.GenerateRandomInteger(0, sut.Count), value);

			Assert.Equal(sut.Count, 100);
		}

		[Fact]
		public void RandomlyRemoveItems()
		{
			RandomGenerator.ResetRandomGenerator();

			var sut = new ObservableList<int>();
			foreach (var value in Enumerable.Range(0, 100))
				sut.Add(sut.Count, value);

			var watcher = new CollectionSynchronizationWatcher<int>(sut);

			foreach (var value in Enumerable.Range(0, 100))
				sut.Remove(RandomGenerator.GenerateRandomInteger(0, sut.Count));

			Assert.Equal(sut.Count, 0);
		}

		[Fact]
		public void RandomlyReplaceItems()
		{
			RandomGenerator.ResetRandomGenerator();

			var sut = new ObservableList<int>();
			foreach (var value in Enumerable.Range(0, 100))
				sut.Add(sut.Count, value);

			var watcher = new CollectionSynchronizationWatcher<int>(sut);

			foreach (var value in Enumerable.Range(0, 100))
				sut.Replace(RandomGenerator.GenerateRandomInteger(0, sut.Count), RandomGenerator.GenerateRandomInteger(0, 1000));

			Assert.Equal(sut.Count, 100);
		}

		[Fact]
		public void RandomlyMoveItems()
		{
			RandomGenerator.ResetRandomGenerator();

			var sut = new ObservableList<int>();
			foreach (var value in Enumerable.Range(0, 100))
				sut.Add(sut.Count, value);

			var watcher = new CollectionSynchronizationWatcher<int>(sut);

			foreach (var value in Enumerable.Range(0, 100))
				sut.Move(RandomGenerator.GenerateRandomInteger(0, sut.Count), RandomGenerator.GenerateRandomInteger(0, sut.Count));

			Assert.Equal(sut.Count, 100);
		}

		[Fact]
		public void ResetWithRandomItems()
		{
			RandomGenerator.ResetRandomGenerator();

			var sut = new ObservableList<int>();
			foreach (var value in Enumerable.Range(0, 100))
				sut.Add(sut.Count, value);

			var watcher = new CollectionSynchronizationWatcher<int>(sut);

			int count = 0;
			foreach (var value in Enumerable.Range(0, 50))
			{
				count = RandomGenerator.GenerateRandomInteger(0, 30);
				sut.Reset(Enumerable.Range(0, count).Select(i => RandomGenerator.GenerateRandomInteger(0, 1000)));
			}

			Assert.Equal(sut.Count, count);
		}

		[Fact]
		public void RandomlyReplaceRangesOfItems()
		{
			RandomGenerator.ResetRandomGenerator();

			var sut = new ObservableList<int>();
			foreach (var value in Enumerable.Range(0, 100))
				sut.Add(sut.Count, value);

			var watcher = new CollectionSynchronizationWatcher<int>(sut);

			var count = 100;
			foreach (var value in Enumerable.Range(0, 50))
			{
				var start = RandomGenerator.GenerateRandomInteger(0, sut.Count);
				var oldCount = RandomGenerator.GenerateRandomInteger(0, sut.Count - start);
				var newItems = Enumerable.Range(0, 50).Select(i => RandomGenerator.GenerateRandomInteger(0, 1000)).ToArray();
				count -= oldCount;
				count += newItems.Length;
				sut.ReplaceRange(start, oldCount, newItems);
			}

			Assert.Equal(sut.Count, count);
		}

		[Fact]
		public void RandomlyMoveRangesOfItems()
		{
			RandomGenerator.ResetRandomGenerator();

			var sut = new ObservableList<int>();
			foreach (var value in Enumerable.Range(0, 100))
				sut.Add(sut.Count, value);

			var watcher = new CollectionSynchronizationWatcher<int>(sut);

			foreach (var value in Enumerable.Range(0, 50))
			{
				var count = RandomGenerator.GenerateRandomInteger(0, sut.Count + 1);
				var start = RandomGenerator.GenerateRandomInteger(0, sut.Count - count + 1);
				var target = RandomGenerator.GenerateRandomInteger(0, sut.Count - count + 1);
				sut.MoveRange(start, target, count);
			}

			Assert.Equal(sut.Count, 100);
		}

		[Fact]
		public void MoveRangeUp()
		{
			var sut = new ObservableList<int>();
			foreach (var value in Enumerable.Range(0, 11))
				sut.Add(sut.Count, value);

			sut.MoveRange(5, 3, 3);

			Assert.True(sut.SequenceEqual(new[] { 0, 1, 2, 5, 6, 7, 3, 4, 8, 9, 10 }));
		}

		[Fact]
		public void MoveRangeDown()
		{
			var sut = new ObservableList<int>();
			foreach (var value in Enumerable.Range(0, 11))
				sut.Add(sut.Count, value);

			sut.MoveRange(3, 6, 2);

			Assert.True(sut.SequenceEqual(new[] { 0, 1, 2, 5, 6, 7, 3, 4, 8, 9, 10 }));
		}
	}
}
