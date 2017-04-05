using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ActiveListExtensions.Tests.Helpers;
using ActiveListExtensions.Utilities;
using Xunit;

namespace ActiveListExtensions.Tests.Modifiers
{
	public class ActiveSelectManyTests
	{
		[Fact]
		public void RandomlyChangeParameter() => CollectionTestHelpers.RandomlyChangeParameter((l, p) => l.ActiveSelectMany((o, i) => o.Property.Select(x => x * i.Property), p), (l, p) => l.SelectMany(o => o.Property.Select(x => x * p.Property)), () => new ActiveSelectManyTestClass() { Property = RandomGenerator.GenerateRandomIntegerList() });

		[Fact]
		public void RandomlyInsertItems() => CollectionTestHelpers.RandomlyInsertItems(l => l.ActiveSelectMany(o => o.Property), l => l.SelectMany(o => o.Property), () => new ActiveSelectManyTestClass() { Property = RandomGenerator.GenerateRandomIntegerList() });

		[Fact]
		public void RandomlyRemoveItems() => CollectionTestHelpers.RandomlyRemoveItems(l => l.ActiveSelectMany(o => o.Property), l => l.SelectMany(o => o.Property), () => new ActiveSelectManyTestClass() { Property = RandomGenerator.GenerateRandomIntegerList() });

		[Fact]
		public void RandomlyReplaceItems() => CollectionTestHelpers.RandomlyReplaceItems(l => l.ActiveSelectMany(o => o.Property), l => l.SelectMany(o => o.Property), () => new ActiveSelectManyTestClass() { Property = RandomGenerator.GenerateRandomIntegerList() });

		[Fact]
		public void RandomlyMoveItems() => CollectionTestHelpers.RandomlyMoveItems(l => l.ActiveSelectMany(o => o.Property), l => l.SelectMany(o => o.Property), () => new ActiveSelectManyTestClass() { Property = RandomGenerator.GenerateRandomIntegerList() });

		[Fact]
		public void ResetWithRandomItems() => CollectionTestHelpers.ResetWithRandomItems(l => l.ActiveSelectMany(o => o.Property), l => l.SelectMany(o => o.Property), () => new ActiveSelectManyTestClass() { Property = RandomGenerator.GenerateRandomIntegerList() });

		[Fact]
		public void RandomlyReplaceEntirePropertyCollections() => CollectionTestHelpers.RandomlyChangePropertyValues(l => l.ActiveSelectMany(o => o.Property), l => l.SelectMany(o => o.Property), () => new ActiveSelectManyTestClass() { Property = RandomGenerator.GenerateRandomIntegerList() }, o => o.Property = RandomGenerator.GenerateRandomIntegerList());

		private void SetupForPropertyChangeTests(Action<ObservableList<int>> randomChangeFunction)
		{
			RandomGenerator.ResetRandomGenerator();

			var lists = Enumerable.Range(0, 10).Select(_ => new ObservableList<int>()).ToArray();

			var list = new ObservableList<ActiveSelectManyTestClass>();
			foreach (var value in lists)
			{
				var item = new ActiveSelectManyTestClass() { Property = value };
				foreach (var num in Enumerable.Range(0, 100).Select(i => RandomGenerator.GenerateRandomInteger()))
					value.Add(value.Count, num);
				list.Add(list.Count, item);
			}

			var sut = list.ToActiveList().ActiveSelectMany(o => o.Property);
			var watcher = new CollectionSynchronizationWatcher<int>(sut);
			var validator = new LinqValidator<ActiveSelectManyTestClass, ActiveSelectManyTestClass, int>(list, sut, l => l.SelectMany(o => o.Property), false, null);

			foreach (var value in Enumerable.Range(0, 100))
			{
				var listIndex = RandomGenerator.GenerateRandomInteger(0, lists.Length);
				randomChangeFunction.Invoke(lists[listIndex]);
				validator.Validate();
			}
		}

		[Fact]
		public void RandomlyInsertIntoPropertyCollections() => SetupForPropertyChangeTests(l => l.Add(RandomGenerator.GenerateRandomInteger(0, l.Count), RandomGenerator.GenerateRandomInteger()));

		[Fact]
		public void RandomlyRemoveFromPropertyCollections() => SetupForPropertyChangeTests(l => { if (l.Count > 0) { l.Remove(RandomGenerator.GenerateRandomInteger(0, l.Count)); } });

		[Fact]
		public void RandomlyMoveWithinPropertyCollections() => SetupForPropertyChangeTests(l => l.Move(RandomGenerator.GenerateRandomInteger(0, l.Count), RandomGenerator.GenerateRandomInteger(0, l.Count)));

		[Fact]
		public void RandomlyReplaceInPropertyCollections() => SetupForPropertyChangeTests(l => l.Replace(RandomGenerator.GenerateRandomInteger(0, l.Count), RandomGenerator.GenerateRandomInteger()));

		[Fact]
		public void ResetPropertyCollectionsWithRandomItems() => SetupForPropertyChangeTests(l => l.Reset(Enumerable.Range(0, RandomGenerator.GenerateRandomInteger(0, 30)).Select(i => RandomGenerator.GenerateRandomInteger())));
	}

	public class ActiveSelectManyTestClass : INotifyPropertyChanged
	{
		private IReadOnlyList<int> _property;
		public IReadOnlyList<int> Property
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
