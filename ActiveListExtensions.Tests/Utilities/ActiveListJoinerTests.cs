using ActiveListExtensions.Tests.Helpers;
using ActiveListExtensions.Utilities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xunit;

namespace ActiveListExtensions.Tests.Utilities
{
	public class ActiveListJoinerTests
	{
		private static ActiveListJoinerToReadOnlyList<int, int, int> CreateEmpty(ActiveListJoinBehaviour joinBehaviour, out ObservableList<int> right)
		{
			right = new ObservableList<int>();
			var joiner = new ActiveListJoiner<int, int, int>(joinBehaviour, (l, r) => l + r, null, null);
			var result = new ActiveListJoinerToReadOnlyList<int, int, int>(joiner);
			joiner.SetRight(right);
			return result;
		}

		private static ActiveListJoinerToReadOnlyList<int, int, int> CreateHasLeft(ActiveListJoinBehaviour joinBehaviour, out ObservableList<int> right)
		{
			right = new ObservableList<int>();
			var joiner = new ActiveListJoiner<int, int, int>(joinBehaviour, (l, r) => l + r, null, null);
			var result = new ActiveListJoinerToReadOnlyList<int, int, int>(joiner);
			joiner.SetBoth(1, right);
			return result;
		}

		private static ActiveListJoinerToReadOnlyList<int, int, int> CreateHasRight(ActiveListJoinBehaviour joinBehaviour, out ObservableList<int> right)
		{
			right = new ObservableList<int>();
			right.Add(0, 10);
			right.Add(1, 100);
			right.Add(2, 1000);
			var joiner = new ActiveListJoiner<int, int, int>(joinBehaviour, (l, r) => l + r, null, null);
			var result = new ActiveListJoinerToReadOnlyList<int, int, int>(joiner);
			joiner.SetRight(right);
			return result;
		}

		private static ActiveListJoinerToReadOnlyList<int, int, int> CreateHasBoth(ActiveListJoinBehaviour joinBehaviour, out ObservableList<int> right)
		{
			right = new ObservableList<int>();
			right.Add(0, 10);
			right.Add(1, 100);
			right.Add(2, 1000);
			var joiner = new ActiveListJoiner<int, int, int>(joinBehaviour, (l, r) => l + r, null, null);
			var result = new ActiveListJoinerToReadOnlyList<int, int, int>(joiner);
			joiner.SetBoth(1, right);
			return result;
		}

		private static void TestEmptySetLeft(ActiveListJoinBehaviour joinBehaviour, int[] initial, int[] result)
		{
			var sut = CreateEmpty(joinBehaviour, out var right);

			Assert.True(sut.SequenceEqual(initial));

			sut.SetLeft(3);

			Assert.True(sut.SequenceEqual(result));
		}

		private static void TestEmptySetRight(ActiveListJoinBehaviour joinBehaviour, int[] initial, int[] result)
		{
			var sut = CreateEmpty(joinBehaviour, out var right);

			Assert.True(sut.SequenceEqual(initial));

			sut.SetRight(new[] { 20, 200, 2000 });

			Assert.True(sut.SequenceEqual(result));
		}

		private static void TestHasLeftSetLeft(ActiveListJoinBehaviour joinBehaviour, int[] initial, int[] result)
		{
			var sut = CreateHasLeft(joinBehaviour, out var right);

			Assert.True(sut.SequenceEqual(initial));

			sut.SetLeft(3);

			Assert.True(sut.SequenceEqual(result));
		}

		private static void TestHasLeftSetRight(ActiveListJoinBehaviour joinBehaviour, int[] initial, int[] result)
		{
			var sut = CreateHasLeft(joinBehaviour, out var right);

			Assert.True(sut.SequenceEqual(initial));

			sut.SetRight(new[] { 20, 200, 2000 });

			Assert.True(sut.SequenceEqual(result));
		}

		private static void TestHasRightSetLeft(ActiveListJoinBehaviour joinBehaviour, int[] initial, int[] result)
		{
			var sut = CreateHasRight(joinBehaviour, out var right);

			Assert.True(sut.SequenceEqual(initial));

			sut.SetLeft(3);

			Assert.True(sut.SequenceEqual(result));
		}

		private static void TestHasRightSetRight(ActiveListJoinBehaviour joinBehaviour, int[] initial, int[] result)
		{
			var sut = CreateHasRight(joinBehaviour, out var right);

			Assert.True(sut.SequenceEqual(initial));

			sut.SetRight(new[] { 20, 200, 2000 });

			Assert.True(sut.SequenceEqual(result));
		}

