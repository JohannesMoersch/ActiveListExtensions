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
		public void RandomlyInsertItems() => ValueTestHelpers.RandomlyInsertItems(l => l.ActiveFirstOrDefault(i => i.Property % 3 == 0), l => l.FirstOrDefault(i => i.Property % 3 == 0), () => new ActiveFirstOrDefaultTestClass() { Property = RandomGenerator.GenerateRandomInteger() });

		[Fact]
		public void RandomlyRemoveItems() => ValueTestHelpers.RandomlyRemoveItems(l => l.ActiveFirstOrDefault(i => i.Property % 3 == 0), l => l.FirstOrDefault(i => i.Property % 3 == 0), () => new ActiveFirstOrDefaultTestClass() { Property = RandomGenerator.GenerateRandomInteger() });

		[Fact]
		public void RandomlyReplaceItems() => ValueTestHelpers.RandomlyReplaceItems(l => l.ActiveFirstOrDefault(i => i.Property % 3 == 0), l => l.FirstOrDefault(i => i.Property % 3 == 0), () => new ActiveFirstOrDefaultTestClass() { Property = RandomGenerator.GenerateRandomInteger() });

		[Fact]
		public void RandomlyMoveItems() => ValueTestHelpers.RandomlyMoveItems(l => l.ActiveFirstOrDefault(i => i.Property % 3 == 0), l => l.FirstOrDefault(i => i.Property % 3 == 0), () => new ActiveFirstOrDefaultTestClass() { Property = RandomGenerator.GenerateRandomInteger() });

		[Fact]
		public void ResetWithRandomItems() => ValueTestHelpers.ResetWithRandomItems(l => l.ActiveFirstOrDefault(i => i.Property % 3 == 0), l => l.FirstOrDefault(i => i.Property % 3 == 0), () => new ActiveFirstOrDefaultTestClass() { Property = RandomGenerator.GenerateRandomInteger() });

		[Fact]
		public void RandomlyChangePropertyValues() => ValueTestHelpers.RandomlyChangePropertyValues(l => l.ActiveFirstOrDefault(i => i.Property % 3 == 0), l => l.FirstOrDefault(i => i.Property % 3 == 0), () => new ActiveFirstOrDefaultTestClass() { Property = RandomGenerator.GenerateRandomInteger() }, o => o.Property = RandomGenerator.GenerateRandomInteger());
	}

	public class ActiveFirstOrDefaultTestClass
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
