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
		public void RandomlyChangeParameter() => ValueTestHelpers.RandomlyChangeParameter((l, p) => l.ActiveSingleOrDefault(p, (o, i) => o.Property > i.Property), (l, p) => { try { return l.SingleOrDefault(i => i.Property > p.Property); } catch { return null; } }, () => new IntegerTestClass() { Property = RandomGenerator.GenerateRandomInteger(0, 100) }, () => RandomGenerator.GenerateRandomInteger(-50, 150));

		[Fact]
		public void RandomlyInsertItems() => ValueTestHelpers.RandomlyInsertItems(l => l.ActiveSingleOrDefault(i => i.Property % 20 == 0), l => { try { return l.SingleOrDefault(i => i.Property % 20 == 0); } catch { return null; } }, () => new ActiveSingleOrDefaultTestClass() { Property = RandomGenerator.GenerateRandomInteger() });

		[Fact]
		public void RandomlyRemoveItems() => ValueTestHelpers.RandomlyRemoveItems(l => l.ActiveSingleOrDefault(i => i.Property % 20 == 0), l => { try { return l.SingleOrDefault(i => i.Property % 20 == 0); } catch { return null; } }, () => new ActiveSingleOrDefaultTestClass() { Property = RandomGenerator.GenerateRandomInteger() });

		[Fact]
		public void RandomlyReplaceItems() => ValueTestHelpers.RandomlyReplaceItems(l => l.ActiveSingleOrDefault(i => i.Property % 20 == 0), l => { try { return l.SingleOrDefault(i => i.Property % 20 == 0); } catch { return null; } }, () => new ActiveSingleOrDefaultTestClass() { Property = RandomGenerator.GenerateRandomInteger() });

		[Fact]
		public void RandomlyMoveItemsWithNoMatches() => ValueTestHelpers.RandomlyMoveItems(l => l.ActiveSingleOrDefault(i => i.Property == -1), l => { try { return l.SingleOrDefault(i => i.Property == -1); } catch { return null; } }, () => new ActiveSingleOrDefaultTestClass() { Property = RandomGenerator.GenerateRandomInteger() });

		[Fact]
		public void RandomlyMoveItemsWithOneMatch() => ValueTestHelpers.RandomlyMoveItems(l => l.ActiveSingleOrDefault(i => i.Property == 550), l => { try { return l.SingleOrDefault(i => i.Property == 550); } catch { return null; } }, () => new ActiveSingleOrDefaultTestClass() { Property = RandomGenerator.GenerateRandomInteger() });

		[Fact]
		public void RandomlyMoveItemsWithManyMatches() => ValueTestHelpers.RandomlyMoveItems(l => l.ActiveSingleOrDefault(i => i.Property % 20 == 0), l => { try { return l.SingleOrDefault(i => i.Property % 20 == 0); } catch { return null; } }, () => new ActiveSingleOrDefaultTestClass() { Property = RandomGenerator.GenerateRandomInteger() });

		[Fact]
		public void RandomMixedOperations() => ValueTestHelpers.RandomMixedOperations(l => l.ActiveSingleOrDefault(i => i.Property % 20 == 0), l => { try { return l.SingleOrDefault(i => i.Property % 20 == 0); } catch { return null; } }, () => new ActiveSingleOrDefaultTestClass() { Property = RandomGenerator.GenerateRandomInteger() });

		[Fact]
		public void ResetWithRandomItems() => ValueTestHelpers.ResetWithRandomItems(l => l.ActiveSingleOrDefault(i => i.Property % 20 == 0), l => { try { return l.SingleOrDefault(i => i.Property % 20 == 0); } catch { return null; } }, () => new ActiveSingleOrDefaultTestClass() { Property = RandomGenerator.GenerateRandomInteger() });

		[Fact]
		public void RandomlyChangePropertyValues() => ValueTestHelpers.RandomlyChangePropertyValues(l => l.ActiveSingleOrDefault(i => i.Property % 20 == 0), l => { try { return l.SingleOrDefault(i => i.Property % 20 == 0); } catch { return null; } }, () => new ActiveSingleOrDefaultTestClass() { Property = RandomGenerator.GenerateRandomInteger() }, o => o.Property = RandomGenerator.GenerateRandomInteger());
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

		public override string ToString() => Property.ToString();
	}
}