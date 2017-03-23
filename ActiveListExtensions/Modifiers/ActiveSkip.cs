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
	internal class ActiveSkip<TSource> : ActiveListListenerBase<TSource, TSource>
	{
		private int _lastCount;
		public override int Count
		{
			get
			{
				var count = SourceList.Count > _skipCount ? SourceList.Count - _skipCount : 0;
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
				return SourceList[index + _skipCount];
			}
		}

		private int _skipCount;

		public ActiveSkip(IActiveList<TSource> source, int count) 
			: base(source)
		{
			_skipCount = count;
			Initialize();
		}

		private void UpdateCount()
		{
			if (_lastCount == Count)
				return;
			_lastCount = Count;
			NotifyOfPropertyChange(new PropertyChangedEventArgs(nameof(Count)));
		}

		protected override void OnAdded(int index, TSource value)
		{
			if (index >= _skipCount)
				NotifyOfCollectionChange(new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Add, value, index - _skipCount));
			else if (SourceList.Count > _skipCount)
				NotifyOfCollectionChange(new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Add, SourceList[_skipCount], 0));
			UpdateCount();
		}

		protected override void OnRemoved(int index, TSource value)
		{
			if (index >= _skipCount)
				NotifyOfCollectionChange(new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Remove, value, index - _skipCount));
			else if (SourceList.Count >= _skipCount)
				NotifyOfCollectionChange(new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Remove, SourceList[_skipCount - 1], 0));
			UpdateCount();
		}

		protected override void OnReplaced(int index, TSource oldValue, TSource newValue)
		{
			if (index >= _skipCount)
				NotifyOfCollectionChange(new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Replace, newValue, oldValue, index - _skipCount));
		}

		protected override void OnMoved(int oldIndex, int newIndex, TSource value)
		{
			if (oldIndex >= _skipCount && newIndex >= _skipCount)
				NotifyOfCollectionChange(new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Move, value, newIndex - _skipCount, oldIndex - _skipCount));
			else if (oldIndex >= _skipCount || newIndex >= _skipCount)
			{
				_skipIndex = newIndex - _skipCount;
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
