using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ActiveListExtensions.Tests.Helpers
{
	public static class LinqJoinExtensions
	{
		public static IEnumerable<TResult> InnerJoin<TLeft, TRight, TKey, TResult>(this IEnumerable<TLeft> source, IEnumerable<TRight> join, Func<TLeft, TKey> leftKeySelector, Func<TRight, TKey> rightKeySelector, Func<TLeft, TRight, TResult> resultSelector)
			=> source.Join(join, leftKeySelector, rightKeySelector, resultSelector).ToArray();

		public static IEnumerable<TResult> LeftJoin<TLeft, TRight, TKey, TResult>(this IEnumerable<TLeft> source, IEnumerable<TRight> join, Func<TLeft, TKey> leftKeySelector, Func<TRight, TKey> rightKeySelector, Func<TLeft, JoinOption<TRight>, TResult> resultSelector)
			=> source.GroupJoin(join, leftKeySelector, rightKeySelector, (l, rs) => rs.Select(r => JoinOption.Some(r)).DefaultIfEmpty(JoinOption.None<TRight>()).Select(r => resultSelector.Invoke(l, r))).SelectMany(o => o).ToArray();

		public static IEnumerable<TResult> LeftExcludingJoin<TLeft, TRight, TKey, TResult>(this IEnumerable<TLeft> source, IEnumerable<TRight> join, Func<TLeft, TKey> leftKeySelector, Func<TRight, TKey> rightKeySelector, Func<TLeft, JoinOption<TRight>, TResult> resultSelector)
			=> source.GroupJoin(join, leftKeySelector, rightKeySelector, (l, rs) => rs.DefaultIfEmpty().Where(o => o == null).Select(_ => resultSelector.Invoke(l, JoinOption.None<TRight>()))).SelectMany(o => o).ToArray();

		public static IEnumerable<TResult> RightJoin<TLeft, TRight, TKey, TResult>(this IEnumerable<TLeft> source, IEnumerable<TRight> join, Func<TLeft, TKey> leftKeySelector, Func<TRight, TKey> rightKeySelector, Func<JoinOption<TLeft>, TRight, TResult> resultSelector)
			=> source.InnerJoin(join, leftKeySelector, rightKeySelector, (l, r) => resultSelector.Invoke(JoinOption.Some(l), r)).Concat(source.RightExcludingJoin(join, leftKeySelector, rightKeySelector, resultSelector));

		public static IEnumerable<TResult> RightExcludingJoin<TLeft, TRight, TKey, TResult>(this IEnumerable<TLeft> source, IEnumerable<TRight> join, Func<TLeft, TKey> leftKeySelector, Func<TRight, TKey> rightKeySelector, Func<JoinOption<TLeft>, TRight, TResult> resultSelector)
			=> join.GroupBy(rightKeySelector).SelectMany(g => g).GroupJoin(source, rightKeySelector, leftKeySelector, (r, ls) => ls.DefaultIfEmpty().Where(o => o == null).Select(_ => resultSelector.Invoke(JoinOption.None<TLeft>(), r))).SelectMany(o => o).ToArray();

		public static IEnumerable<TResult> OuterJoin<TLeft, TRight, TKey, TResult>(this IEnumerable<TLeft> source, IEnumerable<TRight> join, Func<TLeft, TKey> leftKeySelector, Func<TRight, TKey> rightKeySelector, Func<JoinOption<TLeft>, JoinOption<TRight>, TResult> resultSelector)
			=> source.LeftJoin(join, leftKeySelector, rightKeySelector, (l, r) => resultSelector.Invoke(JoinOption.Some(l), r)).Concat(source.RightExcludingJoin(join, leftKeySelector, rightKeySelector, (l, r) => resultSelector.Invoke(l, JoinOption.Some(r))));

		public static IEnumerable<TResult> OuterExcludingJoin<TLeft, TRight, TKey, TResult>(this IEnumerable<TLeft> source, IEnumerable<TRight> join, Func<TLeft, TKey> leftKeySelector, Func<TRight, TKey> rightKeySelector, Func<JoinOption<TLeft>, JoinOption<TRight>, TResult> resultSelector)
			=> source.LeftExcludingJoin(join, leftKeySelector, rightKeySelector, (l, r) => resultSelector.Invoke(JoinOption.Some(l), r)).Concat(source.RightExcludingJoin(join, leftKeySelector, rightKeySelector, (l, r) => resultSelector.Invoke(l, JoinOption.Some(r))));
	}
}
