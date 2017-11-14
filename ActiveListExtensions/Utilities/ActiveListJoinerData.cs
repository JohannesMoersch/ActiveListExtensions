using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ActiveListExtensions.Utilities
{
	internal class ActiveListJoinerData<TLeft, TRight, TResult, TKey> : IDisposable
	{
		public int SourceIndex { get; set; }

		public int Offset { get; set; }

		public int Count { get; set; }

		public ActiveListJoiner<TLeft, TRight, TResult> Joiner { get; }

		public TKey Key { get; }

		public bool IsLeftJoiner { get; }

		public ActiveListJoinerData(bool isLeftJoiner, ActiveListJoinBehaviour joinBehaviour, TKey key, Func<TLeft, TRight, TResult> resultSelector, IEnumerable<string> leftResultSelectorPropertiesToWatch, IEnumerable<string> rightResultSelectorPropertiesToWatch)
		{
			Key = key;

			IsLeftJoiner = isLeftJoiner;

			Joiner = CreateJoiner(joinBehaviour, resultSelector, leftResultSelectorPropertiesToWatch, rightResultSelectorPropertiesToWatch);
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

		private ActiveListJoiner<TLeft, TRight, TResult> CreateJoiner(ActiveListJoinBehaviour joinBehaviour, Func<TLeft, TRight, TResult> resultSelector, IEnumerable<string> leftResultSelectorPropertiesToWatch, IEnumerable<string> rightResultSelectorPropertiesToWatch)
			=> new ActiveListJoiner<TLeft, TRight, TResult>(joinBehaviour, resultSelector, leftResultSelectorPropertiesToWatch, rightResultSelectorPropertiesToWatch);
	}
}
