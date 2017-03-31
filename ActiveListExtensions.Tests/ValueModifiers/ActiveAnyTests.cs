using ActiveListExtensions.Tests.Helpers;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xunit;

namespace ActiveListExtensions.Tests.ValueModifiers
{
	public class ActiveAnyTests
	{
		[Fact]
		public void RandomlyInsertItems() => ValueTestHelpers.RandomlyInsertItems(l => l.ActiveAny(), l => l.Any(), () => new ActiveAnyTestClass() { Property = RandomGenerator.GenerateRandomInteger() });

		[Fact]
		public void RandomlyRemoveItems() => ValueTestHelpers.RandomlyRemoveItems(l => l.ActiveAny(), l => l.Any(), () => new ActiveAnyTestClass() { Property = RandomGenerator.GenerateRandomInteger() });

		[Fact]
		public void RandomlyReplaceItems() => ValueTestHelpers.RandomlyReplaceItems(l => l.ActiveAny(), l => l.Any(), () => new ActiveAnyTestClass() { Property = RandomGenerator.GenerateRandomInteger() });

		[Fact]
		public void RandomlyMoveItems() => ValueTestHelpers.RandomlyMoveItems(l => l.ActiveAny(), l => l.Any(), () => new ActiveAnyTestClass() { Property = RandomGenerator.GenerateRandomInteger() });

		[Fact]
		public void RandomMixedOperations() => ValueTestHelpers.RandomMixedOperations(l => l.ActiveAny(), l => l.Any(), () => new ActiveAnyTestClass() { Property = RandomGenerator.GenerateRandomInteger() });

		[Fact]
		public void ResetWithRandomItems() => ValueTestHelpers.ResetWithRandomItems(l => l.ActiveAny(), l => l.Any(), () => new ActiveAnyTestClass() { Property = RandomGenerator.GenerateRandomInteger() });

		[Fact]
		public void RandomlyInsertItemsWithPredicate() => ValueTestHelpers.RandomlyInsertItems(l => l.ActiveAny(i => i.Property % 20 == 0), l => l.Any(i => i.Property % 20 == 0), () => new ActiveAnyTestClass() { Property = RandomGenerator.GenerateRandomInteger() });

		[Fact]
		public void RandomlyRemoveItemsWithPredicate() => ValueTestHelpers.RandomlyRemoveItems(l => l.ActiveAny(i => i.Property % 20 == 0), l => l.Any(i => i.Property % 20 == 0), () => new ActiveAnyTestClass() { Property = RandomGenerator.GenerateRandomInteger() });

		[Fact]
		public void RandomlyReplaceItemsWithPredicate() => ValueTestHelpers.RandomlyReplaceItems(l => l.ActiveAny(i => i.Property % 20 == 0), l => l.Any(i => i.Property % 20 == 0), () => new ActiveAnyTestClass() { Property = RandomGenerator.GenerateRandomInteger() });

		[Fact]
		public void RandomlyMoveItemsWithPredicate() => ValueTestHelpers.RandomlyMoveItems(l => l.ActiveAny(i => i.Property % 20 == 0), l => l.Any(i => i.Property % 20 == 0), () => new ActiveAnyTestClass() { Property = RandomGenerator.GenerateRandomInteger() });

		[Fact]
		public void RandomMixedOperationsWithPredicate() => ValueTestHelpers.RandomMixedOperations(l => l.ActiveAny(i => i.Property % 20 == 0), l => l.Any(i => i.Property % 20 == 0), () => new ActiveAnyTestClass() { Property = RandomGenerator.GenerateRandomInteger() });

		[Fact]
		public void ResetWithRandomItemsWithPredicate() => ValueTestHelpers.ResetWithRandomItems(l => l.ActiveAny(i => i.Property % 20 == 0), l => l.Any(i => i.Property % 20 == 0), () => new ActiveAnyTestClass() { Property = RandomGenerator.GenerateRandomInteger() });

		[Fact]
		public void RandomlyChangePropertyValues() => ValueTestHelpers.RandomlyChangePropertyValues(l => l.ActiveAny(i => i.Property % 20 == 0), l => l.Any(i => i.Property % 20 == 0), () => new ActiveAnyTestClass() { Property = RandomGenerator.GenerateRandomInteger() }, o => o.Property = RandomGenerator.GenerateRandomInteger());
	}

	public class ActiveAnyTestClass : INotifyPropertyChanged
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
