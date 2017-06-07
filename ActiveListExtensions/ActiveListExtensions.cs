using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;
using ActiveListExtensions.ListModifiers;
using ActiveListExtensions.Utilities;

namespace ActiveListExtensions
{
	[EditorBrowsable(EditorBrowsableState.Never)]
	public static class ActiveListExtensions
	{
		public static IActiveList<T> ToActiveList<T>(this IEnumerable<T> source)
		{
			if (source == null)
				throw new ArgumentNullException(nameof(source));
			if (source is IActiveList<T> list)
				return list;
			var readonlyList = source.ToReadOnlyList();
			return new ActiveListWrapper<T>(new ActiveValueWrapper<IReadOnlyList<T>>(readonlyList));
		}

		public static IActiveList<T> ToActiveList<T>(this IActiveValue<IEnumerable<T>> source)
		{
			if (source == null)
				throw new ArgumentNullException(nameof(source));
			var readonlyListValue = source as IActiveValue<IReadOnlyList<T>> ?? source.ActiveMutate(l => l?.ToReadOnlyList() ?? new T[0], null);
			return new ActiveListWrapper<T>(readonlyListValue);
		}


		public static IActiveList<TResult> ActiveOfType<TResult>(this IActiveList<object> source) => new ActiveOfType<TResult>(source);


		public static IActiveList<TSource> ActiveWhere<TSource>(this IActiveList<TSource> source, Expression<Func<TSource, bool>> predicate) => ActiveWhere(source, predicate.Compile(), predicate.GetReferencedProperties());

		public static IActiveList<TSource> ActiveWhere<TSource>(this IActiveList<TSource> source, Func<TSource, bool> predicate, IEnumerable<string> propertiesToWatch) => new ActiveWhere<TSource, object>(source, predicate, propertiesToWatch);

		public static IActiveList<TSource> ActiveWhere<TSource, TParameter>(this IActiveList<TSource> source, IActiveValue<TParameter> parameter, Expression<Func<TSource, TParameter, bool>> predicate) => ActiveWhere(source, parameter, predicate.Compile(), predicate.GetReferencedProperties());

		public static IActiveList<TSource> ActiveWhere<TSource, TParameter>(this IActiveList<TSource> source, IActiveValue<TParameter> parameter, Func<TSource, TParameter, bool> predicate, IEnumerable<string> sourcePropertiesToWatch, IEnumerable<string> parameterPropertiesToWatch) => ActiveWhere(source, parameter, predicate, Tuple.Create(sourcePropertiesToWatch, parameterPropertiesToWatch));

		private static IActiveList<TSource> ActiveWhere<TSource, TParameter>(this IActiveList<TSource> source, IActiveValue<TParameter> parameter, Func<TSource, TParameter, bool> predicate, Tuple<IEnumerable<string>, IEnumerable<string>> propertiesToWatch) => new ActiveWhere<TSource, TParameter>(source, predicate, parameter, propertiesToWatch.Item1, propertiesToWatch.Item2);


		public static IActiveList<TSource> ActiveWhere<TSource>(this IActiveList<TSource> source, Expression<Func<TSource, IActiveValue<bool>>> predicate) => ActiveWhere(source, predicate.Compile(), predicate.GetReferencedProperties());

		public static IActiveList<TSource> ActiveWhere<TSource>(this IActiveList<TSource> source, Func<TSource, IActiveValue<bool>> predicate, IEnumerable<string> propertiesToWatch) => new ActiveWhereValue<TSource, object>(source, predicate, propertiesToWatch);

		public static IActiveList<TSource> ActiveWhere<TSource, TParameter>(this IActiveList<TSource> source, IActiveValue<TParameter> parameter, Expression<Func<TSource, TParameter, IActiveValue<bool>>> predicate) => ActiveWhere(source, parameter, predicate.Compile(), predicate.GetReferencedProperties());

		public static IActiveList<TSource> ActiveWhere<TSource, TParameter>(this IActiveList<TSource> source, IActiveValue<TParameter> parameter, Func<TSource, TParameter, IActiveValue<bool>> predicate, IEnumerable<string> sourcePropertiesToWatch, IEnumerable<string> parameterPropertiesToWatch) => ActiveWhere(source, parameter, predicate, Tuple.Create(sourcePropertiesToWatch, parameterPropertiesToWatch));

