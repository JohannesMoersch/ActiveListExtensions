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
	public class ActiveSingleOrDefaultTests
	{
		[Fact]
		public void RandomlyInsertItems() => ValueTestHelpers.RandomlyInsertItems(l => l.ActiveSingleOrDefault(i => i.Property % 3 == 0), l => l.SingleOrDefault(i => i.Property % 3 == 0), () => new ActiveSingleOrDefaultTestClass() { Property = RandomGenerator.GenerateRandomInteger() });

		[Fact]
		public void RandomlyRemoveItems() => ValueTestHelpers.RandomlyRemoveItems(l => l.ActiveSingleOrDefault(i => i.Property % 3 == 0), l => l.SingleOrDefault(i => i.Property % 3 == 0), () => new ActiveSingleOrDefaultTestClass() { Property = RandomGenerator.GenerateRandomInteger() });

		[Fact]
		public void RandomlyReplaceItems() => ValueTestHelpers.RandomlyReplaceItems(l => l.ActiveSingleOrDefault(i => i.Property % 3 == 0), l => l.SingleOrDefault(i => i.Property % 3 == 0), () => new ActiveSingleOrDefaultTestClass() { Property = RandomGenerator.GenerateRandomInteger() });

		[Fact]
		public void RandomlyMoveItems() => ValueTestHelpers.RandomlyMoveItems(l => l.ActiveSingleOrDefault(i => i.Property % 3 == 0), l => l.SingleOrDefault(i => i.Property % 3 == 0), () => new ActiveSingleOrDefaultTestClass() { Property = RandomGenerator.GenerateRandomInteger() });

		[Fact]
		public void ResetWithRandomItems() => ValueTestHelpers.ResetWithRandomItems(l => l.ActiveSingleOrDefault(i => i.Property % 3 == 0), l => l.SingleOrDefault(i => i.Property % 3 == 0), () => new ActiveSingleOrDefaultTestClass() { Property = RandomGenerator.GenerateRandomInteger() });

		[Fact]
		public void RandomlyChangePropertyValues() => ValueTestHelpers.RandomlyChangePropertyValues(l => l.ActiveSingleOrDefault(i => i.Property % 3 == 0), l => l.SingleOrDefault(i => i.Property % 3 == 0), () => new ActiveSingleOrDefaultTestClass() { Property = RandomGenerator.GenerateRandomInteger() }, o => o.Property = RandomGenerator.GenerateRandomInteger());
	}

	public class ActiveSingleOrDefaultTestClass : INotifyPropertyChanged
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