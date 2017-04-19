using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ActiveListExtensions
{
	public static class DataTableExtensions
	{
		public static IActiveList<DataRow> ToActiveList(this DataTable table)
		{
			throw new NotImplementedException();
		}

		public static IActiveList<T> ToActiveList<T>(this TypedTableBase<T> table)
			where T : DataRow
		{
			throw new NotImplementedException();
		}
	}
}
