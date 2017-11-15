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
using System.Collections;
using ActiveListExtensions.ValueModifiers;

namespace ActiveListExtensions
{
	[EditorBrowsable(EditorBrowsableState.Never)]
	public static class ActiveListExtensions
	{
		public static IActiveList<object> ToActiveList(this IEnumerable source)
		{
			if (source == null)
				throw new ArgumentNullException(nameof(source));
			if (source is IActiveList<object> list)
				return list;
			var readonlyList = source.ToReadOnlyList();
			return new ActiveListWrapper<object>(new ActiveValueWrapper<IReadOnlyList<object>>(readonlyList));
		}

		public static IActiveList<T> ToActiveList<T>(this IEnumerable<T> source)
		{
			if (source == null)
				throw new ArgumentNullException(nameof(source));
			if (source is IActiveList<T> list)
				return list;
			var readonlyList = source.ToReadOnlyList();
			return new ActiveListWrapper<T>(new ActiveValueWrapper<IReadOnlyList<T>>(readonlyList));
		}

		public static IActiveList<object> ToActiveList(this IActiveValue<IEnumerable> source)
		{
			if (source == null)
				throw new ArgumentNullException(nameof(source));
			var readonlyListValue = source as IActiveValue<IReadOnlyList<object>> ?? source.ActiveSelect(l => l?.ToReadOnlyList() ?? new object[0], null);
			return new ActiveListWrapper<object>(readonlyListValue);
		}

		public static IActiveList<T> ToActiveList<T>(this IActiveValue<IEnumerable<T>> source)
		{
			if (source == null)
				throw new ArgumentNullException(nameof(source));
			var readonlyListValue = source as IActiveValue<IReadOnlyList<T>> ?? source.ActiveSelect(l => l?.ToReadOnlyList() ?? new T[0], null);
			return new ActiveListWrapper<T>(readonlyListValue);
		}


		public static IActiveList<TResult> ActiveOfType<TResult>(this IActiveList<object> source) 
			=> new ActiveOfType<TResult>(source);


		public static IActiveList<TSource> ActiveWhere<TSource>(this IActiveList<TSource> source, Expression<Func<TSource, bool>> predicate) 
			=> ActiveWhere(source, predicate.Compile(), predicate.GetReferencedProperties());

		public static IActiveList<TSource> ActiveWhere<TSource>(this IActiveList<TSource> source, Func<TSource, bool> predicate, IEnumerable<string> propertiesToWatch) 
			=> new ActiveWhere<TSource, object>(source, predicate, propertiesToWatch);

		public static IActiveList<TSource> ActiveWhere<TSource, TParameter>(this IActiveList<TSource> source, IActiveValue<TParameter> parameter, Expression<Func<TSource, TParameter, bool>> predicate) 
			=> ActiveWhere(source, parameter, predicate.Compile(), predicate.GetReferencedProperties());

		public static IActiveList<TSource> ActiveWhere<TSource, TParameter>(this IActiveList<TSource> source, IActiveValue<TParameter> parameter, Func<TSource, TParameter, bool> predicate, IEnumerable<string> sourcePropertiesToWatch, IEnumerable<string> parameterPropertiesToWatch) 
			=> ActiveWhere(source, parameter, predicate, Tuple.Create(sourcePropertiesToWatch, parameterPropertiesToWatch));

		private static IActiveList<TSource> ActiveWhere<TSource, TParameter>(this IActiveList<TSource> source, IActiveValue<TParameter> parameter, Func<TSource, TParameter, bool> predicate, Tuple<IEnumerable<string>, IEnumerable<string>> propertiesToWatch) 
			=> new ActiveWhere<TSource, TParameter>(source, predicate, parameter, propertiesToWatch.Item1, propertiesToWatch.Item2);


		public static IActiveList<TSource> ActiveWhere<TSource>(this IActiveList<TSource> source, Expression<Func<TSource, IActiveValue<bool>>> predicate)
			=> ActiveWhere(source, predicate.Compile(), predicate.GetReferencedProperties());

		public static IActiveList<TSource> ActiveWhere<TSource>(this IActiveList<TSource> source, Func<TSource, IActiveValue<bool>> predicate, IEnumerable<string> propertiesToWatch)
			=> new ActiveWhereValue<TSource, object>(source, predicate, propertiesToWatch);

		public static IActiveList<TSource> ActiveWhere<TSource, TParameter>(this IActiveList<TSource> source, IActiveValue<TParameter> parameter, Expression<Func<TSource, TParameter, IActiveValue<bool>>> predicate)
			=> ActiveWhere(source, parameter, predicate.Compile(), predicate.GetReferencedProperties());

		public static IActiveList<TSource> ActiveWhere<TSource, TParameter>(this IActiveList<TSource> source, IActiveValue<TParameter> parameter, Func<TSource, TParameter, IActiveValue<bool>> predicate, IEnumerable<string> sourcePropertiesToWatch, IEnumerable<string> parameterPropertiesToWatch)
			=> ActiveWhere(source, parameter, predicate, Tuple.Create(sourcePropertiesToWatch, parameterPropertiesToWatch));

		private static IActiveList<TSource> ActiveWhere<TSource, TParameter>(this IActiveList<TSource> source, IActiveValue<TParameter> parameter, Func<TSource, TParameter, IActiveValue<bool>> predicate, Tuple<IEnumerable<string>, IEnumerable<string>> propertiesToWatch)
			=> new ActiveWhereValue<TSource, TParameter>(source, predicate, parameter, propertiesToWatch.Item1, propertiesToWatch.Item2);


		public static IActiveList<TSource> ActiveWhere<TSource>(this IActiveList<TSource> source, Expression<Func<TSource, IMutableActiveValue<bool>>> predicate)
			=> ActiveWhere(source, predicate.Compile(), predicate.GetReferencedProperties());

		public static IActiveList<TSource> ActiveWhere<TSource>(this IActiveList<TSource> source, Func<TSource, IMutableActiveValue<bool>> predicate, IEnumerable<string> propertiesToWatch)
			=> new ActiveWhereValue<TSource, object>(source, predicate, propertiesToWatch);

		public static IActiveList<TSource> ActiveWhere<TSource, TParameter>(this IActiveList<TSource> source, IActiveValue<TParameter> parameter, Expression<Func<TSource, TParameter, IMutableActiveValue<bool>>> predicate)
			=> ActiveWhere(source, parameter, predicate.Compile(), predicate.GetReferencedProperties());

		public static IActiveList<TSource> ActiveWhere<TSource, TParameter>(this IActiveList<TSource> source, IActiveValue<TParameter> parameter, Func<TSource, TParameter, IMutableActiveValue<bool>> predicate, IEnumerable<string> sourcePropertiesToWatch, IEnumerable<string> parameterPropertiesToWatch)
			=> ActiveWhere(source, parameter, predicate, Tuple.Create(sourcePropertiesToWatch, parameterPropertiesToWatch));

		private static IActiveList<TSource> ActiveWhere<TSource, TParameter>(this IActiveList<TSource> source, IActiveValue<TParameter> parameter, Func<TSource, TParameter, IMutableActiveValue<bool>> predicate, Tuple<IEnumerable<string>, IEnumerable<string>> propertiesToWatch)
			=> new ActiveWhereValue<TSource, TParameter>(source, predicate, parameter, propertiesToWatch.Item1, propertiesToWatch.Item2);


		public static IActiveList<TResult> ActiveSelect<TSource, TResult>(this IActiveList<TSource> source, Expression<Func<TSource, TResult>> selector)
			=> ActiveSelect(source, selector.Compile(), selector.GetReferencedProperties());

		public static IActiveList<TResult> ActiveSelect<TSource, TResult>(this IActiveList<TSource> source, Func<TSource, TResult> selector, IEnumerable<string> propertiesToWatch)
			=> new ActiveSelect<TSource, object, TResult>(source, selector, propertiesToWatch);

		public static IActiveList<TResult> ActiveSelect<TSource, TParameter, TResult>(this IActiveList<TSource> source, IActiveValue<TParameter> parameter, Expression<Func<TSource, TParameter, TResult>> selector)
			=> ActiveSelect(source, parameter, selector.Compile(), selector.GetReferencedProperties());

		public static IActiveList<TResult> ActiveSelect<TSource, TParameter, TResult>(this IActiveList<TSource> source, IActiveValue<TParameter> parameter, Func<TSource, TParameter, TResult> selector, IEnumerable<string> sourcePropertiesToWatch, IEnumerable<string> parameterPropertiesToWatch)
			=> ActiveSelect(source, parameter, selector, Tuple.Create(sourcePropertiesToWatch, parameterPropertiesToWatch));

		private static IActiveList<TResult> ActiveSelect<TSource, TParameter, TResult>(this IActiveList<TSource> source, IActiveValue<TParameter> parameter, Func<TSource, TParameter, TResult> selector, Tuple<IEnumerable<string>, IEnumerable<string>> propertiesToWatch)
			=> new ActiveSelect<TSource, TParameter, TResult>(source, selector, parameter, propertiesToWatch.Item1, propertiesToWatch.Item2);


		public static IActiveList<TResult> ActiveSelect<TSource, TResult>(this IActiveList<TSource> source, Expression<Func<TSource, IActiveValue<TResult>>> selector)
			=> ActiveSelect(source, selector.Compile(), selector.GetReferencedProperties());

		public static IActiveList<TResult> ActiveSelect<TSource, TResult>(this IActiveList<TSource> source, Func<TSource, IActiveValue<TResult>> selector, IEnumerable<string> propertiesToWatch)
			=> new ActiveSelect<TSource, object, IActiveValue<TResult>>(source, selector, propertiesToWatch).ActiveSelect(value => value.Value);

