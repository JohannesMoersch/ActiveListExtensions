using ActiveListExtensions.Tests.Helpers;
using ActiveListExtensions.Utilities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xunit;

namespace ActiveListExtensions.Tests.ValueModifiers
{
	public class ActiveSetContainsTests
	{
		[Fact]
		public void DistinctContains() => TestWithRandomOperations(list => list.ActiveDistinct());

		[Fact]
		public void LookupContains() => TestWithRandomOperations(list => list.ToActiveLookup(i => i));

		[Fact]
		public void UnionContains() => TestTwoWithRandomOperations((l1, l2) => l1.ActiveUnion(l2), (l1, l2) => l1.Union(l2));

		[Fact]
		public void IntersectContains() => TestTwoWithRandomOperations((l1, l2) => l1.ActiveIntersect(l2), (l1, l2) => l1.Intersect(l2));

		[Fact]
		public void ExceptContains() => TestTwoWithRandomOperations((l1, l2) => l1.ActiveExcept(l2), (l1, l2) => l1.Except(l2));

		private void TestWithRandomOperations(Func<IActiveList<int>, IActiveSet<int>> operation)
		{
			RandomGenerator.ResetRandomGenerator();

			var list = new ObservableList<int>();
			foreach (var value in RandomGenerator.GenerateRandomIntegerList(100, 0, 50))
				list.Add(list.Count, value);

			var set = operation.Invoke(list);

			IActiveValue<bool> sut;
			if (set is IActiveSetList<int> setList)
				sut = setList.ActiveSetContains(25);
			else
				sut = (set as IActiveLookup<int, int>).ActiveSetContains(25);

			foreach (var value in Enumerable.Range(0, 1000))
			{
				switch (list.Count > 0 ? RandomGenerator.GenerateRandomInteger(0, 4) : 0)
				{
					case 0:
						list.Add(RandomGenerator.GenerateRandomInteger(0, list.Count), RandomGenerator.GenerateRandomInteger(0, 50));
						break;
					case 1:
						list.Remove(RandomGenerator.GenerateRandomInteger(0, list.Count));
						break;
					case 2:
						list.Replace(RandomGenerator.GenerateRandomInteger(0, list.Count), RandomGenerator.GenerateRandomInteger(0, 50));
						break;
					case 3:
						list.Move(RandomGenerator.GenerateRandomInteger(0, list.Count), RandomGenerator.GenerateRandomInteger(0, list.Count));
						break;
				}
				Assert.Equal(sut.Value, list.Contains(25));
			}
		}

		private void TestTwoWithRandomOperations(Func<IActiveList<int>, IActiveList<int>, IActiveSet<int>> operation, Func<IReadOnlyList<int>, IReadOnlyList<int>, IEnumerable<int>> linqOperation)
		{
			RandomGenerator.ResetRandomGenerator();

			var list1 = new ObservableList<int>();
			foreach (var value in RandomGenerator.GenerateRandomIntegerList(100, 0, 50))
				list1.Add(list1.Count, value);

			var list2 = new ObservableList<int>();
			foreach (var value in RandomGenerator.GenerateRandomIntegerList(100, 0, 50))
				list2.Add(list2.Count, value);

			var set = operation.Invoke(list1, list2);

			IActiveValue<bool> sut;
			if (set is IActiveSetList<int> setList)
				sut = setList.ActiveSetContains(25);
			else
				sut = (set as IActiveLookup<int, int>).ActiveSetContains(25);

			foreach (var value in Enumerable.Range(0, 1000))
			{
				switch (list1.Count > 0 ? RandomGenerator.GenerateRandomInteger(0, 4) : 0)
				{
					case 0:
						list1.Add(RandomGenerator.GenerateRandomInteger(0, list1.Count), RandomGenerator.GenerateRandomInteger(0, 50));
						break;
					case 1:
						list1.Remove(RandomGenerator.GenerateRandomInteger(0, list1.Count));
						break;
					case 2:
						list1.Replace(RandomGenerator.GenerateRandomInteger(0, list1.Count), RandomGenerator.GenerateRandomInteger(0, 50));
						break;
					case 3:
						list1.Move(RandomGenerator.GenerateRandomInteger(0, list1.Count), RandomGenerator.GenerateRandomInteger(0, list1.Count));
						break;
				}
				switch (list2.Count > 0 ? RandomGenerator.GenerateRandomInteger(0, 4) : 0)
				{
					case 0:
						list2.Add(RandomGenerator.GenerateRandomInteger(0, list2.Count), RandomGenerator.GenerateRandomInteger(0, 50));
						break;
					case 1:
						list2.Remove(RandomGenerator.GenerateRandomInteger(0, list2.Count));
						break;
					case 2:
						list2.Replace(RandomGenerator.GenerateRandomInteger(0, list2.Count), RandomGenerator.GenerateRandomInteger(0, 50));
						break;
					case 3:
						list2.Move(RandomGenerator.GenerateRandomInteger(0, list2.Count), RandomGenerator.GenerateRandomInteger(0, list2.Count));
						break;
				}
				Assert.Equal(sut.Value, linqOperation.Invoke(list1, list2).Contains(25));
			}
		}
	}
}