		private static void TestHasBothSetLeft(ActiveListJoinBehaviour joinBehaviour, int[] initial, int[] result)
		{
			var sut = CreateHasBoth(joinBehaviour, out var right);

			Assert.True(sut.SequenceEqual(initial));

			sut.SetLeft(3);

			Assert.True(sut.SequenceEqual(result));
		}

		private static void TestHasBothSetRight(ActiveListJoinBehaviour joinBehaviour, int[] initial, int[] result)
		{
			var sut = CreateHasBoth(joinBehaviour, out var right);

			Assert.True(sut.SequenceEqual(initial));

			sut.SetRight(new[] { 20, 200, 2000 });

			Assert.True(sut.SequenceEqual(result));
		}

		private static void TestHasLeftClearLeft(ActiveListJoinBehaviour joinBehaviour, int[] initial, int[] result)
		{
			var sut = CreateHasLeft(joinBehaviour, out var right);

			Assert.True(sut.SequenceEqual(initial));

			sut.ClearLeft();

			Assert.True(sut.SequenceEqual(result));
		}

		private static void TestHasRightClearRight(ActiveListJoinBehaviour joinBehaviour, int[] initial, int[] result)
		{
			var sut = CreateHasRight(joinBehaviour, out var right);

			Assert.True(sut.SequenceEqual(initial));

			right.Clear();

			Assert.True(sut.SequenceEqual(result));
		}

		private static void TestHasBothClearLeft(ActiveListJoinBehaviour joinBehaviour, int[] initial, int[] result)
		{
			var sut = CreateHasBoth(joinBehaviour, out var right);

			Assert.True(sut.SequenceEqual(initial));

			sut.ClearLeft();

			Assert.True(sut.SequenceEqual(result));
		}

		private static void TestHasBothClearRight(ActiveListJoinBehaviour joinBehaviour, int[] initial, int[] result)
		{
			var sut = CreateHasBoth(joinBehaviour, out var right);

			Assert.True(sut.SequenceEqual(initial));

			right.Clear();

			Assert.True(sut.SequenceEqual(result));
		}

		private static void TestEmptyAddToRight(ActiveListJoinBehaviour joinBehaviour, int[] initial, int[] result)
		{
			var sut = CreateEmpty(joinBehaviour, out var right);

			Assert.True(sut.SequenceEqual(initial));

			right.Add(0, 20);
			right.Add(1, 2000);
			right.Add(1, 200);

			Assert.True(sut.SequenceEqual(result));
		}

		private static void TestHasLeftAddToRight(ActiveListJoinBehaviour joinBehaviour, int[] initial, int[] result)
		{
			var sut = CreateHasLeft(joinBehaviour, out var right);

			Assert.True(sut.SequenceEqual(initial));

			right.Add(0, 20);
			right.Add(1, 2000);
			right.Add(1, 200);

			Assert.True(sut.SequenceEqual(result));
		}

		private static void TestHasRightReplaceInRight(ActiveListJoinBehaviour joinBehaviour, int[] initial, int[] result)
		{
			var sut = CreateHasRight(joinBehaviour, out var right);

			Assert.True(sut.SequenceEqual(initial));

			right.Replace(0, 20);
			right.Replace(2, 2000);
			right.Replace(1, 200);

			Assert.True(sut.SequenceEqual(result));
		}

		private static void TestHasBothReplaceInRight(ActiveListJoinBehaviour joinBehaviour, int[] initial, int[] result)
		{
			var sut = CreateHasBoth(joinBehaviour, out var right);

			Assert.True(sut.SequenceEqual(initial));

			right.Replace(0, 20);
			right.Replace(2, 2000);
			right.Replace(1, 200);

			Assert.True(sut.SequenceEqual(result));
		}

		private static void TestHasRightRemoveFromRight(ActiveListJoinBehaviour joinBehaviour, int[] initial, int[] result)
		{
			var sut = CreateHasRight(joinBehaviour, out var right);

			Assert.True(sut.SequenceEqual(initial));

			right.Remove(1);
			right.Remove(1);
			right.Remove(0);

			Assert.True(sut.SequenceEqual(result));
		}

		private static void TestHasBothRemoveFromRight(ActiveListJoinBehaviour joinBehaviour, int[] initial, int[] result)
		{
			var sut = CreateHasBoth(joinBehaviour, out var right);

			Assert.True(sut.SequenceEqual(initial));

			right.Remove(1);
			right.Remove(1);
			right.Remove(0);

			Assert.True(sut.SequenceEqual(result));
		}

