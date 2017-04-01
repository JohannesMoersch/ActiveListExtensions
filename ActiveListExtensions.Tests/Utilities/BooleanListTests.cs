using ActiveListExtensions.Tests.Helpers;
using ActiveListExtensions.Utilities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xunit;

namespace ActiveListExtensions.Tests.Utilities
{
	public class BooleanListTests
	{
		private const int MaxCount = 1000;

		[Fact]
		public void RandomlyAddItems()
		{
			RandomGenerator.ResetRandomGenerator();

			var list = new List<bool>();
			var sut = new BooleanList();

			foreach (var value in Enumerable.Range(0, MaxCount))
			{
				var newValue = RandomGenerator.GenerateRandomInteger(0, 2) == 0;
				list.Add(newValue);
				sut.Add(newValue);
				ValidateLists(list, sut);
			}
		}

		[Fact]
		public void RandomlyInsertItems()
		{
			RandomGenerator.ResetRandomGenerator();

			var list = new List<bool>();
			var sut = new BooleanList();

			foreach (var value in Enumerable.Range(0, MaxCount))
			{
				var newIndex = RandomGenerator.GenerateRandomInteger(0, list.Count + 1);
				var newValue = RandomGenerator.GenerateRandomInteger(0, 2) == 0;
				list.Insert(newIndex, newValue);
				sut.Insert(newIndex, newValue);
				ValidateLists(list, sut);
			}
		}

		[Fact]
		public void RandomlyRemoveItems()
		{
			RandomGenerator.ResetRandomGenerator();

			CreateAndFillLists(out IList<bool> list, out IList<bool> sut);

			foreach (var value in Enumerable.Range(0, MaxCount))
			{
				var oldIndex = RandomGenerator.GenerateRandomInteger(0, list.Count);
				list.RemoveAt(oldIndex);
				sut.RemoveAt(oldIndex);
				ValidateLists(list, sut);
			}
		}

		[Fact]
		public void RandomlyReplaceItems()
		{
			RandomGenerator.ResetRandomGenerator();

			CreateAndFillLists(out IList<bool> list, out IList<bool> sut);

			foreach (var value in Enumerable.Range(0, MaxCount))
			{
				var newIndex = RandomGenerator.GenerateRandomInteger(0, list.Count);
				var newValue = RandomGenerator.GenerateRandomInteger(0, 2) == 0;
				list[newIndex] = newValue;
				sut[newIndex] = newValue;
				ValidateLists(list, sut);
			}
		}

		[Fact]
		public void RandomMixedOperations()
		{
			RandomGenerator.ResetRandomGenerator();

			CreateAndFillLists(out IList<bool> list, out IList<bool> sut);

			foreach (var value in Enumerable.Range(0, MaxCount))
			{
				var newIndex = RandomGenerator.GenerateRandomInteger(0, list.Count);
				var newValue = RandomGenerator.GenerateRandomInteger(0, 2) == 0;

				switch (list.Count > 0 ? RandomGenerator.GenerateRandomInteger(0, 50) : 0)
				{
					case 0:
						list.Insert(newIndex, newValue);
						sut.Insert(newIndex, newValue);
						break;
					case 1:
						list.RemoveAt(newIndex);
						sut.RemoveAt(newIndex);
						break;
					case 2:
						list[newIndex] = newValue;
						sut[newIndex] = newValue;
						break;
					default:
						list.Clear();
						sut.Clear();
						break;
				}
				ValidateLists(list, sut);
			}
		}

		private void CreateAndFillLists(out IList<bool> listOne, out IList<bool> listTwo)
		{
			var list = new List<bool>();
			var sut = new BooleanList();

			for (int i = 0; i < MaxCount; ++i)
			{
				var newValue = RandomGenerator.GenerateRandomInteger(0, 2) == 0;
				list.Add(newValue);
				sut.Add(newValue);
			}

			listOne = list;
			listTwo = sut;
		}

		private void ValidateLists(IList<bool> listOne, IList<bool> listTwo)
		{
			if (!listOne.SequenceEqual(listTwo))
				throw new Exception("Lists do not match.");
		}
	}
}
