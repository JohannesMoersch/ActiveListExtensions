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
	public class ActiveMinOrDefaultTests
	{
		[Fact]
		public void RandomlyInsertItems() => ValueTestHelpers.RandomlyInsertItems(l => l.ActiveMinOrDefault(i => i.Property), l => { try { return l.Min(i => i.Property); } catch { return 0; } }, () => new ActiveMinOrDefaultTestClass() { Property = RandomGenerator.GenerateRandomInteger() });

		[Fact]
		public void RandomlyRemoveItems() => ValueTestHelpers.RandomlyRemoveItems(l => l.ActiveMinOrDefault(i => i.Property), l => { try { return l.Min(i => i.Property); } catch { return 0; } }, () => new ActiveMinOrDefaultTestClass() { Property = RandomGenerator.GenerateRandomInteger() });

		[Fact]
		public void RandomlyReplaceItems() => ValueTestHelpers.RandomlyReplaceItems(l => l.ActiveMinOrDefault(i => i.Property), l => { try { return l.Min(i => i.Property); } catch { return 0; } }, () => new ActiveMinOrDefaultTestClass() { Property = RandomGenerator.GenerateRandomInteger() });

		[Fact]
		public void RandomlyMoveItems() => ValueTestHelpers.RandomlyMoveItems(l => l.ActiveMinOrDefault(i => i.Property), l => { try { return l.Min(i => i.Property); } catch { return 0; } }, () => new ActiveMinOrDefaultTestClass() { Property = RandomGenerator.GenerateRandomInteger() });

		[Fact]
		public void RandomMixedOperations() => ValueTestHelpers.RandomMixedOperations(l => l.ActiveMinOrDefault(i => i.Property), l => { try { return l.Min(i => i.Property); } catch { return 0; } }, () => new ActiveMinOrDefaultTestClass() { Property = RandomGenerator.GenerateRandomInteger() });

		[Fact]
		public void ResetWithRandomItems() => ValueTestHelpers.ResetWithRandomItems(l => l.ActiveMinOrDefault(i => i.Property), l => { try { return l.Min(i => i.Property); } catch { return 0; } }, () => new ActiveMinOrDefaultTestClass() { Property = RandomGenerator.GenerateRandomInteger() });

		[Fact]
		public void RandomlyChangePropertyValues() => ValueTestHelpers.RandomlyChangePropertyValues(l => l.ActiveMinOrDefault(i => i.Property), l => { try { return l.Min(i => i.Property); } catch { return 0; } }, () => new ActiveMinOrDefaultTestClass() { Property = RandomGenerator.GenerateRandomInteger() }, o => o.Property = RandomGenerator.GenerateRandomInteger());
	}

	public class ActiveMinOrDefaultTestClass : INotifyPropertyChanged
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
