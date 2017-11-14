using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ActiveListExtensions.Utilities
{
	internal class OnDisposeAction : IDisposable
	{
		private Action _onDispose;

		public OnDisposeAction(Action onDispose)
			=> _onDispose = onDispose;

		public void Dispose()
		{
			try
			{
				_onDispose?.Invoke();
			}
			finally
			{
				_onDispose = null;
			}
		}
	}
}
