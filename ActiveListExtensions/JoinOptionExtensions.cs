using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ActiveListExtensions
{
	[EditorBrowsable(EditorBrowsableState.Never)]
	public static class JoinOptionExtensions
	{
		public static T GetOrElse<T>(this JoinOption<T> option, T defaultValue)
			=> option.HasValue ? option.Value : defaultValue;

		public static TResult Match<T, TResult>(this JoinOption<T> option, Func<T, TResult> some, TResult none)
			=> option.HasValue ? some.Invoke(option.Value) : none;

		public static TResult Match<T, TResult>(this JoinOption<T> option, Func<T, TResult> some, Func<TResult> none)
			=> option.HasValue ? some.Invoke(option.Value) : none.Invoke();
	}
}
