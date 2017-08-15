using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Data;

namespace ActiveListExtensions.Wpf
{
	internal class DependencyPropertyWatcher<T> : DependencyObject, IActiveValue<T>
	{
		public T Value
		{
			get => (T)GetValue(ValueProperty);
			set => SetValue(ValueProperty, value);
		}
		public static readonly DependencyProperty ValueProperty = DependencyProperty.Register(nameof(Value), typeof(T), typeof(DependencyPropertyWatcher<T>), new PropertyMetadata(default(T), (s, e) => (s as DependencyPropertyWatcher<T>).OnValueChanged((T)e.OldValue, (T)e.NewValue)));

		public DependencyPropertyWatcher(DependencyObject source, DependencyProperty property)
		{
			var binding = new Binding()
			{
				Source = source,
				Path = new PropertyPath(property),
				Mode = BindingMode.OneWay,
				UpdateSourceTrigger = UpdateSourceTrigger.PropertyChanged
			};

			BindingOperations.SetBinding(this, ValueProperty, binding);
		}

		public void Dispose() => BindingOperations.ClearBinding(this, ValueProperty);

		private void OnValueChanged(T oldValue, T newValue)
		{
			ValueChanged?.Invoke(this, new ActiveValueChangedEventArgs<T>(oldValue, newValue));

			PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(Value)));
		}
		
		public event PropertyChangedEventHandler PropertyChanged;
		public event ActiveValueChangedEventHandler<T> ValueChanged;
	}
}
