using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ActiveListExtensions.Utilities;

namespace ActiveListExtensions.Modifiers.Bases
{
	internal abstract class ActiveSetBase<T, U> : ActiveMultiListListenerBase<T, T>
	{
		protected enum SetAction
		{
			None,
			Add,
			Remove
		}

		protected struct ResultSet
		{
			public readonly int Index;
			public readonly T Value;

			public ResultSet(int index, T value)
			{
				Index = index;
				Value = value;
			}
		}

		protected struct SourceSet
		{
			public readonly int Index;
			public readonly int Count;

			public SourceSet(int index, int count)
			{
				Index = index;
				Count = count;
			}
		}

		private int _nextIndex;

		public override int Count => ResultList.Count;

		public override T this[int index] => ResultList[index].Value;

		protected ObservableList<ResultSet> ResultList { get; }

		protected IDictionary<U, SourceSet> LeftSource { get; }

		protected IDictionary<U, SourceSet> RightSource { get; }

		private IList<KeyValuePair<U, T>> _leftValues;

		private IList<KeyValuePair<U, T>> _rightValues;

		private Func<T, U> _keySelector;

		public ActiveSetBase(IActiveList<T> leftSource, IActiveList<T> rightSource, Func<T, U> keySelector, IEnumerable<string> propertiesToWatch = null) 
			: base(leftSource, propertiesToWatch)
		{
			_leftValues = new List<KeyValuePair<U, T>>();
			_rightValues = new List<KeyValuePair<U, T>>();

			_keySelector = keySelector;
			LeftSource = new Dictionary<U, SourceSet>();

			ResultList = new ObservableList<ResultSet>();
			ResultList.CollectionChanged += (s, e) => NotifyOfCollectionChange(RewrapEventArgs(e));
			ResultList.PropertyChanged += (s, e) => NotifyOfPropertyChange(e);

			Initialize();
			if (rightSource != null)
			{
				RightSource = new Dictionary<U, SourceSet>();
				AddSourceCollection(0, rightSource, true);
			}
		}

