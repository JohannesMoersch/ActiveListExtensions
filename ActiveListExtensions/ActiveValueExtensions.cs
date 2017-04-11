using ActiveListExtensions.Utilities;
using ActiveListExtensions.ValueModifiers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace ActiveListExtensions
{
	public static class ActiveValueExtensions
	{
		public static IActiveValue<TValue> ToActiveValue<TSource, TValue>(this TSource source, Expression<Func<TSource, TValue>> valueGetter) => ToActiveValue<TSource, TValue>(source, valueGetter.Compile(), valueGetter.GetReferencedProperties());

		public static IActiveValue<TValue> ToActiveValue<TSource, TValue>(this TSource source, Func<TSource, TValue> valueGetter, IEnumerable<string> propertiesToWatch) => new ActiveValueListener<TSource, TValue>(source, valueGetter, propertiesToWatch);


		public static IActiveValue<TResult> ActiveMutate<TValue, TResult>(this IActiveValue<TValue> source, Expression<Func<TValue, TResult>> mutator) => ActiveMutate(source, mutator.Compile(), mutator.GetReferencedProperties());

		public static IActiveValue<TResult> ActiveMutate<TValue, TResult>(this IActiveValue<TValue> source, Func<TValue, TResult> mutator, IEnumerable<string> propertiesToWatch) => new ActiveMutateValue<TValue, TResult>(source, mutator, propertiesToWatch);

		public static IActiveValue<TResult> ActiveMutate<TSource, TResult>(this IActiveList<TSource> source, Func<IReadOnlyList<TSource>, TResult> mutator) => new ActiveMutateList<TSource, TResult>(source, mutator);


		public static IActiveValue<bool> ActiveSequenceEqual<TSource>(this IActiveList<TSource> source, IReadOnlyList<TSource> otherSource)
		{
			var comparer = EqualityComparer<TSource>.Default;
			return new ActiveSequenceEqual<TSource, object>(source, otherSource, (o1, o2) => comparer.Equals(o1, o2), null);
		}

		public static IActiveValue<bool> ActiveSequenceEqual<TSource>(this IActiveList<TSource> source, IReadOnlyList<TSource> otherSource, Expression<Func<TSource, TSource, bool>> comparer)
		{
			var properties = comparer.GetReferencedProperties();
			return ActiveSequenceEqual(source, otherSource, comparer.Compile(), properties.Item1.Concat(properties.Item2).Distinct());
		}

		public static IActiveValue<bool> ActiveSequenceEqual<TSource>(this IActiveList<TSource> source, IReadOnlyList<TSource> otherSource, Func<TSource, TSource, bool> comparer, IEnumerable<string> propertiesToWatch) => new ActiveSequenceEqual<TSource, object>(source, otherSource, comparer, propertiesToWatch);

		public static IActiveValue<bool> ActiveSequenceEqual<TSource, TParameter>(this IActiveList<TSource> source, IReadOnlyList<TSource> otherSource, IActiveValue<TParameter> parameter, Expression<Func<TSource, TSource, TParameter, bool>> comparer) => ActiveSequenceEqual(source, otherSource, null, comparer.Compile(), comparer.GetReferencedProperties());

		public static IActiveValue<bool> ActiveSequenceEqual<TSource, TParameter>(this IActiveList<TSource> source, IReadOnlyList<TSource> otherSource, IActiveValue<TParameter> parameter, Func<TSource, TSource, TParameter, bool> comparer, IEnumerable<string> sourcePropertiesToWatch, IEnumerable<string> parameterPropertiesToWatch) => ActiveSequenceEqual(source, otherSource, parameter, comparer, Tuple.Create(sourcePropertiesToWatch, Enumerable.Empty<string>(), parameterPropertiesToWatch));

		private static IActiveValue<bool> ActiveSequenceEqual<TSource, TParameter>(this IActiveList<TSource> source, IReadOnlyList<TSource> otherSource, IActiveValue<TParameter> parameter, Func<TSource, TSource, TParameter, bool> comparer, Tuple<IEnumerable<string>, IEnumerable<string>, IEnumerable<string>> propertiesToWatch) => new ActiveSequenceEqual<TSource, TParameter>(source, otherSource, parameter, comparer, propertiesToWatch.Item1.Concat(propertiesToWatch.Item2).Distinct(), propertiesToWatch.Item3);


		public static IActiveValue<TResult> ActiveCombine<TValue1, TValue2, TResult>(this IActiveValue<TValue1> value1, TValue2 value2, Expression<Func<TValue1, TValue2, TResult>> resultCombiner) => ActiveCombine(value1, new ActiveValueWrapper<TValue2>(value2), resultCombiner);

		public static IActiveValue<TResult> ActiveCombine<TValue1, TValue2, TResult>(this IActiveValue<TValue1> value1, TValue2 value2, Func<TValue1, TValue2, TResult> resultCombiner, IEnumerable<string> value1PropertiesToWatch, IEnumerable<string> value2PropertiesToWatch) => ActiveCombine(value1, new ActiveValueWrapper<TValue2>(value2), resultCombiner, value1PropertiesToWatch, value2PropertiesToWatch);

		public static IActiveValue<TResult> ActiveCombine<TValue1, TValue2, TResult>(this IActiveValue<TValue1> value1, IActiveValue<TValue2> value2, Expression<Func<TValue1, TValue2, TResult>> resultCombiner) => ActiveCombine(value1, value2, resultCombiner.Compile(), resultCombiner.GetReferencedProperties());

		public static IActiveValue<TResult> ActiveCombine<TValue1, TValue2, TResult>(this IActiveValue<TValue1> value1, IActiveValue<TValue2> value2, Func<TValue1, TValue2, TResult> resultCombiner, IEnumerable<string> value1PropertiesToWatch, IEnumerable<string> value2PropertiesToWatch) => ActiveCombine(value1, value2, resultCombiner, Tuple.Create(value1PropertiesToWatch, value2PropertiesToWatch));

		private static IActiveValue<TResult> ActiveCombine<TValue1, TValue2, TResult>(this IActiveValue<TValue1> value1, IActiveValue<TValue2> value2, Func<TValue1, TValue2, TResult> resultCombiner, Tuple<IEnumerable<string>, IEnumerable<string>> propertiesToWatch) => new ActiveCombine<TValue1, TValue2, TResult>(value1, value2, resultCombiner, propertiesToWatch.Item1, propertiesToWatch.Item2);


		public static IActiveValue<TSource> ActiveFirstOrDefault<TSource>(this IActiveList<TSource> source) => ActiveFirstOrDefault(source, i => true, null);

		public static IActiveValue<TSource> ActiveFirstOrDefault<TSource>(this IActiveList<TSource> source, Expression<Func<TSource, bool>> predicate) => ActiveFirstOrDefault(source, predicate.Compile(), predicate.GetReferencedProperties());

		public static IActiveValue<TSource> ActiveFirstOrDefault<TSource>(this IActiveList<TSource> source, Func<TSource, bool> predicate, IEnumerable<string> propertiesToWatch) => new ActiveFirstOrDefault<TSource, object>(source, predicate, propertiesToWatch);

		public static IActiveValue<TSource> ActiveFirstOrDefault<TSource, TParameter>(this IActiveList<TSource> source, IActiveValue<TParameter> parameter, Expression<Func<TSource, TParameter, bool>> predicate) => ActiveFirstOrDefault(source, parameter, predicate.Compile(), predicate.GetReferencedProperties());

		public static IActiveValue<TSource> ActiveFirstOrDefault<TSource, TParameter>(this IActiveList<TSource> source, IActiveValue<TParameter> parameter, Func<TSource, TParameter, bool> predicate, IEnumerable<string> sourcePropertiesToWatch, IEnumerable<string> parameterPropertiesToWatch) => ActiveFirstOrDefault(source, parameter, predicate, Tuple.Create(sourcePropertiesToWatch, parameterPropertiesToWatch));

		private static IActiveValue<TSource> ActiveFirstOrDefault<TSource, TParameter>(this IActiveList<TSource> source, IActiveValue<TParameter> parameter, Func<TSource, TParameter, bool> predicate, Tuple<IEnumerable<string>, IEnumerable<string>> propertiesToWatch) => new ActiveFirstOrDefault<TSource, TParameter>(source, parameter, predicate, propertiesToWatch.Item1, propertiesToWatch.Item2);


		public static IActiveValue<TSource> ActiveLastOrDefault<TSource>(this IActiveList<TSource> source) => ActiveLastOrDefault(source, i => true, null);

		public static IActiveValue<TSource> ActiveLastOrDefault<TSource>(this IActiveList<TSource> source, Expression<Func<TSource, bool>> predicate) => ActiveLastOrDefault(source, predicate.Compile(), predicate.GetReferencedProperties());

		public static IActiveValue<TSource> ActiveLastOrDefault<TSource>(this IActiveList<TSource> source, Func<TSource, bool> predicate, IEnumerable<string> propertiesToWatch) => new ActiveLastOrDefault<TSource, object>(source, predicate, propertiesToWatch);

		public static IActiveValue<TSource> ActiveLastOrDefault<TSource, TParameter>(this IActiveList<TSource> source, IActiveValue<TParameter> parameter, Expression<Func<TSource, TParameter, bool>> predicate) => ActiveLastOrDefault(source, parameter, predicate.Compile(), predicate.GetReferencedProperties());

		public static IActiveValue<TSource> ActiveLastOrDefault<TSource, TParameter>(this IActiveList<TSource> source, IActiveValue<TParameter> parameter, Func<TSource, TParameter, bool> predicate, IEnumerable<string> sourcePropertiesToWatch, IEnumerable<string> parameterPropertiesToWatch) => ActiveLastOrDefault(source, parameter, predicate, Tuple.Create(sourcePropertiesToWatch, parameterPropertiesToWatch));

		private static IActiveValue<TSource> ActiveLastOrDefault<TSource, TParameter>(this IActiveList<TSource> source, IActiveValue<TParameter> parameter, Func<TSource, TParameter, bool> predicate, Tuple<IEnumerable<string>, IEnumerable<string>> propertiesToWatch) => new ActiveLastOrDefault<TSource, TParameter>(source, parameter, predicate, propertiesToWatch.Item1, propertiesToWatch.Item2);


		public static IActiveValue<TSource> ActiveSingleOrDefault<TSource>(this IActiveList<TSource> source) => ActiveSingleOrDefault(source, i => true, null);

		public static IActiveValue<TSource> ActiveSingleOrDefault<TSource>(this IActiveList<TSource> source, Expression<Func<TSource, bool>> predicate) => ActiveSingleOrDefault(source, predicate.Compile(), predicate.GetReferencedProperties());

		public static IActiveValue<TSource> ActiveSingleOrDefault<TSource>(this IActiveList<TSource> source, Func<TSource, bool> predicate, IEnumerable<string> propertiesToWatch) => new ActiveSingleOrDefault<TSource, object>(source, predicate, propertiesToWatch);

		public static IActiveValue<TSource> ActiveSingleOrDefault<TSource, TParameter>(this IActiveList<TSource> source, IActiveValue<TParameter> parameter, Expression<Func<TSource, TParameter, bool>> predicate) => ActiveSingleOrDefault(source, parameter, predicate.Compile(), predicate.GetReferencedProperties());

		public static IActiveValue<TSource> ActiveSingleOrDefault<TSource, TParameter>(this IActiveList<TSource> source, IActiveValue<TParameter> parameter, Func<TSource, TParameter, bool> predicate, IEnumerable<string> sourcePropertiesToWatch, IEnumerable<string> parameterPropertiesToWatch) => ActiveSingleOrDefault(source, parameter, predicate, Tuple.Create(sourcePropertiesToWatch, parameterPropertiesToWatch));

		private static IActiveValue<TSource> ActiveSingleOrDefault<TSource, TParameter>(this IActiveList<TSource> source, IActiveValue<TParameter> parameter, Func<TSource, TParameter, bool> predicate, Tuple<IEnumerable<string>, IEnumerable<string>> propertiesToWatch) => new ActiveSingleOrDefault<TSource, TParameter>(source, parameter, predicate, propertiesToWatch.Item1, propertiesToWatch.Item2);


		public static IActiveValue<TSource> ActiveElementAtOrDefault<TSource>(this IActiveList<TSource> source, int index) => ActiveElementAtOrDefault(source, new ActiveValueWrapper<int>(index));

		public static IActiveValue<TSource> ActiveElementAtOrDefault<TSource>(this IActiveList<TSource> source, IActiveValue<int> index) => new ActiveElementAtOrDefault<TSource>(source, index);


		public static IActiveValue<bool> ActiveAll<TSource>(this IActiveList<TSource> source, Expression<Func<TSource, bool>> predicate) => ActiveAll(source, predicate.Compile(), predicate.GetReferencedProperties());

		public static IActiveValue<bool> ActiveAll<TSource>(this IActiveList<TSource> source, Func<TSource, bool> predicate, IEnumerable<string> propertiesToWatch) => new ActiveAll<TSource, object>(source, predicate, propertiesToWatch);

		public static IActiveValue<bool> ActiveAll<TSource, TParameter>(this IActiveList<TSource> source, IActiveValue<TParameter> parameter, Expression<Func<TSource, TParameter, bool>> predicate) => ActiveAll(source, parameter, predicate.Compile(), predicate.GetReferencedProperties());

		public static IActiveValue<bool> ActiveAll<TSource, TParameter>(this IActiveList<TSource> source, IActiveValue<TParameter> parameter, Func<TSource, TParameter, bool> predicate, IEnumerable<string> sourcePropertiesToWatch, IEnumerable<string> parameterPropertiesToWatch) => ActiveAll(source, parameter, predicate, Tuple.Create(sourcePropertiesToWatch, parameterPropertiesToWatch));

		private static IActiveValue<bool> ActiveAll<TSource, TParameter>(this IActiveList<TSource> source, IActiveValue<TParameter> parameter, Func<TSource, TParameter, bool> predicate, Tuple<IEnumerable<string>, IEnumerable<string>> propertiesToWatch) => new ActiveAll<TSource, TParameter>(source, parameter, predicate, propertiesToWatch.Item1, propertiesToWatch.Item2);


		public static IActiveValue<bool> ActiveAny<TSource>(this IActiveList<TSource> source) => ToActiveValue(source, l => l.Count > 0, new[] { nameof(IActiveList<TSource>.Count) });

		public static IActiveValue<bool> ActiveAny<TSource>(this IActiveList<TSource> source, Expression<Func<TSource, bool>> predicate) => ActiveAny(source, predicate.Compile(), predicate.GetReferencedProperties());

		public static IActiveValue<bool> ActiveAny<TSource>(this IActiveList<TSource> source, Func<TSource, bool> predicate, IEnumerable<string> propertiesToWatch) => new ActiveAny<TSource, object>(source, predicate, propertiesToWatch);

		public static IActiveValue<bool> ActiveAny<TSource, TParameter>(this IActiveList<TSource> source, IActiveValue<TParameter> parameter, Expression<Func<TSource, TParameter, bool>> predicate) => ActiveAny(source, parameter, predicate.Compile(), predicate.GetReferencedProperties());

		public static IActiveValue<bool> ActiveAny<TSource, TParameter>(this IActiveList<TSource> source, IActiveValue<TParameter> parameter, Func<TSource, TParameter, bool> predicate, IEnumerable<string> sourcePropertiesToWatch, IEnumerable<string> parameterPropertiesToWatch) => ActiveAny(source, parameter, predicate, Tuple.Create(sourcePropertiesToWatch, parameterPropertiesToWatch));

		private static IActiveValue<bool> ActiveAny<TSource, TParameter>(this IActiveList<TSource> source, IActiveValue<TParameter> parameter, Func<TSource, TParameter, bool> predicate, Tuple<IEnumerable<string>, IEnumerable<string>> propertiesToWatch) => new ActiveAny<TSource, TParameter>(source, parameter, predicate, propertiesToWatch.Item1, propertiesToWatch.Item2);


		public static IActiveValue<int> ActiveCount<TSource>(this IActiveList<TSource> source) => ToActiveValue(source, l => l.Count, new[] { nameof(IActiveList<TSource>.Count) });

		public static IActiveValue<int> ActiveCount<TSource>(this IActiveList<TSource> source, Expression<Func<TSource, bool>> predicate) => ActiveCount(source, predicate.Compile(), predicate.GetReferencedProperties());

		public static IActiveValue<int> ActiveCount<TSource>(this IActiveList<TSource> source, Func<TSource, bool> predicate, IEnumerable<string> propertiesToWatch) => new ActiveCount<TSource, object>(source, predicate, propertiesToWatch);

		public static IActiveValue<int> ActiveCount<TSource, TParameter>(this IActiveList<TSource> source, IActiveValue<TParameter> parameter, Expression<Func<TSource, TParameter, bool>> predicate) => ActiveCount(source, parameter, predicate.Compile(), predicate.GetReferencedProperties());

		public static IActiveValue<int> ActiveCount<TSource, TParameter>(this IActiveList<TSource> source, IActiveValue<TParameter> parameter, Func<TSource, TParameter, bool> predicate, IEnumerable<string> sourcePropertiesToWatch, IEnumerable<string> parameterPropertiesToWatch) => ActiveCount(source, parameter, predicate, Tuple.Create(sourcePropertiesToWatch, parameterPropertiesToWatch));

		private static IActiveValue<int> ActiveCount<TSource, TParameter>(this IActiveList<TSource> source, IActiveValue<TParameter> parameter, Func<TSource, TParameter, bool> predicate, Tuple<IEnumerable<string>, IEnumerable<string>> propertiesToWatch) => new ActiveCount<TSource, TParameter>(source, parameter, predicate, propertiesToWatch.Item1, propertiesToWatch.Item2);


		public static IActiveValue<bool> ActiveContains<TSource>(this IActiveList<TSource> source, TSource value) => ActiveContains(source, new ActiveValueWrapper<TSource>(value));

		public static IActiveValue<bool> ActiveContains<TSource>(this IActiveList<TSource> source, IActiveValue<TSource> value) => new ActiveContains<TSource>(source, value);


		public static IActiveValue<TSource> ActiveMaxOrDefault<TSource>(this IActiveList<TSource> source) => ActiveMaxOrDefault(source, i => i, null);

		public static IActiveValue<TResult> ActiveMaxOrDefault<TSource, TResult>(this IActiveList<TSource> source, Expression<Func<TSource, TResult>> selector) => ActiveMaxOrDefault(source, selector.Compile(), selector.GetReferencedProperties());

		public static IActiveValue<TResult> ActiveMaxOrDefault<TSource, TResult>(this IActiveList<TSource> source, Func<TSource, TResult> selector, IEnumerable<string> propertiesToWatch) => new ActiveMaxOrDefault<TSource, object, TResult>(source, selector, propertiesToWatch);

		public static IActiveValue<TResult> ActiveMaxOrDefault<TSource, TParameter, TResult>(this IActiveList<TSource> source, IActiveValue<TParameter> parameter, Expression<Func<TSource, TParameter, TResult>> selector) => ActiveMaxOrDefault(source, parameter, selector.Compile(), selector.GetReferencedProperties());

		public static IActiveValue<TResult> ActiveMaxOrDefault<TSource, TParameter, TResult>(this IActiveList<TSource> source, IActiveValue<TParameter> parameter, Func<TSource, TParameter, TResult> selector, IEnumerable<string> sourcePropertiesToWatch, IEnumerable<string> parameterPropertiesToWatch) => ActiveMaxOrDefault(source, parameter, selector, Tuple.Create(sourcePropertiesToWatch, parameterPropertiesToWatch));

		private static IActiveValue<TResult> ActiveMaxOrDefault<TSource, TParameter, TResult>(this IActiveList<TSource> source, IActiveValue<TParameter> parameter, Func<TSource, TParameter, TResult> selector, Tuple<IEnumerable<string>, IEnumerable<string>> propertiesToWatch) => new ActiveMaxOrDefault<TSource, TParameter, TResult>(source, parameter, selector, propertiesToWatch.Item1, propertiesToWatch.Item2);


		public static IActiveValue<TSource> ActiveMinOrDefault<TSource>(this IActiveList<TSource> source) => ActiveMinOrDefault(source, i => i, null);

		public static IActiveValue<TResult> ActiveMinOrDefault<TSource, TResult>(this IActiveList<TSource> source, Expression<Func<TSource, TResult>> selector) => ActiveMinOrDefault(source, selector.Compile(), selector.GetReferencedProperties());

		public static IActiveValue<TResult> ActiveMinOrDefault<TSource, TResult>(this IActiveList<TSource> source, Func<TSource, TResult> selector, IEnumerable<string> propertiesToWatch) => new ActiveMinOrDefault<TSource, object, TResult>(source, selector, propertiesToWatch);

		public static IActiveValue<TResult> ActiveMinOrDefault<TSource, TParameter, TResult>(this IActiveList<TSource> source, IActiveValue<TParameter> parameter, Expression<Func<TSource, TParameter, TResult>> selector) => ActiveMinOrDefault(source, parameter, selector.Compile(), selector.GetReferencedProperties());

		public static IActiveValue<TResult> ActiveMinOrDefault<TSource, TParameter, TResult>(this IActiveList<TSource> source, IActiveValue<TParameter> parameter, Func<TSource, TParameter, TResult> selector, IEnumerable<string> sourcePropertiesToWatch, IEnumerable<string> parameterPropertiesToWatch) => ActiveMinOrDefault(source, parameter, selector, Tuple.Create(sourcePropertiesToWatch, parameterPropertiesToWatch));

		private static IActiveValue<TResult> ActiveMinOrDefault<TSource, TParameter, TResult>(this IActiveList<TSource> source, IActiveValue<TParameter> parameter, Func<TSource, TParameter, TResult> selector, Tuple<IEnumerable<string>, IEnumerable<string>> propertiesToWatch) => new ActiveMinOrDefault<TSource, TParameter, TResult>(source, parameter, selector, propertiesToWatch.Item1, propertiesToWatch.Item2);

		// SequenceEqual (use Zip + All?)
		// --FirstOrDefault
		// --LastOrDefault
		// --SingleOrDefault
		// --ElementAtOrDefault
		// ElementsAtOrDefault
		// --Any
		// --All
		// --Count
		// --Contains
		// AggregateOrDefault
		// SumOrDefault
		// --MinOrDefault
		// --MaxOrDefault
		// AverageOrDefault
	}
}
