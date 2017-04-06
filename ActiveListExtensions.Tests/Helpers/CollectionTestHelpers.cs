using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ActiveListExtensions.Utilities;
using System.ComponentModel;

namespace ActiveListExtensions.Tests.Helpers
{
	public static class CollectionTestHelpers
	{
		public static void RandomlyChangeParameter<T, U>(Func<IActiveList<T>, IActiveValue<IntegerTestClass>, IActiveList<U>> activeExpression, Func<IReadOnlyList<T>, IntegerTestClass, IEnumerable<U>> linqExpression, Func<T> randomValueGenerator, bool useSetComparison = false, Func<U, object> keySelector = null, Func<U, U, bool> additonalComparer = null)
		{
			RandomGenerator.ResetRandomGenerator();

			var list = new ObservableList<T>();
			foreach (var value in Enumerable.Range(0, 100))
				list.Add(list.Count, randomValueGenerator.Invoke());

			var parameter = new ActiveValue<IntegerTestClass>() { Value = new IntegerTestClass() { Property = 1 } };
			var sut = activeExpression.Invoke(list.ToActiveList(), parameter);
			var watcher = new CollectionSynchronizationWatcher<U>(sut);
			var validator = new LinqValidator<T, T, U>(list, sut, l => linqExpression.Invoke(l, parameter.Value), useSetComparison, keySelector, additonalComparer);

			foreach (var value in Enumerable.Range(0, 30))
			{
				parameter.Value.Property = RandomGenerator.GenerateRandomInteger(1, 11);
				validator.Validate();
			}
		}

		public static void RandomlyInsertItems<T, U>(Func<IActiveList<T>, IActiveList<U>> activeExpression, Func<IReadOnlyList<T>, IEnumerable<U>> linqExpression, Func<T> randomValueGenerator, bool useSetComparison = false, Func<U, object> keySelector = null, Func<U, U, bool> additonalComparer = null)
		{
			RandomGenerator.ResetRandomGenerator();

			var list = new ObservableList<T>();
			var sut = activeExpression.Invoke(list.ToActiveList());
			var watcher = new CollectionSynchronizationWatcher<U>(sut);
			var validator = new LinqValidator<T, T, U>(list, sut, linqExpression, useSetComparison, keySelector, additonalComparer);

			foreach (var value in Enumerable.Range(0, 100))
			{
				list.Add(RandomGenerator.GenerateRandomInteger(0, list.Count), randomValueGenerator.Invoke());
				validator.Validate();
			}
		}

		public static void RandomlyRemoveItems<T, U>(Func<IActiveList<T>, IActiveList<U>> activeExpression, Func<IReadOnlyList<T>, IEnumerable<U>> linqExpression, Func<T> randomValueGenerator, bool useSetComparison = false, Func<U, object> keySelector = null, Func<U, U, bool> additonalComparer = null)
		{
			RandomGenerator.ResetRandomGenerator();

			var list = new ObservableList<T>();
			foreach (var value in Enumerable.Range(0, 100))
				list.Add(list.Count, randomValueGenerator.Invoke());
			var sut = activeExpression.Invoke(list.ToActiveList());
			var watcher = new CollectionSynchronizationWatcher<U>(sut);
			var validator = new LinqValidator<T, T, U>(list, sut, linqExpression, useSetComparison, keySelector, additonalComparer);

			foreach (var value in Enumerable.Range(0, 100))
			{
				list.Remove(RandomGenerator.GenerateRandomInteger(0, list.Count));
				validator.Validate();
			}
		}

		public static void RandomlyReplaceItems<T, U>(Func<IActiveList<T>, IActiveList<U>> activeExpression, Func<IReadOnlyList<T>, IEnumerable<U>> linqExpression, Func<T> randomValueGenerator, bool useSetComparison = false, Func<U, object> keySelector = null, Func<U, U, bool> additonalComparer = null)
		{
			RandomGenerator.ResetRandomGenerator();

			var list = new ObservableList<T>();
			foreach (var value in Enumerable.Range(0, 100))
				list.Add(list.Count, randomValueGenerator.Invoke());
			var sut = activeExpression.Invoke(list.ToActiveList());
			var watcher = new CollectionSynchronizationWatcher<U>(sut);
			var validator = new LinqValidator<T, T, U>(list, sut, linqExpression, useSetComparison, keySelector, additonalComparer);

			foreach (var value in Enumerable.Range(0, 100))
			{
				list.Replace(RandomGenerator.GenerateRandomInteger(0, list.Count), randomValueGenerator.Invoke());
				validator.Validate();
			}
		}