		private static IActiveList<TSource> ActiveWhere<TSource, TParameter>(this IActiveList<TSource> source, IActiveValue<TParameter> parameter, Func<TSource, TParameter, IActiveValue<bool>> predicate, Tuple<IEnumerable<string>, IEnumerable<string>> propertiesToWatch) => new ActiveWhereValue<TSource, TParameter>(source, predicate, parameter, propertiesToWatch.Item1, propertiesToWatch.Item2);


		public static IActiveList<TResult> ActiveSelect<TSource, TResult>(this IActiveList<TSource> source, Expression<Func<TSource, TResult>> selector) => ActiveSelect(source, selector.Compile(), selector.GetReferencedProperties());

		public static IActiveList<TResult> ActiveSelect<TSource, TResult>(this IActiveList<TSource> source, Func<TSource, TResult> selector, IEnumerable<string> propertiesToWatch) => new ActiveSelect<TSource, object, TResult>(source, selector, propertiesToWatch);

		public static IActiveList<TResult> ActiveSelect<TSource, TParameter, TResult>(this IActiveList<TSource> source, IActiveValue<TParameter> parameter, Expression<Func<TSource, TParameter, TResult>> selector) => ActiveSelect(source, parameter, selector.Compile(), selector.GetReferencedProperties());

		public static IActiveList<TResult> ActiveSelect<TSource, TParameter, TResult>(this IActiveList<TSource> source, IActiveValue<TParameter> parameter, Func<TSource, TParameter, TResult> selector, IEnumerable<string> sourcePropertiesToWatch, IEnumerable<string> parameterPropertiesToWatch) => ActiveSelect(source, parameter, selector, Tuple.Create(sourcePropertiesToWatch, parameterPropertiesToWatch));

		private static IActiveList<TResult> ActiveSelect<TSource, TParameter, TResult>(this IActiveList<TSource> source, IActiveValue<TParameter> parameter, Func<TSource, TParameter, TResult> selector, Tuple<IEnumerable<string>, IEnumerable<string>> propertiesToWatch) => new ActiveSelect<TSource, TParameter, TResult>(source, selector, parameter, propertiesToWatch.Item1, propertiesToWatch.Item2);


		public static IActiveList<TResult> ActiveSelect<TSource, TResult>(this IActiveList<TSource> source, Expression<Func<TSource, IActiveValue<TResult>>> selector) => ActiveSelect(source, selector.Compile(), selector.GetReferencedProperties());

		public static IActiveList<TResult> ActiveSelect<TSource, TResult>(this IActiveList<TSource> source, Func<TSource, IActiveValue<TResult>> selector, IEnumerable<string> propertiesToWatch) => new ActiveSelectValue<TSource, object, TResult>(source, selector, propertiesToWatch);

		public static IActiveList<TResult> ActiveSelect<TSource, TParameter, TResult>(this IActiveList<TSource> source, IActiveValue<TParameter> parameter, Expression<Func<TSource, TParameter, IActiveValue<TResult>>> selector) => ActiveSelect(source, parameter, selector.Compile(), selector.GetReferencedProperties());

		public static IActiveList<TResult> ActiveSelect<TSource, TParameter, TResult>(this IActiveList<TSource> source, IActiveValue<TParameter> parameter, Func<TSource, TParameter, IActiveValue<TResult>> selector, IEnumerable<string> sourcePropertiesToWatch, IEnumerable<string> parameterPropertiesToWatch) => ActiveSelect(source, parameter, selector, Tuple.Create(sourcePropertiesToWatch, parameterPropertiesToWatch));

		private static IActiveList<TResult> ActiveSelect<TSource, TParameter, TResult>(this IActiveList<TSource> source, IActiveValue<TParameter> parameter, Func<TSource, TParameter, IActiveValue<TResult>> selector, Tuple<IEnumerable<string>, IEnumerable<string>> propertiesToWatch) => new ActiveSelectValue<TSource, TParameter, TResult>(source, selector, parameter, propertiesToWatch.Item1, propertiesToWatch.Item2);


