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

		public static IActiveValue<TSource> ActiveFirstOrDefault<TSource>(this IActiveList<TSource> source) => new ActiveFirstOrDefault<TSource>(source);

		public static IActiveValue<TSource> ActiveLastOrDefault<TSource>(this IActiveList<TSource> source) => new ActiveLastOrDefault<TSource>(source);

		// SequenceEqual
		// FirstOrDefault
		// LastOrDefault
		// ElementAtOrDefault
		// Any
		// All
		// Count
		// Contains
		// Aggregate
		// Sum
		// Min
		// Max
		// Average
	}
}
