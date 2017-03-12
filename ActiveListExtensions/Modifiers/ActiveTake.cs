using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ActiveListExtensions.Modifiers.Bases;

namespace ActiveListExtensions.Modifiers
{
	internal class ActiveTake<T> : ActiveListListenerBase<T, T>
	{
		private int _lastCount;
		public override int Count
		{
			get
			{
				var count = SourceList.Count > _takeCount ? _takeCount : SourceList.Count;
				if (count > _skipIndex)
					--count;
				return count;
			}
		}

		private int _skipIndex = int.MaxValue;

		public override T this[int index]
		{
			get
			{
				if (index >= _takeCount)
					throw new ArgumentOutOfRangeException(nameof(index));
				if (index >= _skipIndex)
					++index;
				return SourceList[index];
			}
		}

		private int _takeCount;

		public ActiveTake(IActiveList<T> source, int count) 
			: base(source)
		{
			_takeCount = count;
			Initialize();
		}

		private void UpdateCount()
		{
			if (_lastCount == Count)
				return;
			_lastCount = Count;
			NotifyOfPropertyChange(new PropertyChangedEventArgs(nameof(Count)));
		}

		protected override void OnAdded(int index, T value)
		{
			if (index >= _takeCount)
				return;
			if (SourceList.Count - 1 >= _takeCount)
			{
				_skipIndex = index;
				try { NotifyOfCollectionChange(new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Remove, SourceList[_takeCount], _takeCount - 1)); }
				finally { _skipIndex = int.MaxValue; }
			}
			NotifyOfCollectionChange(new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Add, value, index));
			UpdateCount();
		}

		protected override void OnRemoved(int index, T value)
		{
			if (index >= _takeCount)
				return;
			_skipIndex = _takeCount - 1;
			try { NotifyOfCollectionChange(new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Remove, value, index)); }
			finally { _skipIndex = int.MaxValue; }
			if (SourceList.Count >= _takeCount)
				NotifyOfCollectionChange(new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Add, SourceList[_takeCount - 1], _takeCount - 1));
			UpdateCount();
		}

		protected override void OnReplaced(int index, T oldValue, T newValue)
		{
			if (index < _takeCount)
				NotifyOfCollectionChange(new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Replace, newValue, oldValue, index));
		}

		protected override void OnMoved(int oldIndex, int newIndex, T value)
		{
			if (oldIndex < _takeCount)
			{
				if (newIndex < _takeCount)
					NotifyOfCollectionChange(new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Move, value, newIndex, oldIndex));
				else
					OnRemoved(oldIndex, value);
			}
			else
				OnAdded(newIndex, value);
			UpdateCount();
		}

		protected override void OnReset(IReadOnlyList<T> newItems)
		{
			NotifyOfCollectionChange(new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Reset));
			UpdateCount();
		}

		private IEnumerable<T> EnumerateCollection()
		{
			for (int i = 0; i < Count; ++i)
				yield return this[i];
		}

		public override IEnumerator<T> GetEnumerator() => EnumerateCollection().GetEnumerator();
	}
}
