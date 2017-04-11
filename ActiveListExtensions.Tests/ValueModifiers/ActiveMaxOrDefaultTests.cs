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
	public class ActiveMaxOrDefaultTests
	{
		[Fact]
		public void RandomlyChangeParameter() => ValueTestHelpers.RandomlyChangeParameter((l, p) => l.ActiveMaxOrDefault(p, (o, i) => o.Property * i.Property), (l, p) => { try { return l.Max(i => i.Property * p.Property); } catch { return 0; } }, () => new IntegerTestClass() { Property = RandomGenerator.GenerateRandomInteger(0, 100) }, () => RandomGenerator.GenerateRandomInteger(-50, 150));

		[Fact]
		public void RandomlyInsertItems() => ValueTestHelpers.RandomlyInsertItems(l => l.ActiveMaxOrDefault(i => i.Property), l => { try { return l.Max(i => i.Property); } catch { return 0; } }, () => new ActiveMaxOrDefaultTestClass() { Property = RandomGenerator.GenerateRandomInteger() });

		[Fact]
		public void RandomlyRemoveItems() => ValueTestHelpers.RandomlyRemoveItems(l => l.ActiveMaxOrDefault(i => i.Property), l => { try { return l.Max(i => i.Property); } catch { return 0; } }, () => new ActiveMaxOrDefaultTestClass() { Property = RandomGenerator.GenerateRandomInteger() });

		[Fact]
		public void RandomlyReplaceItems() => ValueTestHelpers.RandomlyReplaceItems(l => l.ActiveMaxOrDefault(i => i.Property), l => { try { return l.Max(i => i.Property); } catch { return 0; } }, () => new ActiveMaxOrDefaultTestClass() { Property = RandomGenerator.GenerateRandomInteger() });

		[Fact]
		public void RandomlyMoveItems() => ValueTestHelpers.RandomlyMoveItems(l => l.ActiveMaxOrDefault(i => i.Property), l => { try { return l.Max(i => i.Property); } catch { return 0; } }, () => new ActiveMaxOrDefaultTestClass() { Property = RandomGenerator.GenerateRandomInteger() });

		[Fact]
		public void RandomMixedOperations() => ValueTestHelpers.RandomMixedOperations(l => l.ActiveMaxOrDefault(i => i.Property), l => { try { return l.Max(i => i.Property); } catch { return 0; } }, () => new ActiveMaxOrDefaultTestClass() { Property = RandomGenerator.GenerateRandomInteger() });

		[Fact]
		public void ResetWithRandomItems() => ValueTestHelpers.ResetWithRandomItems(l => l.ActiveMaxOrDefault(i => i.Property), l => { try { return l.Max(i => i.Property); } catch { return 0; } }, () => new ActiveMaxOrDefaultTestClass() { Property = RandomGenerator.GenerateRandomInteger() });

		[Fact]
		public void RandomlyChangePropertyValues() => ValueTestHelpers.RandomlyChangePropertyValues(l => l.ActiveMaxOrDefault(i => i.Property), l => { try { return l.Max(i => i.Property); } catch { return 0; } }, () => new ActiveMaxOrDefaultTestClass() { Property = RandomGenerator.GenerateRandomInteger() }, o => o.Property = RandomGenerator.GenerateRandomInteger());
	}

	public class ActiveMaxOrDefaultTestClass : INotifyPropertyChanged
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