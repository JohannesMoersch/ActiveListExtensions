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

		public static IActiveValue<TResult> ActiveMutate<TValue, TResult>(this IActiveValue<TValue> source, Func<TValue, TResult> mutator, IEnumerable<string> propertiesToWatch) => new ActiveMutate<TValue, TResult>(source, mutator, propertiesToWatch);

		//public static IActiveValue<TResult> ActiveCombine<TValue1, TValue2, TResult>(this IActiveValue<TValue1> value1, TValue2 value2, Expression<Func<TValue1, TValue2, TResult>> resultCombiner) => ActiveCombine(value1, )

		//public static IActiveValue<TResult> ActiveCombine<TValue1, TValue2, TResult>(this IActiveValue<TValue1> value1, IActiveValue<TValue2> value2, Func<TValue1, TValue2, TResult> resultCombiner) => new ActiveCombine<TValue1, TValue2, TResult>(value1, value2, resultCombiner);

		public static IActiveValue<TSource> ActiveFirstOrDefault<TSource>(this IActiveList<TSource> source) => ActiveFirstOrDefault(source, i => true, null);

		public static IActiveValue<TSource> ActiveFirstOrDefault<TSource>(this IActiveList<TSource> source, Expression<Func<TSource, bool>> predicate) => ActiveFirstOrDefault(source, predicate.Compile(), predicate.GetReferencedProperties());

		public static IActiveValue<TSource> ActiveFirstOrDefault<TSource>(this IActiveList<TSource> source, Func<TSource, bool> predicate, IEnumerable<string> propertiesToWatch) => new ActiveFirstOrDefault<TSource>(source, predicate, propertiesToWatch);

		public static IActiveValue<TSource> ActiveLastOrDefault<TSource>(this IActiveList<TSource> source) => ActiveLastOrDefault(source, i => true, null);

		public static IActiveValue<TSource> ActiveLastOrDefault<TSource>(this IActiveList<TSource> source, Expression<Func<TSource, bool>> predicate) => ActiveLastOrDefault(source, predicate.Compile(), predicate.GetReferencedProperties());

		public static IActiveValue<TSource> ActiveLastOrDefault<TSource>(this IActiveList<TSource> source, Func<TSource, bool> predicate, IEnumerable<string> propertiesToWatch) => new ActiveLastOrDefault<TSource>(source, predicate, propertiesToWatch);

		public static IActiveValue<TSource> ActiveSingleOrDefault<TSource>(this IActiveList<TSource> source) => ActiveSingleOrDefault(source, i => true, null);

		public static IActiveValue<TSource> ActiveSingleOrDefault<TSource>(this IActiveList<TSource> source, Expression<Func<TSource, bool>> predicate) => ActiveSingleOrDefault(source, predicate.Compile(), predicate.GetReferencedProperties());

		public static IActiveValue<TSource> ActiveSingleOrDefault<TSource>(this IActiveList<TSource> source, Func<TSource, bool> predicate, IEnumerable<string> propertiesToWatch) => new ActiveSingleOrDefault<TSource>(source, predicate, propertiesToWatch);

		// SequenceEqual (use Zip + All?)
		// FirstOrDefault
		// LastOrDefault
		// SingleOrDefault
		// ElementAtOrDefault
		// ElementsAtOrDefault
		// Any
		// All
		// Count
		// Contains
		// AggregateOrDefault
		// SumOrDefault
		// MinOrDefault
		// MaxOrDefault
		// AverageOrDefault
	}
}
