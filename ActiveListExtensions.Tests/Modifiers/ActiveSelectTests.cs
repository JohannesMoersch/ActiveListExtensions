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
	public class ActiveSelectTests
	{
		[Fact]
		public void RandomlyInsertItems() => CollectionTestHelpers.RandomlyInsertItems(l => l.ActiveSelect(o => o.Property), l => l.Select(o => o.Property), () => new ActiveSelectTestClass() { Property = RandomGenerator.GenerateRandomInteger() });

		[Fact]
		public void RandomlyRemoveItems() => CollectionTestHelpers.RandomlyRemoveItems(l => l.ActiveSelect(o => o.Property), l => l.Select(o => o.Property), () => new ActiveSelectTestClass() { Property = RandomGenerator.GenerateRandomInteger() });

		[Fact]
		public void RandomlyReplaceItems() => CollectionTestHelpers.RandomlyReplaceItems(l => l.ActiveSelect(o => o.Property), l => l.Select(o => o.Property), () => new ActiveSelectTestClass() { Property = RandomGenerator.GenerateRandomInteger() });

		[Fact]
		public void RandomlyMoveItems() => CollectionTestHelpers.RandomlyMoveItems(l => l.ActiveSelect(o => o.Property), l => l.Select(o => o.Property), () => new ActiveSelectTestClass() { Property = RandomGenerator.GenerateRandomInteger() });

		[Fact]
		public void ResetWithRandomItems() => CollectionTestHelpers.ResetWithRandomItems(l => l.ActiveSelect(o => o.Property), l => l.Select(o => o.Property), () => new ActiveSelectTestClass() { Property = RandomGenerator.GenerateRandomInteger() });

		[Fact]
		public void RandomlyChangePropertyValues() => CollectionTestHelpers.RandomlyChangePropertyValues(l => l.ActiveSelect(o => o.Property), l => l.Select(o => o.Property), () => new ActiveSelectTestClass() { Property = RandomGenerator.GenerateRandomInteger() }, o => o.Property = RandomGenerator.GenerateRandomInteger());
	}

	public class ActiveSelectTestClass : INotifyPropertyChanged
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