		public static IActiveList<TResult> ActiveSelect<TSource, TParameter, TResult>(this IActiveList<TSource> source, IActiveValue<TParameter> parameter, Expression<Func<TSource, TParameter, IActiveValue<TResult>>> selector)
			=> ActiveSelect(source, parameter, selector.Compile(), selector.GetReferencedProperties());

		public static IActiveList<TResult> ActiveSelect<TSource, TParameter, TResult>(this IActiveList<TSource> source, IActiveValue<TParameter> parameter, Func<TSource, TParameter, IActiveValue<TResult>> selector, IEnumerable<string> sourcePropertiesToWatch, IEnumerable<string> parameterPropertiesToWatch)
			=> ActiveSelect(source, parameter, selector, Tuple.Create(sourcePropertiesToWatch, parameterPropertiesToWatch));

		private static IActiveList<TResult> ActiveSelect<TSource, TParameter, TResult>(this IActiveList<TSource> source, IActiveValue<TParameter> parameter, Func<TSource, TParameter, IActiveValue<TResult>> selector, Tuple<IEnumerable<string>, IEnumerable<string>> propertiesToWatch)
			=> new ActiveSelect<TSource, TParameter, IActiveValue<TResult>>(source, selector, parameter, propertiesToWatch.Item1, propertiesToWatch.Item2).ActiveSelect(value => value.Value);


		public static IActiveList<TResult> ActiveSelect<TSource, TResult>(this IActiveList<TSource> source, Expression<Func<TSource, IMutableActiveValue<TResult>>> selector)
			=> ActiveSelect(source, selector.Compile(), selector.GetReferencedProperties());

		public static IActiveList<TResult> ActiveSelect<TSource, TResult>(this IActiveList<TSource> source, Func<TSource, IMutableActiveValue<TResult>> selector, IEnumerable<string> propertiesToWatch)
			=> new ActiveSelect<TSource, object, IMutableActiveValue<TResult>>(source, selector, propertiesToWatch).ActiveSelect(value => value.Value);

		public static IActiveList<TResult> ActiveSelect<TSource, TParameter, TResult>(this IActiveList<TSource> source, IActiveValue<TParameter> parameter, Expression<Func<TSource, TParameter, IMutableActiveValue<TResult>>> selector)
			=> ActiveSelect(source, parameter, selector.Compile(), selector.GetReferencedProperties());

		public static IActiveList<TResult> ActiveSelect<TSource, TParameter, TResult>(this IActiveList<TSource> source, IActiveValue<TParameter> parameter, Func<TSource, TParameter, IMutableActiveValue<TResult>> selector, IEnumerable<string> sourcePropertiesToWatch, IEnumerable<string> parameterPropertiesToWatch)
			=> ActiveSelect(source, parameter, selector, Tuple.Create(sourcePropertiesToWatch, parameterPropertiesToWatch));

		private static IActiveList<TResult> ActiveSelect<TSource, TParameter, TResult>(this IActiveList<TSource> source, IActiveValue<TParameter> parameter, Func<TSource, TParameter, IMutableActiveValue<TResult>> selector, Tuple<IEnumerable<string>, IEnumerable<string>> propertiesToWatch)
			=> new ActiveSelect<TSource, TParameter, IActiveValue<TResult>>(source, selector, parameter, propertiesToWatch.Item1, propertiesToWatch.Item2).ActiveSelect(value => value.Value);


		public static IActiveList<TResult> ActiveSelectMany<TSource, TResult>(this IActiveList<TSource> source, Expression<Func<TSource, IEnumerable<TResult>>> selector)
			=> ActiveSelectMany(source, selector.Compile(), selector.GetReferencedProperties());

		public static IActiveList<TResult> ActiveSelectMany<TSource, TResult>(this IActiveList<TSource> source, Func<TSource, IEnumerable<TResult>> selector, IEnumerable<string> propertiesToWatch)
			=> new ActiveSelectMany<TSource, object, TResult>(source, selector, propertiesToWatch);

		public static IActiveList<TResult> ActiveSelectMany<TSource, TParameter, TResult>(this IActiveList<TSource> source, IActiveValue<TParameter> parameter, Expression<Func<TSource, TParameter, IEnumerable<TResult>>> selector)
			=> ActiveSelectMany(source, parameter, selector.Compile(), selector.GetReferencedProperties());

		public static IActiveList<TResult> ActiveSelectMany<TSource, TParameter, TResult>(this IActiveList<TSource> source, IActiveValue<TParameter> parameter, Func<TSource, TParameter, IEnumerable<TResult>> selector, IEnumerable<string> sourcePropertiesToWatch, IEnumerable<string> parameterPropertiesToWatch)
			=> ActiveSelectMany(source, parameter, selector, Tuple.Create(sourcePropertiesToWatch, parameterPropertiesToWatch));

		private static IActiveList<TResult> ActiveSelectMany<TSource, TParameter, TResult>(this IActiveList<TSource> source, IActiveValue<TParameter> parameter, Func<TSource, TParameter, IEnumerable<TResult>> selector, Tuple<IEnumerable<string>, IEnumerable<string>> propertiesToWatch)
			=> new ActiveSelectMany<TSource, TParameter, TResult>(source, selector, parameter, propertiesToWatch.Item1, propertiesToWatch.Item2);


		public static IActiveList<TSource> ActiveElementsAtOrEmpty<TSource>(this IActiveList<TSource> source, params int[] indexes)
			=> new ActiveElementsOrEmpty<TSource>(source, indexes);

		public static IActiveList<TSource> ActiveElementsAtOrEmpty<TSource>(this IActiveList<TSource> source, IEnumerable<int> indexes)
			=> new ActiveElementsOrEmpty<TSource>(source, indexes.ToReadOnlyList());


		public static IActiveList<TSource> ActiveTake<TSource>(this IActiveList<TSource> source, int count)
			=> new ActiveTake<TSource>(source, new ActiveValueWrapper<int>(count));

		public static IActiveList<TSource> ActiveTake<TSource>(this IActiveList<TSource> source, IActiveValue<int> count)
			=> new ActiveTake<TSource>(source, count);


		public static IActiveList<TSource> ActiveSkip<TSource>(this IActiveList<TSource> source, int count)
			=> new ActiveSkip<TSource>(source, new ActiveValueWrapper<int>(count));

		public static IActiveList<TSource> ActiveSkip<TSource>(this IActiveList<TSource> source, IActiveValue<int> count)
			=> new ActiveSkip<TSource>(source, count);


		public static IActiveList<TSource> ActiveConcat<TSource>(this IActiveList<TSource> source, IEnumerable<TSource> concat)
			=> new ActiveConcat<TSource>(source, concat);


		public static IActiveList<TSource> ActiveReverse<TSource>(this IActiveList<TSource> source)
			=> new ActiveReverse<TSource>(source);


		public static IActiveSetList<TSource> ActiveDistinct<TSource>(this IActiveList<TSource> source)
			=> new ActiveDistinct<TSource, TSource, object>(source, o => o, null);

		public static IActiveSetList<TSource> ActiveDistinct<TSource, TKey>(this IActiveList<TSource> source, Expression<Func<TSource, TKey>> keySelector)
			=> ActiveDistinct(source, keySelector.Compile(), keySelector.GetReferencedProperties());

		public static IActiveSetList<TSource> ActiveDistinct<TSource, TKey>(this IActiveList<TSource> source, Func<TSource, TKey> keySelector, IEnumerable<string> propertiesToWatch)
			=> new ActiveDistinct<TKey, TSource, object>(source, keySelector, propertiesToWatch);

		public static IActiveSetList<TSource> ActiveDistinct<TSource, TKey, TParameter>(this IActiveList<TSource> source, IActiveValue<TParameter> parameter, Expression<Func<TSource, TParameter, TKey>> keySelector)
			=> ActiveDistinct(source, parameter, keySelector.Compile(), keySelector.GetReferencedProperties());

		public static IActiveSetList<TSource> ActiveDistinct<TSource, TKey, TParameter>(this IActiveList<TSource> source, IActiveValue<TParameter> parameter, Func<TSource, TParameter, TKey> keySelector, IEnumerable<string> sourcePropertiesToWatch, IEnumerable<string> parameterPropertiesToWatch)
			=> ActiveDistinct(source, parameter, keySelector, Tuple.Create(sourcePropertiesToWatch, parameterPropertiesToWatch));

		private static IActiveSetList<TSource> ActiveDistinct<TSource, TKey, TParameter>(this IActiveList<TSource> source, IActiveValue<TParameter> parameter, Func<TSource, TParameter, TKey> keySelector, Tuple<IEnumerable<string>, IEnumerable<string>> propertiesToWatch)
			=> new ActiveDistinct<TKey, TSource, TParameter>(source, keySelector, parameter, propertiesToWatch.Item1, propertiesToWatch.Item2);


		public static IActiveSetList<TSource> ActiveUnion<TSource>(this IActiveList<TSource> source, IEnumerable<TSource> union)
			=> new ActiveUnion<TSource, TSource, object>(source, union.ToReadOnlyList(), o => o, null);

		public static IActiveSetList<TSource> ActiveUnion<TSource, TKey>(this IActiveList<TSource> source, IEnumerable<TSource> union, Expression<Func<TSource, TKey>> keySelector)
			=> ActiveUnion(source, union, keySelector.Compile(), keySelector.GetReferencedProperties());

