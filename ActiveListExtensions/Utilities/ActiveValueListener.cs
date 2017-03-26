using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ActiveListExtensions.Utilities
{
	internal class ActiveValueListener<TSource, TValue> : IActiveValue<TValue>
	{
		public TValue Value => _valueGetter.Invoke(_source);

		private TSource _source;

		private Func<TSource, TValue> _valueGetter;

		private string[] _propertiesToWatch;

		private bool _isDisposed;

		public ActiveValueListener(TSource source, Func<TSource, TValue> valueGetter, IEnumerable<string> propertiesToWatch)
		{
			_source = source;
			_valueGetter = valueGetter;
			_propertiesToWatch = propertiesToWatch.ToArray();

			if (_source is INotifyPropertyChanged propertyChangedSource)
			{
				foreach (var propertyName in _propertiesToWatch)
					PropertyChangedEventManager.AddHandler(propertyChangedSource, SourcePropertyChanged, propertyName);
			}
		}

		public void Dispose()
		{
			if (_isDisposed)
				return;

			_isDisposed = true;

			if (_source is INotifyPropertyChanged propertyChangedSource)
			{
				foreach (var propertyName in _propertiesToWatch)
					PropertyChangedEventManager.RemoveHandler(propertyChangedSource, SourcePropertyChanged, propertyName);
			}
		}

		private void SourcePropertyChanged(object key, PropertyChangedEventArgs args) => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(Value)));

		public event PropertyChangedEventHandler PropertyChanged;
	}
}
