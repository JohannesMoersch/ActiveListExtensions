using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Xunit;

namespace ActiveListExtensions.Tests.ValueModifiers
{
	public class TaskActiveValueTests
	{
		[Fact]
		public void InitialValueIsCorrect()
		{
			var task = new TaskCompletionSource<int>();

			var sut = task.Task.ToActiveValue(-1);

			Assert.Equal(-1, sut.Value);
		}

		[Fact]
		public void ValueUpdatedWhenTaskCompleted()
		{
			var task = new TaskCompletionSource<int>();

			var sut = task.Task.ToActiveValue();

			task.SetResult(-1);

			Thread.Sleep(50);

			Assert.Equal(-1, sut.Value);
		}

		[Fact]
		public void ValueRemainsOnTaskFailed()
		{
			var task = new TaskCompletionSource<int>();

			var sut = task.Task.ToActiveValue(-1);

			task.SetException(new Exception());

			Assert.Equal(-1, sut.Value);
		}

		[Fact]
		public void ValueRemainsOnTaskCancelled()
		{
			var task = new TaskCompletionSource<int>();

			var sut = task.Task.ToActiveValue(-1);

			task.SetCanceled();

			Assert.Equal(-1, sut.Value);
		}
	}
}