		public static IActiveSetList<TSource> ActiveUnion<TSource, TKey>(this IActiveList<TSource> source, IEnumerable<TSource> union, Func<TSource, TKey> keySelector, IEnumerable<string> propertiesToWatch)
			=> new ActiveUnion<TKey, TSource, object>(source, union.ToReadOnlyList(), keySelector, propertiesToWatch);

		public static IActiveSetList<TSource> ActiveUnion<TSource, TKey, TParameter>(this IActiveList<TSource> source, IEnumerable<TSource> union, IActiveValue<TParameter> parameter, Expression<Func<TSource, TParameter, TKey>> keySelector)
			=> ActiveUnion(source, union, parameter, keySelector.Compile(), keySelector.GetReferencedProperties());

		public static IActiveSetList<TSource> ActiveUnion<TSource, TKey, TParameter>(this IActiveList<TSource> source, IEnumerable<TSource> union, IActiveValue<TParameter> parameter, Func<TSource, TParameter, TKey> keySelector, IEnumerable<string> sourcePropertiesToWatch, IEnumerable<string> parameterPropertiesToWatch)
			=> ActiveUnion(source, union, parameter, keySelector, Tuple.Create(sourcePropertiesToWatch, parameterPropertiesToWatch));

		private static IActiveSetList<TSource> ActiveUnion<TSource, TKey, TParameter>(this IActiveList<TSource> source, IEnumerable<TSource> union, IActiveValue<TParameter> parameter, Func<TSource, TParameter, TKey> keySelector, Tuple<IEnumerable<string>, IEnumerable<string>> propertiesToWatch)
			=> new ActiveUnion<TKey, TSource, TParameter>(source, union.ToReadOnlyList(), keySelector, parameter, propertiesToWatch.Item1, propertiesToWatch.Item2);


		public static IActiveSetList<TSource> ActiveIntersect<TSource>(this IActiveList<TSource> source, IEnumerable<TSource> intersect)
			=> new ActiveIntersect<TSource, TSource, object>(source, intersect.ToReadOnlyList(), o => o, null);

		public static IActiveSetList<TSource> ActiveIntersect<TSource, TKey>(this IActiveList<TSource> source, IEnumerable<TSource> intersect, Expression<Func<TSource, TKey>> keySelector)
			=> ActiveIntersect(source, intersect, keySelector.Compile(), keySelector.GetReferencedProperties());

		public static IActiveSetList<TSource> ActiveIntersect<TSource, TKey>(this IActiveList<TSource> source, IEnumerable<TSource> intersect, Func<TSource, TKey> keySelector, IEnumerable<string> propertiesToWatch)
			=> new ActiveIntersect<TKey, TSource, object>(source, intersect.ToReadOnlyList(), keySelector, propertiesToWatch);

		public static IActiveSetList<TSource> ActiveIntersect<TSource, TKey, TParameter>(this IActiveList<TSource> source, IEnumerable<TSource> intersect, IActiveValue<TParameter> parameter, Expression<Func<TSource, TParameter, TKey>> keySelector)
			=> ActiveIntersect(source, intersect, parameter, keySelector.Compile(), keySelector.GetReferencedProperties());

		public static IActiveSetList<TSource> ActiveIntersect<TSource, TKey, TParameter>(this IActiveList<TSource> source, IEnumerable<TSource> intersect, IActiveValue<TParameter> parameter, Func<TSource, TParameter, TKey> keySelector, IEnumerable<string> sourcePropertiesToWatch, IEnumerable<string> parameterPropertiesToWatch)
			=> ActiveIntersect(source, intersect, parameter, keySelector, Tuple.Create(sourcePropertiesToWatch, parameterPropertiesToWatch));

		private static IActiveSetList<TSource> ActiveIntersect<TSource, TKey, TParameter>(this IActiveList<TSource> source, IEnumerable<TSource> intersect, IActiveValue<TParameter> parameter, Func<TSource, TParameter, TKey> keySelector, Tuple<IEnumerable<string>, IEnumerable<string>> propertiesToWatch)
			=> new ActiveIntersect<TKey, TSource, TParameter>(source, intersect.ToReadOnlyList(), keySelector, parameter, propertiesToWatch.Item1, propertiesToWatch.Item2);


		public static IActiveSetList<TSource> ActiveExcept<TSource>(this IActiveList<TSource> source, IEnumerable<TSource> except)
			=> new ActiveExcept<TSource, TSource, object>(source, except.ToReadOnlyList(), o => o, null);

		public static IActiveSetList<TSource> ActiveExcept<TSource, TKey>(this IActiveList<TSource> source, IEnumerable<TSource> except, Expression<Func<TSource, TKey>> keySelector)
			=> ActiveExcept(source, except, keySelector.Compile(), keySelector.GetReferencedProperties());

		public static IActiveSetList<TSource> ActiveExcept<TSource, TKey>(this IActiveList<TSource> source, IEnumerable<TSource> except, Func<TSource, TKey> keySelector, IEnumerable<string> propertiesToWatch)
			=> new ActiveExcept<TKey, TSource, object>(source, except.ToReadOnlyList(), keySelector, propertiesToWatch);

		public static IActiveSetList<TSource> ActiveExcept<TSource, TKey, TParameter>(this IActiveList<TSource> source, IEnumerable<TSource> except, IActiveValue<TParameter> parameter, Expression<Func<TSource, TParameter, TKey>> keySelector)
			=> ActiveExcept(source, except, parameter, keySelector.Compile(), keySelector.GetReferencedProperties());

		public static IActiveSetList<TSource> ActiveExcept<TSource, TKey, TParameter>(this IActiveList<TSource> source, IEnumerable<TSource> except, IActiveValue<TParameter> parameter, Func<TSource, TParameter, TKey> keySelector, IEnumerable<string> sourcePropertiesToWatch, IEnumerable<string> parameterPropertiesToWatch)
			=> ActiveExcept(source, except, parameter, keySelector, Tuple.Create(sourcePropertiesToWatch, parameterPropertiesToWatch));

		private static IActiveSetList<TSource> ActiveExcept<TSource, TKey, TParameter>(this IActiveList<TSource> source, IEnumerable<TSource> except, IActiveValue<TParameter> parameter, Func<TSource, TParameter, TKey> keySelector, Tuple<IEnumerable<string>, IEnumerable<string>> propertiesToWatch)
			=> new ActiveExcept<TKey, TSource, TParameter>(source, except.ToReadOnlyList(), keySelector, parameter, propertiesToWatch.Item1, propertiesToWatch.Item2);


		public static IActiveList<TResult> ActiveInnerJoin<TLeft, TRight, TResult, TKey>(this IActiveList<TLeft> source, IEnumerable<TRight> join, Expression<Func<TLeft, TKey>> leftKeySelector, Expression<Func<TRight, TKey>> rightKeySelector, Expression<Func<TLeft, TRight, TResult>> resultSelector)
			=> ActiveInnerJoin(source, join, leftKeySelector.Compile(), rightKeySelector.Compile(), resultSelector.Compile(), CreateJoinPropertiesToWatchTuple(leftKeySelector.GetReferencedProperties(), rightKeySelector.GetReferencedProperties(), resultSelector.GetReferencedProperties()));

		public static IActiveList<TResult> ActiveInnerJoin<TLeft, TRight, TResult, TKey>(this IActiveList<TLeft> source, IEnumerable<TRight> join, Func<TLeft, TKey> leftKeySelector, Func<TRight, TKey> rightKeySelector, Func<TLeft, TRight, TResult> resultSelector, IEnumerable<string> leftKeySelectorPropertiesToWatch, IEnumerable<string> rightKeySelectorPropertiesToWatch, IEnumerable<string> leftResultSelectorPropertiesToWatch, IEnumerable<string> rightResultSelectorPropertiesToWatch)
			=> ActiveInnerJoin(source, join, leftKeySelector, rightKeySelector, resultSelector, Tuple.Create(leftKeySelectorPropertiesToWatch, rightKeySelectorPropertiesToWatch, leftResultSelectorPropertiesToWatch, rightResultSelectorPropertiesToWatch));

		private static IActiveList<TResult> ActiveInnerJoin<TLeft, TRight, TResult, TKey>(this IActiveList<TLeft> source, IEnumerable<TRight> join, Func<TLeft, TKey> leftKeySelector, Func<TRight, TKey> rightKeySelector, Func<TLeft, TRight, TResult> resultSelector, Tuple<IEnumerable<string>, IEnumerable<string>, IEnumerable<string>, IEnumerable<string>> propertiesToWatch)
			=> new ActiveJoin<TLeft, TRight, TResult, TKey, object>(ActiveListJoinBehaviour.Inner, source, join.ToReadOnlyList(), leftKeySelector, rightKeySelector, (l, r) => resultSelector.Invoke(l.Value, r.Value), propertiesToWatch.Item1, propertiesToWatch.Item2, propertiesToWatch.Item3, propertiesToWatch.Item4);

		public static IActiveList<TResult> ActiveInnerJoin<TLeft, TRight, TResult, TKey, TParameter>(this IActiveList<TLeft> source, IEnumerable<TRight> join, IActiveValue<TParameter> parameter, Expression<Func<TLeft, TParameter, TKey>> leftKeySelector, Expression<Func<TRight, TParameter, TKey>> rightKeySelector, Expression<Func<TLeft, TRight, TParameter, TResult>> resultSelector)
			=> ActiveInnerJoin(source, join, parameter, leftKeySelector.Compile(), rightKeySelector.Compile(), resultSelector.Compile(), CreateJoinPropertiesToWatchTuple(leftKeySelector.GetReferencedProperties(), rightKeySelector.GetReferencedProperties(), resultSelector.GetReferencedProperties()));