		private static void TestHasRightMoveRight(ActiveListJoinBehaviour joinBehaviour, int[] initial, int[] result)
		{
			var sut = CreateHasRight(joinBehaviour, out var right);

			Assert.True(sut.SequenceEqual(initial));

			right.Move(1, 0);

			Assert.True(sut.SequenceEqual(result));
		}

		private static void TestHasBothMoveRight(ActiveListJoinBehaviour joinBehaviour, int[] initial, int[] result)
		{
			var sut = CreateHasBoth(joinBehaviour, out var right);

			Assert.True(sut.SequenceEqual(initial));

			right.Move(1, 0);

			Assert.True(sut.SequenceEqual(result));
		}

		public class InnerJoin
		{
			[Fact]
			public void EmptyAddToRight()
				=> TestEmptyAddToRight(ActiveListJoinBehaviour.Inner, new int[0], new int[0]);

			[Fact]
			public void HasLeftAddToRight()
				=> TestHasLeftAddToRight(ActiveListJoinBehaviour.Inner, new int[0], new[] { 21, 201, 2001 });

			[Fact]
			public void HasRightReplaceInRight()
				=> TestHasRightReplaceInRight(ActiveListJoinBehaviour.Inner, new int[0], new int[0]);

			[Fact]
			public void HasBothReplaceInRight()
				=> TestHasBothReplaceInRight(ActiveListJoinBehaviour.Inner, new[] { 11, 101, 1001 }, new[] { 21, 201, 2001 });

			[Fact]
			public void HasRightRemoveFromRight()
				=> TestHasRightRemoveFromRight(ActiveListJoinBehaviour.Inner, new int[0], new int[0]);

			[Fact]
			public void HasBothRemoveFromRight()
				=> TestHasBothRemoveFromRight(ActiveListJoinBehaviour.Inner, new[] { 11, 101, 1001 }, new int[0]);

			[Fact]
			public void HasRightMoveRight()
				=> TestHasRightMoveRight(ActiveListJoinBehaviour.Inner, new int[0], new int[0]);

			[Fact]
			public void HasBothMoveRight()
				=> TestHasBothMoveRight(ActiveListJoinBehaviour.Inner, new[] { 11, 101, 1001 }, new[] { 101, 11, 1001 });

			[Fact]
			public void EmptySetLeft()
				=> TestEmptySetLeft(ActiveListJoinBehaviour.Inner, new int[0], new int[0]);

			[Fact]
			public void EmptySetRight()
				=> TestEmptySetRight(ActiveListJoinBehaviour.Inner, new int[0], new int[0]);

			[Fact]
			public void HasLeftSetLeft()
				=> TestHasLeftSetLeft(ActiveListJoinBehaviour.Inner, new int[0], new int[0]);

			[Fact]
			public void HasLeftSetRight()
				=> TestHasLeftSetRight(ActiveListJoinBehaviour.Inner, new int[0], new[] { 21, 201, 2001 });

			[Fact]
			public void HasRightSetLeft()
				=> TestHasRightSetLeft(ActiveListJoinBehaviour.Inner, new int[0], new[] { 13, 103, 1003 });

			[Fact]
			public void HasRightSetRight()
				=> TestHasRightSetRight(ActiveListJoinBehaviour.Inner, new int[0], new int[0]);

			[Fact]
			public void HasBothSetLeft()
				=> TestHasBothSetLeft(ActiveListJoinBehaviour.Inner, new[] { 11, 101, 1001 }, new[] { 13, 103, 1003 });

			[Fact]
			public void HasBothSetRight()
				=> TestHasBothSetRight(ActiveListJoinBehaviour.Inner, new[] { 11, 101, 1001 }, new[] { 21, 201, 2001 });

			[Fact]
			public void HasLeftClearLeft()
				=> TestHasLeftClearLeft(ActiveListJoinBehaviour.Inner, new int[0], new int[0]);

			[Fact]
			public void HasRightClearRight()
				=> TestHasRightClearRight(ActiveListJoinBehaviour.Inner, new int[0], new int[0]);

			[Fact]
			public void HasBothClearLeft()
				=> TestHasBothClearLeft(ActiveListJoinBehaviour.Inner, new[] { 11, 101, 1001 }, new int[0]);

			[Fact]
			public void HasBothClearRight()
				=> TestHasBothClearRight(ActiveListJoinBehaviour.Inner, new[] { 11, 101, 1001 }, new int[0]);
		}

		public class LeftJoin
		{
			[Fact]
			public void EmptyAddToRight()
				=> TestEmptyAddToRight(ActiveListJoinBehaviour.Left, new int[0], new int[0]);

			[Fact]
			public void HasLeftAddToRight()
				=> TestHasLeftAddToRight(ActiveListJoinBehaviour.Left, new[] { 1 }, new[] { 21, 201, 2001 });

