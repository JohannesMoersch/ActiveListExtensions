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
	public class ActiveFirstOrDefaultTests
	{
		[Fact]
		public void RandomlyChangeParameter() => ValueTestHelpers.RandomlyChangeParameter((l, p) => l.ActiveFirstOrDefault(p, (o, i) => o.Property > i.Property), (l, p) => l.FirstOrDefault(i => i.Property > p.Property), () => new IntegerTestClass() { Property = RandomGenerator.GenerateRandomInteger(0, 100) }, () => RandomGenerator.GenerateRandomInteger(-50, 150));

		[Fact]
		public void RandomlyInsertItems() => ValueTestHelpers.RandomlyInsertItems(l => l.ActiveFirstOrDefault(i => i.Property % 20 == 0), l => l.FirstOrDefault(i => i.Property % 20 == 0), () => new ActiveFirstOrDefaultTestClass() { Property = RandomGenerator.GenerateRandomInteger() });

		[Fact]
		public void RandomlyRemoveItems() => ValueTestHelpers.RandomlyRemoveItems(l => l.ActiveFirstOrDefault(i => i.Property % 20 == 0), l => l.FirstOrDefault(i => i.Property % 20 == 0), () => new ActiveFirstOrDefaultTestClass() { Property = RandomGenerator.GenerateRandomInteger() });

		[Fact]
		public void RandomlyReplaceItems() => ValueTestHelpers.RandomlyReplaceItems(l => l.ActiveFirstOrDefault(i => i.Property % 20 == 0), l => l.FirstOrDefault(i => i.Property % 20 == 0), () => new ActiveFirstOrDefaultTestClass() { Property = RandomGenerator.GenerateRandomInteger() });

		[Fact]
		public void RandomlyMoveItems() => ValueTestHelpers.RandomlyMoveItems(l => l.ActiveFirstOrDefault(i => i.Property % 20 == 0), l => l.FirstOrDefault(i => i.Property % 20 == 0), () => new ActiveFirstOrDefaultTestClass() { Property = RandomGenerator.GenerateRandomInteger() });

		[Fact]
		public void RandomMixedOperations() => ValueTestHelpers.RandomMixedOperations(l => l.ActiveFirstOrDefault(i => i.Property % 20 == 0), l => l.FirstOrDefault(i => i.Property % 20 == 0), () => new ActiveFirstOrDefaultTestClass() { Property = RandomGenerator.GenerateRandomInteger() });

		[Fact]
		public void ResetWithRandomItems() => ValueTestHelpers.ResetWithRandomItems(l => l.ActiveFirstOrDefault(i => i.Property % 20 == 0), l => l.FirstOrDefault(i => i.Property % 20 == 0), () => new ActiveFirstOrDefaultTestClass() { Property = RandomGenerator.GenerateRandomInteger() });

		[Fact]
		public void RandomlyChangePropertyValues() => ValueTestHelpers.RandomlyChangePropertyValues(l => l.ActiveFirstOrDefault(i => i.Property % 20 == 0), l => l.FirstOrDefault(i => i.Property % 20 == 0), () => new ActiveFirstOrDefaultTestClass() { Property = RandomGenerator.GenerateRandomInteger() }, o => o.Property = RandomGenerator.GenerateRandomInteger());
	}

	public class ActiveFirstOrDefaultTestClass : INotifyPropertyChanged
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