		public static IActiveList<TResult> ActiveInnerJoin<TLeft, TRight, TResult, TKey, TParameter>(this IActiveList<TLeft> source, IEnumerable<TRight> join, IActiveValue<TParameter> parameter, Func<TLeft, TParameter, TKey> leftKeySelector, Func<TRight, TParameter, TKey> rightKeySelector, Func<TLeft, TRight, TParameter, TResult> resultSelector, IEnumerable<string> leftKeySelectorPropertiesToWatch, IEnumerable<string> rightKeySelectorPropertiesToWatch, IEnumerable<string> leftResultSelectorPropertiesToWatch, IEnumerable<string> rightResultSelectorPropertiesToWatch, IEnumerable<string> leftKeySelectorParameterPropertiesToWatch, IEnumerable<string> rightKeySelectorParameterPropertiesToWatch, IEnumerable<string> resultSelectorParameterPropertiesToWatch)
			=> ActiveInnerJoin(source, join, parameter, leftKeySelector, rightKeySelector, resultSelector, Tuple.Create(leftKeySelectorPropertiesToWatch, rightKeySelectorPropertiesToWatch, leftResultSelectorPropertiesToWatch, rightResultSelectorPropertiesToWatch, leftKeySelectorParameterPropertiesToWatch, rightKeySelectorParameterPropertiesToWatch, resultSelectorParameterPropertiesToWatch));

		private static IActiveList<TResult> ActiveInnerJoin<TLeft, TRight, TResult, TKey, TParameter>(this IActiveList<TLeft> source, IEnumerable<TRight> join, IActiveValue<TParameter> parameter, Func<TLeft, TParameter, TKey> leftKeySelector, Func<TRight, TParameter, TKey> rightKeySelector, Func<TLeft, TRight, TParameter, TResult> resultSelector, Tuple<IEnumerable<string>, IEnumerable<string>, IEnumerable<string>, IEnumerable<string>, IEnumerable<string>, IEnumerable<string>, IEnumerable<string>> propertiesToWatch)
			=> new ActiveJoin<TLeft, TRight, TResult, TKey, TParameter>(ActiveListJoinBehaviour.Inner, source, join.ToReadOnlyList(), parameter, leftKeySelector, rightKeySelector, (l, r, p) => resultSelector.Invoke(l.Value, r.Value, p), propertiesToWatch.Item1, propertiesToWatch.Item2, propertiesToWatch.Item3, propertiesToWatch.Item4, propertiesToWatch.Item5, propertiesToWatch.Item6, propertiesToWatch.Item7);


		public static IActiveList<TResult> ActiveLeftJoin<TLeft, TRight, TResult, TKey>(this IActiveList<TLeft> source, IEnumerable<TRight> join, Expression<Func<TLeft, TKey>> leftKeySelector, Expression<Func<TRight, TKey>> rightKeySelector, Expression<Func<TLeft, JoinOption<TRight>, TResult>> resultSelector)
			=> ActiveLeftJoin(source, join, leftKeySelector.Compile(), rightKeySelector.Compile(), resultSelector.Compile(), CreateJoinPropertiesToWatchTuple(leftKeySelector.GetReferencedProperties(), rightKeySelector.GetReferencedProperties(), resultSelector.GetReferencedProperties()));

		public static IActiveList<TResult> ActiveLeftJoin<TLeft, TRight, TResult, TKey>(this IActiveList<TLeft> source, IEnumerable<TRight> join, Func<TLeft, TKey> leftKeySelector, Func<TRight, TKey> rightKeySelector, Func<TLeft, JoinOption<TRight>, TResult> resultSelector, IEnumerable<string> leftKeySelectorPropertiesToWatch, IEnumerable<string> rightKeySelectorPropertiesToWatch, IEnumerable<string> leftResultSelectorPropertiesToWatch, IEnumerable<string> rightResultSelectorPropertiesToWatch)
			=> ActiveLeftJoin(source, join, leftKeySelector, rightKeySelector, resultSelector, Tuple.Create(leftKeySelectorPropertiesToWatch, rightKeySelectorPropertiesToWatch, leftResultSelectorPropertiesToWatch, rightResultSelectorPropertiesToWatch));

		private static IActiveList<TResult> ActiveLeftJoin<TLeft, TRight, TResult, TKey>(this IActiveList<TLeft> source, IEnumerable<TRight> join, Func<TLeft, TKey> leftKeySelector, Func<TRight, TKey> rightKeySelector, Func<TLeft, JoinOption<TRight>, TResult> resultSelector, Tuple<IEnumerable<string>, IEnumerable<string>, IEnumerable<string>, IEnumerable<string>> propertiesToWatch)
			=> new ActiveJoin<TLeft, TRight, TResult, TKey, object>(ActiveListJoinBehaviour.Left, source, join.ToReadOnlyList(), leftKeySelector, rightKeySelector, (l, r) => resultSelector.Invoke(l.Value, r), propertiesToWatch.Item1, propertiesToWatch.Item2, propertiesToWatch.Item3, propertiesToWatch.Item4);

		public static IActiveList<TResult> ActiveLeftJoin<TLeft, TRight, TResult, TKey, TParameter>(this IActiveList<TLeft> source, IEnumerable<TRight> join, IActiveValue<TParameter> parameter, Expression<Func<TLeft, TParameter, TKey>> leftKeySelector, Expression<Func<TRight, TParameter, TKey>> rightKeySelector, Expression<Func<TLeft, JoinOption<TRight>, TParameter, TResult>> resultSelector)
			=> ActiveLeftJoin(source, join, parameter, leftKeySelector.Compile(), rightKeySelector.Compile(), resultSelector.Compile(), CreateJoinPropertiesToWatchTuple(leftKeySelector.GetReferencedProperties(), rightKeySelector.GetReferencedProperties(), resultSelector.GetReferencedProperties()));

		public static IActiveList<TResult> ActiveLeftJoin<TLeft, TRight, TResult, TKey, TParameter>(this IActiveList<TLeft> source, IEnumerable<TRight> join, IActiveValue<TParameter> parameter, Func<TLeft, TParameter, TKey> leftKeySelector, Func<TRight, TParameter, TKey> rightKeySelector, Func<TLeft, JoinOption<TRight>, TParameter, TResult> resultSelector, IEnumerable<string> leftKeySelectorPropertiesToWatch, IEnumerable<string> rightKeySelectorPropertiesToWatch, IEnumerable<string> leftResultSelectorPropertiesToWatch, IEnumerable<string> rightResultSelectorPropertiesToWatch, IEnumerable<string> leftKeySelectorParameterPropertiesToWatch, IEnumerable<string> rightKeySelectorParameterPropertiesToWatch, IEnumerable<string> resultSelectorParameterPropertiesToWatch)
			=> ActiveLeftJoin(source, join, parameter, leftKeySelector, rightKeySelector, resultSelector, Tuple.Create(leftKeySelectorPropertiesToWatch, rightKeySelectorPropertiesToWatch, leftResultSelectorPropertiesToWatch, rightResultSelectorPropertiesToWatch, leftKeySelectorParameterPropertiesToWatch, rightKeySelectorParameterPropertiesToWatch, resultSelectorParameterPropertiesToWatch));

		private static IActiveList<TResult> ActiveLeftJoin<TLeft, TRight, TResult, TKey, TParameter>(this IActiveList<TLeft> source, IEnumerable<TRight> join, IActiveValue<TParameter> parameter, Func<TLeft, TParameter, TKey> leftKeySelector, Func<TRight, TParameter, TKey> rightKeySelector, Func<TLeft, JoinOption<TRight>, TParameter, TResult> resultSelector, Tuple<IEnumerable<string>, IEnumerable<string>, IEnumerable<string>, IEnumerable<string>, IEnumerable<string>, IEnumerable<string>, IEnumerable<string>> propertiesToWatch)
			=> new ActiveJoin<TLeft, TRight, TResult, TKey, TParameter>(ActiveListJoinBehaviour.Left, source, join.ToReadOnlyList(), parameter, leftKeySelector, rightKeySelector, (l, r, p) => resultSelector.Invoke(l.Value, r, p), propertiesToWatch.Item1, propertiesToWatch.Item2, propertiesToWatch.Item3, propertiesToWatch.Item4, propertiesToWatch.Item5, propertiesToWatch.Item6, propertiesToWatch.Item7);


		public static IActiveList<TResult> ActiveLeftExcludingJoin<TLeft, TRight, TResult, TKey>(this IActiveList<TLeft> source, IEnumerable<TRight> join, Expression<Func<TLeft, TKey>> leftKeySelector, Expression<Func<TRight, TKey>> rightKeySelector, Expression<Func<TLeft, JoinOption<TRight>, TResult>> resultSelector)
			=> ActiveLeftExcludingJoin(source, join, leftKeySelector.Compile(), rightKeySelector.Compile(), resultSelector.Compile(), CreateJoinPropertiesToWatchTuple(leftKeySelector.GetReferencedProperties(), rightKeySelector.GetReferencedProperties(), resultSelector.GetReferencedProperties()));

		public static IActiveList<TResult> ActiveLeftExcludingJoin<TLeft, TRight, TResult, TKey>(this IActiveList<TLeft> source, IEnumerable<TRight> join, Func<TLeft, TKey> leftKeySelector, Func<TRight, TKey> rightKeySelector, Func<TLeft, JoinOption<TRight>, TResult> resultSelector, IEnumerable<string> leftKeySelectorPropertiesToWatch, IEnumerable<string> rightKeySelectorPropertiesToWatch, IEnumerable<string> leftResultSelectorPropertiesToWatch, IEnumerable<string> rightResultSelectorPropertiesToWatch)
			=> ActiveLeftExcludingJoin(source, join, leftKeySelector, rightKeySelector, resultSelector, Tuple.Create(leftKeySelectorPropertiesToWatch, rightKeySelectorPropertiesToWatch, leftResultSelectorPropertiesToWatch, rightResultSelectorPropertiesToWatch));