			[Fact]
			public void HasRightReplaceInRight()
				=> TestHasRightReplaceInRight(ActiveListJoinBehaviour.Left, new int[0], new int[0]);

			[Fact]
			public void HasBothReplaceInRight()
				=> TestHasBothReplaceInRight(ActiveListJoinBehaviour.Left, new[] { 11, 101, 1001 }, new[] { 21, 201, 2001 });

			[Fact]
			public void HasRightRemoveFromRight()
				=> TestHasRightRemoveFromRight(ActiveListJoinBehaviour.Left, new int[0], new int[0]);

			[Fact]
			public void HasBothRemoveFromRight()
				=> TestHasBothRemoveFromRight(ActiveListJoinBehaviour.Left, new[] { 11, 101, 1001 }, new[] { 1 });

			[Fact]
			public void HasRightMoveRight()
				=> TestHasRightMoveRight(ActiveListJoinBehaviour.Left, new int[0], new int[0]);

			[Fact]
			public void HasBothMoveRight()
				=> TestHasBothMoveRight(ActiveListJoinBehaviour.Left, new[] { 11, 101, 1001 }, new[] { 101, 11, 1001 });

			[Fact]
			public void EmptySetLeft()
				=> TestEmptySetLeft(ActiveListJoinBehaviour.Left, new int[0], new[] { 3 });

			[Fact]
			public void EmptySetRight()
				=> TestEmptySetRight(ActiveListJoinBehaviour.Left, new int[0], new int[0]);

			[Fact]
			public void HasLeftSetLeft()
				=> TestHasLeftSetLeft(ActiveListJoinBehaviour.Left, new[] { 1 }, new[] { 3 });

			[Fact]
			public void HasLeftSetRight()
				=> TestHasLeftSetRight(ActiveListJoinBehaviour.Left, new[] { 1 }, new[] { 21, 201, 2001 });

			[Fact]
			public void HasRightSetLeft()
				=> TestHasRightSetLeft(ActiveListJoinBehaviour.Left, new int[0], new[] { 13, 103, 1003 });

			[Fact]
			public void HasRightSetRight()
				=> TestHasRightSetRight(ActiveListJoinBehaviour.Left, new int[0], new int[0]);

			[Fact]
			public void HasBothSetLeft()
				=> TestHasBothSetLeft(ActiveListJoinBehaviour.Left, new[] { 11, 101, 1001 }, new[] { 13, 103, 1003 });

			[Fact]
			public void HasBothSetRight()
				=> TestHasBothSetRight(ActiveListJoinBehaviour.Left, new[] { 11, 101, 1001 }, new[] { 21, 201, 2001 });

			[Fact]
			public void HasLeftClearLeft()
				=> TestHasLeftClearLeft(ActiveListJoinBehaviour.Left, new[] { 1 }, new int[0]);

			[Fact]
			public void HasRightClearRight()
				=> TestHasRightClearRight(ActiveListJoinBehaviour.Left, new int[0], new int[0]);

			[Fact]
			public void HasBothClearLeft()
				=> TestHasBothClearLeft(ActiveListJoinBehaviour.Left, new[] { 11, 101, 1001 }, new int[0]);

			[Fact]
			public void HasBothClearRight()
				=> TestHasBothClearRight(ActiveListJoinBehaviour.Left, new[] { 11, 101, 1001 }, new[] { 1 });
		}

		public class LeftExcludingJoin
		{
			[Fact]
			public void EmptyAddToRight()
				=> TestEmptyAddToRight(ActiveListJoinBehaviour.LeftExcluding, new int[0], new int[0]);

			[Fact]
			public void HasLeftAddToRight()
				=> TestHasLeftAddToRight(ActiveListJoinBehaviour.LeftExcluding, new[] { 1 }, new int[0]);

			[Fact]
			public void HasRightReplaceInRight()
				=> TestHasRightReplaceInRight(ActiveListJoinBehaviour.LeftExcluding, new int[0], new int[0]);

			[Fact]
			public void HasBothReplaceInRight()
				=> TestHasBothReplaceInRight(ActiveListJoinBehaviour.LeftExcluding, new int[0], new int[0]);

			[Fact]
			public void HasRightRemoveFromRight()
				=> TestHasRightRemoveFromRight(ActiveListJoinBehaviour.LeftExcluding, new int[0], new int[0]);

			[Fact]
			public void HasBothRemoveFromRight()
				=> TestHasBothRemoveFromRight(ActiveListJoinBehaviour.LeftExcluding, new int[0], new[] { 1 });

