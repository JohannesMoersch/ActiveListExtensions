using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ActiveListExtensions.Reactive
{
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
}
