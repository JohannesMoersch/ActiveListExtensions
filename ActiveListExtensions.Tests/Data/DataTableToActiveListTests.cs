using ActiveListExtensions.Tests.Helpers;
using ActiveListExtensions.Utilities;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xunit;

namespace ActiveListExtensions.Tests.Data
{
	public class DataTableToActiveListTests
	{
		[Fact]
		public void AddRowToDataTable()
		{
			Setup(out var table, out var sut, out var watcher);

			table.Rows.Add(table.NewRow());

			Assert.True(table.Rows.Cast<DataRow>().SequenceEqual(sut));
		}

		[Fact]
		public void RemoveRowToDataTable()
		{
			Setup(out var table, out var sut, out var watcher);

			table.Rows.RemoveAt(1);

			Assert.True(table.Rows.Cast<DataRow>().SequenceEqual(sut));
		}

		[Fact]
		public void ClearDataTable()
		{
			Setup(out var table, out var sut, out var watcher);

			table.Clear();

			Assert.True(table.Rows.Cast<DataRow>().SequenceEqual(sut));
		}

		[Fact]
		public void MergeDataTable()
		{
			Setup(out var table, out var sut, out var watcher);

			var otherTable = new DataTable();

			otherTable.Rows.Add(otherTable.NewRow());
			otherTable.Rows.Add(otherTable.NewRow());

			table.Merge(otherTable);

			Assert.True(table.Rows.Cast<DataRow>().SequenceEqual(sut));
		}

		private void Setup(out DataTable table, out IActiveList<DataRow> sut, out CollectionSynchronizationWatcher<DataRow> watcher)
		{
			RandomGenerator.ResetRandomGenerator();

			table = new DataTable();

			table.Rows.Add(table.NewRow());
			table.Rows.Add(table.NewRow());

			sut = table.ToActiveList();

			watcher = new CollectionSynchronizationWatcher<DataRow>(sut);
		}
	}
}