			[Fact]
			public void HasRightMoveRight()
				=> TestHasRightMoveRight(ActiveListJoinBehaviour.LeftExcluding, new int[0], new int[0]);

			[Fact]
			public void HasBothMoveRight()
				=> TestHasBothMoveRight(ActiveListJoinBehaviour.LeftExcluding, new int[0], new int[0]);

			[Fact]
			public void EmptySetLeft()
				=> TestEmptySetLeft(ActiveListJoinBehaviour.LeftExcluding, new int[0], new[] { 3 });

			[Fact]
			public void EmptySetRight()
				=> TestEmptySetRight(ActiveListJoinBehaviour.LeftExcluding, new int[0], new int[0]);

			[Fact]
			public void HasLeftSetLeft()
				=> TestHasLeftSetLeft(ActiveListJoinBehaviour.LeftExcluding, new[] { 1 }, new[] { 3 });

			[Fact]
			public void HasLeftSetRight()
				=> TestHasLeftSetRight(ActiveListJoinBehaviour.LeftExcluding, new[] { 1 }, new int[0]);

			[Fact]
			public void HasRightSetLeft()
				=> TestHasRightSetLeft(ActiveListJoinBehaviour.LeftExcluding, new int[0], new int[0]);

			[Fact]
			public void HasRightSetRight()
				=> TestHasRightSetRight(ActiveListJoinBehaviour.LeftExcluding, new int[0], new int[0]);

			[Fact]
			public void HasBothSetLeft()
				=> TestHasBothSetLeft(ActiveListJoinBehaviour.LeftExcluding, new int[0], new int[0]);

			[Fact]
			public void HasBothSetRight()
				=> TestHasBothSetRight(ActiveListJoinBehaviour.LeftExcluding, new int[0], new int[0]);

			[Fact]
			public void HasLeftClearLeft()
				=> TestHasLeftClearLeft(ActiveListJoinBehaviour.LeftExcluding, new[] { 1 }, new int[0]);

			[Fact]
			public void HasRightClearRight()
				=> TestHasRightClearRight(ActiveListJoinBehaviour.LeftExcluding, new int[0], new int[0]);

			[Fact]
			public void HasBothClearLeft()
				=> TestHasBothClearLeft(ActiveListJoinBehaviour.LeftExcluding, new int[0], new int[0]);

			[Fact]
			public void HasBothClearRight()
				=> TestHasBothClearRight(ActiveListJoinBehaviour.LeftExcluding, new int[0], new[] { 1 });
		}

		public class RightJoin
		{
			[Fact]
			public void EmptyAddToRight()
				=> TestEmptyAddToRight(ActiveListJoinBehaviour.Right, new int[0], new[] { 20, 200, 2000 });

			[Fact]
			public void HasLeftAddToRight()
				=> TestHasLeftAddToRight(ActiveListJoinBehaviour.Right, new int[0], new[] { 21, 201, 2001 });

			[Fact]
			public void HasRightReplaceInRight()
				=> TestHasRightReplaceInRight(ActiveListJoinBehaviour.Right, new[] { 10, 100, 1000 }, new[] { 20, 200, 2000 });

			[Fact]
			public void HasBothReplaceInRight()
				=> TestHasBothReplaceInRight(ActiveListJoinBehaviour.Right, new[] { 11, 101, 1001 }, new[] { 21, 201, 2001 });

			[Fact]
			public void HasRightRemoveFromRight()
				=> TestHasRightRemoveFromRight(ActiveListJoinBehaviour.Right, new[] { 10, 100, 1000 }, new int[0]);

			[Fact]
			public void HasBothRemoveFromRight()
				=> TestHasBothRemoveFromRight(ActiveListJoinBehaviour.Right, new[] { 11, 101, 1001 }, new int[0]);

			[Fact]
			public void HasRightMoveRight()
				=> TestHasRightMoveRight(ActiveListJoinBehaviour.Right, new[] { 10, 100, 1000 }, new[] { 100, 10, 1000 });

			[Fact]
			public void HasBothMoveRight()
				=> TestHasBothMoveRight(ActiveListJoinBehaviour.Right, new[] { 11, 101, 1001 }, new[] { 101, 11, 1001 });

			[Fact]
			public void EmptySetLeft()
				=> TestEmptySetLeft(ActiveListJoinBehaviour.Right, new int[0], new int[0]);

			[Fact]
			public void EmptySetRight()
				=> TestEmptySetRight(ActiveListJoinBehaviour.Right, new int[0], new[] { 20, 200, 2000 });

