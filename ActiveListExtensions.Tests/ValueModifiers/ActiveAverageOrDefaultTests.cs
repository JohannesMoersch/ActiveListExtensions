using ActiveListExtensions.Tests.Helpers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xunit;

namespace ActiveListExtensions.Tests.ValueModifiers
{
	public class ActiveAverageOrDefaultTests
	{
		[Fact]
		public void RandomlyChangeParameter() => ValueTestHelpers.RandomlyChangeParameter((l, p) => l.ActiveAverageOrDefault(p, (o, i) => o.Property * i.Property), (l, p) => { try { return l.Average(i => i.Property * p.Property); } catch { return 0.0; } }, () => new IntegerTestClass() { Property = RandomGenerator.GenerateRandomInteger(0, 100) }, () => RandomGenerator.GenerateRandomInteger(-10, 10));

		[Fact]
		public void RandomlyInsertItems() => ValueTestHelpers.RandomlyInsertItems(l => l.ActiveAverageOrDefault(i => i.Property), l => { try { return l.Average(i => i.Property); } catch { return 0.0; } }, () => new IntegerTestClass() { Property = RandomGenerator.GenerateRandomInteger() });

		[Fact]
		public void RandomlyRemoveItems() => ValueTestHelpers.RandomlyRemoveItems(l => l.ActiveAverageOrDefault(i => i.Property), l => { try { return l.Average(i => i.Property); } catch { return 0.0; } }, () => new IntegerTestClass() { Property = RandomGenerator.GenerateRandomInteger() });

		[Fact]
		public void RandomlyReplaceItems() => ValueTestHelpers.RandomlyReplaceItems(l => l.ActiveAverageOrDefault(i => i.Property), l => { try { return l.Average(i => i.Property); } catch { return 0.0; } }, () => new IntegerTestClass() { Property = RandomGenerator.GenerateRandomInteger() });

		[Fact]
		public void RandomlyMoveItems() => ValueTestHelpers.RandomlyMoveItems(l => l.ActiveAverageOrDefault(i => i.Property), l => { try { return l.Average(i => i.Property); } catch { return 0.0; } }, () => new IntegerTestClass() { Property = RandomGenerator.GenerateRandomInteger() });

		[Fact]
		public void RandomMixedOperations() => ValueTestHelpers.RandomMixedOperations(l => l.ActiveAverageOrDefault(i => i.Property), l => { try { return l.Average(i => i.Property); } catch { return 0.0; } }, () => new IntegerTestClass() { Property = RandomGenerator.GenerateRandomInteger() });

		[Fact]
		public void ResetWithRandomItems() => ValueTestHelpers.ResetWithRandomItems(l => l.ActiveAverageOrDefault(i => i.Property), l => { try { return l.Average(i => i.Property); } catch { return 0.0; } }, () => new IntegerTestClass() { Property = RandomGenerator.GenerateRandomInteger() });

		[Fact]
		public void RandomlyChangePropertyValues() => ValueTestHelpers.RandomlyChangePropertyValues(l => l.ActiveAverageOrDefault(i => i.Property), l => { try { return l.Average(i => i.Property); } catch { return 0.0; } }, () => new IntegerTestClass() { Property = RandomGenerator.GenerateRandomInteger() }, o => o.Property = RandomGenerator.GenerateRandomInteger());
	}
}
