using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ActiveListExtensions.Data
{
	public class DataTableWrapper<TDataRow> : IActiveList<TDataRow>
		where TDataRow : DataRow
	{
		public TDataRow this[int index] => _collection[index];

		public int Count => _collection.Count;

		private DataView _dataView;

		private IList<TDataRow> _collection;

		public DataTableWrapper(DataTable table)
		{
			_dataView = table.AsDataView();

			_collection = new List<TDataRow>(_dataView.Count + 8);

			foreach (var item in _dataView)
				_collection.Add((item as DataRowView).Row as TDataRow);

			_dataView.ListChanged += ListChanged;
		}

		public void Dispose()
		{
			if (_dataView == null)
				return;

			_dataView.ListChanged -= ListChanged;
			_dataView.Dispose();
			_dataView = null;

			_collection.Clear();
		}

		private void ListChanged(object sender, ListChangedEventArgs e)
		{
			switch (e.ListChangedType)
			{
				case ListChangedType.ItemAdded:
					var newRow = (_dataView[e.NewIndex] as DataRowView).Row as TDataRow;
					_collection.Insert(e.NewIndex, newRow);
					CollectionChanged?.Invoke(this, new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Add, newRow, e.NewIndex));
					PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(Count)));
					break;
				case ListChangedType.ItemDeleted:
					var oldRow = _collection[e.NewIndex];
					_collection.RemoveAt(e.NewIndex);
					CollectionChanged?.Invoke(this, new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Remove, oldRow, e.NewIndex));
					PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(Count)));
					break;
				case ListChangedType.ItemMoved:
					var row = _collection[e.OldIndex];
					_collection.RemoveAt(e.OldIndex);
					_collection.Insert(e.NewIndex, row);
					CollectionChanged?.Invoke(this, new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Move, row, e.NewIndex, e.OldIndex));
					break;
				case ListChangedType.Reset:
					var count = _collection.Count;
					_collection.Clear();
					foreach (var item in _dataView.Cast<DataRowView>().Select(view => view.Row as TDataRow))
						_collection.Add(item);
					CollectionChanged?.Invoke(this, new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Reset));
					if (_collection.Count == count)
						PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(Count)));
					break;
			}
		}

		public IEnumerator<TDataRow> GetEnumerator() => _collection.GetEnumerator();

		IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();

		public event PropertyChangedEventHandler PropertyChanged;
		
		public event NotifyCollectionChangedEventHandler CollectionChanged;
	}
}
