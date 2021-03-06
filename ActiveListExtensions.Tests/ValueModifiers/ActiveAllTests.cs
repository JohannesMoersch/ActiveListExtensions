﻿using ActiveListExtensions.Tests.Helpers;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xunit;

namespace ActiveListExtensions.Tests.ValueModifiers
{
	public class ActiveAllTests
	{
		[Fact]
		public void RandomlyChangeParameter() => ValueTestHelpers.RandomlyChangeParameter((l, p) => l.ActiveAll(p, (o, i) => o.Property > i.Property), (l, p) => l.All(i => i.Property > p.Property), () => new IntegerTestClass() { Property = RandomGenerator.GenerateRandomInteger(0, 100) }, () => RandomGenerator.GenerateRandomInteger(-50, 150));

		[Fact]
		public void RandomlyInsertItems() => ValueTestHelpers.RandomlyInsertItems(l => l.ActiveAll(i => i.Property > 5), l => l.All(i => i.Property > 5), () => new IntegerTestClass() { Property = RandomGenerator.GenerateRandomInteger() });

		[Fact]
		public void RandomlyRemoveItems() => ValueTestHelpers.RandomlyRemoveItems(l => l.ActiveAll(i => i.Property > 5), l => l.All(i => i.Property > 5), () => new IntegerTestClass() { Property = RandomGenerator.GenerateRandomInteger() });

		[Fact]
		public void RandomlyReplaceItems() => ValueTestHelpers.RandomlyReplaceItems(l => l.ActiveAll(i => i.Property > 5), l => l.All(i => i.Property > 5), () => new IntegerTestClass() { Property = RandomGenerator.GenerateRandomInteger() });

		[Fact]
		public void RandomlyMoveItems() => ValueTestHelpers.RandomlyMoveItems(l => l.ActiveAll(i => i.Property > 5), l => l.All(i => i.Property > 5), () => new IntegerTestClass() { Property = RandomGenerator.GenerateRandomInteger() });

		[Fact]
		public void RandomMixedOperations() => ValueTestHelpers.RandomMixedOperations(l => l.ActiveAll(i => i.Property > 5), l => l.All(i => i.Property > 5), () => new IntegerTestClass() { Property = RandomGenerator.GenerateRandomInteger() });

		[Fact]
		public void ResetWithRandomItems() => ValueTestHelpers.ResetWithRandomItems(l => l.ActiveAll(i => i.Property > 5), l => l.All(i => i.Property > 5), () => new IntegerTestClass() { Property = RandomGenerator.GenerateRandomInteger() });

		[Fact]
		public void RandomlyChangePropertyValues() => ValueTestHelpers.RandomlyChangePropertyValues(l => l.ActiveAll(i => i.Property > 5), l => l.All(i => i.Property > 5), () => new IntegerTestClass() { Property = RandomGenerator.GenerateRandomInteger() }, o => o.Property = RandomGenerator.GenerateRandomInteger());

		[Fact]
		public void OnlyThrowChangeNotificationWhenValueChanges()
		{
			var list = ActiveList.Create<int>();

			int changeCount = 0;

			var sut = list.ActiveAll(i => i % 2 == 0);

			sut.ValueChanged += (s, e) => ++changeCount;

			list.Add(0, 2);
			list.Add(0, 4);
			list.Add(0, 1);

			list.Clear();

			Assert.Equal(2, changeCount);
		}
	}
}