		public static IActiveList<TResult> ActiveSelectMany<TSource, TResult>(this IActiveList<TSource> source, Expression<Func<TSource, IEnumerable<TResult>>> selector) => ActiveSelectMany(source, selector.Compile(), selector.GetReferencedProperties());

		public static IActiveList<TResult> ActiveSelectMany<TSource, TResult>(this IActiveList<TSource> source, Func<TSource, IEnumerable<TResult>> selector, IEnumerable<string> propertiesToWatch) => new ActiveSelectMany<TSource, object, TResult>(source, selector, propertiesToWatch);

		public static IActiveList<TResult> ActiveSelectMany<TSource, TParameter, TResult>(this IActiveList<TSource> source, IActiveValue<TParameter> parameter, Expression<Func<TSource, TParameter, IEnumerable<TResult>>> selector) => ActiveSelectMany(source, parameter, selector.Compile(), selector.GetReferencedProperties());

		public static IActiveList<TResult> ActiveSelectMany<TSource, TParameter, TResult>(this IActiveList<TSource> source, IActiveValue<TParameter> parameter, Func<TSource, TParameter, IEnumerable<TResult>> selector, IEnumerable<string> sourcePropertiesToWatch, IEnumerable<string> parameterPropertiesToWatch) => ActiveSelectMany(source, parameter, selector, Tuple.Create(sourcePropertiesToWatch, parameterPropertiesToWatch));

		private static IActiveList<TResult> ActiveSelectMany<TSource, TParameter, TResult>(this IActiveList<TSource> source, IActiveValue<TParameter> parameter, Func<TSource, TParameter, IEnumerable<TResult>> selector, Tuple<IEnumerable<string>, IEnumerable<string>> propertiesToWatch) => new ActiveSelectMany<TSource, TParameter, TResult>(source, selector, parameter, propertiesToWatch.Item1, propertiesToWatch.Item2);


		public static IActiveList<TSource> ActiveElementsAtOrEmpty<TSource>(this IActiveList<TSource> source, params int[] indexes) => new ActiveElementsOrEmpty<TSource>(source, indexes);

		public static IActiveList<TSource> ActiveElementsAtOrEmpty<TSource>(this IActiveList<TSource> source, IEnumerable<int> indexes) => new ActiveElementsOrEmpty<TSource>(source, indexes.ToReadOnlyList());


		public static IActiveList<TSource> ActiveTake<TSource>(this IActiveList<TSource> source, int count) => new ActiveTake<TSource>(source, new ActiveValueWrapper<int>(count));

		public static IActiveList<TSource> ActiveTake<TSource>(this IActiveList<TSource> source, IActiveValue<int> count) => new ActiveTake<TSource>(source, count);


		public static IActiveList<TSource> ActiveSkip<TSource>(this IActiveList<TSource> source, int count) => new ActiveSkip<TSource>(source, new ActiveValueWrapper<int>(count));

		public static IActiveList<TSource> ActiveSkip<TSource>(this IActiveList<TSource> source, IActiveValue<int> count) => new ActiveSkip<TSource>(source, count);


		public static IActiveList<TSource> ActiveConcat<TSource>(this IActiveList<TSource> source, IEnumerable<TSource> concat) => new ActiveConcat<TSource>(source, concat);


		public static IActiveList<TSource> ActiveReverse<TSource>(this IActiveList<TSource> source) => new ActiveReverse<TSource>(source);


		public static IActiveList<TSource> ActiveDistinct<TSource>(this IActiveList<TSource> source) => new ActiveDistinct<TSource, TSource, object>(source, o => o, null);

		public static IActiveList<TSource> ActiveDistinct<TSource, TKey>(this IActiveList<TSource> source, Expression<Func<TSource, TKey>> keySelector) => ActiveDistinct(source, keySelector.Compile(), keySelector.GetReferencedProperties());

		public static IActiveList<TSource> ActiveDistinct<TSource, TKey>(this IActiveList<TSource> source, Func<TSource, TKey> keySelector, IEnumerable<string> propertiesToWatch) => new ActiveDistinct<TKey, TSource, object>(source, keySelector, propertiesToWatch);

