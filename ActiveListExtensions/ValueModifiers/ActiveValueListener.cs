using ActiveListExtensions.ValueModifiers.Bases;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ActiveListExtensions.ValueModifiers
{
	internal class ActiveValueListener<TSource, TValue> : ActiveValueBase<TValue>
	{
		private TSource _source;

		private Func<TSource, TValue> _valueGetter;

		private string[] _propertiesToWatch;

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

		protected override void OnDisposed()
		{
			if (_source is INotifyPropertyChanged propertyChangedSource)
			{
				foreach (var propertyName in _propertiesToWatch)
					PropertyChangedEventManager.RemoveHandler(propertyChangedSource, SourcePropertyChanged, propertyName);
			}
		}

		private void SourcePropertyChanged(object key, PropertyChangedEventArgs args) => Value = _valueGetter.Invoke(_source);
	}
}
