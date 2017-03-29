using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;
using ActiveListExtensions.Modifiers;
using ActiveListExtensions.Utilities;

namespace ActiveListExtensions
{
    public static class ActiveListExtensions
    {
		public static IActiveList<T> ToActiveList<T>(this IList<T> source)
		{
			if (source == null)
				throw new ArgumentNullException(nameof(source));
			return ToActiveList(source as IReadOnlyList<T> ?? new ListToReadOnlyWrapper<T>(source));
		}

		public static IActiveList<T> ToActiveList<T>(this IReadOnlyList<T> source)
		{
			if (source == null)
				throw new ArgumentNullException(nameof(source));
			if (source is IActiveList<T> list)
				return list;
			return new ActiveList<T>(source);
		}

		public static IActiveList<TSource> ActiveWhere<TSource>(this IActiveList<TSource> source, Expression<Func<TSource, bool>> predicate) => ActiveWhere(source, predicate.Compile(), predicate.GetReferencedProperties());

		public static IActiveList<TSource> ActiveWhere<TSource>(this IActiveList<TSource> source, Func<TSource, bool> predicate, IEnumerable<string> propertiesToWatch) => new ActiveWhere<TSource>(source, predicate, propertiesToWatch);

		public static IActiveList<TResult> ActiveSelect<TSource, TResult>(this IActiveList<TSource> source, Expression<Func<TSource, TResult>> selector) => ActiveSelect(source, selector.Compile(), selector.GetReferencedProperties());

		public static IActiveList<TResult> ActiveSelect<TSource, TResult>(this IActiveList<TSource> source, Func<TSource, TResult> selector, IEnumerable<string> propertiesToWatch) => new ActiveSelect<TSource, TResult>(source, selector, propertiesToWatch);

		public static IActiveList<TResult> ActiveSelectMany<TSource, TResult>(this IActiveList<TSource> source, Expression<Func<TSource, IEnumerable<TResult>>> selector) => ActiveSelectMany(source, selector.Compile(), selector.GetReferencedProperties());

		public static IActiveList<TResult> ActiveSelectMany<TSource, TResult>(this IActiveList<TSource> source, Func<TSource, IEnumerable<TResult>> selector, IEnumerable<string> propertiesToWatch) => new ActiveSelectMany<TSource, TResult>(source, selector, propertiesToWatch);

		public static IActiveList<TSource> ActiveTake<TSource>(this IActiveList<TSource> source, int count) => new ActiveTake<TSource>(source, new ActiveValueWrapper<int>(count));

		public static IActiveList<TSource> ActiveTake<TSource>(this IActiveList<TSource> source, IActiveValue<int> count) => new ActiveTake<TSource>(source, count);

		public static IActiveList<TSource> ActiveSkip<TSource>(this IActiveList<TSource> source, int count) => new ActiveSkip<TSource>(source, new ActiveValueWrapper<int>(count));

		public static IActiveList<TSource> ActiveSkip<TSource>(this IActiveList<TSource> source, IActiveValue<int> count) => new ActiveSkip<TSource>(source, count);

		public static IActiveList<TSource> ActiveConcat<TSource>(this IActiveList<TSource> source, IEnumerable<TSource> concat) => new ActiveConcat<TSource>(source, concat);

		public static IActiveList<TSource> ActiveReverse<TSource>(this IActiveList<TSource> source) => new ActiveReverse<TSource>(source);

		public static IActiveList<TSource> ActiveDistinct<TSource>(this IActiveList<TSource> source) => new ActiveDistinct<TSource, TSource>(source, o => o);

		public static IActiveList<TSource> ActiveDistinct<TSource, TKey>(this IActiveList<TSource> source, Expression<Func<TSource, TKey>> keySelector) => ActiveDistinct(source, keySelector.Compile(), keySelector.GetReferencedProperties());

		public static IActiveList<TSource> ActiveDistinct<TSource, TKey>(this IActiveList<TSource> source, Func<TSource, TKey> keySelector, IEnumerable<string> propertiesToWatch) => new ActiveDistinct<TSource, TKey>(source, keySelector, propertiesToWatch);

		public static IActiveList<TSource> ActiveUnion<TSource>(this IActiveList<TSource> source, IActiveList<TSource> union) => new ActiveUnion<TSource, TSource>(source, union, o => o);

		public static IActiveList<TSource> ActiveUnion<TSource, TKey>(this IActiveList<TSource> source, IActiveList<TSource> union, Expression<Func<TSource, TKey>> keySelector) => ActiveUnion(source, union, keySelector.Compile(), keySelector.GetReferencedProperties());

		public static IActiveList<TSource> ActiveUnion<TSource, TKey>(this IActiveList<TSource> source, IActiveList<TSource> union, Func<TSource, TKey> keySelector, IEnumerable<string> propertiesToWatch) => new ActiveUnion<TSource, TKey>(source, union, keySelector, propertiesToWatch);

