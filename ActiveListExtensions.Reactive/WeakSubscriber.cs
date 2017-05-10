using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ActiveListExtensions.Reactive
{
	internal class WeakSubscriber<T> : IObserver<T>
	{
		private readonly WeakReference<IObserver<T>> _observer;

		public WeakSubscriber(IObserver<T> observer) => _observer = new WeakReference<IObserver<T>>(observer ?? throw new ArgumentNullException(nameof(observer)));

		public void OnCompleted()
		{
			if (_observer.TryGetTarget(out var target))
				target.OnCompleted();
		}

		public void OnError(Exception error)
		{
			if (_observer.TryGetTarget(out var target))
				target.OnError(error);
		}

		public void OnNext(T value)
		{
			if (_observer.TryGetTarget(out var target))
				target.OnNext(value);
		}
	}
}
