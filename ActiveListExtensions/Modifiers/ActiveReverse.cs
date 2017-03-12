using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ActiveListExtensions.Modifiers.Bases;

namespace ActiveListExtensions.Modifiers
{
	internal class ActiveReverse<T> : ActiveListListenerBase<T, T>
	{
		public override int Count => SourceList.Count;

		public override T this[int index] => SourceList[SourceList.Count - index - 1];

		public ActiveReverse(IActiveList<T> source) 
			: base(source)
		{

			if (SourceList is INotifyPropertyChanged)
				PropertyChangedEventManager.AddHandler(SourceList as INotifyPropertyChanged, SourceCountChanged, nameof(IReadOnlyList<T>.Count));
			Initialize();
		}

		protected override void OnDisposed()
		{
			PropertyChangedEventManager.RemoveHandler(SourceList as INotifyPropertyChanged, SourceCountChanged, nameof(IReadOnlyList<T>.Count));
		}

		private void SourceCountChanged(object sender, PropertyChangedEventArgs args) => NotifyOfPropertyChange(new PropertyChangedEventArgs(nameof(Count)));

		protected override void OnAdded(int index, T value) => NotifyOfCollectionChange(new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Add, value, Count - index - 1));

		protected override void OnRemoved(int index, T value) => NotifyOfCollectionChange(new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Remove, value, Count - index));

		protected override void OnMoved(int oldIndex, int newIndex, T value) => NotifyOfCollectionChange(new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Move, value, Count - newIndex - 1, Count - oldIndex - 1));

		protected override void OnReplaced(int index, T oldValue, T newValue) => NotifyOfCollectionChange(new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Replace, newValue, oldValue, Count - index - 1));

		protected override void OnReset(IReadOnlyList<T> newItems) => NotifyOfCollectionChange(new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Reset));

		private IEnumerable<T> Reverse(IReadOnlyList<T> list)
		{
			for (int i = list.Count - 1; i >= 0; --i)
				yield return list[i];
		}

		public override IEnumerator<T> GetEnumerator() => Reverse(SourceList).GetEnumerator();
	}
}