		public static IActiveList<TSource> ActiveIntersect<TSource>(this IActiveList<TSource> source, IActiveList<TSource> intersect) => new ActiveIntersect<TSource, TSource>(source, intersect, o => o);

		public static IActiveList<TSource> ActiveIntersect<TSource, TKey>(this IActiveList<TSource> source, IActiveList<TSource> intersect, Expression<Func<TSource, TKey>> keySelector) => ActiveIntersect(source, intersect, keySelector.Compile(), keySelector.GetReferencedProperties());

		public static IActiveList<TSource> ActiveIntersect<TSource, TKey>(this IActiveList<TSource> source, IActiveList<TSource> intersect, Func<TSource, TKey> keySelector, IEnumerable<string> propertiesToWatch) => new ActiveIntersect<TSource, TKey>(source, intersect, keySelector, propertiesToWatch);

		public static IActiveList<TSource> ActiveExcept<TSource>(this IActiveList<TSource> source, IActiveList<TSource> except) => new ActiveExcept<TSource, TSource>(source, except, o => o);

		public static IActiveList<TSource> ActiveExcept<TSource, TKey>(this IActiveList<TSource> source, IActiveList<TSource> except, Expression<Func<TSource, TKey>> keySelector) => ActiveExcept(source, except, keySelector.Compile(), keySelector.GetReferencedProperties());

		public static IActiveList<TSource> ActiveExcept<TSource, TKey>(this IActiveList<TSource> source, IActiveList<TSource> except, Func<TSource, TKey> keySelector, IEnumerable<string> propertiesToWatch) => new ActiveExcept<TSource, TKey>(source, except, keySelector, propertiesToWatch);

		public static IActiveList<TSource> ActiveOrderBy<TSource, TKey>(this IActiveList<TSource> source, Expression<Func<TSource, TKey>> keySelector) where TKey : IComparable<TKey> => ActiveOrderBy(source, keySelector.Compile(), keySelector.GetReferencedProperties());

		public static IActiveList<TSource> ActiveOrderBy<TSource, TKey>(this IActiveList<TSource> source, Func<TSource, TKey> keySelector, IEnumerable<string> propertiesToWatch) where TKey : IComparable<TKey> => new ActiveOrderBy<TSource, TKey>(source, keySelector, false, propertiesToWatch);

		public static IActiveList<TSource> ActiveOrderByDescending<TSource, TKey>(this IActiveList<TSource> source, Expression<Func<TSource, TKey>> keySelector) where TKey : IComparable<TKey> => ActiveOrderByDescending(source, keySelector.Compile(), keySelector.GetReferencedProperties());

		public static IActiveList<TSource> ActiveOrderByDescending<TSource, TKey>(this IActiveList<TSource> source, Func<TSource, TKey> keySelector, IEnumerable<string> propertiesToWatch) where TKey : IComparable<TKey> => new ActiveOrderBy<TSource, TKey>(source, keySelector, true, propertiesToWatch);

		public static IActiveList<TResult> ActiveZip<TFirst, TSecond, TResult>(this IActiveList<TFirst> source, IEnumerable<TSecond> otherSource, Expression<Func<TFirst, TSecond, TResult>> resultSelector)
		{
			var properties = resultSelector.GetReferencedProperties();
			return ActiveZip(source, otherSource, resultSelector.Compile(), properties.Item1, properties.Item2);
		}

        public static IActiveList<TResult> ActiveZip<TFirst, TSecond, TResult>(this IActiveList<TFirst> source, IEnumerable<TSecond> otherSource, Func<TFirst, TSecond, TResult> resultSelector, IEnumerable<string> sourcePropertiesToWatch, IEnumerable<string> otherSourcePropertiesToWatch) => new ActiveZip<TFirst, TSecond, TResult>(source, otherSource, resultSelector, sourcePropertiesToWatch, otherSourcePropertiesToWatch);

		public static IActiveList<IActiveGrouping<TKey, TSource>> ActiveGroupBy<TSource, TKey>(this IActiveList<TSource> source, Expression<Func<TSource, TKey>> keySelector) => ToActiveLookup(source, keySelector.Compile(), keySelector.GetReferencedProperties());

		public static IActiveList<IActiveGrouping<TKey, TSource>> ActiveGroupBy<TSource, TKey>(this IActiveList<TSource> source, Func<TSource, TKey> keySelector, IEnumerable<string> propertiesToWatch) => ToActiveLookup(source, keySelector, propertiesToWatch);

		public static IActiveLookup<TKey, TSource> ToActiveLookup<TSource, TKey>(this IActiveList<TSource> source, Expression<Func<TSource, TKey>> keySelector) => ToActiveLookup(source, keySelector.Compile(), keySelector.GetReferencedProperties());

		public static IActiveLookup<TKey, TSource> ToActiveLookup<TSource, TKey>(this IActiveList<TSource> source, Func<TSource, TKey> keySelector, IEnumerable<string> propertiesToWatch) => new ActiveLookup<TSource, TKey>(source, keySelector, propertiesToWatch);
	}
}
