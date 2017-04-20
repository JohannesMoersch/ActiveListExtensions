using ActiveListExtensions.Data;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ActiveListExtensions
{
	[EditorBrowsable(EditorBrowsableState.Never)]
	public static class DataTableExtensions
	{
		public static IActiveList<DataRow> ToActiveList(this DataTable table)
			=> new DataTableWrapper<DataRow>(table);

		public static IActiveList<TDataRow> ToActiveList<TDataRow>(this TypedTableBase<TDataRow> table)
			where TDataRow : DataRow 
			=> new DataTableWrapper<TDataRow>(table);
	}
}