		private static IActiveList<TResult> ActiveLeftExcludingJoin<TLeft, TRight, TResult, TKey>(this IActiveList<TLeft> source, IEnumerable<TRight> join, Func<TLeft, TKey> leftKeySelector, Func<TRight, TKey> rightKeySelector, Func<TLeft, JoinOption<TRight>, TResult> resultSelector, Tuple<IEnumerable<string>, IEnumerable<string>, IEnumerable<string>, IEnumerable<string>> propertiesToWatch)
			=> new ActiveJoin<TLeft, TRight, TResult, TKey, object>(ActiveListJoinBehaviour.LeftExcluding, source, join.ToReadOnlyList(), leftKeySelector, rightKeySelector, (l, r) => resultSelector.Invoke(l.Value, r), propertiesToWatch.Item1, propertiesToWatch.Item2, propertiesToWatch.Item3, propertiesToWatch.Item4);

		public static IActiveList<TResult> ActiveLeftExcludingJoin<TLeft, TRight, TResult, TKey, TParameter>(this IActiveList<TLeft> source, IEnumerable<TRight> join, IActiveValue<TParameter> parameter, Expression<Func<TLeft, TParameter, TKey>> leftKeySelector, Expression<Func<TRight, TParameter, TKey>> rightKeySelector, Expression<Func<TLeft, JoinOption<TRight>, TParameter, TResult>> resultSelector)
			=> ActiveLeftExcludingJoin(source, join, parameter, leftKeySelector.Compile(), rightKeySelector.Compile(), resultSelector.Compile(), CreateJoinPropertiesToWatchTuple(leftKeySelector.GetReferencedProperties(), rightKeySelector.GetReferencedProperties(), resultSelector.GetReferencedProperties()));

		public static IActiveList<TResult> ActiveLeftExcludingJoin<TLeft, TRight, TResult, TKey, TParameter>(this IActiveList<TLeft> source, IEnumerable<TRight> join, IActiveValue<TParameter> parameter, Func<TLeft, TParameter, TKey> leftKeySelector, Func<TRight, TParameter, TKey> rightKeySelector, Func<TLeft, JoinOption<TRight>, TParameter, TResult> resultSelector, IEnumerable<string> leftKeySelectorPropertiesToWatch, IEnumerable<string> rightKeySelectorPropertiesToWatch, IEnumerable<string> leftResultSelectorPropertiesToWatch, IEnumerable<string> rightResultSelectorPropertiesToWatch, IEnumerable<string> leftKeySelectorParameterPropertiesToWatch, IEnumerable<string> rightKeySelectorParameterPropertiesToWatch, IEnumerable<string> resultSelectorParameterPropertiesToWatch)
			=> ActiveLeftExcludingJoin(source, join, parameter, leftKeySelector, rightKeySelector, resultSelector, Tuple.Create(leftKeySelectorPropertiesToWatch, rightKeySelectorPropertiesToWatch, leftResultSelectorPropertiesToWatch, rightResultSelectorPropertiesToWatch, leftKeySelectorParameterPropertiesToWatch, rightKeySelectorParameterPropertiesToWatch, resultSelectorParameterPropertiesToWatch));

		private static IActiveList<TResult> ActiveLeftExcludingJoin<TLeft, TRight, TResult, TKey, TParameter>(this IActiveList<TLeft> source, IEnumerable<TRight> join, IActiveValue<TParameter> parameter, Func<TLeft, TParameter, TKey> leftKeySelector, Func<TRight, TParameter, TKey> rightKeySelector, Func<TLeft, JoinOption<TRight>, TParameter, TResult> resultSelector, Tuple<IEnumerable<string>, IEnumerable<string>, IEnumerable<string>, IEnumerable<string>, IEnumerable<string>, IEnumerable<string>, IEnumerable<string>> propertiesToWatch)
			=> new ActiveJoin<TLeft, TRight, TResult, TKey, TParameter>(ActiveListJoinBehaviour.LeftExcluding, source, join.ToReadOnlyList(), parameter, leftKeySelector, rightKeySelector, (l, r, p) => resultSelector.Invoke(l.Value, r, p), propertiesToWatch.Item1, propertiesToWatch.Item2, propertiesToWatch.Item3, propertiesToWatch.Item4, propertiesToWatch.Item5, propertiesToWatch.Item6, propertiesToWatch.Item7);


		public static IActiveList<TResult> ActiveRightJoin<TLeft, TRight, TResult, TKey>(this IActiveList<TLeft> source, IEnumerable<TRight> join, Expression<Func<TLeft, TKey>> leftKeySelector, Expression<Func<TRight, TKey>> rightKeySelector, Expression<Func<JoinOption<TLeft>, TRight, TResult>> resultSelector)
			=> ActiveRightJoin(source, join, leftKeySelector.Compile(), rightKeySelector.Compile(), resultSelector.Compile(), CreateJoinPropertiesToWatchTuple(leftKeySelector.GetReferencedProperties(), rightKeySelector.GetReferencedProperties(), resultSelector.GetReferencedProperties()));

		public static IActiveList<TResult> ActiveRightJoin<TLeft, TRight, TResult, TKey>(this IActiveList<TLeft> source, IEnumerable<TRight> join, Func<TLeft, TKey> leftKeySelector, Func<TRight, TKey> rightKeySelector, Func<JoinOption<TLeft>, TRight, TResult> resultSelector, IEnumerable<string> leftKeySelectorPropertiesToWatch, IEnumerable<string> rightKeySelectorPropertiesToWatch, IEnumerable<string> leftResultSelectorPropertiesToWatch, IEnumerable<string> rightResultSelectorPropertiesToWatch)
			=> ActiveRightJoin(source, join, leftKeySelector, rightKeySelector, resultSelector, Tuple.Create(leftKeySelectorPropertiesToWatch, rightKeySelectorPropertiesToWatch, leftResultSelectorPropertiesToWatch, rightResultSelectorPropertiesToWatch));

		private static IActiveList<TResult> ActiveRightJoin<TLeft, TRight, TResult, TKey>(this IActiveList<TLeft> source, IEnumerable<TRight> join, Func<TLeft, TKey> leftKeySelector, Func<TRight, TKey> rightKeySelector, Func<JoinOption<TLeft>, TRight, TResult> resultSelector, Tuple<IEnumerable<string>, IEnumerable<string>, IEnumerable<string>, IEnumerable<string>> propertiesToWatch)
			=> new ActiveJoin<TLeft, TRight, TResult, TKey, object>(ActiveListJoinBehaviour.Right, source, join.ToReadOnlyList(), leftKeySelector, rightKeySelector, (l, r) => resultSelector.Invoke(l, r.Value), propertiesToWatch.Item1, propertiesToWatch.Item2, propertiesToWatch.Item3, propertiesToWatch.Item4);

		public static IActiveList<TResult> ActiveRightJoin<TLeft, TRight, TResult, TKey, TParameter>(this IActiveList<TLeft> source, IEnumerable<TRight> join, IActiveValue<TParameter> parameter, Expression<Func<TLeft, TParameter, TKey>> leftKeySelector, Expression<Func<TRight, TParameter, TKey>> rightKeySelector, Expression<Func<JoinOption<TLeft>, TRight, TParameter, TResult>> resultSelector)
			=> ActiveRightJoin(source, join, parameter, leftKeySelector.Compile(), rightKeySelector.Compile(), resultSelector.Compile(), CreateJoinPropertiesToWatchTuple(leftKeySelector.GetReferencedProperties(), rightKeySelector.GetReferencedProperties(), resultSelector.GetReferencedProperties()));

		public static IActiveList<TResult> ActiveRightJoin<TLeft, TRight, TResult, TKey, TParameter>(this IActiveList<TLeft> source, IEnumerable<TRight> join, IActiveValue<TParameter> parameter, Func<TLeft, TParameter, TKey> leftKeySelector, Func<TRight, TParameter, TKey> rightKeySelector, Func<JoinOption<TLeft>, TRight, TParameter, TResult> resultSelector, IEnumerable<string> leftKeySelectorPropertiesToWatch, IEnumerable<string> rightKeySelectorPropertiesToWatch, IEnumerable<string> leftResultSelectorPropertiesToWatch, IEnumerable<string> rightResultSelectorPropertiesToWatch, IEnumerable<string> leftKeySelectorParameterPropertiesToWatch, IEnumerable<string> rightKeySelectorParameterPropertiesToWatch, IEnumerable<string> resultSelectorParameterPropertiesToWatch)
			=> ActiveRightJoin(source, join, parameter, leftKeySelector, rightKeySelector, resultSelector, Tuple.Create(leftKeySelectorPropertiesToWatch, rightKeySelectorPropertiesToWatch, leftResultSelectorPropertiesToWatch, rightResultSelectorPropertiesToWatch, leftKeySelectorParameterPropertiesToWatch, rightKeySelectorParameterPropertiesToWatch, resultSelectorParameterPropertiesToWatch));

