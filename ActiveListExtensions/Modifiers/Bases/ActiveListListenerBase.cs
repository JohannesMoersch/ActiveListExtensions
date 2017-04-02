using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ActiveListExtensions.Utilities;
using System.ComponentModel;

namespace ActiveListExtensions.Modifiers.Bases
{
	internal abstract class ActiveListListenerBase<TSource, TResult> : ActiveListListenerBase<TSource, object, TResult>
	{
		public ActiveListListenerBase(IActiveList<TSource> source, IEnumerable<string> propertiesToWatch = null)
			: base(source, propertiesToWatch)
		{
		}
	}

	internal abstract class ActiveListListenerBase<TSource, TParameter, TResult> : ActiveBase<TSource, TResult>
	{
		private CollectionWrapper<TSource> _sourceList;

		protected IReadOnlyList<TSource> SourceList => _sourceList;

		private IEnumerable<string> _parameterPropertiesToWatch;

		private IActiveValue<TParameter> _parameter;

		private TParameter _parameterValue;
		private TParameter ParameterValue
		{
			get => _parameterValue;
			set
			{
				if (_parameterValue is INotifyPropertyChanged oldPropertyChangedSource && _parameterPropertiesToWatch != null)
				{
					foreach (var propertyName in _parameterPropertiesToWatch)
						PropertyChangedEventManager.RemoveHandler(oldPropertyChangedSource, SourcePropertyChanged, propertyName);
				}

				_parameterValue = value;

				if (_parameterValue is INotifyPropertyChanged newPropertyChangedSource && _parameterPropertiesToWatch != null)
				{
					foreach (var propertyName in _parameterPropertiesToWatch)
						PropertyChangedEventManager.AddHandler(newPropertyChangedSource, SourcePropertyChanged, propertyName);
				}
			}
		}

		private void SourcePropertyChanged(object key, PropertyChangedEventArgs args) => OnReset(SourceList);

		public ActiveListListenerBase(IActiveList<TSource> source, IEnumerable<string> propertiesToWatch = null)
			: this(source, null, propertiesToWatch)
		{
		}

		public ActiveListListenerBase(IActiveList<TSource> source, IActiveValue<TParameter> parameter, IEnumerable<string> sourcePropertiesToWatch = null, IEnumerable<string> parameterPropertiesToWatch = null)
		{
			_parameter = parameter;
			_parameterPropertiesToWatch = parameterPropertiesToWatch;

			_sourceList = new CollectionWrapper<TSource>(source, sourcePropertiesToWatch?.ToArray());
			_sourceList.ItemModified += (s, i, v) => ItemModified(i, v);
			_sourceList.ItemAdded += (s, i, v) => OnAdded(i, v);
			_sourceList.ItemRemoved += (s, i, v) => OnRemoved(i, v);
			_sourceList.ItemReplaced += (s, i, o, n) => OnReplaced(i, o, n);
			_sourceList.ItemMoved += (s, o, n, v) => OnMoved(o, n, v);
			_sourceList.ItemsReset += s => OnReset(s);

			if (_parameter != null)
			{
				PropertyChangedEventManager.AddHandler(_parameter, SourceChanged, nameof(IActiveValue<TParameter>.Value));
				ParameterValue = _parameter.Value;
			}
		}

		protected void Initialize() => _sourceList.Reset();

		protected override void OnDisposed()
		{
			_sourceList.Dispose();

			if (_parameter != null)
			{
				PropertyChangedEventManager.RemoveHandler(_parameter, SourceChanged, nameof(IActiveValue<TParameter>.Value));
				ParameterValue = default(TParameter);
			}
		}

		private void SourceChanged(object key, PropertyChangedEventArgs args)
		{
			if (!IsDisposed)
			{
				ParameterValue = _parameter.Value;
				OnReset(SourceList);
			}
		}

		protected abstract void OnAdded(int index, TSource value);

		protected abstract void OnRemoved(int index, TSource value);

		protected abstract void OnReplaced(int index, TSource oldValue, TSource newValue);

		protected abstract void OnMoved(int oldIndex, int newIndex, TSource value);

		protected abstract void OnReset(IReadOnlyList<TSource> newItems);

		protected virtual void ItemModified(int index, TSource value) { }
	}
}