		public static IActiveList<TSource> ActiveDistinct<TSource, TKey, TParameter>(this IActiveList<TSource> source, IActiveValue<TParameter> parameter, Expression<Func<TSource, TParameter, TKey>> keySelector) => ActiveDistinct(source, parameter, keySelector.Compile(), keySelector.GetReferencedProperties());

		public static IActiveList<TSource> ActiveDistinct<TSource, TKey, TParameter>(this IActiveList<TSource> source, IActiveValue<TParameter> parameter, Func<TSource, TParameter, TKey> keySelector, IEnumerable<string> sourcePropertiesToWatch, IEnumerable<string> parameterPropertiesToWatch) => ActiveDistinct(source, parameter, keySelector, Tuple.Create(sourcePropertiesToWatch, parameterPropertiesToWatch));

		private static IActiveList<TSource> ActiveDistinct<TSource, TKey, TParameter>(this IActiveList<TSource> source, IActiveValue<TParameter> parameter, Func<TSource, TParameter, TKey> keySelector, Tuple<IEnumerable<string>, IEnumerable<string>> propertiesToWatch) => new ActiveDistinct<TKey, TSource, TParameter>(source, keySelector, parameter, propertiesToWatch.Item1, propertiesToWatch.Item2);


		public static IActiveList<TSource> ActiveUnion<TSource>(this IActiveList<TSource> source, IEnumerable<TSource> union) => new ActiveUnion<TSource, TSource, object>(source, union.ToReadOnlyList(), o => o, null);

		public static IActiveList<TSource> ActiveUnion<TSource, TKey>(this IActiveList<TSource> source, IEnumerable<TSource> union, Expression<Func<TSource, TKey>> keySelector) => ActiveUnion(source, union, keySelector.Compile(), keySelector.GetReferencedProperties());

		public static IActiveList<TSource> ActiveUnion<TSource, TKey>(this IActiveList<TSource> source, IEnumerable<TSource> union, Func<TSource, TKey> keySelector, IEnumerable<string> propertiesToWatch) => new ActiveUnion<TKey, TSource, object>(source, union.ToReadOnlyList(), keySelector, propertiesToWatch);

		public static IActiveList<TSource> ActiveUnion<TSource, TKey, TParameter>(this IActiveList<TSource> source, IEnumerable<TSource> union, IActiveValue<TParameter> parameter, Expression<Func<TSource, TParameter, TKey>> keySelector) => ActiveUnion(source, union, parameter, keySelector.Compile(), keySelector.GetReferencedProperties());

		public static IActiveList<TSource> ActiveUnion<TSource, TKey, TParameter>(this IActiveList<TSource> source, IEnumerable<TSource> union, IActiveValue<TParameter> parameter, Func<TSource, TParameter, TKey> keySelector, IEnumerable<string> sourcePropertiesToWatch, IEnumerable<string> parameterPropertiesToWatch) => ActiveUnion(source, union, parameter, keySelector, Tuple.Create(sourcePropertiesToWatch, parameterPropertiesToWatch));

		private static IActiveList<TSource> ActiveUnion<TSource, TKey, TParameter>(this IActiveList<TSource> source, IEnumerable<TSource> union, IActiveValue<TParameter> parameter, Func<TSource, TParameter, TKey> keySelector, Tuple<IEnumerable<string>, IEnumerable<string>> propertiesToWatch) => new ActiveUnion<TKey, TSource, TParameter>(source, union.ToReadOnlyList(), keySelector, parameter, propertiesToWatch.Item1, propertiesToWatch.Item2);


		public static IActiveList<TSource> ActiveIntersect<TSource>(this IActiveList<TSource> source, IEnumerable<TSource> intersect) => new ActiveIntersect<TSource, TSource, object>(source, intersect.ToReadOnlyList(), o => o, null);

		public static IActiveList<TSource> ActiveIntersect<TSource, TKey>(this IActiveList<TSource> source, IEnumerable<TSource> intersect, Expression<Func<TSource, TKey>> keySelector) => ActiveIntersect(source, intersect, keySelector.Compile(), keySelector.GetReferencedProperties());