		private static IActiveList<TResult> ActiveRightJoin<TLeft, TRight, TResult, TKey, TParameter>(this IActiveList<TLeft> source, IEnumerable<TRight> join, IActiveValue<TParameter> parameter, Func<TLeft, TParameter, TKey> leftKeySelector, Func<TRight, TParameter, TKey> rightKeySelector, Func<JoinOption<TLeft>, TRight, TParameter, TResult> resultSelector, Tuple<IEnumerable<string>, IEnumerable<string>, IEnumerable<string>, IEnumerable<string>, IEnumerable<string>, IEnumerable<string>, IEnumerable<string>> propertiesToWatch)
			=> new ActiveJoin<TLeft, TRight, TResult, TKey, TParameter>(ActiveListJoinBehaviour.Right, source, join.ToReadOnlyList(), parameter, leftKeySelector, rightKeySelector, (l, r, p) => resultSelector.Invoke(l, r.Value, p), propertiesToWatch.Item1, propertiesToWatch.Item2, propertiesToWatch.Item3, propertiesToWatch.Item4, propertiesToWatch.Item5, propertiesToWatch.Item6, propertiesToWatch.Item7);


		public static IActiveList<TResult> ActiveRightExcludingJoin<TLeft, TRight, TResult, TKey>(this IActiveList<TLeft> source, IEnumerable<TRight> join, Expression<Func<TLeft, TKey>> leftKeySelector, Expression<Func<TRight, TKey>> rightKeySelector, Expression<Func<JoinOption<TLeft>, TRight, TResult>> resultSelector)
			=> ActiveRightExcludingJoin(source, join, leftKeySelector.Compile(), rightKeySelector.Compile(), resultSelector.Compile(), CreateJoinPropertiesToWatchTuple(leftKeySelector.GetReferencedProperties(), rightKeySelector.GetReferencedProperties(), resultSelector.GetReferencedProperties()));

		public static IActiveList<TResult> ActiveRightExcludingJoin<TLeft, TRight, TResult, TKey>(this IActiveList<TLeft> source, IEnumerable<TRight> join, Func<TLeft, TKey> leftKeySelector, Func<TRight, TKey> rightKeySelector, Func<JoinOption<TLeft>, TRight, TResult> resultSelector, IEnumerable<string> leftKeySelectorPropertiesToWatch, IEnumerable<string> rightKeySelectorPropertiesToWatch, IEnumerable<string> leftResultSelectorPropertiesToWatch, IEnumerable<string> rightResultSelectorPropertiesToWatch)
			=> ActiveRightExcludingJoin(source, join, leftKeySelector, rightKeySelector, resultSelector, Tuple.Create(leftKeySelectorPropertiesToWatch, rightKeySelectorPropertiesToWatch, leftResultSelectorPropertiesToWatch, rightResultSelectorPropertiesToWatch));

		private static IActiveList<TResult> ActiveRightExcludingJoin<TLeft, TRight, TResult, TKey>(this IActiveList<TLeft> source, IEnumerable<TRight> join, Func<TLeft, TKey> leftKeySelector, Func<TRight, TKey> rightKeySelector, Func<JoinOption<TLeft>, TRight, TResult> resultSelector, Tuple<IEnumerable<string>, IEnumerable<string>, IEnumerable<string>, IEnumerable<string>> propertiesToWatch)
			=> new ActiveJoin<TLeft, TRight, TResult, TKey, object>(ActiveListJoinBehaviour.RightExcluding, source, join.ToReadOnlyList(), leftKeySelector, rightKeySelector, (l, r) => resultSelector.Invoke(l, r.Value), propertiesToWatch.Item1, propertiesToWatch.Item2, propertiesToWatch.Item3, propertiesToWatch.Item4);

		public static IActiveList<TResult> ActiveRightExcludingJoin<TLeft, TRight, TResult, TKey, TParameter>(this IActiveList<TLeft> source, IEnumerable<TRight> join, IActiveValue<TParameter> parameter, Expression<Func<TLeft, TParameter, TKey>> leftKeySelector, Expression<Func<TRight, TParameter, TKey>> rightKeySelector, Expression<Func<JoinOption<TLeft>, TRight, TParameter, TResult>> resultSelector)
			=> ActiveRightExcludingJoin(source, join, parameter, leftKeySelector.Compile(), rightKeySelector.Compile(), resultSelector.Compile(), CreateJoinPropertiesToWatchTuple(leftKeySelector.GetReferencedProperties(), rightKeySelector.GetReferencedProperties(), resultSelector.GetReferencedProperties()));

		public static IActiveList<TResult> ActiveRightExcludingJoin<TLeft, TRight, TResult, TKey, TParameter>(this IActiveList<TLeft> source, IEnumerable<TRight> join, IActiveValue<TParameter> parameter, Func<TLeft, TParameter, TKey> leftKeySelector, Func<TRight, TParameter, TKey> rightKeySelector, Func<JoinOption<TLeft>, TRight, TParameter, TResult> resultSelector, IEnumerable<string> leftKeySelectorPropertiesToWatch, IEnumerable<string> rightKeySelectorPropertiesToWatch, IEnumerable<string> leftResultSelectorPropertiesToWatch, IEnumerable<string> rightResultSelectorPropertiesToWatch, IEnumerable<string> leftKeySelectorParameterPropertiesToWatch, IEnumerable<string> rightKeySelectorParameterPropertiesToWatch, IEnumerable<string> resultSelectorParameterPropertiesToWatch)
			=> ActiveRightExcludingJoin(source, join, parameter, leftKeySelector, rightKeySelector, resultSelector, Tuple.Create(leftKeySelectorPropertiesToWatch, rightKeySelectorPropertiesToWatch, leftResultSelectorPropertiesToWatch, rightResultSelectorPropertiesToWatch, leftKeySelectorParameterPropertiesToWatch, rightKeySelectorParameterPropertiesToWatch, resultSelectorParameterPropertiesToWatch));

		private static IActiveList<TResult> ActiveRightExcludingJoin<TLeft, TRight, TResult, TKey, TParameter>(this IActiveList<TLeft> source, IEnumerable<TRight> join, IActiveValue<TParameter> parameter, Func<TLeft, TParameter, TKey> leftKeySelector, Func<TRight, TParameter, TKey> rightKeySelector, Func<JoinOption<TLeft>, TRight, TParameter, TResult> resultSelector, Tuple<IEnumerable<string>, IEnumerable<string>, IEnumerable<string>, IEnumerable<string>, IEnumerable<string>, IEnumerable<string>, IEnumerable<string>> propertiesToWatch)
			=> new ActiveJoin<TLeft, TRight, TResult, TKey, TParameter>(ActiveListJoinBehaviour.RightExcluding, source, join.ToReadOnlyList(), parameter, leftKeySelector, rightKeySelector, (l, r, p) => resultSelector.Invoke(l, r.Value, p), propertiesToWatch.Item1, propertiesToWatch.Item2, propertiesToWatch.Item3, propertiesToWatch.Item4, propertiesToWatch.Item5, propertiesToWatch.Item6, propertiesToWatch.Item7);


		public static IActiveList<TResult> ActiveOuterJoin<TLeft, TRight, TResult, TKey>(this IActiveList<TLeft> source, IEnumerable<TRight> join, Expression<Func<TLeft, TKey>> leftKeySelector, Expression<Func<TRight, TKey>> rightKeySelector, Expression<Func<JoinOption<TLeft>, JoinOption<TRight>, TResult>> resultSelector)
			=> ActiveOuterJoin(source, join, leftKeySelector.Compile(), rightKeySelector.Compile(), resultSelector.Compile(), CreateJoinPropertiesToWatchTuple(leftKeySelector.GetReferencedProperties(), rightKeySelector.GetReferencedProperties(), resultSelector.GetReferencedProperties()));

		public static IActiveList<TResult> ActiveOuterJoin<TLeft, TRight, TResult, TKey>(this IActiveList<TLeft> source, IEnumerable<TRight> join, Func<TLeft, TKey> leftKeySelector, Func<TRight, TKey> rightKeySelector, Func<JoinOption<TLeft>, JoinOption<TRight>, TResult> resultSelector, IEnumerable<string> leftKeySelectorPropertiesToWatch, IEnumerable<string> rightKeySelectorPropertiesToWatch, IEnumerable<string> leftResultSelectorPropertiesToWatch, IEnumerable<string> rightResultSelectorPropertiesToWatch)
			=> ActiveOuterJoin(source, join, leftKeySelector, rightKeySelector, resultSelector, Tuple.Create(leftKeySelectorPropertiesToWatch, rightKeySelectorPropertiesToWatch, leftResultSelectorPropertiesToWatch, rightResultSelectorPropertiesToWatch));

		private static IActiveList<TResult> ActiveOuterJoin<TLeft, TRight, TResult, TKey>(this IActiveList<TLeft> source, IEnumerable<TRight> join, Func<TLeft, TKey> leftKeySelector, Func<TRight, TKey> rightKeySelector, Func<JoinOption<TLeft>, JoinOption<TRight>, TResult> resultSelector, Tuple<IEnumerable<string>, IEnumerable<string>, IEnumerable<string>, IEnumerable<string>> propertiesToWatch)
			=> new ActiveJoin<TLeft, TRight, TResult, TKey, object>(ActiveListJoinBehaviour.Outer, source, join.ToReadOnlyList(), leftKeySelector, rightKeySelector, resultSelector, propertiesToWatch.Item1, propertiesToWatch.Item2, propertiesToWatch.Item3, propertiesToWatch.Item4);

