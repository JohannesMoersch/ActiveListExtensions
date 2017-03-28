using ActiveListExtensions;
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
	public class ActiveElementAtOrDefaultTests
	{
		[Fact]
		public void RandomlyInsertItems() => ValueTestHelpers.RandomlyInsertItems(l => l.ActiveElementAtOrDefault(15), l => l.ElementAtOrDefault(15), RandomGenerator.GenerateRandomInteger);

		[Fact]
		public void RandomlyRemoveItems() => ValueTestHelpers.RandomlyRemoveItems(l => l.ActiveElementAtOrDefault(15), l => l.ElementAtOrDefault(15), RandomGenerator.GenerateRandomInteger);

		[Fact]
		public void RandomlyReplaceItems() => ValueTestHelpers.RandomlyReplaceItems(l => l.ActiveElementAtOrDefault(15), l => l.ElementAtOrDefault(15), RandomGenerator.GenerateRandomInteger);

		[Fact]
		public void RandomlyMoveItems() => ValueTestHelpers.RandomlyMoveItems(l => l.ActiveElementAtOrDefault(15), l => l.ElementAtOrDefault(15), RandomGenerator.GenerateRandomInteger);

		[Fact]
		public void RandomMixedOperations() => ValueTestHelpers.RandomMixedOperations(l => l.ActiveElementAtOrDefault(15), l => l.ElementAtOrDefault(15), RandomGenerator.GenerateRandomInteger);

		[Fact]
		public void ResetWithRandomItems() => ValueTestHelpers.ResetWithRandomItems(l => l.ActiveElementAtOrDefault(15), l => l.ElementAtOrDefault(15), RandomGenerator.GenerateRandomInteger);

		[Fact]
		public void RandomlyChangeIndex()
		{
			RandomGenerator.ResetRandomGenerator();

			var list = new ObservableList<int>();
			foreach (var value in Enumerable.Range(0, 100))
				list.Add(list.Count, RandomGenerator.GenerateRandomInteger());

			var index = new CustomActiveValue<int>();

			var sut = list.ActiveElementAtOrDefault(index);

			foreach (var value in Enumerable.Range(0, 100))
			{
				index.Value = RandomGenerator.GenerateRandomInteger(-10, 110);

				Assert.Equal(sut.Value, list.ElementAtOrDefault(index.Value));
			}
		}
	}
}
