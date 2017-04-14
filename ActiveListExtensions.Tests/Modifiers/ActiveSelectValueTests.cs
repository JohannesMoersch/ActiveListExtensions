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
	public class ActiveSelectValueTests
	{
		[Fact]
		public void RandomlyChangeParameter() => CollectionTestHelpers.RandomlyChangeParameter((l, p) => l.ActiveSelect(p, (o, i) => o.IntProperty.ToActiveValue(x => x.Property * i.Property)), (l, p) => l.Select(o => o.IntProperty.Property * p.Property), GenerateRandom);

		[Fact]
		public void RandomlyInsertItems() => CollectionTestHelpers.RandomlyInsertItems(l => l.ActiveSelect(o => o.IntProperty.ToActiveValue(x => x.Property)), l => l.Select(o => o.IntProperty.Property), GenerateRandom);

		[Fact]
		public void RandomlyRemoveItems() => CollectionTestHelpers.RandomlyRemoveItems(l => l.ActiveSelect(o => o.IntProperty.ToActiveValue(x => x.Property)), l => l.Select(o => o.IntProperty.Property), GenerateRandom);

		[Fact]
		public void RandomlyReplaceItems() => CollectionTestHelpers.RandomlyReplaceItems(l => l.ActiveSelect(o => o.IntProperty.ToActiveValue(x => x.Property)), l => l.Select(o => o.IntProperty.Property), GenerateRandom);

		[Fact]
		public void RandomlyMoveItems() => CollectionTestHelpers.RandomlyMoveItems(l => l.ActiveSelect(o => o.IntProperty.ToActiveValue(x => x.Property)), l => l.Select(o => o.IntProperty.Property), GenerateRandom);

		[Fact]
		public void ResetWithRandomItems() => CollectionTestHelpers.ResetWithRandomItems(l => l.ActiveSelect(o => o.IntProperty.ToActiveValue(x => x.Property)), l => l.Select(o => o.IntProperty.Property), GenerateRandom);

		[Fact]
		public void RandomlyChangeOuterPropertyValues() => CollectionTestHelpers.RandomlyChangePropertyValues(l => l.ActiveSelect(o => o.IntProperty.ToActiveValue(x => x.Property)), l => l.Select(o => o.IntProperty.Property), GenerateRandom, o => o.IntProperty = new IntegerTestClass() { Property = RandomGenerator.GenerateRandomInteger() });

		[Fact]
		public void RandomlyChangeInnerPropertyValues() => CollectionTestHelpers.RandomlyChangePropertyValues(l => l.ActiveSelect(o => o.IntProperty.ToActiveValue(x => x.Property)), l => l.Select(o => o.IntProperty.Property), GenerateRandom, o => o.IntProperty.Property = RandomGenerator.GenerateRandomInteger());

		private ActiveSelectValueTestClass GenerateRandom()
		{
			return new ActiveSelectValueTestClass()
			{
				IntProperty = new IntegerTestClass()
				{
					Property = RandomGenerator.GenerateRandomInteger()
				}
			};
		}

		public class ActiveSelectValueTestClass : INotifyPropertyChanged
		{
			private IntegerTestClass _intProperty;
			public IntegerTestClass IntProperty
			{
				get { return _intProperty; }
				set
				{
					if (_intProperty == value)
						return;
					_intProperty = value;
					PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(IntProperty)));
				}
			}

			public event PropertyChangedEventHandler PropertyChanged;
		}
	}
}
