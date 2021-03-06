﻿using ActiveListExtensions.Utilities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ActiveListExtensions.Tests.Helpers
{
	internal class ActiveListJoinerToReadOnlyList<TLeft, TRight, TResult> : ObservableList<TResult>
	{
		private readonly ActiveListJoiner<TLeft, TRight, TResult> _joiner;

		public ActiveListJoinerToReadOnlyList(ActiveListJoiner<TLeft, TRight, TResult> joiner)
		{
			_joiner = joiner;

			joiner.AddRequested += Add;
			joiner.RemoveRequested += Remove;
			joiner.ReplaceRequested += Replace;
			joiner.ReplaceRangeRequested += ReplaceRange;
			joiner.MoveRequested += Move;
			joiner.MoveRangeRequested += MoveRange;
			joiner.ResetRequested += Reset;
		}

		public void SetLeft(TLeft left)
			=> _joiner.SetLeft(left);

		public void ClearLeft()
			=> _joiner.ClearLeft();

		public void SetRight(IReadOnlyList<TRight> right)
			=> _joiner.SetRight(right);
	}
}