		public static IActiveList<TSource> ActiveIntersect<TSource, TKey>(this IActiveList<TSource> source, IEnumerable<TSource> intersect, Func<TSource, TKey> keySelector, IEnumerable<string> propertiesToWatch) => new ActiveIntersect<TKey, TSource, object>(source, intersect.ToReadOnlyList(), keySelector, propertiesToWatch);

		public static IActiveList<TSource> ActiveIntersect<TSource, TKey, TParameter>(this IActiveList<TSource> source, IEnumerable<TSource> intersect, IActiveValue<TParameter> parameter, Expression<Func<TSource, TParameter, TKey>> keySelector) => ActiveIntersect(source, intersect, parameter, keySelector.Compile(), keySelector.GetReferencedProperties());

		public static IActiveList<TSource> ActiveIntersect<TSource, TKey, TParameter>(this IActiveList<TSource> source, IEnumerable<TSource> intersect, IActiveValue<TParameter> parameter, Func<TSource, TParameter, TKey> keySelector, IEnumerable<string> sourcePropertiesToWatch, IEnumerable<string> parameterPropertiesToWatch) => ActiveIntersect(source, intersect, parameter, keySelector, Tuple.Create(sourcePropertiesToWatch, parameterPropertiesToWatch));

		private static IActiveList<TSource> ActiveIntersect<TSource, TKey, TParameter>(this IActiveList<TSource> source, IEnumerable<TSource> intersect, IActiveValue<TParameter> parameter, Func<TSource, TParameter, TKey> keySelector, Tuple<IEnumerable<string>, IEnumerable<string>> propertiesToWatch) => new ActiveIntersect<TKey, TSource, TParameter>(source, intersect.ToReadOnlyList(), keySelector, parameter, propertiesToWatch.Item1, propertiesToWatch.Item2);


		public static IActiveList<TSource> ActiveExcept<TSource>(this IActiveList<TSource> source, IEnumerable<TSource> except) => new ActiveExcept<TSource, TSource, object>(source, except.ToReadOnlyList(), o => o, null);

		public static IActiveList<TSource> ActiveExcept<TSource, TKey>(this IActiveList<TSource> source, IEnumerable<TSource> except, Expression<Func<TSource, TKey>> keySelector) => ActiveExcept(source, except, keySelector.Compile(), keySelector.GetReferencedProperties());

		public static IActiveList<TSource> ActiveExcept<TSource, TKey>(this IActiveList<TSource> source, IEnumerable<TSource> except, Func<TSource, TKey> keySelector, IEnumerable<string> propertiesToWatch) => new ActiveExcept<TKey, TSource, object>(source, except.ToReadOnlyList(), keySelector, propertiesToWatch);

		public static IActiveList<TSource> ActiveExcept<TSource, TKey, TParameter>(this IActiveList<TSource> source, IEnumerable<TSource> except, IActiveValue<TParameter> parameter, Expression<Func<TSource, TParameter, TKey>> keySelector) => ActiveExcept(source, except, parameter, keySelector.Compile(), keySelector.GetReferencedProperties());

		public static IActiveList<TSource> ActiveExcept<TSource, TKey, TParameter>(this IActiveList<TSource> source, IEnumerable<TSource> except, IActiveValue<TParameter> parameter, Func<TSource, TParameter, TKey> keySelector, IEnumerable<string> sourcePropertiesToWatch, IEnumerable<string> parameterPropertiesToWatch) => ActiveExcept(source, except, parameter, keySelector, Tuple.Create(sourcePropertiesToWatch, parameterPropertiesToWatch));

		private static IActiveList<TSource> ActiveExcept<TSource, TKey, TParameter>(this IActiveList<TSource> source, IEnumerable<TSource> except, IActiveValue<TParameter> parameter, Func<TSource, TParameter, TKey> keySelector, Tuple<IEnumerable<string>, IEnumerable<string>> propertiesToWatch) => new ActiveExcept<TKey, TSource, TParameter>(source, except.ToReadOnlyList(), keySelector, parameter, propertiesToWatch.Item1, propertiesToWatch.Item2);