		public static IActiveList<TResult> ActiveOuterJoin<TLeft, TRight, TResult, TKey, TParameter>(this IActiveList<TLeft> source, IEnumerable<TRight> join, IActiveValue<TParameter> parameter, Expression<Func<TLeft, TParameter, TKey>> leftKeySelector, Expression<Func<TRight, TParameter, TKey>> rightKeySelector, Expression<Func<JoinOption<TLeft>, JoinOption<TRight>, TParameter, TResult>> resultSelector)
			=> ActiveOuterJoin(source, join, parameter, leftKeySelector.Compile(), rightKeySelector.Compile(), resultSelector.Compile(), CreateJoinPropertiesToWatchTuple(leftKeySelector.GetReferencedProperties(), rightKeySelector.GetReferencedProperties(), resultSelector.GetReferencedProperties()));

		public static IActiveList<TResult> ActiveOuterJoin<TLeft, TRight, TResult, TKey, TParameter>(this IActiveList<TLeft> source, IEnumerable<TRight> join, IActiveValue<TParameter> parameter, Func<TLeft, TParameter, TKey> leftKeySelector, Func<TRight, TParameter, TKey> rightKeySelector, Func<JoinOption<TLeft>, JoinOption<TRight>, TParameter, TResult> resultSelector, IEnumerable<string> leftKeySelectorPropertiesToWatch, IEnumerable<string> rightKeySelectorPropertiesToWatch, IEnumerable<string> leftResultSelectorPropertiesToWatch, IEnumerable<string> rightResultSelectorPropertiesToWatch, IEnumerable<string> leftKeySelectorParameterPropertiesToWatch, IEnumerable<string> rightKeySelectorParameterPropertiesToWatch, IEnumerable<string> resultSelectorParameterPropertiesToWatch)
			=> ActiveOuterJoin(source, join, parameter, leftKeySelector, rightKeySelector, resultSelector, Tuple.Create(leftKeySelectorPropertiesToWatch, rightKeySelectorPropertiesToWatch, leftResultSelectorPropertiesToWatch, rightResultSelectorPropertiesToWatch, leftKeySelectorParameterPropertiesToWatch, rightKeySelectorParameterPropertiesToWatch, resultSelectorParameterPropertiesToWatch));

		private static IActiveList<TResult> ActiveOuterJoin<TLeft, TRight, TResult, TKey, TParameter>(this IActiveList<TLeft> source, IEnumerable<TRight> join, IActiveValue<TParameter> parameter, Func<TLeft, TParameter, TKey> leftKeySelector, Func<TRight, TParameter, TKey> rightKeySelector, Func<JoinOption<TLeft>, JoinOption<TRight>, TParameter, TResult> resultSelector, Tuple<IEnumerable<string>, IEnumerable<string>, IEnumerable<string>, IEnumerable<string>, IEnumerable<string>, IEnumerable<string>, IEnumerable<string>> propertiesToWatch)
			=> new ActiveJoin<TLeft, TRight, TResult, TKey, TParameter>(ActiveListJoinBehaviour.Outer, source, join.ToReadOnlyList(), parameter, leftKeySelector, rightKeySelector, resultSelector, propertiesToWatch.Item1, propertiesToWatch.Item2, propertiesToWatch.Item3, propertiesToWatch.Item4, propertiesToWatch.Item5, propertiesToWatch.Item6, propertiesToWatch.Item7);


		public static IActiveList<TResult> ActiveOuterExcludingJoin<TLeft, TRight, TResult, TKey>(this IActiveList<TLeft> source, IEnumerable<TRight> join, Expression<Func<TLeft, TKey>> leftKeySelector, Expression<Func<TRight, TKey>> rightKeySelector, Expression<Func<JoinOption<TLeft>, JoinOption<TRight>, TResult>> resultSelector)
			=> ActiveOuterExcludingJoin(source, join, leftKeySelector.Compile(), rightKeySelector.Compile(), resultSelector.Compile(), CreateJoinPropertiesToWatchTuple(leftKeySelector.GetReferencedProperties(), rightKeySelector.GetReferencedProperties(), resultSelector.GetReferencedProperties()));

		public static IActiveList<TResult> ActiveOuterExcludingJoin<TLeft, TRight, TResult, TKey>(this IActiveList<TLeft> source, IEnumerable<TRight> join, Func<TLeft, TKey> leftKeySelector, Func<TRight, TKey> rightKeySelector, Func<JoinOption<TLeft>, JoinOption<TRight>, TResult> resultSelector, IEnumerable<string> leftKeySelectorPropertiesToWatch, IEnumerable<string> rightKeySelectorPropertiesToWatch, IEnumerable<string> leftResultSelectorPropertiesToWatch, IEnumerable<string> rightResultSelectorPropertiesToWatch)
			=> ActiveOuterExcludingJoin(source, join, leftKeySelector, rightKeySelector, resultSelector, Tuple.Create(leftKeySelectorPropertiesToWatch, rightKeySelectorPropertiesToWatch, leftResultSelectorPropertiesToWatch, rightResultSelectorPropertiesToWatch));

		private static IActiveList<TResult> ActiveOuterExcludingJoin<TLeft, TRight, TResult, TKey>(this IActiveList<TLeft> source, IEnumerable<TRight> join, Func<TLeft, TKey> leftKeySelector, Func<TRight, TKey> rightKeySelector, Func<JoinOption<TLeft>, JoinOption<TRight>, TResult> resultSelector, Tuple<IEnumerable<string>, IEnumerable<string>, IEnumerable<string>, IEnumerable<string>> propertiesToWatch)
			=> new ActiveJoin<TLeft, TRight, TResult, TKey, object>(ActiveListJoinBehaviour.OuterExcluding, source, join.ToReadOnlyList(), leftKeySelector, rightKeySelector, resultSelector, propertiesToWatch.Item1, propertiesToWatch.Item2, propertiesToWatch.Item3, propertiesToWatch.Item4);

		public static IActiveList<TResult> ActiveOuterExcludingJoin<TLeft, TRight, TResult, TKey, TParameter>(this IActiveList<TLeft> source, IEnumerable<TRight> join, IActiveValue<TParameter> parameter, Expression<Func<TLeft, TParameter, TKey>> leftKeySelector, Expression<Func<TRight, TParameter, TKey>> rightKeySelector, Expression<Func<JoinOption<TLeft>, JoinOption<TRight>, TParameter, TResult>> resultSelector)
			=> ActiveOuterExcludingJoin(source, join, parameter, leftKeySelector.Compile(), rightKeySelector.Compile(), resultSelector.Compile(), CreateJoinPropertiesToWatchTuple(leftKeySelector.GetReferencedProperties(), rightKeySelector.GetReferencedProperties(), resultSelector.GetReferencedProperties()));

		public static IActiveList<TResult> ActiveOuterExcludingJoin<TLeft, TRight, TResult, TKey, TParameter>(this IActiveList<TLeft> source, IEnumerable<TRight> join, IActiveValue<TParameter> parameter, Func<TLeft, TParameter, TKey> leftKeySelector, Func<TRight, TParameter, TKey> rightKeySelector, Func<JoinOption<TLeft>, JoinOption<TRight>, TParameter, TResult> resultSelector, IEnumerable<string> leftKeySelectorPropertiesToWatch, IEnumerable<string> rightKeySelectorPropertiesToWatch, IEnumerable<string> leftResultSelectorPropertiesToWatch, IEnumerable<string> rightResultSelectorPropertiesToWatch, IEnumerable<string> leftKeySelectorParameterPropertiesToWatch, IEnumerable<string> rightKeySelectorParameterPropertiesToWatch, IEnumerable<string> resultSelectorParameterPropertiesToWatch)
			=> ActiveOuterExcludingJoin(source, join, parameter, leftKeySelector, rightKeySelector, resultSelector, Tuple.Create(leftKeySelectorPropertiesToWatch, rightKeySelectorPropertiesToWatch, leftResultSelectorPropertiesToWatch, rightResultSelectorPropertiesToWatch, leftKeySelectorParameterPropertiesToWatch, rightKeySelectorParameterPropertiesToWatch, resultSelectorParameterPropertiesToWatch));

		private static IActiveList<TResult> ActiveOuterExcludingJoin<TLeft, TRight, TResult, TKey, TParameter>(this IActiveList<TLeft> source, IEnumerable<TRight> join, IActiveValue<TParameter> parameter, Func<TLeft, TParameter, TKey> leftKeySelector, Func<TRight, TParameter, TKey> rightKeySelector, Func<JoinOption<TLeft>, JoinOption<TRight>, TParameter, TResult> resultSelector, Tuple<IEnumerable<string>, IEnumerable<string>, IEnumerable<string>, IEnumerable<string>, IEnumerable<string>, IEnumerable<string>, IEnumerable<string>> propertiesToWatch)
			=> new ActiveJoin<TLeft, TRight, TResult, TKey, TParameter>(ActiveListJoinBehaviour.OuterExcluding, source, join.ToReadOnlyList(), parameter, leftKeySelector, rightKeySelector, resultSelector, propertiesToWatch.Item1, propertiesToWatch.Item2, propertiesToWatch.Item3, propertiesToWatch.Item4, propertiesToWatch.Item5, propertiesToWatch.Item6, propertiesToWatch.Item7);


		public static IActiveList<TSource> ActiveOrderBy<TSource, TKey>(this IActiveList<TSource> source, Expression<Func<TSource, TKey>> keySelector, ListSortDirection sortDirection = ListSortDirection.Ascending) 
			where TKey : IComparable<TKey> 
			=> ActiveOrderBy(source, keySelector.Compile(), keySelector.GetReferencedProperties(), new ActiveValueWrapper<ListSortDirection>(sortDirection));

		public static IActiveList<TSource> ActiveOrderBy<TSource, TKey>(this IActiveList<TSource> source, Func<TSource, TKey> keySelector, IEnumerable<string> propertiesToWatch, ListSortDirection sortDirection = ListSortDirection.Ascending) 
			where TKey : IComparable<TKey> 
			=> ActiveOrderBy(source, keySelector, propertiesToWatch, new ActiveValueWrapper<ListSortDirection>(sortDirection));

