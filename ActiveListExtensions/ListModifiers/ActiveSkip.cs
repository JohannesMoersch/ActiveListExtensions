using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ActiveListExtensions.ListModifiers.Bases;

namespace ActiveListExtensions.ListModifiers
{
	internal class ActiveSkip<TSource> : ActiveListListenerBase<TSource, object, TSource>
	{
		private int _lastCount;
		public override int Count
		{
			get
			{
				var count = SourceList.Count > _currentSkipCount ? SourceList.Count - _currentSkipCount : 0;
				if (count > _skipIndex)
					--count;
				return count;
			}
		}

		private int _skipIndex = int.MaxValue;

		public override TSource this[int index]
		{
			get
			{
				if (index < 0)
					throw new ArgumentOutOfRangeException(nameof(index));
				if (index >= _skipIndex)
					++index;
				return SourceList[index + _currentSkipCount];
			}
		}

		private int _currentSkipCount;

		private IActiveValue<int> _skipCount;

		public ActiveSkip(IActiveList<TSource> source, IActiveValue<int> count) 
			: base(source)
		{
			_skipCount = count;

			PropertyChangedEventManager.AddHandler(_skipCount, SkipCountChanged, nameof(IActiveValue<int>.Value));

			UpdateSkipCount();

			Initialize();
		}

		protected override void OnDisposed()
		{
			PropertyChangedEventManager.RemoveHandler(_skipCount, SkipCountChanged, nameof(IActiveValue<int>.Value));
			base.OnDisposed();
		}

		private void SkipCountChanged(object key, PropertyChangedEventArgs args)
		{
			if (_skipCount.Value > _currentSkipCount)
			{
				var start = _currentSkipCount < SourceList.Count ? _currentSkipCount : SourceList.Count - 1;

				var max = _skipCount.Value < SourceList.Count ? _skipCount.Value : SourceList.Count;

				for (int i = start; i < max; ++i)
				{
					_currentSkipCount = i + 1;
					NotifyOfCollectionChange(new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Remove, SourceList[i], 0));
				}
			}
			else
			{
				var start = _currentSkipCount - 1 < SourceList.Count ? _currentSkipCount - 1 : SourceList.Count - 1;

				var min = _skipCount.Value >= 0 ? _skipCount.Value : 0;

				for (int i = start; i >= min; --i)
				{
					_currentSkipCount = i;
					NotifyOfCollectionChange(new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Add, SourceList[i], 0));
				}
			}
			UpdateSkipCount();
		}

		private void UpdateSkipCount() => _currentSkipCount = _skipCount.Value >= 0 ? _skipCount.Value : 0;

		private void UpdateCount()
		{
			if (_lastCount == Count)
				return;
			_lastCount = Count;
			NotifyOfPropertyChange(new PropertyChangedEventArgs(nameof(Count)));
		}

		protected override void OnAdded(int index, TSource value)
		{
			if (index >= _currentSkipCount)
				NotifyOfCollectionChange(new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Add, value, index - _currentSkipCount));
			else if (SourceList.Count > _currentSkipCount)
				NotifyOfCollectionChange(new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Add, SourceList[_currentSkipCount], 0));
			UpdateCount();
		}

		protected override void OnRemoved(int index, TSource value)
		{
			if (index >= _currentSkipCount)
				NotifyOfCollectionChange(new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Remove, value, index - _currentSkipCount));
			else if (SourceList.Count >= _currentSkipCount)
				NotifyOfCollectionChange(new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Remove, SourceList[_currentSkipCount - 1], 0));
			UpdateCount();
		}

		protected override void OnReplaced(int index, TSource oldValue, TSource newValue)
		{
			if (index >= _currentSkipCount)
				NotifyOfCollectionChange(new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Replace, newValue, oldValue, index - _currentSkipCount));
		}

		protected override void OnMoved(int oldIndex, int newIndex, TSource value)
		{
			if (oldIndex >= _currentSkipCount && newIndex >= _currentSkipCount)
				NotifyOfCollectionChange(new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Move, value, newIndex - _currentSkipCount, oldIndex - _currentSkipCount));
			else if (oldIndex >= _currentSkipCount || newIndex >= _currentSkipCount)
			{
				_skipIndex = newIndex - _currentSkipCount;
				try { OnRemoved(oldIndex, value); }
				finally { _skipIndex = int.MaxValue; }
				OnAdded(newIndex, value);
			}
			UpdateCount();
		}

		protected override void OnReset(IReadOnlyList<TSource> newItems)
		{
			NotifyOfCollectionChange(new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Reset));
			UpdateCount();
		}

		private IEnumerable<TSource> EnumerateCollection()
		{
			for (int i = 0; i < Count; ++i)
				yield return this[i];
		}

		public override IEnumerator<TSource> GetEnumerator() => EnumerateCollection().GetEnumerator();
	}
}