			[Fact]
			public void HasLeftSetLeft()
				=> TestHasLeftSetLeft(ActiveListJoinBehaviour.Right, new int[0], new int[0]);

			[Fact]
			public void HasLeftSetRight()
				=> TestHasLeftSetRight(ActiveListJoinBehaviour.Right, new int[0], new[] { 21, 201, 2001 });

			[Fact]
			public void HasRightSetLeft()
				=> TestHasRightSetLeft(ActiveListJoinBehaviour.Right, new[] { 10, 100, 1000 }, new[] { 13, 103, 1003 });

			[Fact]
			public void HasRightSetRight()
				=> TestHasRightSetRight(ActiveListJoinBehaviour.Right, new[] { 10, 100, 1000 }, new[] { 20, 200, 2000 });

			[Fact]
			public void HasBothSetLeft()
				=> TestHasBothSetLeft(ActiveListJoinBehaviour.Right, new[] { 11, 101, 1001 }, new[] { 13, 103, 1003 });

			[Fact]
			public void HasBothSetRight()
				=> TestHasBothSetRight(ActiveListJoinBehaviour.Right, new[] { 11, 101, 1001 }, new[] { 21, 201, 2001 });

			[Fact]
			public void HasLeftClearLeft()
				=> TestHasLeftClearLeft(ActiveListJoinBehaviour.Right, new int[0], new int[0]);

			[Fact]
			public void HasRightClearRight()
				=> TestHasRightClearRight(ActiveListJoinBehaviour.Right, new[] { 10, 100, 1000 }, new int[0]);

			[Fact]
			public void HasBothClearLeft()
				=> TestHasBothClearLeft(ActiveListJoinBehaviour.Right, new[] { 11, 101, 1001 }, new[] { 10, 100, 1000 });

			[Fact]
			public void HasBothClearRight()
				=> TestHasBothClearRight(ActiveListJoinBehaviour.Right, new[] { 11, 101, 1001 }, new int[0]);
		}

		public class RightExcludingJoin
		{
			[Fact]
			public void EmptyAddToRight()
				=> TestEmptyAddToRight(ActiveListJoinBehaviour.RightExcluding, new int[0], new[] { 20, 200, 2000 });

			[Fact]
			public void HasLeftAddToRight()
				=> TestHasLeftAddToRight(ActiveListJoinBehaviour.RightExcluding, new int[0], new int[0]);

			[Fact]
			public void HasRightReplaceInRight()
				=> TestHasRightReplaceInRight(ActiveListJoinBehaviour.RightExcluding, new[] { 10, 100, 1000 }, new[] { 20, 200, 2000 });

			[Fact]
			public void HasBothReplaceInRight()
				=> TestHasBothReplaceInRight(ActiveListJoinBehaviour.RightExcluding, new int[0], new int[0]);

			[Fact]
			public void HasRightRemoveFromRight()
				=> TestHasRightRemoveFromRight(ActiveListJoinBehaviour.RightExcluding, new[] { 10, 100, 1000 }, new int[0]);

			[Fact]
			public void HasBothRemoveFromRight()
				=> TestHasBothRemoveFromRight(ActiveListJoinBehaviour.RightExcluding, new int[0], new int[0]);

			[Fact]
			public void HasRightMoveRight()
				=> TestHasRightMoveRight(ActiveListJoinBehaviour.RightExcluding, new[] { 10, 100, 1000 }, new[] { 100, 10, 1000 });

			[Fact]
			public void HasBothMoveRight()
				=> TestHasBothMoveRight(ActiveListJoinBehaviour.RightExcluding, new int[0], new int[0]);

			[Fact]
			public void EmptySetLeft()
				=> TestEmptySetLeft(ActiveListJoinBehaviour.RightExcluding, new int[0], new int[0]);

			[Fact]
			public void EmptySetRight()
				=> TestEmptySetRight(ActiveListJoinBehaviour.RightExcluding, new int[0], new[] { 20, 200, 2000 });

			[Fact]
			public void HasLeftSetLeft()
				=> TestHasLeftSetLeft(ActiveListJoinBehaviour.RightExcluding, new int[0], new int[0]);

			[Fact]
			public void HasLeftSetRight()
				=> TestHasLeftSetRight(ActiveListJoinBehaviour.RightExcluding, new int[0], new int[0]);

			[Fact]
			public void HasRightSetLeft()
				=> TestHasRightSetLeft(ActiveListJoinBehaviour.RightExcluding, new[] { 10, 100, 1000 }, new int[0]);

			[Fact]
			public void HasRightSetRight()
				=> TestHasRightSetRight(ActiveListJoinBehaviour.RightExcluding, new[] { 10, 100, 1000 }, new[] { 20, 200, 2000 });

