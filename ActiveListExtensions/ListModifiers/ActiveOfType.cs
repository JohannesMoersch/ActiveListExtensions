using ActiveListExtensions.ListModifiers.Bases;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ActiveListExtensions.ListModifiers
{
	internal class ActiveOfType<TResult> : ActiveWhereBase<object, object, TResult>
	{
		public ActiveOfType(IActiveList<object> source) 
			: base(source, i => i is TResult, i => (TResult)(object)i, null, null, null)
		{
			Initialize();
		}
	}
}