		public static void RandomlyMoveItems<T, U>(Func<IActiveList<T>, IActiveList<U>> activeExpression, Func<IReadOnlyList<T>, IEnumerable<U>> linqExpression, Func<T> randomValueGenerator, bool useSetComparison = false, Func<U, object> keySelector = null, Func<U, U, bool> additonalComparer = null)
		{
			RandomGenerator.ResetRandomGenerator();

			var list = new ObservableList<T>();
			foreach (var value in Enumerable.Range(0, 100))
				list.Add(list.Count, randomValueGenerator.Invoke());
			var sut = activeExpression.Invoke(list.ToActiveList());
			var watcher = new CollectionSynchronizationWatcher<U>(sut);
			var validator = new LinqValidator<T, T, U>(list, sut, linqExpression, useSetComparison, keySelector, additonalComparer);

			foreach (var value in Enumerable.Range(0, 100))
			{
				list.Move(RandomGenerator.GenerateRandomInteger(0, list.Count), RandomGenerator.GenerateRandomInteger(0, list.Count));
				validator.Validate();
			}
		}

		public static void ResetWithRandomItems<T, U>(Func<IActiveList<T>, IActiveList<U>> activeExpression, Func<IReadOnlyList<T>, IEnumerable<U>> linqExpression, Func<T> randomValueGenerator, bool useSetComparison = false, Func<U, object> keySelector = null, Func<U, U, bool> additonalComparer = null)
		{
			RandomGenerator.ResetRandomGenerator();

			var list = new ObservableList<T>();
			foreach (var value in Enumerable.Range(0, 100))
				list.Add(list.Count, randomValueGenerator.Invoke());
			var sut = activeExpression.Invoke(list.ToActiveList());
			var watcher = new CollectionSynchronizationWatcher<U>(sut);
			var validator = new LinqValidator<T, T, U>(list, sut, linqExpression, useSetComparison, keySelector, additonalComparer);

			foreach (var value in Enumerable.Range(0, 20))
			{
				list.Reset(Enumerable.Range(0, RandomGenerator.GenerateRandomInteger(0, 30)).Select(i => randomValueGenerator.Invoke()));
				validator.Validate();
			}
		}

		public static void RandomlyChangePropertyValues<T, U>(Func<IActiveList<T>, IActiveList<U>> activeExpression, Func<IReadOnlyList<T>, IEnumerable<U>> linqExpression, Func<T> randomValueGenerator, Action<T> randomPropertySetter, bool useSetComparison = false, Func<U, object> keySelector = null, Func<U, U, bool> additonalComparer = null)
		{
			RandomGenerator.ResetRandomGenerator();

			var list = new ObservableList<T>();
			foreach (var value in Enumerable.Range(0, 100))
				list.Add(list.Count, randomValueGenerator.Invoke());
			var sut = activeExpression.Invoke(list.ToActiveList());
			var watcher = new CollectionSynchronizationWatcher<U>(sut);
			var validator = new LinqValidator<T, T, U>(list, sut, linqExpression, useSetComparison, keySelector, additonalComparer);

			foreach (var value in Enumerable.Range(0, 100))
			{
				randomPropertySetter.Invoke(list[RandomGenerator.GenerateRandomInteger(0, list.Count)]);
				validator.Validate();
			}
		}

		public static void RandomlyChangeParameterInTwoCollections<T1, T2, U>(Func<IActiveList<T1>, IActiveList<T2>, IActiveValue<IntegerTestClass>, IActiveList<U>> activeExpression, Func<IReadOnlyList<T1>, IReadOnlyList<T2>, IntegerTestClass, IEnumerable<U>> linqExpression, Func<T1> randomValueGenerator1, Func<T2> randomValueGenerator2, bool useSetComparison = false, Func<U, object> keySelector = null)
		{
			RandomGenerator.ResetRandomGenerator();

			var list1 = new ObservableList<T1>();
			var list2 = new ObservableList<T2>();
			foreach (var value in Enumerable.Range(0, 100))
			{
				list1.Add(list1.Count, randomValueGenerator1.Invoke());
				list2.Add(list2.Count, randomValueGenerator2.Invoke());
			}

			var parameter = new ActiveValue<IntegerTestClass>() { Value = new IntegerTestClass() { Property = 1 } };
			var sut = activeExpression.Invoke(list1.ToActiveList(), list2.ToActiveList(), parameter);
			var watcher = new CollectionSynchronizationWatcher<U>(sut);
			var validator = new LinqValidator<T1, T2, U>(list1, list2, sut, (l1, l2) => linqExpression.Invoke(l1, l2, parameter.Value), useSetComparison, keySelector);

			foreach (var value in Enumerable.Range(0, 30))
			{
				parameter.Value.Property = RandomGenerator.GenerateRandomInteger(1, 11);
				validator.Validate();
			}
		}

