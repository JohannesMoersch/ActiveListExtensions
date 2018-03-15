using ActiveListExtensions.Utilities;
using ActiveListExtensions.ValueModifiers;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace ActiveListExtensions
{
	[EditorBrowsable(EditorBrowsableState.Never)]
	public static class ActiveValueExtensions
	{
		public static IActiveValue<TValue> ToActiveValue<TSource, TValue>(this TSource source, Expression<Func<TSource, TValue>> valueGetter)
			=> ToActiveValue(source, valueGetter.Compile(), valueGetter.GetReferencedProperties());

		public static IActiveValue<TValue> ToActiveValue<TSource, TValue>(this TSource source, Func<TSource, TValue> valueGetter, IEnumerable<string> propertiesToWatch)
			=> new ActiveValueListener<TSource, TValue>(source, valueGetter, propertiesToWatch);

		public static IActiveValue<TValue> ToActiveValue<TValue>(this Task<TValue> source)
			=> ToActiveValue(source, default(TValue));

		public static IActiveValue<TValue> ToActiveValue<TValue>(this Task<TValue> source, TValue defaultValue)
			=> new TaskActiveValue<TValue>(source, defaultValue);


		public static IActiveValue<TResult> ActiveSelect<TValue, TResult>(this IActiveValue<TValue> source, Expression<Func<TValue, TResult>> mutator)
			=> ActiveSelect(source, mutator.Compile(), mutator.GetReferencedProperties());

		public static IActiveValue<TResult> ActiveSelect<TValue, TResult>(this IActiveValue<TValue> source, Func<TValue, TResult> mutator, IEnumerable<string> propertiesToWatch)
			=> new ActiveSelectValue<TValue, TResult>(source, mutator, propertiesToWatch);


		public static IActiveValue<IReadOnlyList<TSource>> AsActiveValue<TSource>(this IActiveList<TSource> source)
			=> new ActiveListAsActiveValue<TSource>(source);


		public static IActiveValue<bool> ActiveSequenceEqual<TSource>(this IActiveList<TSource> source, IEnumerable<TSource> otherSource)
		{
			var comparer = EqualityComparer<TSource>.Default;
			return new ActiveSequenceEqual<TSource, object>(source, otherSource.ToReadOnlyList(), (o1, o2)
			=> comparer.Equals(o1, o2), null);
		}

		public static IActiveValue<bool> ActiveSequenceEqual<TSource>(this IActiveList<TSource> source, IEnumerable<TSource> otherSource, Expression<Func<TSource, TSource, bool>> comparer)
		{
			var properties = comparer.GetReferencedProperties();
			return ActiveSequenceEqual(source, otherSource, comparer.Compile(), properties.Item1.Concat(properties.Item2).Distinct());
		}

		public static IActiveValue<bool> ActiveSequenceEqual<TSource>(this IActiveList<TSource> source, IEnumerable<TSource> otherSource, Func<TSource, TSource, bool> comparer, IEnumerable<string> propertiesToWatch)
			=> new ActiveSequenceEqual<TSource, object>(source, otherSource.ToReadOnlyList(), comparer, propertiesToWatch);

		public static IActiveValue<bool> ActiveSequenceEqual<TSource, TParameter>(this IActiveList<TSource> source, IEnumerable<TSource> otherSource, IActiveValue<TParameter> parameter, Expression<Func<TSource, TSource, TParameter, bool>> comparer)
			=> ActiveSequenceEqual(source, otherSource, parameter, comparer.Compile(), comparer.GetReferencedProperties());

		public static IActiveValue<bool> ActiveSequenceEqual<TSource, TParameter>(this IActiveList<TSource> source, IEnumerable<TSource> otherSource, IActiveValue<TParameter> parameter, Func<TSource, TSource, TParameter, bool> comparer, IEnumerable<string> sourcePropertiesToWatch, IEnumerable<string> parameterPropertiesToWatch)
			=> ActiveSequenceEqual(source, otherSource, parameter, comparer, Tuple.Create(sourcePropertiesToWatch, Enumerable.Empty<string>(), parameterPropertiesToWatch));

		private static IActiveValue<bool> ActiveSequenceEqual<TSource, TParameter>(this IActiveList<TSource> source, IEnumerable<TSource> otherSource, IActiveValue<TParameter> parameter, Func<TSource, TSource, TParameter, bool> comparer, Tuple<IEnumerable<string>, IEnumerable<string>, IEnumerable<string>> propertiesToWatch)
			=> new ActiveSequenceEqual<TSource, TParameter>(source, otherSource.ToReadOnlyList(), parameter, comparer, propertiesToWatch.Item1.Concat(propertiesToWatch.Item2).Distinct(), propertiesToWatch.Item3);


		public static IActiveValue<TResult> ActiveCombine<TValue1, TValue2, TResult>(this IActiveValue<TValue1> value1, IActiveValue<TValue2> value2, Expression<Func<TValue1, TValue2, TResult>> resultCombiner)
			=> ActiveCombine(value1, value2, resultCombiner.Compile(), resultCombiner.GetReferencedProperties());

		public static IActiveValue<TResult> ActiveCombine<TValue1, TValue2, TResult>(this IActiveValue<TValue1> value1, IActiveValue<TValue2> value2, Func<TValue1, TValue2, TResult> resultCombiner, IEnumerable<string> value1PropertiesToWatch, IEnumerable<string> value2PropertiesToWatch)
			=> ActiveCombine(value1, value2, resultCombiner, Tuple.Create(value1PropertiesToWatch, value2PropertiesToWatch));

		private static IActiveValue<TResult> ActiveCombine<TValue1, TValue2, TResult>(this IActiveValue<TValue1> value1, IActiveValue<TValue2> value2, Func<TValue1, TValue2, TResult> resultCombiner, Tuple<IEnumerable<string>, IEnumerable<string>> propertiesToWatch)
			=> new ActiveCombine<TValue1, TValue2, TResult>(value1, value2, resultCombiner, propertiesToWatch.Item1, propertiesToWatch.Item2);


		public static IActiveValue<TResult> ActiveCastOrDefault<TResult>(this IActiveValue<object> source, TResult defaultValue = default(TResult))
			=> source.ActiveSelect(value => value is TResult ? (TResult)(object)value : defaultValue);


		public static IActiveValue<TSource> ActiveFirstOrDefault<TSource>(this IActiveList<TSource> source)
			=> ActiveFirstOrDefault(source, i => true, null);

		public static IActiveValue<TSource> ActiveFirstOrDefault<TSource>(this IActiveList<TSource> source, Expression<Func<TSource, bool>> predicate)
			=> ActiveFirstOrDefault(source, predicate.Compile(), predicate.GetReferencedProperties());

		public static IActiveValue<TSource> ActiveFirstOrDefault<TSource>(this IActiveList<TSource> source, Func<TSource, bool> predicate, IEnumerable<string> propertiesToWatch)
			=> new ActiveFirstOrDefault<TSource, object>(source, predicate, propertiesToWatch);

		public static IActiveValue<TSource> ActiveFirstOrDefault<TSource, TParameter>(this IActiveList<TSource> source, IActiveValue<TParameter> parameter, Expression<Func<TSource, TParameter, bool>> predicate)
			=> ActiveFirstOrDefault(source, parameter, predicate.Compile(), predicate.GetReferencedProperties());

		public static IActiveValue<TSource> ActiveFirstOrDefault<TSource, TParameter>(this IActiveList<TSource> source, IActiveValue<TParameter> parameter, Func<TSource, TParameter, bool> predicate, IEnumerable<string> sourcePropertiesToWatch, IEnumerable<string> parameterPropertiesToWatch)
			=> ActiveFirstOrDefault(source, parameter, predicate, Tuple.Create(sourcePropertiesToWatch, parameterPropertiesToWatch));

		private static IActiveValue<TSource> ActiveFirstOrDefault<TSource, TParameter>(this IActiveList<TSource> source, IActiveValue<TParameter> parameter, Func<TSource, TParameter, bool> predicate, Tuple<IEnumerable<string>, IEnumerable<string>> propertiesToWatch)
			=> new ActiveFirstOrDefault<TSource, TParameter>(source, parameter, predicate, propertiesToWatch.Item1, propertiesToWatch.Item2);


		public static IActiveValue<TSource> ActiveLastOrDefault<TSource>(this IActiveList<TSource> source)
			=> ActiveLastOrDefault(source, i => true, null);

		public static IActiveValue<TSource> ActiveLastOrDefault<TSource>(this IActiveList<TSource> source, Expression<Func<TSource, bool>> predicate)
			=> ActiveLastOrDefault(source, predicate.Compile(), predicate.GetReferencedProperties());

		public static IActiveValue<TSource> ActiveLastOrDefault<TSource>(this IActiveList<TSource> source, Func<TSource, bool> predicate, IEnumerable<string> propertiesToWatch)
			=> new ActiveLastOrDefault<TSource, object>(source, predicate, propertiesToWatch);

		public static IActiveValue<TSource> ActiveLastOrDefault<TSource, TParameter>(this IActiveList<TSource> source, IActiveValue<TParameter> parameter, Expression<Func<TSource, TParameter, bool>> predicate)
			=> ActiveLastOrDefault(source, parameter, predicate.Compile(), predicate.GetReferencedProperties());

		public static IActiveValue<TSource> ActiveLastOrDefault<TSource, TParameter>(this IActiveList<TSource> source, IActiveValue<TParameter> parameter, Func<TSource, TParameter, bool> predicate, IEnumerable<string> sourcePropertiesToWatch, IEnumerable<string> parameterPropertiesToWatch)
			=> ActiveLastOrDefault(source, parameter, predicate, Tuple.Create(sourcePropertiesToWatch, parameterPropertiesToWatch));

		private static IActiveValue<TSource> ActiveLastOrDefault<TSource, TParameter>(this IActiveList<TSource> source, IActiveValue<TParameter> parameter, Func<TSource, TParameter, bool> predicate, Tuple<IEnumerable<string>, IEnumerable<string>> propertiesToWatch)
			=> new ActiveLastOrDefault<TSource, TParameter>(source, parameter, predicate, propertiesToWatch.Item1, propertiesToWatch.Item2);


		public static IActiveValue<TSource> ActiveSingleOrDefault<TSource>(this IActiveList<TSource> source)
			=> ActiveSingleOrDefault(source, i => true, null);

		public static IActiveValue<TSource> ActiveSingleOrDefault<TSource>(this IActiveList<TSource> source, Expression<Func<TSource, bool>> predicate)
			=> ActiveSingleOrDefault(source, predicate.Compile(), predicate.GetReferencedProperties());

		public static IActiveValue<TSource> ActiveSingleOrDefault<TSource>(this IActiveList<TSource> source, Func<TSource, bool> predicate, IEnumerable<string> propertiesToWatch)
			=> new ActiveSingleOrDefault<TSource, object>(source, predicate, propertiesToWatch);

		public static IActiveValue<TSource> ActiveSingleOrDefault<TSource, TParameter>(this IActiveList<TSource> source, IActiveValue<TParameter> parameter, Expression<Func<TSource, TParameter, bool>> predicate)
			=> ActiveSingleOrDefault(source, parameter, predicate.Compile(), predicate.GetReferencedProperties());

		public static IActiveValue<TSource> ActiveSingleOrDefault<TSource, TParameter>(this IActiveList<TSource> source, IActiveValue<TParameter> parameter, Func<TSource, TParameter, bool> predicate, IEnumerable<string> sourcePropertiesToWatch, IEnumerable<string> parameterPropertiesToWatch)
			=> ActiveSingleOrDefault(source, parameter, predicate, Tuple.Create(sourcePropertiesToWatch, parameterPropertiesToWatch));

		private static IActiveValue<TSource> ActiveSingleOrDefault<TSource, TParameter>(this IActiveList<TSource> source, IActiveValue<TParameter> parameter, Func<TSource, TParameter, bool> predicate, Tuple<IEnumerable<string>, IEnumerable<string>> propertiesToWatch)
			=> new ActiveSingleOrDefault<TSource, TParameter>(source, parameter, predicate, propertiesToWatch.Item1, propertiesToWatch.Item2);


		public static IActiveValue<TSource> ActiveElementAtOrDefault<TSource>(this IActiveList<TSource> source, int index)
			=> ActiveElementAtOrDefault(source, new ActiveValueWrapper<int>(index));

		public static IActiveValue<TSource> ActiveElementAtOrDefault<TSource>(this IActiveList<TSource> source, IActiveValue<int> index)
			=> new ActiveElementAtOrDefault<TSource>(source, index);


		public static IActiveValue<bool> ActiveAll<TSource>(this IActiveList<TSource> source, Expression<Func<TSource, bool>> predicate)
			=> ActiveAll(source, predicate.Compile(), predicate.GetReferencedProperties());

		public static IActiveValue<bool> ActiveAll<TSource>(this IActiveList<TSource> source, Func<TSource, bool> predicate, IEnumerable<string> propertiesToWatch)
			=> new ActiveAll<TSource, object>(source, predicate, propertiesToWatch);

		public static IActiveValue<bool> ActiveAll<TSource, TParameter>(this IActiveList<TSource> source, IActiveValue<TParameter> parameter, Expression<Func<TSource, TParameter, bool>> predicate)
			=> ActiveAll(source, parameter, predicate.Compile(), predicate.GetReferencedProperties());

		public static IActiveValue<bool> ActiveAll<TSource, TParameter>(this IActiveList<TSource> source, IActiveValue<TParameter> parameter, Func<TSource, TParameter, bool> predicate, IEnumerable<string> sourcePropertiesToWatch, IEnumerable<string> parameterPropertiesToWatch)
			=> ActiveAll(source, parameter, predicate, Tuple.Create(sourcePropertiesToWatch, parameterPropertiesToWatch));

		private static IActiveValue<bool> ActiveAll<TSource, TParameter>(this IActiveList<TSource> source, IActiveValue<TParameter> parameter, Func<TSource, TParameter, bool> predicate, Tuple<IEnumerable<string>, IEnumerable<string>> propertiesToWatch)
			=> new ActiveAll<TSource, TParameter>(source, parameter, predicate, propertiesToWatch.Item1, propertiesToWatch.Item2);


		public static IActiveValue<bool> ActiveAny<TSource>(this IActiveList<TSource> source)
			=> ToActiveValue(source, l => l.Count > 0, new[] { nameof(IActiveList<TSource>.Count) });

		public static IActiveValue<bool> ActiveAny<TSource>(this IActiveList<TSource> source, Expression<Func<TSource, bool>> predicate)
			=> ActiveAny(source, predicate.Compile(), predicate.GetReferencedProperties());

		public static IActiveValue<bool> ActiveAny<TSource>(this IActiveList<TSource> source, Func<TSource, bool> predicate, IEnumerable<string> propertiesToWatch)
			=> new ActiveAny<TSource, object>(source, predicate, propertiesToWatch);

		public static IActiveValue<bool> ActiveAny<TSource, TParameter>(this IActiveList<TSource> source, IActiveValue<TParameter> parameter, Expression<Func<TSource, TParameter, bool>> predicate)
			=> ActiveAny(source, parameter, predicate.Compile(), predicate.GetReferencedProperties());

		public static IActiveValue<bool> ActiveAny<TSource, TParameter>(this IActiveList<TSource> source, IActiveValue<TParameter> parameter, Func<TSource, TParameter, bool> predicate, IEnumerable<string> sourcePropertiesToWatch, IEnumerable<string> parameterPropertiesToWatch)
			=> ActiveAny(source, parameter, predicate, Tuple.Create(sourcePropertiesToWatch, parameterPropertiesToWatch));

		private static IActiveValue<bool> ActiveAny<TSource, TParameter>(this IActiveList<TSource> source, IActiveValue<TParameter> parameter, Func<TSource, TParameter, bool> predicate, Tuple<IEnumerable<string>, IEnumerable<string>> propertiesToWatch)
			=> new ActiveAny<TSource, TParameter>(source, parameter, predicate, propertiesToWatch.Item1, propertiesToWatch.Item2);


		public static IActiveValue<int> ActiveCount<TSource>(this IActiveList<TSource> source)
			=> ToActiveValue(source, l => l.Count, new[] { nameof(IActiveList<TSource>.Count) });

		public static IActiveValue<int> ActiveCount<TSource>(this IActiveList<TSource> source, Expression<Func<TSource, bool>> predicate)
			=> ActiveCount(source, predicate.Compile(), predicate.GetReferencedProperties());

		public static IActiveValue<int> ActiveCount<TSource>(this IActiveList<TSource> source, Func<TSource, bool> predicate, IEnumerable<string> propertiesToWatch)
			=> new ActiveCount<TSource, object>(source, predicate, propertiesToWatch);

		public static IActiveValue<int> ActiveCount<TSource, TParameter>(this IActiveList<TSource> source, IActiveValue<TParameter> parameter, Expression<Func<TSource, TParameter, bool>> predicate)
			=> ActiveCount(source, parameter, predicate.Compile(), predicate.GetReferencedProperties());

		public static IActiveValue<int> ActiveCount<TSource, TParameter>(this IActiveList<TSource> source, IActiveValue<TParameter> parameter, Func<TSource, TParameter, bool> predicate, IEnumerable<string> sourcePropertiesToWatch, IEnumerable<string> parameterPropertiesToWatch)
			=> ActiveCount(source, parameter, predicate, Tuple.Create(sourcePropertiesToWatch, parameterPropertiesToWatch));

		private static IActiveValue<int> ActiveCount<TSource, TParameter>(this IActiveList<TSource> source, IActiveValue<TParameter> parameter, Func<TSource, TParameter, bool> predicate, Tuple<IEnumerable<string>, IEnumerable<string>> propertiesToWatch)
			=> new ActiveCount<TSource, TParameter>(source, parameter, predicate, propertiesToWatch.Item1, propertiesToWatch.Item2);


		public static IActiveValue<bool> ActiveContains<TSource>(this IActiveList<TSource> source, TSource value)
			=> ActiveContains(source, new ActiveValueWrapper<TSource>(value));

		public static IActiveValue<bool> ActiveContains<TSource>(this IActiveList<TSource> source, IActiveValue<TSource> value)
			=> new ActiveContains<TSource>(source, value);


		public static IActiveValue<TSource> ActiveMaxOrDefault<TSource>(this IActiveList<TSource> source)
			=> ActiveMaxOrDefault(source, i => i, null);

		public static IActiveValue<TResult> ActiveMaxOrDefault<TSource, TResult>(this IActiveList<TSource> source, Expression<Func<TSource, TResult>> selector)
			=> ActiveMaxOrDefault(source, selector.Compile(), selector.GetReferencedProperties());

		public static IActiveValue<TResult> ActiveMaxOrDefault<TSource, TResult>(this IActiveList<TSource> source, Func<TSource, TResult> selector, IEnumerable<string> propertiesToWatch)
			=> new ActiveMaxOrDefault<TSource, object, TResult>(source, selector, propertiesToWatch);

		public static IActiveValue<TResult> ActiveMaxOrDefault<TSource, TParameter, TResult>(this IActiveList<TSource> source, IActiveValue<TParameter> parameter, Expression<Func<TSource, TParameter, TResult>> selector)
			=> ActiveMaxOrDefault(source, parameter, selector.Compile(), selector.GetReferencedProperties());

		public static IActiveValue<TResult> ActiveMaxOrDefault<TSource, TParameter, TResult>(this IActiveList<TSource> source, IActiveValue<TParameter> parameter, Func<TSource, TParameter, TResult> selector, IEnumerable<string> sourcePropertiesToWatch, IEnumerable<string> parameterPropertiesToWatch)
			=> ActiveMaxOrDefault(source, parameter, selector, Tuple.Create(sourcePropertiesToWatch, parameterPropertiesToWatch));

		private static IActiveValue<TResult> ActiveMaxOrDefault<TSource, TParameter, TResult>(this IActiveList<TSource> source, IActiveValue<TParameter> parameter, Func<TSource, TParameter, TResult> selector, Tuple<IEnumerable<string>, IEnumerable<string>> propertiesToWatch)
			=> new ActiveMaxOrDefault<TSource, TParameter, TResult>(source, parameter, selector, propertiesToWatch.Item1, propertiesToWatch.Item2);


		public static IActiveValue<TSource> ActiveMinOrDefault<TSource>(this IActiveList<TSource> source)
			=> ActiveMinOrDefault(source, i => i, null);

		public static IActiveValue<TResult> ActiveMinOrDefault<TSource, TResult>(this IActiveList<TSource> source, Expression<Func<TSource, TResult>> selector)
			=> ActiveMinOrDefault(source, selector.Compile(), selector.GetReferencedProperties());

		public static IActiveValue<TResult> ActiveMinOrDefault<TSource, TResult>(this IActiveList<TSource> source, Func<TSource, TResult> selector, IEnumerable<string> propertiesToWatch)
			=> new ActiveMinOrDefault<TSource, object, TResult>(source, selector, propertiesToWatch);

		public static IActiveValue<TResult> ActiveMinOrDefault<TSource, TParameter, TResult>(this IActiveList<TSource> source, IActiveValue<TParameter> parameter, Expression<Func<TSource, TParameter, TResult>> selector)
			=> ActiveMinOrDefault(source, parameter, selector.Compile(), selector.GetReferencedProperties());

		public static IActiveValue<TResult> ActiveMinOrDefault<TSource, TParameter, TResult>(this IActiveList<TSource> source, IActiveValue<TParameter> parameter, Func<TSource, TParameter, TResult> selector, IEnumerable<string> sourcePropertiesToWatch, IEnumerable<string> parameterPropertiesToWatch)
			=> ActiveMinOrDefault(source, parameter, selector, Tuple.Create(sourcePropertiesToWatch, parameterPropertiesToWatch));

		private static IActiveValue<TResult> ActiveMinOrDefault<TSource, TParameter, TResult>(this IActiveList<TSource> source, IActiveValue<TParameter> parameter, Func<TSource, TParameter, TResult> selector, Tuple<IEnumerable<string>, IEnumerable<string>> propertiesToWatch)
			=> new ActiveMinOrDefault<TSource, TParameter, TResult>(source, parameter, selector, propertiesToWatch.Item1, propertiesToWatch.Item2);


		public static IActiveValue<int> ActiveSum(this IActiveList<int> source)
			=> ActiveSum(source, i => i, null);

		public static IActiveValue<int> ActiveSum<TSource>(this IActiveList<TSource> source, Expression<Func<TSource, int>> selector)
			=> ActiveSum(source, selector.Compile(), selector.GetReferencedProperties());

		public static IActiveValue<int> ActiveSum<TSource>(this IActiveList<TSource> source, Func<TSource, int> selector, IEnumerable<string> propertiesToWatch)
			=> new ActiveSum<TSource, object, int>(source, selector, (i1, i2) => i1 + i2, (i1, i2) => i1 - i2, propertiesToWatch);

		public static IActiveValue<int> ActiveSum<TSource, TParameter>(this IActiveList<TSource> source, IActiveValue<TParameter> parameter, Expression<Func<TSource, TParameter, int>> selector)
			=> ActiveSum(source, parameter, selector.Compile(), selector.GetReferencedProperties());

		public static IActiveValue<int> ActiveSum<TSource, TParameter>(this IActiveList<TSource> source, IActiveValue<TParameter> parameter, Func<TSource, TParameter, int> selector, IEnumerable<string> sourcePropertiesToWatch, IEnumerable<string> parameterPropertiesToWatch)
			=> ActiveSum(source, parameter, selector, Tuple.Create(sourcePropertiesToWatch, parameterPropertiesToWatch));

		private static IActiveValue<int> ActiveSum<TSource, TParameter>(this IActiveList<TSource> source, IActiveValue<TParameter> parameter, Func<TSource, TParameter, int> selector, Tuple<IEnumerable<string>, IEnumerable<string>> propertiesToWatch)
			=> new ActiveSum<TSource, TParameter, int>(source, parameter, selector, (i1, i2) => i1 + i2, (i1, i2) => i1 - i2, propertiesToWatch.Item1, propertiesToWatch.Item2);

		public static IActiveValue<long> ActiveSum(this IActiveList<long> source)
			=> ActiveSum(source, i => i, null);

		public static IActiveValue<long> ActiveSum<TSource>(this IActiveList<TSource> source, Expression<Func<TSource, long>> selector)
			=> ActiveSum(source, selector.Compile(), selector.GetReferencedProperties());

		public static IActiveValue<long> ActiveSum<TSource>(this IActiveList<TSource> source, Func<TSource, long> selector, IEnumerable<string> propertiesToWatch)
			=> new ActiveSum<TSource, object, long>(source, selector, (i1, i2) => i1 + i2, (i1, i2) => i1 - i2, propertiesToWatch);

		public static IActiveValue<long> ActiveSum<TSource, TParameter>(this IActiveList<TSource> source, IActiveValue<TParameter> parameter, Expression<Func<TSource, TParameter, long>> selector)
			=> ActiveSum(source, parameter, selector.Compile(), selector.GetReferencedProperties());

		public static IActiveValue<long> ActiveSum<TSource, TParameter>(this IActiveList<TSource> source, IActiveValue<TParameter> parameter, Func<TSource, TParameter, long> selector, IEnumerable<string> sourcePropertiesToWatch, IEnumerable<string> parameterPropertiesToWatch)
			=> ActiveSum(source, parameter, selector, Tuple.Create(sourcePropertiesToWatch, parameterPropertiesToWatch));

		private static IActiveValue<long> ActiveSum<TSource, TParameter>(this IActiveList<TSource> source, IActiveValue<TParameter> parameter, Func<TSource, TParameter, long> selector, Tuple<IEnumerable<string>, IEnumerable<string>> propertiesToWatch)
			=> new ActiveSum<TSource, TParameter, long>(source, parameter, selector, (i1, i2) => i1 + i2, (i1, i2) => i1 - i2, propertiesToWatch.Item1, propertiesToWatch.Item2);

		public static IActiveValue<float> ActiveSum(this IActiveList<float> source)
			=> ActiveSum(source, i => i, null);

		public static IActiveValue<float> ActiveSum<TSource>(this IActiveList<TSource> source, Expression<Func<TSource, float>> selector)
			=> ActiveSum(source, selector.Compile(), selector.GetReferencedProperties());

		public static IActiveValue<float> ActiveSum<TSource>(this IActiveList<TSource> source, Func<TSource, float> selector, IEnumerable<string> propertiesToWatch)
			=> new ActiveSum<TSource, object, float>(source, selector, (i1, i2) => i1 + i2, (i1, i2) => i1 - i2, propertiesToWatch);

		public static IActiveValue<float> ActiveSum<TSource, TParameter>(this IActiveList<TSource> source, IActiveValue<TParameter> parameter, Expression<Func<TSource, TParameter, float>> selector)
			=> ActiveSum(source, parameter, selector.Compile(), selector.GetReferencedProperties());

		public static IActiveValue<float> ActiveSum<TSource, TParameter>(this IActiveList<TSource> source, IActiveValue<TParameter> parameter, Func<TSource, TParameter, float> selector, IEnumerable<string> sourcePropertiesToWatch, IEnumerable<string> parameterPropertiesToWatch)
			=> ActiveSum(source, parameter, selector, Tuple.Create(sourcePropertiesToWatch, parameterPropertiesToWatch));

		private static IActiveValue<float> ActiveSum<TSource, TParameter>(this IActiveList<TSource> source, IActiveValue<TParameter> parameter, Func<TSource, TParameter, float> selector, Tuple<IEnumerable<string>, IEnumerable<string>> propertiesToWatch)
			=> new ActiveSum<TSource, TParameter, float>(source, parameter, selector, (i1, i2) => i1 + i2, (i1, i2) => i1 - i2, propertiesToWatch.Item1, propertiesToWatch.Item2);

		public static IActiveValue<double> ActiveSum(this IActiveList<double> source)
			=> ActiveSum(source, i => i, null);

		public static IActiveValue<double> ActiveSum<TSource>(this IActiveList<TSource> source, Expression<Func<TSource, double>> selector)
			=> ActiveSum(source, selector.Compile(), selector.GetReferencedProperties());

		public static IActiveValue<double> ActiveSum<TSource>(this IActiveList<TSource> source, Func<TSource, double> selector, IEnumerable<string> propertiesToWatch)
			=> new ActiveSum<TSource, object, double>(source, selector, (i1, i2) => i1 + i2, (i1, i2) => i1 - i2, propertiesToWatch);

		public static IActiveValue<double> ActiveSum<TSource, TParameter>(this IActiveList<TSource> source, IActiveValue<TParameter> parameter, Expression<Func<TSource, TParameter, double>> selector)
			=> ActiveSum(source, parameter, selector.Compile(), selector.GetReferencedProperties());

		public static IActiveValue<double> ActiveSum<TSource, TParameter>(this IActiveList<TSource> source, IActiveValue<TParameter> parameter, Func<TSource, TParameter, double> selector, IEnumerable<string> sourcePropertiesToWatch, IEnumerable<string> parameterPropertiesToWatch)
			=> ActiveSum(source, parameter, selector, Tuple.Create(sourcePropertiesToWatch, parameterPropertiesToWatch));

		private static IActiveValue<double> ActiveSum<TSource, TParameter>(this IActiveList<TSource> source, IActiveValue<TParameter> parameter, Func<TSource, TParameter, double> selector, Tuple<IEnumerable<string>, IEnumerable<string>> propertiesToWatch)
			=> new ActiveSum<TSource, TParameter, double>(source, parameter, selector, (i1, i2) => i1 + i2, (i1, i2) => i1 - i2, propertiesToWatch.Item1, propertiesToWatch.Item2);

		public static IActiveValue<decimal> ActiveSum(this IActiveList<decimal> source)
			=> ActiveSum(source, i => i, null);

		public static IActiveValue<decimal> ActiveSum<TSource>(this IActiveList<TSource> source, Expression<Func<TSource, decimal>> selector)
			=> ActiveSum(source, selector.Compile(), selector.GetReferencedProperties());

		public static IActiveValue<decimal> ActiveSum<TSource>(this IActiveList<TSource> source, Func<TSource, decimal> selector, IEnumerable<string> propertiesToWatch)
			=> new ActiveSum<TSource, object, decimal>(source, selector, (i1, i2) => i1 + i2, (i1, i2) => i1 - i2, propertiesToWatch);

		public static IActiveValue<decimal> ActiveSum<TSource, TParameter>(this IActiveList<TSource> source, IActiveValue<TParameter> parameter, Expression<Func<TSource, TParameter, decimal>> selector)
			=> ActiveSum(source, parameter, selector.Compile(), selector.GetReferencedProperties());

		public static IActiveValue<decimal> ActiveSum<TSource, TParameter>(this IActiveList<TSource> source, IActiveValue<TParameter> parameter, Func<TSource, TParameter, decimal> selector, IEnumerable<string> sourcePropertiesToWatch, IEnumerable<string> parameterPropertiesToWatch)
			=> ActiveSum(source, parameter, selector, Tuple.Create(sourcePropertiesToWatch, parameterPropertiesToWatch));

		private static IActiveValue<decimal> ActiveSum<TSource, TParameter>(this IActiveList<TSource> source, IActiveValue<TParameter> parameter, Func<TSource, TParameter, decimal> selector, Tuple<IEnumerable<string>, IEnumerable<string>> propertiesToWatch)
			=> new ActiveSum<TSource, TParameter, decimal>(source, parameter, selector, (i1, i2) => i1 + i2, (i1, i2) => i1 - i2, propertiesToWatch.Item1, propertiesToWatch.Item2);


		public static IActiveValue<double> ActiveAverageOrDefault(this IActiveList<int> source)
			=> ActiveAverageOrDefault(source, i => i, null);

		public static IActiveValue<double> ActiveAverageOrDefault<TSource>(this IActiveList<TSource> source, Expression<Func<TSource, int>> selector)
			=> ActiveAverageOrDefault(source, selector.Compile(), selector.GetReferencedProperties());

		public static IActiveValue<double> ActiveAverageOrDefault<TSource>(this IActiveList<TSource> source, Func<TSource, int> selector, IEnumerable<string> propertiesToWatch)
			=> new ActiveAverageOrDefault<TSource, object, int, double>(source, selector, (i1, i2) => i1 + i2, (i1, i2) => i1 - i2, (i1, i2) => (double)i1 / i2, propertiesToWatch);

		public static IActiveValue<double> ActiveAverageOrDefault<TSource, TParameter>(this IActiveList<TSource> source, IActiveValue<TParameter> parameter, Expression<Func<TSource, TParameter, int>> selector)
			=> ActiveAverageOrDefault(source, parameter, selector.Compile(), selector.GetReferencedProperties());

		public static IActiveValue<double> ActiveAverageOrDefault<TSource, TParameter>(this IActiveList<TSource> source, IActiveValue<TParameter> parameter, Func<TSource, TParameter, int> selector, IEnumerable<string> sourcePropertiesToWatch, IEnumerable<string> parameterPropertiesToWatch)
			=> ActiveAverageOrDefault(source, parameter, selector, Tuple.Create(sourcePropertiesToWatch, parameterPropertiesToWatch));

		private static IActiveValue<double> ActiveAverageOrDefault<TSource, TParameter>(this IActiveList<TSource> source, IActiveValue<TParameter> parameter, Func<TSource, TParameter, int> selector, Tuple<IEnumerable<string>, IEnumerable<string>> propertiesToWatch)
			=> new ActiveAverageOrDefault<TSource, TParameter, int, double>(source, parameter, selector, (i1, i2) => i1 + i2, (i1, i2) => i1 - i2, (i1, i2) => (double)i1 / i2, propertiesToWatch.Item1, propertiesToWatch.Item2);

		public static IActiveValue<double> ActiveAverageOrDefault(this IActiveList<long> source)
			=> ActiveAverageOrDefault(source, i => i, null);

		public static IActiveValue<double> ActiveAverageOrDefault<TSource>(this IActiveList<TSource> source, Expression<Func<TSource, long>> selector)
			=> ActiveAverageOrDefault(source, selector.Compile(), selector.GetReferencedProperties());

		public static IActiveValue<double> ActiveAverageOrDefault<TSource>(this IActiveList<TSource> source, Func<TSource, long> selector, IEnumerable<string> propertiesToWatch)
			=> new ActiveAverageOrDefault<TSource, object, long, double>(source, selector, (i1, i2) => i1 + i2, (i1, i2) => i1 - i2, (i1, i2) => (double)i1 / i2, propertiesToWatch);

		public static IActiveValue<double> ActiveAverageOrDefault<TSource, TParameter>(this IActiveList<TSource> source, IActiveValue<TParameter> parameter, Expression<Func<TSource, TParameter, long>> selector)
			=> ActiveAverageOrDefault(source, parameter, selector.Compile(), selector.GetReferencedProperties());

		public static IActiveValue<double> ActiveAverageOrDefault<TSource, TParameter>(this IActiveList<TSource> source, IActiveValue<TParameter> parameter, Func<TSource, TParameter, long> selector, IEnumerable<string> sourcePropertiesToWatch, IEnumerable<string> parameterPropertiesToWatch)
			=> ActiveAverageOrDefault(source, parameter, selector, Tuple.Create(sourcePropertiesToWatch, parameterPropertiesToWatch));

		private static IActiveValue<double> ActiveAverageOrDefault<TSource, TParameter>(this IActiveList<TSource> source, IActiveValue<TParameter> parameter, Func<TSource, TParameter, long> selector, Tuple<IEnumerable<string>, IEnumerable<string>> propertiesToWatch)
			=> new ActiveAverageOrDefault<TSource, TParameter, long, double>(source, parameter, selector, (i1, i2) => i1 + i2, (i1, i2) => i1 - i2, (i1, i2) => (double)i1 / i2, propertiesToWatch.Item1, propertiesToWatch.Item2);

		public static IActiveValue<float> ActiveAverageOrDefault(this IActiveList<float> source)
			=> ActiveAverageOrDefault(source, i => i, null);

		public static IActiveValue<float> ActiveAverageOrDefault<TSource>(this IActiveList<TSource> source, Expression<Func<TSource, float>> selector)
			=> ActiveAverageOrDefault(source, selector.Compile(), selector.GetReferencedProperties());

		public static IActiveValue<float> ActiveAverageOrDefault<TSource>(this IActiveList<TSource> source, Func<TSource, float> selector, IEnumerable<string> propertiesToWatch)
			=> new ActiveAverageOrDefault<TSource, object, float, float>(source, selector, (i1, i2) => i1 + i2, (i1, i2) => i1 - i2, (i1, i2) => i1 / i2, propertiesToWatch);

		public static IActiveValue<float> ActiveAverageOrDefault<TSource, TParameter>(this IActiveList<TSource> source, IActiveValue<TParameter> parameter, Expression<Func<TSource, TParameter, float>> selector)
			=> ActiveAverageOrDefault(source, parameter, selector.Compile(), selector.GetReferencedProperties());

		public static IActiveValue<float> ActiveAverageOrDefault<TSource, TParameter>(this IActiveList<TSource> source, IActiveValue<TParameter> parameter, Func<TSource, TParameter, float> selector, IEnumerable<string> sourcePropertiesToWatch, IEnumerable<string> parameterPropertiesToWatch)
			=> ActiveAverageOrDefault(source, parameter, selector, Tuple.Create(sourcePropertiesToWatch, parameterPropertiesToWatch));

		private static IActiveValue<float> ActiveAverageOrDefault<TSource, TParameter>(this IActiveList<TSource> source, IActiveValue<TParameter> parameter, Func<TSource, TParameter, float> selector, Tuple<IEnumerable<string>, IEnumerable<string>> propertiesToWatch)
			=> new ActiveAverageOrDefault<TSource, TParameter, float, float>(source, parameter, selector, (i1, i2) => i1 + i2, (i1, i2) => i1 - i2, (i1, i2) => i1 / i2, propertiesToWatch.Item1, propertiesToWatch.Item2);

		public static IActiveValue<double> ActiveAverageOrDefault(this IActiveList<double> source)
			=> ActiveAverageOrDefault(source, i => i, null);

		public static IActiveValue<double> ActiveAverageOrDefault<TSource>(this IActiveList<TSource> source, Expression<Func<TSource, double>> selector)
			=> ActiveAverageOrDefault(source, selector.Compile(), selector.GetReferencedProperties());

		public static IActiveValue<double> ActiveAverageOrDefault<TSource>(this IActiveList<TSource> source, Func<TSource, double> selector, IEnumerable<string> propertiesToWatch)
			=> new ActiveAverageOrDefault<TSource, object, double, double>(source, selector, (i1, i2) => i1 + i2, (i1, i2) => i1 - i2, (i1, i2) => i1 / i2, propertiesToWatch);

		public static IActiveValue<double> ActiveAverageOrDefault<TSource, TParameter>(this IActiveList<TSource> source, IActiveValue<TParameter> parameter, Expression<Func<TSource, TParameter, double>> selector)
			=> ActiveAverageOrDefault(source, parameter, selector.Compile(), selector.GetReferencedProperties());

		public static IActiveValue<double> ActiveAverageOrDefault<TSource, TParameter>(this IActiveList<TSource> source, IActiveValue<TParameter> parameter, Func<TSource, TParameter, double> selector, IEnumerable<string> sourcePropertiesToWatch, IEnumerable<string> parameterPropertiesToWatch)
			=> ActiveAverageOrDefault(source, parameter, selector, Tuple.Create(sourcePropertiesToWatch, parameterPropertiesToWatch));

		private static IActiveValue<double> ActiveAverageOrDefault<TSource, TParameter>(this IActiveList<TSource> source, IActiveValue<TParameter> parameter, Func<TSource, TParameter, double> selector, Tuple<IEnumerable<string>, IEnumerable<string>> propertiesToWatch)
			=> new ActiveAverageOrDefault<TSource, TParameter, double, double>(source, parameter, selector, (i1, i2) => i1 + i2, (i1, i2) => i1 - i2, (i1, i2) => i1 / i2, propertiesToWatch.Item1, propertiesToWatch.Item2);

		public static IActiveValue<decimal> ActiveAverageOrDefault(this IActiveList<decimal> source)
			=> ActiveAverageOrDefault(source, i => i, null);

		public static IActiveValue<decimal> ActiveAverageOrDefault<TSource>(this IActiveList<TSource> source, Expression<Func<TSource, decimal>> selector)
			=> ActiveAverageOrDefault(source, selector.Compile(), selector.GetReferencedProperties());

		public static IActiveValue<decimal> ActiveAverageOrDefault<TSource>(this IActiveList<TSource> source, Func<TSource, decimal> selector, IEnumerable<string> propertiesToWatch)
			=> new ActiveAverageOrDefault<TSource, object, decimal, decimal>(source, selector, (i1, i2) => i1 + i2, (i1, i2) => i1 - i2, (i1, i2) => i1 / i2, propertiesToWatch);

		public static IActiveValue<decimal> ActiveAverageOrDefault<TSource, TParameter>(this IActiveList<TSource> source, IActiveValue<TParameter> parameter, Expression<Func<TSource, TParameter, decimal>> selector)
			=> ActiveAverageOrDefault(source, parameter, selector.Compile(), selector.GetReferencedProperties());

		public static IActiveValue<decimal> ActiveAverageOrDefault<TSource, TParameter>(this IActiveList<TSource> source, IActiveValue<TParameter> parameter, Func<TSource, TParameter, decimal> selector, IEnumerable<string> sourcePropertiesToWatch, IEnumerable<string> parameterPropertiesToWatch)
			=> ActiveAverageOrDefault(source, parameter, selector, Tuple.Create(sourcePropertiesToWatch, parameterPropertiesToWatch));

		private static IActiveValue<decimal> ActiveAverageOrDefault<TSource, TParameter>(this IActiveList<TSource> source, IActiveValue<TParameter> parameter, Func<TSource, TParameter, decimal> selector, Tuple<IEnumerable<string>, IEnumerable<string>> propertiesToWatch)
			=> new ActiveAverageOrDefault<TSource, TParameter, decimal, decimal>(source, parameter, selector, (i1, i2) => i1 + i2, (i1, i2) => i1 - i2, (i1, i2) => i1 / i2, propertiesToWatch.Item1, propertiesToWatch.Item2);


		public static IActiveValue<bool> ActiveSetContains<TSource>(this IActiveSetList<TSource> source, TSource value)
			=> ActiveSetContains(source, new ActiveValueWrapper<TSource>(value));

		public static IActiveValue<bool> ActiveSetContains<TSource>(this IActiveSetList<TSource> source, IActiveValue<TSource> value)
			=> new ActiveSetContains<TSource, TSource>(source, source, value);

		public static IActiveValue<bool> ActiveSetContains<TKey, TSource>(this IActiveLookup<TKey, TSource> source, TKey value)
			=> ActiveSetContains(source, new ActiveValueWrapper<TKey>(value));

		public static IActiveValue<bool> ActiveSetContains<TKey, TSource>(this IActiveLookup<TKey, TSource> source, IActiveValue<TKey> value)
			=> new ActiveSetContains<TKey, IActiveGrouping<TKey, TSource>>(source, source, value);


		public static IActiveValue<TValue> ActiveAggregate<TSource, TValue>(this IActiveValue<TSource> source, Expression<Func<TValue, TSource, TValue>> aggregator)
			=> ActiveAggregate(source, default(TValue), aggregator);

		public static IActiveValue<TValue> ActiveAggregate<TSource, TValue>(this IActiveValue<TSource> source, TValue initialValue, Expression<Func<TValue, TSource, TValue>> aggregator)
			=> new ActiveAggregate<TSource, TValue>(initialValue, source, aggregator.Compile(), aggregator.GetReferencedProperties().Item2);

		public static IActiveValue<TValue> ActiveAggregate<TSource, TValue>(this IActiveValue<TSource> source, Func<TValue, TSource, TValue> aggregator, IEnumerable<string> sourcePropertiesToWatch)
			=> ActiveAggregate(source, default(TValue), aggregator, sourcePropertiesToWatch);

		public static IActiveValue<TValue> ActiveAggregate<TSource, TValue>(this IActiveValue<TSource> source, TValue initialValue, Func<TValue, TSource, TValue> aggregator, IEnumerable<string> sourcePropertiesToWatch)
			=> new ActiveAggregate<TSource, TValue>(initialValue, source, aggregator, sourcePropertiesToWatch);


		public static IActiveValue<TSource> ActiveDo<TSource>(this IActiveValue<TSource> source, Expression<Action<TSource>> doAction)
			=> ActiveDo(source, doAction.Compile(), doAction.GetReferencedProperties());

		public static IActiveValue<TSource> ActiveDo<TSource>(this IActiveValue<TSource> source, Action<TSource> doAction, IEnumerable<string> sourcePropertiesToWatch)
			=> source.ActiveSelect(s =>
			{
				doAction.Invoke(s);
				return s;
			}, sourcePropertiesToWatch);

		public static IActiveValue<TSource> ActiveDo<TSource, TParameter>(this IActiveValue<TSource> source, IActiveValue<TParameter> parameter, Expression<Action<TSource, TParameter>> doAction)
			=> ActiveDo(source, parameter, doAction.Compile(), doAction.GetReferencedProperties());

		public static IActiveValue<TSource> ActiveDo<TSource, TParameter>(this IActiveValue<TSource> source, IActiveValue<TParameter> parameter, Action<TSource, TParameter> doAction, IEnumerable<string> sourcePropertiesToWatch, IEnumerable<string> parameterPropertiesToWatch)
			=> ActiveDo(source, parameter, doAction, Tuple.Create(sourcePropertiesToWatch, parameterPropertiesToWatch));

		private static IActiveValue<TSource> ActiveDo<TSource, TParameter>(this IActiveValue<TSource> source, IActiveValue<TParameter> parameter, Action<TSource, TParameter> doAction, Tuple<IEnumerable<string>, IEnumerable<string>> propertiesToWatch)
			=> source.ActiveCombine(parameter, (s, e) =>
			{
				doAction.Invoke(s, e);
				return s;
			}, propertiesToWatch.Item1, propertiesToWatch.Item2);
	}
}