		public static IActiveList<TSource> ActiveOrderBy<TSource, TKey>(this IActiveList<TSource> source, Expression<Func<TSource, TKey>> keySelector, ListSortDirection sortDirection = ListSortDirection.Ascending) where TKey : IComparable<TKey> => ActiveOrderBy(source, keySelector.Compile(), keySelector.GetReferencedProperties(), new ActiveValueWrapper<ListSortDirection>(sortDirection));

		public static IActiveList<TSource> ActiveOrderBy<TSource, TKey>(this IActiveList<TSource> source, Func<TSource, TKey> keySelector, IEnumerable<string> propertiesToWatch, ListSortDirection sortDirection = ListSortDirection.Ascending) where TKey : IComparable<TKey> => ActiveOrderBy(source, keySelector, propertiesToWatch, new ActiveValueWrapper<ListSortDirection>(sortDirection));

		public static IActiveList<TSource> ActiveOrderBy<TSource, TKey>(this IActiveList<TSource> source, Expression<Func<TSource, TKey>> keySelector, IActiveValue<ListSortDirection> sortDirection) where TKey : IComparable<TKey> => ActiveOrderBy(source, keySelector.Compile(), keySelector.GetReferencedProperties(), sortDirection);

		public static IActiveList<TSource> ActiveOrderBy<TSource, TKey>(this IActiveList<TSource> source, Func<TSource, TKey> keySelector, IEnumerable<string> propertiesToWatch, IActiveValue<ListSortDirection> sortDirection) where TKey : IComparable<TKey> => new ActiveOrderBy<TSource, TKey>(source, keySelector, sortDirection, propertiesToWatch);


		public static IActiveList<TResult> ActiveZip<TFirst, TSecond, TResult>(this IActiveList<TFirst> source, IEnumerable<TSecond> otherSource, Expression<Func<TFirst, TSecond, TResult>> resultSelector) => ActiveZip(source, otherSource, resultSelector.Compile(), resultSelector.GetReferencedProperties());

		public static IActiveList<TResult> ActiveZip<TFirst, TSecond, TResult>(this IActiveList<TFirst> source, IEnumerable<TSecond> otherSource, Func<TFirst, TSecond, TResult> resultSelector, IEnumerable<string> sourcePropertiesToWatch, IEnumerable<string> otherSourcePropertiesToWatch) => ActiveZip(source, otherSource, resultSelector, Tuple.Create(sourcePropertiesToWatch, otherSourcePropertiesToWatch));

		private static IActiveList<TResult> ActiveZip<TFirst, TSecond, TResult>(this IActiveList<TFirst> source, IEnumerable<TSecond> otherSource, Func<TFirst, TSecond, TResult> resultSelector, Tuple<IEnumerable<string>, IEnumerable<string>> propertiesToWatch) => new ActiveZip<TFirst, TSecond, object, TResult>(source, otherSource.ToReadOnlyList(), resultSelector, propertiesToWatch.Item1, propertiesToWatch.Item2);

		public static IActiveList<TResult> ActiveZip<TFirst, TSecond, TParameter, TResult>(this IActiveList<TFirst> source, IEnumerable<TSecond> otherSource, IActiveValue<TParameter> parameter, Expression<Func<TFirst, TSecond, TParameter, TResult>> resultSelector) => ActiveZip(source, otherSource, parameter, resultSelector.Compile(), resultSelector.GetReferencedProperties());

		public static IActiveList<TResult> ActiveZip<TFirst, TSecond, TParameter, TResult>(this IActiveList<TFirst> source, IEnumerable<TSecond> otherSource, IActiveValue<TParameter> parameter, Func<TFirst, TSecond, TParameter, TResult> resultSelector, IEnumerable<string> sourcePropertiesToWatch, IEnumerable<string> otherSourcePropertiesToWatch, IEnumerable<string> parameterPropertiesToWatch) => ActiveZip(source, otherSource, parameter, resultSelector, Tuple.Create(sourcePropertiesToWatch, otherSourcePropertiesToWatch, parameterPropertiesToWatch));