			[Fact]
			public void HasBothSetLeft()
				=> TestHasBothSetLeft(ActiveListJoinBehaviour.RightExcluding, new int[0], new int[0]);

			[Fact]
			public void HasBothSetRight()
				=> TestHasBothSetRight(ActiveListJoinBehaviour.RightExcluding, new int[0], new int[0]);

			[Fact]
			public void HasLeftClearLeft()
				=> TestHasLeftClearLeft(ActiveListJoinBehaviour.RightExcluding, new int[0], new int[0]);

			[Fact]
			public void HasRightClearRight()
				=> TestHasRightClearRight(ActiveListJoinBehaviour.RightExcluding, new[] { 10, 100, 1000 }, new int[0]);

			[Fact]
			public void HasBothClearLeft()
				=> TestHasBothClearLeft(ActiveListJoinBehaviour.RightExcluding, new int[0], new[] { 10, 100, 1000 });

			[Fact]
			public void HasBothClearRight()
				=> TestHasBothClearRight(ActiveListJoinBehaviour.RightExcluding, new int[0], new int[0]);
		}

		public class OuterJoin
		{
			[Fact]
			public void EmptyAddToRight()
				=> TestEmptyAddToRight(ActiveListJoinBehaviour.Outer, new int[0], new[] { 20, 200, 2000 });

			[Fact]
			public void HasLeftAddToRight()
				=> TestHasLeftAddToRight(ActiveListJoinBehaviour.Outer, new[] { 1 }, new[] { 21, 201, 2001 });

			[Fact]
			public void HasRightReplaceInRight()
				=> TestHasRightReplaceInRight(ActiveListJoinBehaviour.Outer, new[] { 10, 100, 1000 }, new[] { 20, 200, 2000 });

			[Fact]
			public void HasBothReplaceInRight()
				=> TestHasBothReplaceInRight(ActiveListJoinBehaviour.Outer, new[] { 11, 101, 1001 }, new[] { 21, 201, 2001 });

			[Fact]
			public void HasRightRemoveFromRight()
				=> TestHasRightRemoveFromRight(ActiveListJoinBehaviour.Outer, new[] { 10, 100, 1000 }, new int[0]);

			[Fact]
			public void HasBothRemoveFromRight()
				=> TestHasBothRemoveFromRight(ActiveListJoinBehaviour.Outer, new[] { 11, 101, 1001 }, new[] { 1 });

			[Fact]
			public void HasRightMoveRight()
				=> TestHasRightMoveRight(ActiveListJoinBehaviour.Outer, new[] { 10, 100, 1000 }, new[] { 100, 10, 1000 });

			[Fact]
			public void HasBothMoveRight()
				=> TestHasBothMoveRight(ActiveListJoinBehaviour.Outer, new[] { 11, 101, 1001 }, new[] { 101, 11, 1001 });

			[Fact]
			public void EmptySetLeft()
				=> TestEmptySetLeft(ActiveListJoinBehaviour.Outer, new int[0], new[] { 3 });

			[Fact]
			public void EmptySetRight()
				=> TestEmptySetRight(ActiveListJoinBehaviour.Outer, new int[0], new[] { 20, 200, 2000 });

			[Fact]
			public void HasLeftSetLeft()
				=> TestHasLeftSetLeft(ActiveListJoinBehaviour.Outer, new[] { 1 }, new[] { 3 });

			[Fact]
			public void HasLeftSetRight()
				=> TestHasLeftSetRight(ActiveListJoinBehaviour.Outer, new[] { 1 }, new[] { 21, 201, 2001 });

			[Fact]
			public void HasRightSetLeft()
				=> TestHasRightSetLeft(ActiveListJoinBehaviour.Outer, new[] { 10, 100, 1000 }, new[] { 13, 103, 1003 });

			[Fact]
			public void HasRightSetRight()
				=> TestHasRightSetRight(ActiveListJoinBehaviour.Outer, new[] { 10, 100, 1000 }, new[] { 20, 200, 2000 });

			[Fact]
			public void HasBothSetLeft()
				=> TestHasBothSetLeft(ActiveListJoinBehaviour.Outer, new[] { 11, 101, 1001 }, new[] { 13, 103, 1003 });

			[Fact]
			public void HasBothSetRight()
				=> TestHasBothSetRight(ActiveListJoinBehaviour.Outer, new[] { 11, 101, 1001 }, new[] { 21, 201, 2001 });

			[Fact]
			public void HasLeftClearLeft()
				=> TestHasLeftClearLeft(ActiveListJoinBehaviour.Outer, new[] { 1 }, new int[0]);

