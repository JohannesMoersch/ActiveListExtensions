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
		private static IEnumerable<string> GetReferencedProperties<T, U>(Expression<Func<T, U>> expression) => typeof(INotifyPropertyChanged).IsAssignableFrom(typeof(T)) ? expression.GetReferencedProperties() : Enumerable.Empty<string>();

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
			return new ActiveList<T>(source);
		}

		public static IActiveList<T> ActiveWhere<T>(this IActiveList<T> source, Expression<Func<T, bool>> predicate) => ActiveWhere(source, predicate.Compile(), GetReferencedProperties(predicate));

		public static IActiveList<T> ActiveWhere<T>(this IActiveList<T> source, Func<T, bool> predicate, IEnumerable<string> propertiesToWatch) => new ActiveWhere<T>(source, predicate, propertiesToWatch);

		public static IActiveList<U> ActiveSelect<T, U>(this IActiveList<T> source, Expression<Func<T, U>> selector) => ActiveSelect(source, selector.Compile(), GetReferencedProperties(selector));

		public static IActiveList<U> ActiveSelect<T, U>(this IActiveList<T> source, Func<T, U> selector, IEnumerable<string> propertiesToWatch) => new ActiveSelect<T, U>(source, selector, propertiesToWatch);

		public static IActiveList<U> ActiveSelectMany<T, U>(this IActiveList<T> source, Expression<Func<T, IEnumerable<U>>> selector) => ActiveSelectMany(source, selector.Compile(), GetReferencedProperties(selector));

		public static IActiveList<U> ActiveSelectMany<T, U>(this IActiveList<T> source, Func<T, IEnumerable<U>> selector, IEnumerable<string> propertiesToWatch) => new ActiveSelectMany<T, U>(source, selector, propertiesToWatch);

		public static IActiveList<T> ActiveTake<T>(this IActiveList<T> source, int count) => new ActiveTake<T>(source, count);

		public static IActiveList<T> ActiveSkip<T>(this IActiveList<T> source, int count) => new ActiveSkip<T>(source, count);

		public static IActiveList<T> ActiveConcat<T>(this IActiveList<T> source, IEnumerable<T> concat) => new ActiveConcat<T>(source, concat);

		public static IActiveList<T> ActiveReverse<T>(this IActiveList<T> source) => new ActiveReverse<T>(source);

		public static IActiveList<T> ActiveDistinct<T>(this IActiveList<T> source) => new ActiveDistinct<T, T>(source, o => o);

		public static IActiveList<T> ActiveDistinct<T, U>(this IActiveList<T> source, Expression<Func<T, U>> keySelector) => ActiveDistinct(source, keySelector.Compile(), GetReferencedProperties(keySelector));

		public static IActiveList<T> ActiveDistinct<T, U>(this IActiveList<T> source, Func<T, U> keySelector, IEnumerable<string> propertiesToWatch) => new ActiveDistinct<T, U>(source, keySelector, propertiesToWatch);

		public static IActiveList<T> ActiveUnion<T>(this IActiveList<T> source, IActiveList<T> union) => new ActiveUnion<T, T>(source, union, o => o);

		public static IActiveList<T> ActiveUnion<T, U>(this IActiveList<T> source, IActiveList<T> union, Expression<Func<T, U>> keySelector) => ActiveUnion(source, union, keySelector.Compile(), GetReferencedProperties(keySelector));

		public static IActiveList<T> ActiveUnion<T, U>(this IActiveList<T> source, IActiveList<T> union, Func<T, U> keySelector, IEnumerable<string> propertiesToWatch) => new ActiveUnion<T, U>(source, union, keySelector, propertiesToWatch);

		public static IActiveList<T> ActiveIntersect<T>(this IActiveList<T> source, IActiveList<T> intersect) => new ActiveIntersect<T, T>(source, intersect, o => o);

		public static IActiveList<T> ActiveIntersect<T, U>(this IActiveList<T> source, IActiveList<T> intersect, Expression<Func<T, U>> keySelector) => ActiveIntersect(source, intersect, keySelector.Compile(), GetReferencedProperties(keySelector));

		public static IActiveList<T> ActiveIntersect<T, U>(this IActiveList<T> source, IActiveList<T> intersect, Func<T, U> keySelector, IEnumerable<string> propertiesToWatch) => new ActiveIntersect<T, U>(source, intersect, keySelector, propertiesToWatch);

		public static IActiveList<T> ActiveExcept<T>(this IActiveList<T> source, IActiveList<T> except) => new ActiveExcept<T, T>(source, except, o => o);

		public static IActiveList<T> ActiveExcept<T, U>(this IActiveList<T> source, IActiveList<T> except, Expression<Func<T, U>> keySelector) => ActiveExcept(source, except, keySelector.Compile(), GetReferencedProperties(keySelector));

		public static IActiveList<T> ActiveExcept<T, U>(this IActiveList<T> source, IActiveList<T> except, Func<T, U> keySelector, IEnumerable<string> propertiesToWatch) => new ActiveExcept<T, U>(source, except, keySelector, propertiesToWatch);

		// --Where
		// --Select
		// --SelectMany
		// --Take
		// --Skip
		// OrderBy
		// OrderByDescending
		// GroupBy
		// --Concat
		// Zip
		// --Distinct
		// --Union
		// --Intersect
		// --Except
		// --Reverse

		// ToLookup (Not an IActiveList)

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
