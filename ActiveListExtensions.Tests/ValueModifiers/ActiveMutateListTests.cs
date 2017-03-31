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
	public class ActiveMutateListTests
	{
		[Fact]
		public void RandomlyInsertItems() => ValueTestHelpers.RandomlyInsertItems(l => l.ActiveMutate(i => i.Select(o => o.Property)), l => l.Select(o => o.Property), () => new ActiveMutateListTestClass() { Property = RandomGenerator.GenerateRandomInteger() }, (l1, l2) => l1.SequenceEqual(l2));

		[Fact]
		public void RandomlyRemoveItems() => ValueTestHelpers.RandomlyRemoveItems(l => l.ActiveMutate(i => i.Select(o => o.Property)), l => l.Select(o => o.Property), () => new ActiveMutateListTestClass() { Property = RandomGenerator.GenerateRandomInteger() }, (l1, l2) => l1.SequenceEqual(l2));

		[Fact]
		public void RandomlyReplaceItems() => ValueTestHelpers.RandomlyReplaceItems(l => l.ActiveMutate(i => i.Select(o => o.Property)), l => l.Select(o => o.Property), () => new ActiveMutateListTestClass() { Property = RandomGenerator.GenerateRandomInteger() }, (l1, l2) => l1.SequenceEqual(l2));

		[Fact]
		public void RandomlyMoveItems() => ValueTestHelpers.RandomlyMoveItems(l => l.ActiveMutate(i => i.Select(o => o.Property)), l => l.Select(o => o.Property), () => new ActiveMutateListTestClass() { Property = RandomGenerator.GenerateRandomInteger() }, (l1, l2) => l1.SequenceEqual(l2));

		[Fact]
		public void ResetWithRandomItems() => ValueTestHelpers.ResetWithRandomItems(l => l.ActiveMutate(i => i.Select(o => o.Property)), l => l.Select(o => o.Property), () => new ActiveMutateListTestClass() { Property = RandomGenerator.GenerateRandomInteger() }, (l1, l2) => l1.SequenceEqual(l2));
	}

	public class ActiveMutateListTestClass : INotifyPropertyChanged
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