		private static IActiveList<TResult> ActiveZip<TFirst, TSecond, TParameter, TResult>(this IActiveList<TFirst> source, IEnumerable<TSecond> otherSource, IActiveValue<TParameter> parameter, Func<TFirst, TSecond, TParameter, TResult> resultSelector, Tuple<IEnumerable<string>, IEnumerable<string>, IEnumerable<string>> propertiesToWatch) => new ActiveZip<TFirst, TSecond, TParameter, TResult>(source, otherSource.ToReadOnlyList(), resultSelector, parameter, propertiesToWatch.Item1, propertiesToWatch.Item2, propertiesToWatch.Item3);


		public static IActiveList<IActiveGrouping<TKey, TSource>> ActiveGroupBy<TKey, TSource>(this IActiveList<TSource> source, Expression<Func<TSource, TKey>> keySelector) => ToActiveLookup(source, keySelector.Compile(), keySelector.GetReferencedProperties());

		public static IActiveList<IActiveGrouping<TKey, TSource>> ActiveGroupBy<TKey, TSource>(this IActiveList<TSource> source, Func<TSource, TKey> keySelector, IEnumerable<string> propertiesToWatch) => ToActiveLookup(source, keySelector, propertiesToWatch);

		public static IActiveList<IActiveGrouping<TKey, TSource>> ActiveGroupBy<TKey, TSource, TParameter>(this IActiveList<TSource> source, IActiveValue<TParameter> parameter, Expression<Func<TSource, TParameter, TKey>> keySelector) => ToActiveLookup(source, parameter, keySelector.Compile(), keySelector.GetReferencedProperties());

		public static IActiveList<IActiveGrouping<TKey, TSource>> ActiveGroupBy<TKey, TSource, TParameter>(this IActiveList<TSource> source, IActiveValue<TParameter> parameter, Func<TSource, TParameter, TKey> keySelector, IEnumerable<string> sourcePropertiesToWatch, IEnumerable<string> parameterPropertiesToWatch) => ToActiveLookup(source, parameter, keySelector, sourcePropertiesToWatch, parameterPropertiesToWatch);


		public static IActiveLookup<TKey, TSource> ToActiveLookup<TKey, TSource>(this IActiveList<TSource> source, Expression<Func<TSource, TKey>> keySelector) => ToActiveLookup(source, keySelector.Compile(), keySelector.GetReferencedProperties());

		public static IActiveLookup<TKey, TSource> ToActiveLookup<TKey, TSource>(this IActiveList<TSource> source, Func<TSource, TKey> keySelector, IEnumerable<string> propertiesToWatch) => new ActiveLookup<TKey, TSource, object>(source, keySelector, propertiesToWatch);

		public static IActiveLookup<TKey, TSource> ToActiveLookup<TKey, TSource, TParameter>(this IActiveList<TSource> source, IActiveValue<TParameter> parameter, Expression<Func<TSource, TParameter, TKey>> keySelector) => ToActiveLookup(source, parameter, keySelector.Compile(), keySelector.GetReferencedProperties());

		public static IActiveLookup<TKey, TSource> ToActiveLookup<TKey, TSource, TParameter>(this IActiveList<TSource> source, IActiveValue<TParameter> parameter, Func<TSource, TParameter, TKey> keySelector, IEnumerable<string> sourcePropertiesToWatch, IEnumerable<string> parameterPropertiesToWatch) => ToActiveLookup(source, parameter, keySelector, Tuple.Create(sourcePropertiesToWatch, parameterPropertiesToWatch));

		private static IActiveLookup<TKey, TSource> ToActiveLookup<TKey, TSource, TParameter>(this IActiveList<TSource> source, IActiveValue<TParameter> parameter, Func<TSource, TParameter, TKey> keySelector, Tuple<IEnumerable<string>, IEnumerable<string>> propertiesToWatch) => new ActiveLookup<TKey, TSource, TParameter>(source, keySelector, parameter, propertiesToWatch.Item1, propertiesToWatch.Item2);


		public static IActiveList<TSource> ActiveTranslateResetNotifications<TSource>(this IActiveList<TSource> source) => new ActiveTranslateResetNotifications<TSource>(source);
	}
}