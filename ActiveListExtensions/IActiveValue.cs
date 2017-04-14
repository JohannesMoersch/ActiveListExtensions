using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ActiveListExtensions
{
	public delegate void ActiveValueChangedEventHandler<in T>(IActiveValue<T> activeValue, IActiveValueChangedEventArgs<T> eventArgs);

	public interface IActiveValueChangedEventArgs<out T>
	{
		T OldValue { get; }

		T NewValue { get; }
	}

	internal class ActiveValueChangedEventArgs<T> : EventArgs, IActiveValueChangedEventArgs<T>
	{
		public T OldValue { get; }

		public T NewValue { get; }

		public ActiveValueChangedEventArgs(T oldValue, T newValue)
		{
			OldValue = oldValue;
			NewValue = newValue;
		}
	}

	public interface IActiveValue<out T> : INotifyPropertyChanged, IDisposable
	{
		T Value { get; }

		event ActiveValueChangedEventHandler<T> ValueChanged;
	}
}
