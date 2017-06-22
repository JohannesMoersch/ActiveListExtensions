using ActiveListExtensions.Reactive;
using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Linq;
using System.Reactive.Linq;
using System.Reactive.Subjects;
using System.Text;
using System.Threading.Tasks;

namespace ActiveListExtensions
{
	[EditorBrowsable(EditorBrowsableState.Never)]
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

		public static IObservable<IReadOnlyList<T>> ObserveAll<T>(this IActiveList<T> list)
		{
			var subject = new Subject<IReadOnlyList<T>>();

			var handler = new EventHandler<NotifyCollectionChangedEventArgs>((o, e) =>
			{
				var count = list.Count;

				var array = new T[count];
				for (int i = 0; i < count; ++i)
					array[i] = list[i];

				subject.OnNext(array);
			});

			CollectionChangedEventManager.AddHandler(list, handler);

			subject.Subscribe(_ => { }, () => CollectionChangedEventManager.RemoveHandler(list, handler));

			return subject;
		}

		public static IObservable<T> ObserveAdded<T>(this IActiveList<T> list) => ObserveAddedWithIndex(list).Select(o => o.Item);

		public static IObservable<ItemAdded<T>> ObserveAddedWithIndex<T>(this IActiveList<T> list)
		{
			var subject = new Subject<ItemAdded<T>>();

			var handler = new EventHandler<NotifyCollectionChangedEventArgs>((o, e) =>
			{
				switch (e.Action)
				{
					case NotifyCollectionChangedAction.Add:
					case NotifyCollectionChangedAction.Replace:
						subject.OnNext(new ItemAdded<T>((T)e.NewItems[0], e.NewStartingIndex));
						break;
					case NotifyCollectionChangedAction.Reset:
						for (int i = 0; i < list.Count; ++i)
							subject.OnNext(new ItemAdded<T>(list[i], i));
						break;
				}
			});

			CollectionChangedEventManager.AddHandler(list, handler);

			subject.Subscribe(_ => { }, () => CollectionChangedEventManager.RemoveHandler(list, handler));

			return subject;
		}

		public static IObservable<T> ObserveRemoved<T>(this IActiveList<T> list) => ObserveRemovedWithIndex(list).Select(o => o.Item);

		public static IObservable<ItemRemoved<T>> ObserveRemovedWithIndex<T>(this IActiveList<T> list)
		{
			var subject = new Subject<ItemRemoved<T>>();

			var copy = new List<T>(list.Count + 8);
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
						subject.OnNext(new ItemRemoved<T>((T)e.OldItems[0], e.OldStartingIndex));
						break;
					case NotifyCollectionChangedAction.Replace:
						copy[e.NewStartingIndex] = (T)e.NewItems[0];
						subject.OnNext(new ItemRemoved<T>((T)e.OldItems[0], e.OldStartingIndex));
						break;
					case NotifyCollectionChangedAction.Move:
						copy.RemoveAt(e.OldStartingIndex);
						copy.Insert(e.NewStartingIndex, (T)e.NewItems[0]);
						break;
					case NotifyCollectionChangedAction.Reset:
						for (int i = copy.Count - 1; i >= 0; --i)
							subject.OnNext(new ItemRemoved<T>(copy[i], i));
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

		public static IActiveValue<T> ToActiveValue<T>(this IObservable<T> observable)
		{
			var activeValue = new ObservableToActiveValue<T>();

			var subscription = observable.Subscribe(new WeakSubscriber<T>(activeValue));
			activeValue.Disposed += subscription.Dispose;

			return activeValue;
		}

		public static IActiveList<T> ToActiveList<T>(this IObservable<IEnumerable<T>> observable) => observable.ToActiveValue().ToActiveList();
	}
}
