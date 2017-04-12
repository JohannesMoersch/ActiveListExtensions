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
	internal class ActiveTake<TSource> : ActiveListListenerBase<TSource, TSource>
	{
		private int _lastCount;
		public override int Count
		{
			get
			{
				var count = SourceList.Count > _currentTakeCount ? _currentTakeCount : SourceList.Count;
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
				if (index >= _currentTakeCount)
					throw new ArgumentOutOfRangeException(nameof(index));
				if (index >= _skipIndex)
					++index;
				return SourceList[index];
			}
		}

		private int _currentTakeCount;

		private IActiveValue<int> _takeCount;

		public ActiveTake(IActiveList<TSource> source, IActiveValue<int> count) 
			: base(source)
		{
			_takeCount = count;

			PropertyChangedEventManager.AddHandler(_takeCount, TakeCountChanged, nameof(IActiveValue<int>.Value));

			UpdateTakeCount();

			Initialize();
		}

		protected override void OnDisposed()
		{
			PropertyChangedEventManager.RemoveHandler(_takeCount, TakeCountChanged, nameof(IActiveValue<int>.Value));
			base.OnDisposed();
		}

		private void TakeCountChanged(object key, PropertyChangedEventArgs args)
		{
			if (_takeCount.Value > _currentTakeCount)
			{
				var start = _currentTakeCount < SourceList.Count ? _currentTakeCount : SourceList.Count - 1;

				var max = _takeCount.Value < SourceList.Count ? _takeCount.Value : SourceList.Count;

				for (int i = start; i < max; ++i)
				{
					_currentTakeCount = i + 1;
					NotifyOfCollectionChange(new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Add, SourceList[i], i));
				}
			}
			else
			{
				var start = _currentTakeCount - 1 < SourceList.Count ? _currentTakeCount - 1 : SourceList.Count - 1;

				var min = _takeCount.Value >= 0 ? _takeCount.Value : 0;

				for (int i = start; i >= min; --i)
				{
					_currentTakeCount = i;
					NotifyOfCollectionChange(new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Remove, SourceList[i], i));
				}
			}
			UpdateTakeCount();
		}

		private void UpdateTakeCount() => _currentTakeCount = _takeCount.Value >= 0 ? _takeCount.Value : 0;

		private void UpdateCount()
		{
			if (_lastCount == Count)
				return;
			_lastCount = Count;
			NotifyOfPropertyChange(new PropertyChangedEventArgs(nameof(Count)));
		}

		protected override void OnAdded(int index, TSource value)
		{
			if (index >= _currentTakeCount)
				return;
			if (SourceList.Count - 1 >= _currentTakeCount)
			{
				_skipIndex = index;
				try { NotifyOfCollectionChange(new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Remove, SourceList[_currentTakeCount], _currentTakeCount - 1)); }
				finally { _skipIndex = int.MaxValue; }
			}
			NotifyOfCollectionChange(new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Add, value, index));
			UpdateCount();
		}

		protected override void OnRemoved(int index, TSource value)
		{
			if (index >= _currentTakeCount)
				return;
			_skipIndex = _currentTakeCount - 1;
			try { NotifyOfCollectionChange(new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Remove, value, index)); }
			finally { _skipIndex = int.MaxValue; }
			if (SourceList.Count >= _currentTakeCount)
				NotifyOfCollectionChange(new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Add, SourceList[_currentTakeCount - 1], _currentTakeCount - 1));
			UpdateCount();
		}

		protected override void OnReplaced(int index, TSource oldValue, TSource newValue)
		{
			if (index < _currentTakeCount)
				NotifyOfCollectionChange(new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Replace, newValue, oldValue, index));
		}

		protected override void OnMoved(int oldIndex, int newIndex, TSource value)
		{
			if (oldIndex < _currentTakeCount)
			{
				if (newIndex < _currentTakeCount)
					NotifyOfCollectionChange(new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Move, value, newIndex, oldIndex));
				else
					OnRemoved(oldIndex, value);
			}
			else
				OnAdded(newIndex, value);
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