		public static void RandomlyInsertItemsIntoTwoCollections<T1, T2, U>(Func<IActiveList<T1>, IActiveList<T2>, IActiveList<U>> activeExpression, Func<IReadOnlyList<T1>, IReadOnlyList<T2>, IEnumerable<U>> linqExpression, Func<T1> randomValueGenerator1, Func<T2> randomValueGenerator2, bool useSetComparison = false, Func<U, object> keySelector = null)
		{
			RandomGenerator.ResetRandomGenerator();

			var list1 = new ObservableList<T1>();
			var list2 = new ObservableList<T2>();
			var sut = activeExpression.Invoke(list1.ToActiveList(), list2.ToActiveList());
			var watcher = new CollectionSynchronizationWatcher<U>(sut);
			var validator = new LinqValidator<T1, T2, U>(list1, list2, sut, linqExpression, useSetComparison, keySelector);

			foreach (var value in Enumerable.Range(0, 100))
			{
				if (RandomGenerator.GenerateRandomInteger(0, 2) == 0)
					list1.Add(RandomGenerator.GenerateRandomInteger(0, list1.Count), randomValueGenerator1.Invoke());
				else
					list2.Add(RandomGenerator.GenerateRandomInteger(0, list2.Count), randomValueGenerator2.Invoke());
				validator.Validate();
			}
		}

		public static void RandomlyRemoveItemsFromTwoCollections<T1, T2, U>(Func<IActiveList<T1>, IActiveList<T2>, IActiveList<U>> activeExpression, Func<IReadOnlyList<T1>, IReadOnlyList<T2>, IEnumerable<U>> linqExpression, Func<T1> randomValueGenerator1, Func<T2> randomValueGenerator2, bool useSetComparison = false, Func<U, object> keySelector = null)
		{
			RandomGenerator.ResetRandomGenerator();

			var list1 = new ObservableList<T1>();
			var list2 = new ObservableList<T2>();
			foreach (var value in Enumerable.Range(0, 100))
			{
				list1.Add(list1.Count, randomValueGenerator1.Invoke());
				list2.Add(list2.Count, randomValueGenerator2.Invoke());
			}
			var sut = activeExpression.Invoke(list1.ToActiveList(), list2.ToActiveList());
			var watcher = new CollectionSynchronizationWatcher<U>(sut);
			var validator = new LinqValidator<T1, T2, U>(list1, list2, sut, linqExpression, useSetComparison, keySelector);

			foreach (var value in Enumerable.Range(0, 100))
			{
				if (RandomGenerator.GenerateRandomInteger(0, 2) == 0 && list1.Count > 0)
					list1.Remove(RandomGenerator.GenerateRandomInteger(0, list1.Count));
				else
					list2.Remove(RandomGenerator.GenerateRandomInteger(0, list2.Count));
				validator.Validate();
			}
		}

		public static void RandomlyReplaceItemsInTwoCollections<T1, T2, U>(Func<IActiveList<T1>, IActiveList<T2>, IActiveList<U>> activeExpression, Func<IReadOnlyList<T1>, IReadOnlyList<T2>, IEnumerable<U>> linqExpression, Func<T1> randomValueGenerator1, Func<T2> randomValueGenerator2, bool useSetComparison = false, Func<U, object> keySelector = null)
		{
			RandomGenerator.ResetRandomGenerator();

			var list1 = new ObservableList<T1>();
			var list2 = new ObservableList<T2>();
			foreach (var value in Enumerable.Range(0, 100))
			{
				list1.Add(list1.Count, randomValueGenerator1.Invoke());
				list2.Add(list2.Count, randomValueGenerator2.Invoke());
			}
			var sut = activeExpression.Invoke(list1.ToActiveList(), list2.ToActiveList());
			var watcher = new CollectionSynchronizationWatcher<U>(sut);
			var validator = new LinqValidator<T1, T2, U>(list1, list2, sut, linqExpression, useSetComparison, keySelector);

			foreach (var value in Enumerable.Range(0, 100))
			{
				if (RandomGenerator.GenerateRandomInteger(0, 2) == 0)
					list1.Replace(RandomGenerator.GenerateRandomInteger(0, list1.Count), randomValueGenerator1.Invoke());
				else
					list2.Replace(RandomGenerator.GenerateRandomInteger(0, list2.Count), randomValueGenerator2.Invoke());
				validator.Validate();
			}
		}

