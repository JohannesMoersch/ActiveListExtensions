using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ActiveListExtensions.Utilities
{
	internal class ActiveListJoinerSet<TLeft, TRight, TResult, TKey, TParameter> : IDisposable
	{
		private readonly ActiveListJoinBehaviour _joinBehaviour;
		private readonly IActiveList<TRight> _right;
		private readonly IActiveValue<TParameter> _parameter;
		private readonly Func<TLeft, TRight, TParameter, TResult> _resultSelector;
		private readonly IEnumerable<string> _leftResultSelectorPropertiesToWatch;
		private readonly IEnumerable<string> _rightResultSelectorPropertiesToWatch;
		private readonly IEnumerable<string> _parameterPropertiesToWatch;

		private readonly List<ActiveListJoinerData<TLeft, TRight, TResult, TKey, TParameter>> _leftJoiners = new List<ActiveListJoinerData<TLeft, TRight, TResult, TKey, TParameter>>();

		private ActiveListJoinerData<TLeft, TRight, TResult, TKey, TParameter> _rightJoiner;

		public TKey Key { get; }

		public bool HasRight { get; private set; }

		public int? RightSourceIndex => HasRight ? _rightJoiner.SourceIndex : (int?)null;

		public ActiveListJoinerSet(ActiveListJoinBehaviour joinBehaviour, TKey key, IActiveLookup<TKey, TRight> right, IActiveValue<TParameter> parameter, Func<TLeft, TRight, TParameter, TResult> resultSelector, IEnumerable<string> leftResultSelectorPropertiesToWatch, IEnumerable<string> rightResultSelectorPropertiesToWatch, IEnumerable<string> parameterPropertiesToWatch)
		{
			Key = key;

			_joinBehaviour = joinBehaviour;
			_right = right[key];
			_parameter = parameter;
			_resultSelector = resultSelector;
			_leftResultSelectorPropertiesToWatch = leftResultSelectorPropertiesToWatch;
			_rightResultSelectorPropertiesToWatch = rightResultSelectorPropertiesToWatch;
			_parameterPropertiesToWatch = parameterPropertiesToWatch;
		}

		public void Dispose()
		{
			foreach (var data in _leftJoiners)
				data.Dispose();
			_rightJoiner?.Dispose();

			_leftJoiners.Clear();

			_rightJoiner = null;
		}
		

		public void SetRight(int rightSourceIndex)
		{
			HasRight = true;

			_rightJoiner = CreateJoinerData(false);

			_rightJoiner.SourceIndex = rightSourceIndex;

			JoinerAdded?.Invoke(_rightJoiner);

			if (_leftJoiners.Count == 0)
				_rightJoiner.Set(_right);
		}

		public void ClearRight()
		{
			HasRight = false;

			var data = _rightJoiner;

			_rightJoiner = null;

			try
			{
				data.Clear();
			}
			finally
			{
				JoinerRemoved.Invoke(data);
				data.Dispose();
			}
		}

		public void AddLeft(int leftSourceIndex, TLeft left)
		{
			if (_leftJoiners.Count == 0)
				_rightJoiner?.Clear();

			var data = CreateJoinerData(true);

			data.SourceIndex = leftSourceIndex;

			_leftJoiners.Add(data);

			JoinerAdded?.Invoke(data);

			data.Set(left, _right);
		}

		public void ReplaceLeft(int leftSourceIndex, TLeft newLeft)
		{
			for (int i = 0; i < _leftJoiners.Count; ++i)
			{
				if (_leftJoiners[i].SourceIndex != leftSourceIndex)
					continue;

				_leftJoiners[i].Set(newLeft);

				break;
			}
		}

		public void RemoveLeft(int leftSourceIndex)
		{
			for (int i = 0; i < _leftJoiners.Count; ++i)
			{
				if (_leftJoiners[i].SourceIndex != leftSourceIndex)
					continue;

				var data = _leftJoiners[i];

				_leftJoiners.RemoveAt(i);

				RemoveLeft(data);

				break;
			}
		}

		private void RemoveLeft(ActiveListJoinerData<TLeft, TRight, TResult, TKey, TParameter> data)
		{
			try
			{
				data.Clear();
			}
			finally
			{
				JoinerRemoved.Invoke(data);
				data.Dispose();

				if (_leftJoiners.Count == 0)
					_rightJoiner?.Set(_right);
			}
		}

		public IDisposable ResetLeft()
		{
			var oldCount = _leftJoiners.Count;

			foreach (var left in _leftJoiners)
				RemoveLeft(left);

			_leftJoiners.Clear();

			return new OnDisposeAction(() => EndLeftReset(oldCount));
		}

		private void EndLeftReset(int oldCount)
		{
			if (oldCount > 0 && _leftJoiners.Count == 0)
				_rightJoiner?.Set(_right);
		}

		private ActiveListJoinerData<TLeft, TRight, TResult, TKey, TParameter> CreateJoinerData(bool isLeftJoiner)
			=> new ActiveListJoinerData<TLeft, TRight, TResult, TKey, TParameter>(isLeftJoiner, _joinBehaviour, Key, _parameter, _resultSelector, _leftResultSelectorPropertiesToWatch, _rightResultSelectorPropertiesToWatch, _parameterPropertiesToWatch);

		public event Action<ActiveListJoinerData<TLeft, TRight, TResult, TKey, TParameter>> JoinerAdded;

		public event Action<ActiveListJoinerData<TLeft, TRight, TResult, TKey, TParameter>> JoinerRemoved;

		public event Action<ActiveListJoinerSet<TLeft, TRight, TResult, TKey, TParameter>> SetEmptied;
	}
}
