using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ActiveListExtensions.ListModifiers.Bases;

namespace ActiveListExtensions.ListModifiers
{
	internal class ActiveReverse<TSource> : ActiveListListenerBase<TSource, object, TSource>
	{
		public override int Count => SourceList.Count;

		public override TSource this[int index] => SourceList[SourceList.Count - index - 1];

		public ActiveReverse(IActiveList<TSource> source) 
			: base(source)
		{

			if (SourceList is INotifyPropertyChanged)
				PropertyChangedEventManager.AddHandler(SourceList as INotifyPropertyChanged, SourceCountChanged, nameof(IReadOnlyList<TSource>.Count));
			Initialize();
		}

		protected override void OnDisposed()
		{
			PropertyChangedEventManager.RemoveHandler(SourceList as INotifyPropertyChanged, SourceCountChanged, nameof(IReadOnlyList<TSource>.Count));
		}

		private void SourceCountChanged(object sender, PropertyChangedEventArgs args) => NotifyOfPropertyChange(new PropertyChangedEventArgs(nameof(Count)));

		protected override void OnAdded(int index, TSource value) => NotifyOfCollectionChange(new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Add, value, Count - index - 1));

		protected override void OnRemoved(int index, TSource value) => NotifyOfCollectionChange(new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Remove, value, Count - index));

		protected override void OnMoved(int oldIndex, int newIndex, TSource value) => NotifyOfCollectionChange(new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Move, value, Count - newIndex - 1, Count - oldIndex - 1));

		protected override void OnReplaced(int index, TSource oldValue, TSource newValue) => NotifyOfCollectionChange(new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Replace, newValue, oldValue, Count - index - 1));

		protected override void OnReset(IReadOnlyList<TSource> newItems) => NotifyOfCollectionChange(new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Reset));

		private IEnumerable<TSource> Reverse(IReadOnlyList<TSource> list)
		{
			for (int i = list.Count - 1; i >= 0; --i)
				yield return list[i];
		}

		public override IEnumerator<TSource> GetEnumerator() => Reverse(SourceList).GetEnumerator();
	}
}
