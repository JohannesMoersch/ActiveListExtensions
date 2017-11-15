using ActiveListExtensions.ValueModifiers.Bases;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel;
using System.Threading;

namespace ActiveListExtensions.ValueModifiers
{
	internal class TaskActiveValue<TValue> : ActiveValueBase<TValue>
	{
		public TaskActiveValue(Task<TValue> task, TValue defaultValue)
		{
			Value = defaultValue;

			task.ContinueWith(t =>
			{
				if (!IsDisposed)
					Value = t.Result;
			}, new CancellationToken(), TaskContinuationOptions.OnlyOnRanToCompletion, TaskScheduler.FromCurrentSynchronizationContext());
		}
	}
}
