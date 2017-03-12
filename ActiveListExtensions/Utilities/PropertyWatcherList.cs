using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ActiveListExtensions.Utilities
{
	internal class PropertyWatcherList<T> : IDisposable
	{
		private class ValueSet
		{
			public object Key { get; }
			public T Value { get; }
			public EventHandler<PropertyChangedEventArgs> Handler { get; }

			public ValueSet(object key, T value, EventHandler<PropertyChangedEventArgs> handler)
			{
				Key = key;
				Value = value;
				Handler = handler;
			}
		}

		private string[] _propertiesToWatch;

		private IDictionary<object, int> _keyToIndexMap = new Dictionary<object, int>();
		private IList<ValueSet> _data = new List<ValueSet>();

		private bool _isDisposed;

		public PropertyWatcherList(string[] propertiesToWatch)
		{
			if (!typeof(INotifyPropertyChanged).IsAssignableFrom(typeof(T)))
				throw new ArgumentException($"\"{typeof(T).Name}\" does not derive from {nameof(INotifyPropertyChanged)}.");
			_propertiesToWatch = propertiesToWatch;
		}

		public void Dispose()
		{
			if (_isDisposed)
				return;
			_isDisposed = true;
			Reset(Enumerable.Empty<T>());
			_propertiesToWatch = null;
			_keyToIndexMap = null;
			_data = null;
		}

		public void Add(int index, T item)
		{
			var key = new object();
			var handler = new EventHandler<PropertyChangedEventArgs>((o, e) => ItemPropertyChanged(key, e));
			_data.Insert(index, new ValueSet(key, item, handler));
			for (int i = index + 1; i < _data.Count; ++i)
				_keyToIndexMap[_data[i].Key] = i;
			_keyToIndexMap.Add(key, index);
			if (item is INotifyPropertyChanged)
			{
				foreach (var property in _propertiesToWatch)
					PropertyChangedEventManager.AddHandler(item as INotifyPropertyChanged, handler, property);
			}
		}

		public void Remove(int index)
		{
			var set = _data[index];
			_data.RemoveAt(index);
			for (int i = index; i < _data.Count; ++i)
				_keyToIndexMap[_data[i].Key] = i;
			_keyToIndexMap.Remove(set.Key);
			if (set.Value is INotifyPropertyChanged)
			{
				foreach (var property in _propertiesToWatch)
					PropertyChangedEventManager.RemoveHandler(set.Value as INotifyPropertyChanged, set.Handler, property);
			}
		}

		public void Reset(IEnumerable<T> newValues)
		{
			for (int i = 0; i < _data.Count; ++i)
			{
				foreach (var property in _propertiesToWatch)
					PropertyChangedEventManager.RemoveHandler(_data[i].Value as INotifyPropertyChanged, _data[i].Handler, property);
			}
			_data.Clear();
			_keyToIndexMap.Clear();
			int index = 0;
			foreach (var item in newValues)
				Add(index++, item);
		}

		private void ItemPropertyChanged(object key, PropertyChangedEventArgs args)
		{
			int index;
			if (_keyToIndexMap.TryGetValue(key, out index))
				_valueChanged?.Invoke(index, _data[index].Value);
		}

		private Action<int, T> _valueChanged;
		public event Action<int, T> ValueChanged
		{
			add
			{
				if (!_isDisposed)
					_valueChanged += value;
			}
			remove
			{
				if (!_isDisposed)
					_valueChanged -= value;
			}
		}
	}
}