		private NotifyCollectionChangedEventArgs RewrapEventArgs(NotifyCollectionChangedEventArgs args)
		{
			switch (args.Action)
			{
				case NotifyCollectionChangedAction.Add:
					return new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Add, ((ResultSet)args.NewItems[0]).Value, args.NewStartingIndex);
				case NotifyCollectionChangedAction.Remove:
					return new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Remove, ((ResultSet)args.OldItems[0]).Value, args.OldStartingIndex);
				case NotifyCollectionChangedAction.Replace:
					return new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Replace, ((ResultSet)args.NewItems[0]).Value, ((ResultSet)args.OldItems[0]).Value, args.NewStartingIndex);
				case NotifyCollectionChangedAction.Move:
					return new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Move, ((ResultSet)args.NewItems[0]).Value, args.NewStartingIndex, args.OldStartingIndex);
				case NotifyCollectionChangedAction.Reset:
					return new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Reset);
			}
			return null;
		}

		protected override void OnDisposed()
		{
			ResultList.Dispose();
			base.OnDisposed();
		}

		protected abstract SetAction OnAddedToLeft(bool existsInRight);

		protected abstract SetAction OnRemovedFromLeft(bool existsInRight);

		protected abstract SetAction OnAddedToRight(bool existsInLeft);

		protected abstract SetAction OnRemovedFromRight(bool existsInLeft);

		// ********************************* //
		// The problem is that when there    //
		// are multiple objects with the     //
		// same key, and the one that is     //
		// exposed has its key change, the   //
		// exposed object isn't replace by   //
		// another that still has that key.  //
		//                                   //
		// Insert them all into the result   //
		// list (duplicates and all), and    //
		// just assure duplicates share an   //
		// index. That way they will be side //
		// by side, and a custom enumerator  //
		// can just skip past the extras.    //
		// ********************************* //

		private bool AddToDictionary(IDictionary<U, SourceSet> dictionary, IDictionary<U, SourceSet> otherDictionary, U key, T value, out int index)
		{
			SourceSet sourceSet;
			if (!dictionary.TryGetValue(key, out sourceSet))
			{
				if (otherDictionary?.TryGetValue(key, out sourceSet) ?? false)
					index = sourceSet.Index;
				else
					index = ++_nextIndex;
				dictionary.Add(key, new SourceSet(index, 1));
				return true;
			}
			index = sourceSet.Index;
			dictionary[key] = new SourceSet(sourceSet.Index, sourceSet.Count + 1);
			return false;
		}

		private bool RemoveFromDictionary(IDictionary<U, SourceSet> dictionary, U key, T value, out int index)
		{
			index = -1;
			SourceSet sourceSet;
			if (!dictionary.TryGetValue(key, out sourceSet))
				return false;
			index = sourceSet.Index;
			if (sourceSet.Count > 1)
			{
				dictionary[key] = new SourceSet(index, sourceSet.Count - 1);
				return false;
			}
			dictionary.Remove(key);
			return true;
		}

		private void Add(U key, T value, IDictionary<U, SourceSet> addTo, IDictionary<U, SourceSet> other, Func<bool, SetAction> onAddedMethod)
		{
			int index;
			if (AddToDictionary(addTo, other, key, value, out index))
				DictionaryModified(key, value, index, addTo, other, onAddedMethod);
		}

		private void Remove(U key, T value, IDictionary<U, SourceSet> removeFrom, IDictionary<U, SourceSet> other, Func<bool, SetAction> onRemovedMethod)
		{
			int index;
			if (RemoveFromDictionary(removeFrom, key, value, out index))
				DictionaryModified(key, value, index, removeFrom, other, onRemovedMethod);
		}

		private void DictionaryModified(U key, T value, int index, IDictionary<U, SourceSet> dictionary, IDictionary<U, SourceSet> otherDictionary, Func<bool, SetAction> onChangeMethod)
		{
			switch (onChangeMethod.Invoke(otherDictionary?.ContainsKey(key) ?? false))
			{
				case SetAction.Add:
					index = ++_nextIndex;
					SourceSet sourceSet;
					if (dictionary.TryGetValue(key, out sourceSet))
						dictionary[key] = new SourceSet(index, sourceSet.Count);
					if (otherDictionary?.TryGetValue(key, out sourceSet) ?? false)
						otherDictionary[key] = new SourceSet(index, sourceSet.Count);
					ResultList.Add(ResultList.Count, new ResultSet(index, value));
					break;
				case SetAction.Remove:
					ResultList.Remove(FindByIndex(index));
					break;
			}
		}

		private int FindByIndex(int index)
		{
			var bottom = 0;
			var top = ResultList.Count - 1;
			while (bottom <= top)
			{
				var mid = bottom + (top - bottom) / 2;
				var midIndex = ResultList[mid].Index;
				if (midIndex == index)
					return mid;
				else if (midIndex < index)
					bottom = mid + 1;
				else
					top = mid - 1;
			}
			return -1;
		}

		protected override void OnAdded(int index, T value)
		{
			var key = _keySelector.Invoke(value);
			if (_leftValues != null)
				_leftValues.Insert(index, new KeyValuePair<U, T>(key, value));
			Add(key, value, LeftSource, RightSource, OnAddedToLeft);
		}

		protected override void OnAdded(int collectionIndex, int index, T value)
		{
			var key = _keySelector.Invoke(value);
			if (_rightValues != null)
				_rightValues.Insert(index, new KeyValuePair<U, T>(key, value));
			Add(key, value, RightSource, LeftSource, OnAddedToRight);
		}
		protected override void OnRemoved(int index, T value)
		{
			var key = _leftValues[index].Key;
			if (_leftValues != null)
				_leftValues.RemoveAt(index);
			Remove(key, value, LeftSource, RightSource, OnRemovedFromLeft);
		}

		protected override void OnRemoved(int collectionIndex, int index, T value)
		{
			var key = _rightValues[index].Key;
			if (_rightValues != null)
				_rightValues.RemoveAt(index);
			Remove(key, value, RightSource, LeftSource, OnRemovedFromRight);
		}

		protected override void OnReplaced(int index, T oldValue, T newValue)
		{
			var key = _leftValues[index].Key;
			Remove(key, oldValue, LeftSource, RightSource, OnRemovedFromLeft);
			key = _keySelector.Invoke(newValue);
			_leftValues[index] = new KeyValuePair<U, T>(key, newValue);
			Add(key, newValue, LeftSource, RightSource, OnAddedToLeft);
		}

		protected override void OnReplaced(int collectionIndex, int index, T oldValue, T newValue)
		{
			var key = _rightValues[index].Key;
			Remove(key, oldValue, RightSource, LeftSource, OnRemovedFromRight);
			key = _keySelector.Invoke(newValue);
			_rightValues[index] = new KeyValuePair<U, T>(key, newValue);
			Add(key, newValue, RightSource, LeftSource, OnAddedToRight);
		}

		protected override void OnMoved(int oldIndex, int newIndex, T value) { }

		protected override void OnMoved(int collectionIndex, int oldIndex, int newIndex, T value) { }

		protected override void OnReset(IReadOnlyList<T> newItems)
		{
			foreach (var value in _leftValues)
				Remove(value.Key, value.Value, LeftSource, RightSource, OnRemovedFromLeft);
			_leftValues.Clear();
			foreach (var value in newItems)
			{
				var key = _keySelector.Invoke(value);
				_leftValues.Add(new KeyValuePair<U, T>(key, value));
				Add(key, value, LeftSource, RightSource, OnAddedToLeft);
			}
		}

		protected override void OnReset(int collectionIndex, IReadOnlyList<T> newItems)
		{
			foreach (var value in _rightValues)
				Remove(value.Key, value.Value, RightSource, LeftSource, OnRemovedFromRight);
			RightSource.Clear();
			_rightValues.Clear();
			foreach (var value in newItems)
			{
				var key = _keySelector.Invoke(value);
				_rightValues.Add(new KeyValuePair<U, T>(key, value));
				Add(key, value, RightSource, LeftSource, OnAddedToRight);
			}
		}

		protected override void ItemModified(int index, T value)
		{
			var old = _leftValues[index];
			if ((int)(object)old.Key == 644)
				Console.WriteLine("A");
			Remove(old.Key, old.Value, LeftSource, RightSource, OnRemovedFromLeft);
			var key = _keySelector.Invoke(value);
			_leftValues[index] = new KeyValuePair<U, T>(key, value);
			Add(key, value, LeftSource, RightSource, OnAddedToLeft);
		}

		protected override void ItemModified(int collectionIndex, int index, T value)
		{
			var old = _rightValues[index];
			Remove(old.Key, old.Value, RightSource, LeftSource, OnRemovedFromRight);
			var key = _keySelector.Invoke(value);
			_rightValues[index] = new KeyValuePair<U, T>(key, value);
			Add(key, value, RightSource, LeftSource, OnAddedToRight);
		}

		public override IEnumerator<T> GetEnumerator() => ResultList.Select(v => v.Value).GetEnumerator();
	}
}
