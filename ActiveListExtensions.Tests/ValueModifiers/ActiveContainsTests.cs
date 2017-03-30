using ActiveListExtensions.Tests.Helpers;
using ActiveListExtensions.Tests.Utilities;
using ActiveListExtensions.Utilities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xunit;

namespace ActiveListExtensions.Tests.ValueModifiers
{
	public class ActiveContainsTests
	{
		[Fact]
		public void RandomlyInsertItems() => ValueTestHelpers.RandomlyInsertItems(l => l.ActiveContains(50), l => l.Contains(50), () => RandomGenerator.GenerateRandomInteger(0, 100));

		[Fact]
		public void RandomlyRemoveItems() => ValueTestHelpers.RandomlyRemoveItems(l => l.ActiveContains(50), l => l.Contains(50), () => RandomGenerator.GenerateRandomInteger(0, 100));

		[Fact]
		public void RandomlyReplaceItems() => ValueTestHelpers.RandomlyReplaceItems(l => l.ActiveContains(50), l => l.Contains(50), () => RandomGenerator.GenerateRandomInteger(0, 100));

		[Fact]
		public void RandomlyMoveItems() => ValueTestHelpers.RandomlyMoveItems(l => l.ActiveContains(50), l => l.Contains(50), () => RandomGenerator.GenerateRandomInteger(0, 100));

		[Fact]
		public void RandomMixedOperations() => ValueTestHelpers.RandomMixedOperations(l => l.ActiveContains(50), l => l.Contains(50), () => RandomGenerator.GenerateRandomInteger(0, 100));

		[Fact]
		public void ResetWithRandomItems() => ValueTestHelpers.ResetWithRandomItems(l => l.ActiveContains(50), l => l.Contains(50), () => RandomGenerator.GenerateRandomInteger(0, 100));

		[Fact]
		public void RandomlyChangeActiveValue()
		{
			RandomGenerator.ResetRandomGenerator();

			var list = new ObservableList<int>();
			foreach (var value in Enumerable.Range(0, 100))
				list.Add(list.Count, RandomGenerator.GenerateRandomInteger());

			var compareValue = new CustomActiveValue<int>();

			var sut = list.ActiveContains(compareValue);

			foreach (var value in Enumerable.Range(0, 100))
			{
				compareValue.Value = RandomGenerator.GenerateRandomInteger(-10, 110);

				Assert.Equal(list.Contains(compareValue.Value), sut.Value);
			}
		}
	}
}
