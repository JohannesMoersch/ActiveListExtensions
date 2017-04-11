using ActiveListExtensions.Utilities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ActiveListExtensions.Tests.Helpers
{
	public static class ValueTestHelpers
	{
		public static void RandomlyChangeParameter<T, U>(Func<IActiveList<T>, IActiveValue<IntegerTestClass>, IActiveValue<U>> activeExpression, Func<IReadOnlyList<T>, IntegerTestClass, U> linqExpression, Func<T> randomValueGenerator, Func<int> parameterRandomValueGenerator, Func<U, U, bool> comparer = null)
		{
			RandomGenerator.ResetRandomGenerator();

			var list = new ObservableList<T>();
			foreach (var value in Enumerable.Range(0, 100))
				list.Add(list.Count, randomValueGenerator.Invoke());

			var parameter = new ActiveValue<IntegerTestClass>() { Value = new IntegerTestClass() { Property = 1 } };
			var sut = activeExpression.Invoke(list.ToActiveList(), parameter);

			foreach (var value in Enumerable.Range(0, 30))
			{
				parameter.Value.Property = parameterRandomValueGenerator.Invoke();
				ValidateResult(sut, linqExpression.Invoke(list, parameter.Value), comparer);
			}
		}

		public static void RandomlyInsertItems<T, U>(Func<IActiveList<T>, IActiveValue<U>> activeExpression, Func<IReadOnlyList<T>, U> linqExpression, Func<T> randomValueGenerator, Func<U, U, bool> comparer = null)
		{
			RandomGenerator.ResetRandomGenerator();

			var list = new ObservableList<T>();
			var sut = activeExpression.Invoke(list);

			foreach (var value in Enumerable.Range(0, 100))
			{
				list.Add(RandomGenerator.GenerateRandomInteger(0, list.Count), randomValueGenerator.Invoke());
				ValidateResult(sut, linqExpression.Invoke(list), comparer);
			}
		}

		public static void RandomlyRemoveItems<T, U>(Func<IActiveList<T>, IActiveValue<U>> activeExpression, Func<IReadOnlyList<T>, U> linqExpression, Func<T> randomValueGenerator, Func<U, U, bool> comparer = null)
		{
			RandomGenerator.ResetRandomGenerator();

			var list = new ObservableList<T>();
			foreach (var value in Enumerable.Range(0, 100))
				list.Add(list.Count, randomValueGenerator.Invoke());
			var sut = activeExpression.Invoke(list);

			foreach (var value in Enumerable.Range(0, 100))
			{
				list.Remove(RandomGenerator.GenerateRandomInteger(0, list.Count));
				ValidateResult(sut, linqExpression.Invoke(list), comparer);
			}
		}

		public static void RandomlyReplaceItems<T, U>(Func<IActiveList<T>, IActiveValue<U>> activeExpression, Func<IReadOnlyList<T>, U> linqExpression, Func<T> randomValueGenerator, Func<U, U, bool> comparer = null)
		{
			RandomGenerator.ResetRandomGenerator();

			var list = new ObservableList<T>();
			foreach (var value in Enumerable.Range(0, 100))
				list.Add(list.Count, randomValueGenerator.Invoke());
			var sut = activeExpression.Invoke(list);

			foreach (var value in Enumerable.Range(0, 100))
			{
				list.Replace(RandomGenerator.GenerateRandomInteger(0, list.Count), randomValueGenerator.Invoke());
				ValidateResult(sut, linqExpression.Invoke(list), comparer);
			}
		}

		public static void RandomlyMoveItems<T, U>(Func<IActiveList<T>, IActiveValue<U>> activeExpression, Func<IReadOnlyList<T>, U> linqExpression, Func<T> randomValueGenerator, Func<U, U, bool> comparer = null)
		{
			RandomGenerator.ResetRandomGenerator();

			var list = new ObservableList<T>();
			foreach (var value in Enumerable.Range(0, 100))
				list.Add(list.Count, randomValueGenerator.Invoke());
			var sut = activeExpression.Invoke(list);

			foreach (var value in Enumerable.Range(0, 100))
			{
				list.Move(RandomGenerator.GenerateRandomInteger(0, list.Count), RandomGenerator.GenerateRandomInteger(0, list.Count));
				ValidateResult(sut, linqExpression.Invoke(list), comparer);
			}
		}

		public static void RandomMixedOperations<T, U>(Func<IActiveList<T>, IActiveValue<U>> activeExpression, Func<IReadOnlyList<T>, U> linqExpression, Func<T> randomValueGenerator, Func<U, U, bool> comparer = null)
		{
			RandomGenerator.ResetRandomGenerator();

			var list = new ObservableList<T>();
			foreach (var value in Enumerable.Range(0, 100))
				list.Add(list.Count, randomValueGenerator.Invoke());
			var sut = activeExpression.Invoke(list);

			foreach (var value in Enumerable.Range(0, 1000))
			{
				switch (list.Count > 0 ? RandomGenerator.GenerateRandomInteger(0, 4) : 0)
				{
					case 0:
						list.Add(RandomGenerator.GenerateRandomInteger(0, list.Count), randomValueGenerator.Invoke());
						break;
					case 1:
						list.Remove(RandomGenerator.GenerateRandomInteger(0, list.Count));
						break;
					case 2:
						list.Replace(RandomGenerator.GenerateRandomInteger(0, list.Count), randomValueGenerator.Invoke());
						break;
					case 3:
						list.Move(RandomGenerator.GenerateRandomInteger(0, list.Count), RandomGenerator.GenerateRandomInteger(0, list.Count));
						break;
				}
				ValidateResult(sut, linqExpression.Invoke(list), comparer);
			}
		}

		public static void ResetWithRandomItems<T, U>(Func<IActiveList<T>, IActiveValue<U>> activeExpression, Func<IReadOnlyList<T>, U> linqExpression, Func<T> randomValueGenerator, Func<U, U, bool> comparer = null)
		{
			RandomGenerator.ResetRandomGenerator();

			var list = new ObservableList<T>();
			foreach (var value in Enumerable.Range(0, 100))
				list.Add(list.Count, randomValueGenerator.Invoke());
			var sut = activeExpression.Invoke(list);

			foreach (var value in Enumerable.Range(0, 20))
			{
				list.Reset(Enumerable.Range(0, RandomGenerator.GenerateRandomInteger(0, 30)).Select(i => randomValueGenerator.Invoke()));
				ValidateResult(sut, linqExpression.Invoke(list), comparer);
			}
		}

		public static void RandomlyChangePropertyValues<T, U>(Func<IActiveList<T>, IActiveValue<U>> activeExpression, Func<IReadOnlyList<T>, U> linqExpression, Func<T> randomValueGenerator, Action<T> randomPropertySetter, Func<U, U, bool> comparer = null)
		{
			RandomGenerator.ResetRandomGenerator();

			var list = new ObservableList<T>();
			foreach (var value in Enumerable.Range(0, 100))
				list.Add(list.Count, randomValueGenerator.Invoke());
			var sut = activeExpression.Invoke(list);

			foreach (var value in Enumerable.Range(0, 100))
			{
				randomPropertySetter.Invoke(list[RandomGenerator.GenerateRandomInteger(0, list.Count)]);
				ValidateResult(sut, linqExpression.Invoke(list), comparer);
			}
		}

		private static void ValidateResult<T>(IActiveValue<T> value, T otherValue, Func<T, T, bool> comparer)
		{
			if (!(comparer?.Invoke(value.Value, otherValue) ?? Equals(value.Value, otherValue)))
				throw new Exception("Linq equivalent differs from processed value.");
		}
	}
}