			[Fact]
			public void HasRightClearRight()
				=> TestHasRightClearRight(ActiveListJoinBehaviour.Outer, new[] { 10, 100, 1000 }, new int[0]);

			[Fact]
			public void HasBothClearLeft()
				=> TestHasBothClearLeft(ActiveListJoinBehaviour.Outer, new[] { 11, 101, 1001 }, new[] { 10, 100, 1000 });

			[Fact]
			public void HasBothClearRight()
				=> TestHasBothClearRight(ActiveListJoinBehaviour.Outer, new[] { 11, 101, 1001 }, new[] { 1 });
		}

		public class OuterExcludingJoin
		{
			[Fact]
			public void EmptyAddToRight()
				=> TestEmptyAddToRight(ActiveListJoinBehaviour.OuterExcluding, new int[0], new[] { 20, 200, 2000 });

			[Fact]
			public void HasLeftAddToRight()
				=> TestHasLeftAddToRight(ActiveListJoinBehaviour.OuterExcluding, new[] { 1 }, new int[0]);

			[Fact]
			public void HasRightReplaceInRight()
				=> TestHasRightReplaceInRight(ActiveListJoinBehaviour.OuterExcluding, new[] { 10, 100, 1000 }, new[] { 20, 200, 2000 });

			[Fact]
			public void HasBothReplaceInRight()
				=> TestHasBothReplaceInRight(ActiveListJoinBehaviour.OuterExcluding, new int[0], new int[0]);

			[Fact]
			public void HasRightRemoveFromRight()
				=> TestHasRightRemoveFromRight(ActiveListJoinBehaviour.OuterExcluding, new[] { 10, 100, 1000 }, new int[0]);

			[Fact]
			public void HasBothRemoveFromRight()
				=> TestHasBothRemoveFromRight(ActiveListJoinBehaviour.OuterExcluding, new int[0], new[] { 1 });

			[Fact]
			public void HasRightMoveRight()
				=> TestHasRightMoveRight(ActiveListJoinBehaviour.OuterExcluding, new[] { 10, 100, 1000 }, new[] { 100, 10, 1000 });

			[Fact]
			public void HasBothMoveRight()
				=> TestHasBothMoveRight(ActiveListJoinBehaviour.OuterExcluding, new int[0], new int[0]);

			[Fact]
			public void EmptySetLeft()
				=> TestEmptySetLeft(ActiveListJoinBehaviour.OuterExcluding, new int[0], new[] { 3 });

			[Fact]
			public void EmptySetRight()
				=> TestEmptySetRight(ActiveListJoinBehaviour.OuterExcluding, new int[0], new[] { 20, 200, 2000 });

			[Fact]
			public void HasLeftSetLeft()
				=> TestHasLeftSetLeft(ActiveListJoinBehaviour.OuterExcluding, new[] { 1 }, new[] { 3 });

			[Fact]
			public void HasLeftSetRight()
				=> TestHasLeftSetRight(ActiveListJoinBehaviour.OuterExcluding, new[] { 1 }, new int[0]);

			[Fact]
			public void HasRightSetLeft()
				=> TestHasRightSetLeft(ActiveListJoinBehaviour.OuterExcluding, new[] { 10, 100, 1000 }, new int[0]);

			[Fact]
			public void HasRightSetRight()
				=> TestHasRightSetRight(ActiveListJoinBehaviour.OuterExcluding, new[] { 10, 100, 1000 }, new[] { 20, 200, 2000 });

			[Fact]
			public void HasBothSetLeft()
				=> TestHasBothSetLeft(ActiveListJoinBehaviour.OuterExcluding, new int[0], new int[0]);

			[Fact]
			public void HasBothSetRight()
				=> TestHasBothSetRight(ActiveListJoinBehaviour.OuterExcluding, new int[0], new int[0]);

			[Fact]
			public void HasLeftClearLeft()
				=> TestHasLeftClearLeft(ActiveListJoinBehaviour.OuterExcluding, new[] { 1 }, new int[0]);

			[Fact]
			public void HasRightClearRight()
				=> TestHasRightClearRight(ActiveListJoinBehaviour.OuterExcluding, new[] { 10, 100, 1000 }, new int[0]);

			[Fact]
			public void HasBothClearLeft()
				=> TestHasBothClearLeft(ActiveListJoinBehaviour.OuterExcluding, new int[0], new[] { 10, 100, 1000 });

			[Fact]
			public void HasBothClearRight()
				=> TestHasBothClearRight(ActiveListJoinBehaviour.OuterExcluding, new int[0], new[] { 1 });
		}
	}
}
