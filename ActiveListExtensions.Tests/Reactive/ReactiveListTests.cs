using ActiveListExtensions.Tests.Helpers;
using ActiveListExtensions.Utilities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xunit;

namespace ActiveListExtensions.Tests.Reactive
{
	public class ReactiveListTests
	{
		[Fact]
		public void ObserveAddedOnRandomOperations()
		{
			RandomGenerator.ResetRandomGenerator();

			var list = new ObservableList<object>();
			foreach (var value in Enumerable.Range(0, 100))
				list.Add(list.Count, new object());

			object lastAdded = null;
			object expected = null;
			var sut = list.ObserveAdded().Subscribe(i => lastAdded = i);

			foreach (var value in Enumerable.Range(0, 1000))
			{
				switch (list.Count > 0 ? RandomGenerator.GenerateRandomInteger(0, 4) : 0)
				{
					case 0:
						expected = new object();
						list.Add(RandomGenerator.GenerateRandomInteger(0, list.Count), expected);
						break;
					case 1:
						list.Remove(RandomGenerator.GenerateRandomInteger(0, list.Count));
						break;
					case 2:
						expected = new object();
						list.Replace(RandomGenerator.GenerateRandomInteger(0, list.Count), expected);
						break;
					case 3:
						list.Move(RandomGenerator.GenerateRandomInteger(0, list.Count), RandomGenerator.GenerateRandomInteger(0, list.Count));
						break;
				}
				if (expected != null)
					Assert.Equal(expected, lastAdded);
				expected = null;
			}
		}

		[Fact]
		public void ObserveAddedOnReset()
		{
			RandomGenerator.ResetRandomGenerator();

			var list = new ObservableList<object>();
			foreach (var value in Enumerable.Range(0, 150))
				list.Add(list.Count, new object());

			var lastAdded = new List<object>();
			var sut = list.ObserveAdded().Subscribe(i => lastAdded.Add(i));

			for (int i = 0; i < 50; ++i)
				list.Remove(i);

			list.Reset(Enumerable.Range(0, 100).Select(i => new object()));

			Assert.Equal(lastAdded.Count, list.Count);
			Assert.True(lastAdded.Intersect(list).Count() == list.Count);
		}

		[Fact]
		public void ObserveRemovedOnRandomOperations()
		{
			RandomGenerator.ResetRandomGenerator();

			var list = new ObservableList<object>();
			foreach (var value in Enumerable.Range(0, 100))
				list.Add(list.Count, new object());

			object lastRemoved = null;
			object expected = null;
			var sut = list.ObserveRemoved().Subscribe(i => lastRemoved = i);

			foreach (var value in Enumerable.Range(0, 1000))
			{
				switch (list.Count > 0 ? RandomGenerator.GenerateRandomInteger(0, 4) : 0)
				{
					case 0:
						list.Add(RandomGenerator.GenerateRandomInteger(0, list.Count), new object());
						break;
					case 1:
						var removeIndex = RandomGenerator.GenerateRandomInteger(0, list.Count);
						expected = list[removeIndex];
						list.Remove(removeIndex);
						break;
					case 2:
						var replaceIndex = RandomGenerator.GenerateRandomInteger(0, list.Count);
						expected = list[replaceIndex];
						list.Replace(replaceIndex, RandomGenerator.GenerateRandomInteger());
						break;
					case 3:
						list.Move(RandomGenerator.GenerateRandomInteger(0, list.Count), RandomGenerator.GenerateRandomInteger(0, list.Count));
						break;
				}
				if (expected != null)
					Assert.Equal(expected, lastRemoved);
				expected = null;
			}
		}

		[Fact]
		public void ObserveRemovedOnReset()
		{
			RandomGenerator.ResetRandomGenerator();

			var list = new ObservableList<object>();
			foreach (var value in Enumerable.Range(0, 50))
				list.Add(list.Count, new object());

			var lastRemoved = new List<object>();
			var sut = list.ObserveRemoved().Subscribe(i => lastRemoved.Add(i));

			foreach (var value in Enumerable.Range(0, 50))
				list.Add(list.Count, new object());

			var originalList = list.ToArray();

			list.Reset(Enumerable.Range(0, 100).Select(i => new object()));

			Assert.Equal(lastRemoved.Count, originalList.Length);
			Assert.True(lastRemoved.Intersect(originalList).Count() == originalList.Length);
		}
	}
}
