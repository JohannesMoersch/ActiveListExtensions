using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ActiveListExtensions.Utilities
{
	internal class ActiveListJoinerData<TLeft, TRight, TResult, TKey, TParameter> : IDisposable
	{
		public int SourceIndex { get; set; }

		public int Offset { get; set; }

		public int Count { get; set; }

		public ActiveListJoiner<TLeft, TRight, TResult, TParameter> Joiner { get; }

		public TKey Key { get; }

		public bool IsLeftJoiner { get; }

		public ActiveListJoinerData(bool isLeftJoiner, ActiveListJoinBehaviour joinBehaviour, TKey key, IActiveValue<TParameter> parameter, Func<TLeft, TRight, TParameter, TResult> resultSelector, IEnumerable<string> leftResultSelectorPropertiesToWatch, IEnumerable<string> rightResultSelectorPropertiesToWatch, IEnumerable<string> parameterPropertiesToWatch)
		{
			Key = key;

			IsLeftJoiner = isLeftJoiner;

			Joiner = CreateJoiner(joinBehaviour, parameter, resultSelector, leftResultSelectorPropertiesToWatch, rightResultSelectorPropertiesToWatch, parameterPropertiesToWatch);
		}

		public void Set(TLeft left)
			=> Joiner.SetLeft(left);

		public void Set(IReadOnlyList<TRight> right)
			=> Joiner.SetRight(right);

		public void Set(TLeft left, IReadOnlyList<TRight> right)
			=> Joiner.SetBoth(left, right);

		public void Clear()
			=> Joiner.ClearBoth();

		public void Dispose()
			=> Joiner.Dispose();

		public int GetTargetIndex(int leftJoinerCount)
			=> IsLeftJoiner ? SourceIndex : SourceIndex + leftJoinerCount;

		private ActiveListJoiner<TLeft, TRight, TResult, TParameter> CreateJoiner(ActiveListJoinBehaviour joinBehaviour, IActiveValue<TParameter> parameter, Func<TLeft, TRight, TParameter, TResult> resultSelector, IEnumerable<string> leftResultSelectorPropertiesToWatch, IEnumerable<string> rightResultSelectorPropertiesToWatch, IEnumerable<string> parameterPropertiesToWatch)
			=> new ActiveListJoiner<TLeft, TRight, TResult, TParameter>(joinBehaviour, parameter, resultSelector, leftResultSelectorPropertiesToWatch, rightResultSelectorPropertiesToWatch, parameterPropertiesToWatch);
	}
}
