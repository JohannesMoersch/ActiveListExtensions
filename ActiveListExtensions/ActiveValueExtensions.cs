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

		public static IActiveValue<TResult> ActiveCombine<TValue1, TValue2, TResult>(this IActiveValue<TValue1> value1, TValue2 value2, Expression<Func<TValue1, TValue2, TResult>> resultCombiner) => ActiveCombine(value1, new ActiveValueWrapper<TValue2>(value2), resultCombiner);

		public static IActiveValue<TResult> ActiveCombine<TValue1, TValue2, TResult>(this IActiveValue<TValue1> value1, TValue2 value2, Func<TValue1, TValue2, TResult> resultCombiner, IEnumerable<string> value1PropertiesToWatch, IEnumerable<string> value2PropertiesToWatch) => ActiveCombine(value1, new ActiveValueWrapper<TValue2>(value2), resultCombiner, value1PropertiesToWatch, value2PropertiesToWatch);

		public static IActiveValue<TResult> ActiveCombine<TValue1, TValue2, TResult>(this IActiveValue<TValue1> value1, IActiveValue<TValue2> value2, Expression<Func<TValue1, TValue2, TResult>> resultCombiner)
		{
			var properties = resultCombiner.GetReferencedProperties();
			return ActiveCombine(value1, value2, resultCombiner.Compile(), properties.Item1, properties.Item2);
		}

		public static IActiveValue<TResult> ActiveCombine<TValue1, TValue2, TResult>(this IActiveValue<TValue1> value1, IActiveValue<TValue2> value2, Func<TValue1, TValue2, TResult> resultCombiner, IEnumerable<string> value1PropertiesToWatch, IEnumerable<string> value2PropertiesToWatch) => new ActiveCombine<TValue1, TValue2, TResult>(value1, value2, resultCombiner, value1PropertiesToWatch, value2PropertiesToWatch);

		public static IActiveValue<TSource> ActiveFirstOrDefault<TSource>(this IActiveList<TSource> source) => ActiveFirstOrDefault(source, i => true, null);

		public static IActiveValue<TSource> ActiveFirstOrDefault<TSource>(this IActiveList<TSource> source, Expression<Func<TSource, bool>> predicate) => ActiveFirstOrDefault(source, predicate.Compile(), predicate.GetReferencedProperties());

		public static IActiveValue<TSource> ActiveFirstOrDefault<TSource>(this IActiveList<TSource> source, Func<TSource, bool> predicate, IEnumerable<string> propertiesToWatch) => new ActiveFirstOrDefault<TSource>(source, predicate, propertiesToWatch);

		public static IActiveValue<TSource> ActiveLastOrDefault<TSource>(this IActiveList<TSource> source) => ActiveLastOrDefault(source, i => true, null);

		public static IActiveValue<TSource> ActiveLastOrDefault<TSource>(this IActiveList<TSource> source, Expression<Func<TSource, bool>> predicate) => ActiveLastOrDefault(source, predicate.Compile(), predicate.GetReferencedProperties());

		public static IActiveValue<TSource> ActiveLastOrDefault<TSource>(this IActiveList<TSource> source, Func<TSource, bool> predicate, IEnumerable<string> propertiesToWatch) => new ActiveLastOrDefault<TSource>(source, predicate, propertiesToWatch);

		public static IActiveValue<TSource> ActiveSingleOrDefault<TSource>(this IActiveList<TSource> source) => ActiveSingleOrDefault(source, i => true, null);

		public static IActiveValue<TSource> ActiveSingleOrDefault<TSource>(this IActiveList<TSource> source, Expression<Func<TSource, bool>> predicate) => ActiveSingleOrDefault(source, predicate.Compile(), predicate.GetReferencedProperties());

		public static IActiveValue<TSource> ActiveSingleOrDefault<TSource>(this IActiveList<TSource> source, Func<TSource, bool> predicate, IEnumerable<string> propertiesToWatch) => new ActiveSingleOrDefault<TSource>(source, predicate, propertiesToWatch);

		public static IActiveValue<TSource> ActiveElementAtOrDefault<TSource>(this IActiveList<TSource> source, int index) => ActiveElementAtOrDefault(source, new ActiveValueWrapper<int>(index));

		public static IActiveValue<TSource> ActiveElementAtOrDefault<TSource>(this IActiveList<TSource> source, IActiveValue<int> index) => new ActiveElementAtOrDefault<TSource>(source, index);

		public static IActiveValue<bool> ActiveAll<TSource>(this IActiveList<TSource> source, Expression<Func<TSource, bool>> predicate) => ActiveAll(source, predicate.Compile(), predicate.GetReferencedProperties());

		public static IActiveValue<bool> ActiveAll<TSource>(this IActiveList<TSource> source, Func<TSource, bool> predicate, IEnumerable<string> propertiesToWatch) => new ActiveAll<TSource>(source, predicate, propertiesToWatch);

		public static IActiveValue<bool> ActiveAny<TSource>(this IActiveList<TSource> source) => ToActiveValue(source, l => l.Count > 0, new[] { nameof(IActiveList<TSource>.Count) });

		public static IActiveValue<bool> ActiveAny<TSource>(this IActiveList<TSource> source, Expression<Func<TSource, bool>> predicate) => ActiveAny(source, predicate.Compile(), predicate.GetReferencedProperties());

		public static IActiveValue<bool> ActiveAny<TSource>(this IActiveList<TSource> source, Func<TSource, bool> predicate, IEnumerable<string> propertiesToWatch) => new ActiveAny<TSource>(source, predicate, propertiesToWatch);

		public static IActiveValue<int> ActiveCount<TSource>(this IActiveList<TSource> source) => ToActiveValue(source, l => l.Count, new[] { nameof(IActiveList<TSource>.Count) });

		public static IActiveValue<int> ActiveCount<TSource>(this IActiveList<TSource> source, Expression<Func<TSource, bool>> predicate) => ActiveCount(source, predicate.Compile(), predicate.GetReferencedProperties());

		public static IActiveValue<int> ActiveCount<TSource>(this IActiveList<TSource> source, Func<TSource, bool> predicate, IEnumerable<string> propertiesToWatch) => new ActiveCount<TSource>(source, predicate, propertiesToWatch);

		public static IActiveValue<bool> ActiveContains<TSource>(this IActiveList<TSource> source, TSource value) => ActiveContains(source, new ActiveValueWrapper<TSource>(value));

		public static IActiveValue<bool> ActiveContains<TSource>(this IActiveList<TSource> source, IActiveValue<TSource> value) => new ActiveContains<TSource>(source, value);

		public static IActiveValue<TSource> ActiveMaxOrDefault<TSource>(this IActiveList<TSource> source) => ActiveMaxOrDefault(source, i => i, null);

		public static IActiveValue<TResult> ActiveMaxOrDefault<TSource, TResult>(this IActiveList<TSource> source, Expression<Func<TSource, TResult>> selector) => ActiveMaxOrDefault(source, selector.Compile(), selector.GetReferencedProperties());

		public static IActiveValue<TResult> ActiveMaxOrDefault<TSource, TResult>(this IActiveList<TSource> source, Func<TSource, TResult> selector, IEnumerable<string> propertiesToWatch) => new ActiveMaxOrDefault<TSource, TResult>(source, selector, propertiesToWatch);

		public static IActiveValue<TSource> ActiveMinOrDefault<TSource>(this IActiveList<TSource> source) => ActiveMinOrDefault(source, i => i, null);

		public static IActiveValue<TResult> ActiveMinOrDefault<TSource, TResult>(this IActiveList<TSource> source, Expression<Func<TSource, TResult>> selector) => ActiveMinOrDefault(source, selector.Compile(), selector.GetReferencedProperties());

		public static IActiveValue<TResult> ActiveMinOrDefault<TSource, TResult>(this IActiveList<TSource> source, Func<TSource, TResult> selector, IEnumerable<string> propertiesToWatch) => new ActiveMinOrDefault<TSource, TResult>(source, selector, propertiesToWatch);

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
