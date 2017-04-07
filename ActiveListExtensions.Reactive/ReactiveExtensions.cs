using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Linq;
using System.Reactive.Subjects;
using System.Text;
using System.Threading.Tasks;

namespace ActiveListExtensions
{
    public static class ReactiveExtensions
    {
		public static IObservable<T> ObserveValue<T>(this IActiveValue<T> value)
		{
			var subject = new ReplaySubject<T>(1);

			var handler = new EventHandler<PropertyChangedEventArgs>((o, e) => subject.OnNext(value.Value));

			PropertyChangedEventManager.AddHandler(value, handler, nameof(IActiveValue<T>.Value));

			subject.Subscribe(_ => { }, () => PropertyChangedEventManager.RemoveHandler(value, handler, nameof(IActiveValue<T>.Value)));

			subject.OnNext(value.Value);

			return subject;
		}

		public static IObservable<T> ObserveAdded<T>(this IActiveList<T> list)
		{
			var subject = new Subject<T>();

			var handler = new EventHandler<NotifyCollectionChangedEventArgs>((o, e) =>
			{
				switch (e.Action)
				{
					case NotifyCollectionChangedAction.Add:
					case NotifyCollectionChangedAction.Replace:
						subject.OnNext((T)e.NewItems[0]);
						break;
					case NotifyCollectionChangedAction.Reset:
						foreach (var item in list)
							subject.OnNext(item);
						break;
				}
				
			});

			CollectionChangedEventManager.AddHandler(list, handler);

			subject.Subscribe(_ => { }, () => CollectionChangedEventManager.RemoveHandler(list, handler));

			return subject;
		}

		public static IObservable<T> ObserveRemoved<T>(this IActiveList<T> list)
		{
			var subject = new Subject<T>();

			var copy = new List<T>(list.Count);
			foreach (var item in list)
				copy.Add(item);

			var handler = new EventHandler<NotifyCollectionChangedEventArgs>((o, e) =>
			{
				switch (e.Action)
				{
					case NotifyCollectionChangedAction.Add:
						copy.Insert(e.NewStartingIndex, (T)e.NewItems[0]);
						break;
					case NotifyCollectionChangedAction.Remove:
						copy.RemoveAt(e.OldStartingIndex);
						subject.OnNext((T)e.OldItems[0]);
						break;
					case NotifyCollectionChangedAction.Replace:
						copy[e.NewStartingIndex] = (T)e.NewItems[0];
						subject.OnNext((T)e.OldItems[0]);
						break;
					case NotifyCollectionChangedAction.Move:
						copy.RemoveAt(e.OldStartingIndex);
						copy.Insert(e.NewStartingIndex, (T)e.NewItems[0]);
						break;
					case NotifyCollectionChangedAction.Reset:
						foreach (var item in copy)
							subject.OnNext(item);
						copy.Clear();
						foreach (var item in list)
							copy.Add(item);
						break;
				}

			});

			CollectionChangedEventManager.AddHandler(list, handler);

			subject.Subscribe(_ => { }, () => CollectionChangedEventManager.RemoveHandler(list, handler));

			return subject;
		}
	}
}
