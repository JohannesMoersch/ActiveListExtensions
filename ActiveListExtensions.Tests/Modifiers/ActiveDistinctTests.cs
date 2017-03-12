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
	public class ActiveDistinctTests
	{
		[Fact]
		public void RandomlyInsertItems() => CollectionTestHelpers.RandomlyInsertItems(l => l.ActiveDistinct(), l => l.Distinct(), () => RandomGenerator.GenerateRandomInteger(0, 10), true);

		[Fact]
		public void RandomlyRemoveItems() => CollectionTestHelpers.RandomlyRemoveItems(l => l.ActiveDistinct(), l => l.Distinct(), () => RandomGenerator.GenerateRandomInteger(0, 10), true);

		[Fact]
		public void RandomlyReplaceItems() => CollectionTestHelpers.RandomlyReplaceItems(l => l.ActiveDistinct(), l => l.Distinct(), () => RandomGenerator.GenerateRandomInteger(0, 10), true);

		[Fact]
		public void RandomlyMoveItems() => CollectionTestHelpers.RandomlyMoveItems(l => l.ActiveDistinct(), l => l.Distinct(), () => RandomGenerator.GenerateRandomInteger(0, 10), true);

		[Fact]
		public void ResetWithRandomItems() => CollectionTestHelpers.ResetWithRandomItems(l => l.ActiveDistinct(), l => l.Distinct(), () => RandomGenerator.GenerateRandomInteger(0, 10), true);

		[Fact]
		public void RandomlyChangePropertyValues() => CollectionTestHelpers.RandomlyChangePropertyValues(l => l.ActiveDistinct(o => o.Property), l => l.Distinct(new KeyEqualityComparer<ActiveDistinctTestClass>(o => o.Property)), () => new ActiveDistinctTestClass() { Property = RandomGenerator.GenerateRandomInteger() }, o => o.Property = RandomGenerator.GenerateRandomInteger(), true, o => o.Property);
	}

	public class ActiveDistinctTestClass : INotifyPropertyChanged
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

		public override string ToString() => $"{Property}";
	}
}