		public static void RandomlyMoveItemsWithinTwoCollections<T1, T2, U>(Func<IActiveList<T1>, IActiveList<T2>, IActiveList<U>> activeExpression, Func<IReadOnlyList<T1>, IReadOnlyList<T2>, IEnumerable<U>> linqExpression, Func<T1> randomValueGenerator1, Func<T2> randomValueGenerator2, bool useSetComparison = false, Func<U, object> keySelector = null)
		{
			RandomGenerator.ResetRandomGenerator();

			var list1 = new ObservableList<T1>();
			var list2 = new ObservableList<T2>();
			foreach (var value in Enumerable.Range(0, 100))
			{
				list1.Add(list1.Count, randomValueGenerator1.Invoke());
				list2.Add(list2.Count, randomValueGenerator2.Invoke());
			}
			var sut = activeExpression.Invoke(list1.ToActiveList(), list2.ToActiveList());
			var watcher = new CollectionSynchronizationWatcher<U>(sut);
			var validator = new LinqValidator<T1, T2, U>(list1, list2, sut, linqExpression, useSetComparison, keySelector);

			foreach (var value in Enumerable.Range(0, 100))
			{
				if (RandomGenerator.GenerateRandomInteger(0, 2) == 0)
					list1.Move(RandomGenerator.GenerateRandomInteger(0, list1.Count), RandomGenerator.GenerateRandomInteger(0, list1.Count));
				else
					list2.Move(RandomGenerator.GenerateRandomInteger(0, list2.Count), RandomGenerator.GenerateRandomInteger(0, list2.Count));
				validator.Validate();
			}
		}

		public static void ResetTwoCollectionsWithRandomItems<T1, T2, U>(Func<IActiveList<T1>, IActiveList<T2>, IActiveList<U>> activeExpression, Func<IReadOnlyList<T1>, IReadOnlyList<T2>, IEnumerable<U>> linqExpression, Func<T1> randomValueGenerator1, Func<T2> randomValueGenerator2, bool useSetComparison = false, Func<U, object> keySelector = null)
		{
			RandomGenerator.ResetRandomGenerator();

			var list1 = new ObservableList<T1>();
			var list2 = new ObservableList<T2>();
			foreach (var value in Enumerable.Range(0, 100))
			{
				list1.Add(list1.Count, randomValueGenerator1.Invoke());
				list2.Add(list2.Count, randomValueGenerator2.Invoke());
			}
			var sut = activeExpression.Invoke(list1.ToActiveList(), list2.ToActiveList());
			var watcher = new CollectionSynchronizationWatcher<U>(sut);
			var validator = new LinqValidator<T1, T2, U>(list1, list2, sut, linqExpression, useSetComparison, keySelector);

			foreach (var value in Enumerable.Range(0, 20))
			{
				if (RandomGenerator.GenerateRandomInteger(0, 2) == 0)
					list1.Reset(Enumerable.Range(0, RandomGenerator.GenerateRandomInteger(0, 30)).Select(i => randomValueGenerator1.Invoke()));
				else
					list2.Reset(Enumerable.Range(0, RandomGenerator.GenerateRandomInteger(0, 30)).Select(i => randomValueGenerator2.Invoke()));
				validator.Validate();
			}
		}

		public static void RandomlyChangePropertyValuesInTwoCollections<T1, T2, U>(Func<IActiveList<T1>, IActiveList<T2>, IActiveList<U>> activeExpression, Func<IReadOnlyList<T1>, IReadOnlyList<T2>, IEnumerable<U>> linqExpression, Func<T1> randomValueGenerator1, Func<T2> randomValueGenerator2, Action<T1> randomPropertySetter1, Action<T2> randomPropertySetter2, bool useSetComparison = false, Func<U, object> keySelector = null)
		{
			RandomGenerator.ResetRandomGenerator();

			var list1 = new ObservableList<T1>();
			var list2 = new ObservableList<T2>();
			foreach (var value in Enumerable.Range(0, 100))
			{
				list1.Add(list1.Count, randomValueGenerator1.Invoke());
				list2.Add(list2.Count, randomValueGenerator2.Invoke());
			}
			var sut = activeExpression.Invoke(list1.ToActiveList(), list2.ToActiveList());
			var watcher = new CollectionSynchronizationWatcher<U>(sut);
			var validator = new LinqValidator<T1, T2, U>(list1, list2, sut, linqExpression, useSetComparison, keySelector);

			foreach (var value in Enumerable.Range(0, 100))
			{
				if (RandomGenerator.GenerateRandomInteger(0, 2) == 0)
					randomPropertySetter1.Invoke(list1[RandomGenerator.GenerateRandomInteger(0, list1.Count)]);
				else
					randomPropertySetter2.Invoke(list2[RandomGenerator.GenerateRandomInteger(0, list2.Count)]);
				validator.Validate();
			}
		}
	}
}
