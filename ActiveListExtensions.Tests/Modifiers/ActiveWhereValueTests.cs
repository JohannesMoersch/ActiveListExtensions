using ActiveListExtensions.Tests.Helpers;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xunit;

namespace ActiveListExtensions.Tests.Modifiers
{
	public class ActiveWhereValueTests
	{
		[Fact]
		public void RandomlyChangeParameter() => CollectionTestHelpers.RandomlyChangeParameter((l, p) => l.ActiveWhere(p, (o, i) => o.IntProperty.ToActiveValue(x => (x.Property * i.Property) % 3 == 0)), (l, p) => l.Where(o => (o.IntProperty.Property * p.Property) % 3 == 0), GenerateRandom);

		[Fact]
		public void RandomlyInsertItems() => CollectionTestHelpers.RandomlyInsertItems(l => l.ActiveWhere(o => o.IntProperty.ToActiveValue(x => x.Property % 3 == 0)), l => l.Where(o => o.IntProperty.Property % 3 == 0), GenerateRandom);

		[Fact]
		public void RandomlyRemoveItems() => CollectionTestHelpers.RandomlyRemoveItems(l => l.ActiveWhere(o => o.IntProperty.ToActiveValue(x => x.Property % 3 == 0)), l => l.Where(o => o.IntProperty.Property % 3 == 0), GenerateRandom);

		[Fact]
		public void RandomlyReplaceItems() => CollectionTestHelpers.RandomlyReplaceItems(l => l.ActiveWhere(o => o.IntProperty.ToActiveValue(x => x.Property % 3 == 0)), l => l.Where(o => o.IntProperty.Property % 3 == 0), GenerateRandom);

		[Fact]
		public void RandomlyMoveItems() => CollectionTestHelpers.RandomlyMoveItems(l => l.ActiveWhere(o => o.IntProperty.ToActiveValue(x => x.Property % 3 == 0)), l => l.Where(o => o.IntProperty.Property % 3 == 0), GenerateRandom);

		[Fact]
		public void ResetWithRandomItems() => CollectionTestHelpers.ResetWithRandomItems(l => l.ActiveWhere(o => o.IntProperty.ToActiveValue(x => x.Property % 3 == 0)), l => l.Where(o => o.IntProperty.Property % 3 == 0), GenerateRandom);

		[Fact]
		public void RandomlyChangeOuterPropertyValues() => CollectionTestHelpers.RandomlyChangePropertyValues(l => l.ActiveWhere(o => o.IntProperty.ToActiveValue(x => x.Property % 3 == 0)), l => l.Where(o => o.IntProperty.Property % 3 == 0), GenerateRandom, o => o.IntProperty = new IntegerTestClass() { Property = RandomGenerator.GenerateRandomInteger() });

		[Fact]
		public void RandomlyChangeInnerPropertyValues() => CollectionTestHelpers.RandomlyChangePropertyValues(l => l.ActiveWhere(o => o.IntProperty.ToActiveValue(x => x.Property % 3 == 0)), l => l.Where(o => o.IntProperty.Property % 3 == 0), GenerateRandom, o => o.IntProperty.Property = RandomGenerator.GenerateRandomInteger());

		private ActiveWhereValueTestClass GenerateRandom()
		{
			return new ActiveWhereValueTestClass()
			{
				IntProperty = new IntegerTestClass()
				{
					Property = RandomGenerator.GenerateRandomInteger()
				}
			};
		}

		public class ActiveWhereValueTestClass : INotifyPropertyChanged
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