		public static IActiveList<TSource> ActiveOrderBy<TSource, TKey>(this IActiveList<TSource> source, Expression<Func<TSource, TKey>> keySelector, IActiveValue<ListSortDirection> sortDirection) 
			where TKey : IComparable<TKey> 
			=> ActiveOrderBy(source, keySelector.Compile(), keySelector.GetReferencedProperties(), sortDirection);

		public static IActiveList<TSource> ActiveOrderBy<TSource, TKey>(this IActiveList<TSource> source, Func<TSource, TKey> keySelector, IEnumerable<string> propertiesToWatch, IActiveValue<ListSortDirection> sortDirection) 
			where TKey : IComparable<TKey> 
			=> new ActiveOrderBy<TSource, TKey>(source, keySelector, sortDirection, propertiesToWatch);


		public static IActiveList<TResult> ActiveZip<TFirst, TSecond, TResult>(this IActiveList<TFirst> source, IEnumerable<TSecond> otherSource, Expression<Func<TFirst, TSecond, TResult>> resultSelector)
			=> ActiveZip(source, otherSource, resultSelector.Compile(), resultSelector.GetReferencedProperties());

		public static IActiveList<TResult> ActiveZip<TFirst, TSecond, TResult>(this IActiveList<TFirst> source, IEnumerable<TSecond> otherSource, Func<TFirst, TSecond, TResult> resultSelector, IEnumerable<string> sourcePropertiesToWatch, IEnumerable<string> otherSourcePropertiesToWatch)
			=> ActiveZip(source, otherSource, resultSelector, Tuple.Create(sourcePropertiesToWatch, otherSourcePropertiesToWatch));

		private static IActiveList<TResult> ActiveZip<TFirst, TSecond, TResult>(this IActiveList<TFirst> source, IEnumerable<TSecond> otherSource, Func<TFirst, TSecond, TResult> resultSelector, Tuple<IEnumerable<string>, IEnumerable<string>> propertiesToWatch)
			=> new ActiveZip<TFirst, TSecond, object, TResult>(source, otherSource.ToReadOnlyList(), resultSelector, propertiesToWatch.Item1, propertiesToWatch.Item2);

		public static IActiveList<TResult> ActiveZip<TFirst, TSecond, TParameter, TResult>(this IActiveList<TFirst> source, IEnumerable<TSecond> otherSource, IActiveValue<TParameter> parameter, Expression<Func<TFirst, TSecond, TParameter, TResult>> resultSelector)
			=> ActiveZip(source, otherSource, parameter, resultSelector.Compile(), resultSelector.GetReferencedProperties());

		public static IActiveList<TResult> ActiveZip<TFirst, TSecond, TParameter, TResult>(this IActiveList<TFirst> source, IEnumerable<TSecond> otherSource, IActiveValue<TParameter> parameter, Func<TFirst, TSecond, TParameter, TResult> resultSelector, IEnumerable<string> sourcePropertiesToWatch, IEnumerable<string> otherSourcePropertiesToWatch, IEnumerable<string> parameterPropertiesToWatch)
			=> ActiveZip(source, otherSource, parameter, resultSelector, Tuple.Create(sourcePropertiesToWatch, otherSourcePropertiesToWatch, parameterPropertiesToWatch));

		private static IActiveList<TResult> ActiveZip<TFirst, TSecond, TParameter, TResult>(this IActiveList<TFirst> source, IEnumerable<TSecond> otherSource, IActiveValue<TParameter> parameter, Func<TFirst, TSecond, TParameter, TResult> resultSelector, Tuple<IEnumerable<string>, IEnumerable<string>, IEnumerable<string>> propertiesToWatch)
			=> new ActiveZip<TFirst, TSecond, TParameter, TResult>(source, otherSource.ToReadOnlyList(), resultSelector, parameter, propertiesToWatch.Item1, propertiesToWatch.Item2, propertiesToWatch.Item3);


		public static IActiveList<IActiveGrouping<TKey, TSource>> ActiveGroupBy<TKey, TSource>(this IActiveList<TSource> source, Expression<Func<TSource, TKey>> keySelector)
			=> ToActiveLookup(source, keySelector.Compile(), keySelector.GetReferencedProperties());

		public static IActiveList<IActiveGrouping<TKey, TSource>> ActiveGroupBy<TKey, TSource>(this IActiveList<TSource> source, Func<TSource, TKey> keySelector, IEnumerable<string> propertiesToWatch)
			=> ToActiveLookup(source, keySelector, propertiesToWatch);

		public static IActiveList<IActiveGrouping<TKey, TSource>> ActiveGroupBy<TKey, TSource, TParameter>(this IActiveList<TSource> source, IActiveValue<TParameter> parameter, Expression<Func<TSource, TParameter, TKey>> keySelector)
			=> ToActiveLookup(source, parameter, keySelector.Compile(), keySelector.GetReferencedProperties());

		public static IActiveList<IActiveGrouping<TKey, TSource>> ActiveGroupBy<TKey, TSource, TParameter>(this IActiveList<TSource> source, IActiveValue<TParameter> parameter, Func<TSource, TParameter, TKey> keySelector, IEnumerable<string> sourcePropertiesToWatch, IEnumerable<string> parameterPropertiesToWatch)
			=> ToActiveLookup(source, parameter, keySelector, sourcePropertiesToWatch, parameterPropertiesToWatch);


		public static IActiveLookup<TKey, TSource> ToActiveLookup<TKey, TSource>(this IActiveList<TSource> source, Expression<Func<TSource, TKey>> keySelector)
			=> ToActiveLookup(source, keySelector.Compile(), keySelector.GetReferencedProperties());

		public static IActiveLookup<TKey, TSource> ToActiveLookup<TKey, TSource>(this IActiveList<TSource> source, Func<TSource, TKey> keySelector, IEnumerable<string> propertiesToWatch)
			=> new ActiveLookup<TKey, TSource, object>(source, keySelector, propertiesToWatch);

		public static IActiveLookup<TKey, TSource> ToActiveLookup<TKey, TSource, TParameter>(this IActiveList<TSource> source, IActiveValue<TParameter> parameter, Expression<Func<TSource, TParameter, TKey>> keySelector)
			=> ToActiveLookup(source, parameter, keySelector.Compile(), keySelector.GetReferencedProperties());

		public static IActiveLookup<TKey, TSource> ToActiveLookup<TKey, TSource, TParameter>(this IActiveList<TSource> source, IActiveValue<TParameter> parameter, Func<TSource, TParameter, TKey> keySelector, IEnumerable<string> sourcePropertiesToWatch, IEnumerable<string> parameterPropertiesToWatch)
			=> ToActiveLookup(source, parameter, keySelector, Tuple.Create(sourcePropertiesToWatch, parameterPropertiesToWatch));

		private static IActiveLookup<TKey, TSource> ToActiveLookup<TKey, TSource, TParameter>(this IActiveList<TSource> source, IActiveValue<TParameter> parameter, Func<TSource, TParameter, TKey> keySelector, Tuple<IEnumerable<string>, IEnumerable<string>> propertiesToWatch)
			=> new ActiveLookup<TKey, TSource, TParameter>(source, keySelector, parameter, propertiesToWatch.Item1, propertiesToWatch.Item2);


		public static IActiveList<TSource> ActiveSelectByKey<TKey, TSource>(this IActiveLookup<TKey, TSource> source, TKey key)
			=> source[key];

		public static IActiveList<TSource> ActiveSelectByKey<TKey, TSource>(this IActiveLookup<TKey, TSource> source, IActiveValue<TKey> key)
			=> new ActiveGetOrDefault<TKey, TSource>(source, key).ToActiveList();


		public static IActiveList<TSource> ActiveTranslateResetNotifications<TSource>(this IActiveList<TSource> source)
			=> new ActiveTranslateResetNotifications<TSource>(source);


		private static Tuple<IEnumerable<string>, IEnumerable<string>, IEnumerable<string>, IEnumerable<string>> CreateJoinPropertiesToWatchTuple(IEnumerable<string> leftKeySelectorPropertiesToWatch, IEnumerable<string> rightKeySelectorPropertiesToWatch, Tuple<IEnumerable<string>, IEnumerable<string>> resultSelectorPropertiesToWatch)
			=> Tuple.Create(leftKeySelectorPropertiesToWatch, rightKeySelectorPropertiesToWatch, resultSelectorPropertiesToWatch.Item1, resultSelectorPropertiesToWatch.Item2);

		private static Tuple<IEnumerable<string>, IEnumerable<string>, IEnumerable<string>, IEnumerable<string>, IEnumerable<string>, IEnumerable<string>, IEnumerable<string>> CreateJoinPropertiesToWatchTuple(Tuple<IEnumerable<string>, IEnumerable<string>> leftKeySelectorPropertiesToWatch, Tuple<IEnumerable<string>, IEnumerable<string>> rightKeySelectorPropertiesToWatch, Tuple<IEnumerable<string>, IEnumerable<string>, IEnumerable<string>> resultSelectorPropertiesToWatch)
			=> Tuple.Create(leftKeySelectorPropertiesToWatch.Item1, rightKeySelectorPropertiesToWatch.Item1, resultSelectorPropertiesToWatch.Item1, resultSelectorPropertiesToWatch.Item2, leftKeySelectorPropertiesToWatch.Item2, rightKeySelectorPropertiesToWatch.Item2, resultSelectorPropertiesToWatch.Item3);
	}
}