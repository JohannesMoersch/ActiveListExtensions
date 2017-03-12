using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ActiveListExtensions.Modifiers.Bases;

namespace ActiveListExtensions.Modifiers
{
	internal class ActiveDistinct<T, U> : ActiveSetBase<T, U>
	{
		public ActiveDistinct(IActiveList<T> source, Func<T, U> keySelector, IEnumerable<string> propertiesToWatch = null) 
			: base(source, null, keySelector, propertiesToWatch)
		{
		}

		protected override SetAction OnAddedToLeft(bool existsInRight) => SetAction.Add;

		protected override SetAction OnAddedToRight(bool existsInLeft) { throw new NotSupportedException(); }

		protected override SetAction OnRemovedFromLeft(bool existsInRight) => SetAction.Remove;

		protected override SetAction OnRemovedFromRight(bool existsInLeft) { throw new NotSupportedException(); }
	}
}
