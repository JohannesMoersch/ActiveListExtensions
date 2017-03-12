using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ActiveListExtensions.Tests.Helpers;
using Xunit;

namespace ActiveListExtensions.Tests.Modifiers
{
	public class ActiveExceptTests
	{
		[Fact]
		public void RandomlyInsertItems() => CollectionTestHelpers.RandomlyInsertItemsIntoTwoCollections((l1, l2) => l1.ActiveExcept(l2), (l1, l2) => l1.Except(l2), () => RandomGenerator.GenerateRandomInteger(0, 10), () => RandomGenerator.GenerateRandomInteger(0, 10), true);

		[Fact]
		public void RandomlyRemoveItems() => CollectionTestHelpers.RandomlyRemoveItemsFromTwoCollections((l1, l2) => l1.ActiveExcept(l2), (l1, l2) => l1.Except(l2), () => RandomGenerator.GenerateRandomInteger(0, 10), () => RandomGenerator.GenerateRandomInteger(0, 10), true);

		[Fact]
		public void RandomlyReplaceItems() => CollectionTestHelpers.RandomlyReplaceItemsInTwoCollections((l1, l2) => l1.ActiveExcept(l2), (l1, l2) => l1.Except(l2), () => RandomGenerator.GenerateRandomInteger(0, 10), () => RandomGenerator.GenerateRandomInteger(0, 10), true);

		[Fact]
		public void RandomlyMoveItems() => CollectionTestHelpers.RandomlyMoveItemsWithinTwoCollections((l1, l2) => l1.ActiveExcept(l2), (l1, l2) => l1.Except(l2), () => RandomGenerator.GenerateRandomInteger(0, 10), () => RandomGenerator.GenerateRandomInteger(0, 10), true);

		[Fact]
		public void ResetWithRandomItems() => CollectionTestHelpers.ResetTwoCollectionsWithRandomItems((l1, l2) => l1.ActiveExcept(l2), (l1, l2) => l1.Except(l2), () => RandomGenerator.GenerateRandomInteger(0, 10), () => RandomGenerator.GenerateRandomInteger(0, 10), true);

		[Fact]
		public void RandomlyChangePropertyValues() => CollectionTestHelpers.RandomlyChangePropertyValuesInTwoCollections((l1, l2) => l1.ActiveExcept(l2, o => o.Property), (l1, l2) => l1.Except(l2, new KeyEqualityComparer<ActiveExceptTestClass>(o => o.Property)), () => new ActiveExceptTestClass() { Property = RandomGenerator.GenerateRandomInteger() }, () => new ActiveExceptTestClass() { Property = RandomGenerator.GenerateRandomInteger() }, o => o.Property = RandomGenerator.GenerateRandomInteger(), o => o.Property = RandomGenerator.GenerateRandomInteger(), true, o => o.Property);
	}

	public class ActiveExceptTestClass : INotifyPropertyChanged
	{
		private int _property;
		public int Property
		{
			get { return _property; }
			set
			{
				if (_property == value)
					return;
				_property = value;
				PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(Property)));
			}
		}

		public event PropertyChangedEventHandler PropertyChanged;
	}
}
