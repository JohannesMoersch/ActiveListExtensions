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
		private static ActiveListJoiner<int, int, int> CreateEmpty(ActiveListJoinBehaviour joinBehaviour, out ObservableList<int> left, out ObservableList<int> right)
		{
			left = new ObservableList<int>();
			right = new ObservableList<int>();
			return new ActiveListJoiner<int, int, int>(joinBehaviour, left, right, (l, r) => l + r, null, null);
		}

		private static ActiveListJoiner<int, int, int> CreateHasLeft(ActiveListJoinBehaviour joinBehaviour, out ObservableList<int> left, out ObservableList<int> right)
		{
			left = new ObservableList<int>();
			left.Add(0, 1);
			left.Add(1, 2);
			left.Add(2, 3);
			right = new ObservableList<int>();
			return new ActiveListJoiner<int, int, int>(joinBehaviour, left, right, (l, r) => l + r, null, null);
		}

		private static ActiveListJoiner<int, int, int> CreateHasRight(ActiveListJoinBehaviour joinBehaviour, out ObservableList<int> left, out ObservableList<int> right)
		{
			left = new ObservableList<int>();
			right = new ObservableList<int>();
			right.Add(0, 10);
			right.Add(1, 100);
			return new ActiveListJoiner<int, int, int>(joinBehaviour, left, right, (l, r) => l + r, null, null);
		}

		private static ActiveListJoiner<int, int, int> CreateHasBoth(ActiveListJoinBehaviour joinBehaviour, out ObservableList<int> left, out ObservableList<int> right)
		{
			left = new ObservableList<int>();
			left.Add(0, 1);
			left.Add(1, 2);
			left.Add(2, 3);
			right = new ObservableList<int>();
			right.Add(0, 10);
			right.Add(1, 100);
			return new ActiveListJoiner<int, int, int>(joinBehaviour, left, right, (l, r) => l + r, null, null);
		}

		private static void TestEmptySetLeft(ActiveListJoinBehaviour joinBehaviour, int[] initial, int[] result)
		{
			var sut = CreateEmpty(joinBehaviour, out var left, out var right);

			Assert.True(sut.SequenceEqual(initial));

			sut.SetLeft(new[] { 3, 6, 9 });

			Assert.True(sut.SequenceEqual(result));
		}

		private static void TestEmptySetRight(ActiveListJoinBehaviour joinBehaviour, int[] initial, int[] result)
		{
			var sut = CreateEmpty(joinBehaviour, out var left, out var right);

			Assert.True(sut.SequenceEqual(initial));

			sut.SetRight(new[] { 20, 200 });

			Assert.True(sut.SequenceEqual(result));
		}

		private static void TestHasLeftSetLeft(ActiveListJoinBehaviour joinBehaviour, int[] initial, int[] result)
		{
			var sut = CreateHasLeft(joinBehaviour, out var left, out var right);

			Assert.True(sut.SequenceEqual(initial));

			sut.SetLeft(new[] { 3, 6, 9 });

			Assert.True(sut.SequenceEqual(result));
		}

		private static void TestHasLeftSetRight(ActiveListJoinBehaviour joinBehaviour, int[] initial, int[] result)
		{
			var sut = CreateHasLeft(joinBehaviour, out var left, out var right);

			Assert.True(sut.SequenceEqual(initial));

			sut.SetRight(new[] { 20, 200 });

			Assert.True(sut.SequenceEqual(result));
		}

		private static void TestHasRightSetLeft(ActiveListJoinBehaviour joinBehaviour, int[] initial, int[] result)
		{
			var sut = CreateHasRight(joinBehaviour, out var left, out var right);

			Assert.True(sut.SequenceEqual(initial));

			sut.SetLeft(new[] { 3, 6, 9 });

			Assert.True(sut.SequenceEqual(result));
		}

		private static void TestHasRightSetRight(ActiveListJoinBehaviour joinBehaviour, int[] initial, int[] result)
		{
			var sut = CreateHasRight(joinBehaviour, out var left, out var right);

			Assert.True(sut.SequenceEqual(initial));

			sut.SetRight(new[] { 20, 200 });

			Assert.True(sut.SequenceEqual(result));
		}

		private static void TestHasBothSetLeft(ActiveListJoinBehaviour joinBehaviour, int[] initial, int[] result)
		{
			var sut = CreateHasBoth(joinBehaviour, out var left, out var right);

			Assert.True(sut.SequenceEqual(initial));

			sut.SetLeft(new[] { 3, 6, 9 });

			Assert.True(sut.SequenceEqual(result));
		}

		private static void TestHasBothSetRight(ActiveListJoinBehaviour joinBehaviour, int[] initial, int[] result)
		{
			var sut = CreateHasBoth(joinBehaviour, out var left, out var right);

			Assert.True(sut.SequenceEqual(initial));

			sut.SetRight(new[] { 20, 200 });

			Assert.True(sut.SequenceEqual(result));
		}

		private static void TestHasLeftClearLeft(ActiveListJoinBehaviour joinBehaviour, int[] initial, int[] result)
		{
			var sut = CreateHasLeft(joinBehaviour, out var left, out var right);

			Assert.True(sut.SequenceEqual(initial));

			left.Clear();

			Assert.True(sut.SequenceEqual(result));
		}

		private static void TestHasRightClearRight(ActiveListJoinBehaviour joinBehaviour, int[] initial, int[] result)
		{
			var sut = CreateHasRight(joinBehaviour, out var left, out var right);

			Assert.True(sut.SequenceEqual(initial));

			right.Clear();

			Assert.True(sut.SequenceEqual(result));
		}

		private static void TestHasBothClearLeft(ActiveListJoinBehaviour joinBehaviour, int[] initial, int[] result)
		{
			var sut = CreateHasBoth(joinBehaviour, out var left, out var right);

			Assert.True(sut.SequenceEqual(initial));

			left.Clear();

			Assert.True(sut.SequenceEqual(result));
		}

		private static void TestHasBothClearRight(ActiveListJoinBehaviour joinBehaviour, int[] initial, int[] result)
		{
			var sut = CreateHasBoth(joinBehaviour, out var left, out var right);

			Assert.True(sut.SequenceEqual(initial));

			right.Clear();

			Assert.True(sut.SequenceEqual(result));
		}

		private static void TestEmptyAddToLeft(ActiveListJoinBehaviour joinBehaviour, int[] initial, int[] result)
		{
			var sut = CreateEmpty(joinBehaviour, out var left, out var right);

			Assert.True(sut.SequenceEqual(initial));

			left.Add(0, 3);
			left.Add(1, 6);
			left.Add(2, 9);

			Assert.True(sut.SequenceEqual(result));
		}

		private static void TestEmptyAddToRight(ActiveListJoinBehaviour joinBehaviour, int[] initial, int[] result)
		{
			var sut = CreateEmpty(joinBehaviour, out var left, out var right);

			Assert.True(sut.SequenceEqual(initial));

			right.Add(0, 20);
			right.Add(1, 200);

			Assert.True(sut.SequenceEqual(result));
		}

		private static void TestHasLeftReplaceInLeft(ActiveListJoinBehaviour joinBehaviour, int[] initial, int[] result)
		{
			var sut = CreateHasLeft(joinBehaviour, out var left, out var right);

			Assert.True(sut.SequenceEqual(initial));

			left.Replace(0, 3);
			left.Replace(1, 6);
			left.Replace(2, 9);

			Assert.True(sut.SequenceEqual(result));
		}

		private static void TestHasLeftAddToRight(ActiveListJoinBehaviour joinBehaviour, int[] initial, int[] result)
		{
			var sut = CreateHasLeft(joinBehaviour, out var left, out var right);

			Assert.True(sut.SequenceEqual(initial));

			right.Add(0, 20);
			right.Add(1, 200);

			Assert.True(sut.SequenceEqual(result));
		}

		private static void TestHasRightAddToLeft(ActiveListJoinBehaviour joinBehaviour, int[] initial, int[] result)
		{
			var sut = CreateHasRight(joinBehaviour, out var left, out var right);

			Assert.True(sut.SequenceEqual(initial));

			left.Add(0, 3);
			left.Add(1, 6);
			left.Add(2, 9);

			Assert.True(sut.SequenceEqual(result));
		}

		private static void TestHasRightReplaceInRight(ActiveListJoinBehaviour joinBehaviour, int[] initial, int[] result)
		{
			var sut = CreateHasRight(joinBehaviour, out var left, out var right);

			Assert.True(sut.SequenceEqual(initial));

			right.Replace(0, 20);
			right.Replace(1, 200);

			Assert.True(sut.SequenceEqual(result));
		}

		private static void TestHasBothReplaceInLeft(ActiveListJoinBehaviour joinBehaviour, int[] initial, int[] result)
		{
			var sut = CreateHasBoth(joinBehaviour, out var left, out var right);

			Assert.True(sut.SequenceEqual(initial));

			left.Replace(0, 3);
			left.Replace(1, 6);
			left.Replace(2, 9);

			Assert.True(sut.SequenceEqual(result));
		}

		private static void TestHasBothReplaceInRight(ActiveListJoinBehaviour joinBehaviour, int[] initial, int[] result)
		{
			var sut = CreateHasBoth(joinBehaviour, out var left, out var right);

			Assert.True(sut.SequenceEqual(initial));

			right.Replace(0, 20);
			right.Replace(1, 200);

			Assert.True(sut.SequenceEqual(result));
		}

		private static void TestHasLeftRemoveFromLeft(ActiveListJoinBehaviour joinBehaviour, int[] initial, int[] result)
		{
			var sut = CreateHasLeft(joinBehaviour, out var left, out var right);

			Assert.True(sut.SequenceEqual(initial));

			left.Remove(2);
			left.Remove(1);
			left.Remove(0);

			Assert.True(sut.SequenceEqual(result));
		}

		private static void TestHasRightRemoveFromRight(ActiveListJoinBehaviour joinBehaviour, int[] initial, int[] result)
		{
			var sut = CreateHasRight(joinBehaviour, out var left, out var right);

			Assert.True(sut.SequenceEqual(initial));

			right.Remove(1);
			right.Remove(0);

			Assert.True(sut.SequenceEqual(result));
		}

		private static void TestHasBothRemoveFromLeft(ActiveListJoinBehaviour joinBehaviour, int[] initial, int[] result)
		{
			var sut = CreateHasBoth(joinBehaviour, out var left, out var right);

			Assert.True(sut.SequenceEqual(initial));

			left.Remove(2);
			left.Remove(1);
			left.Remove(0);

			Assert.True(sut.SequenceEqual(result));
		}

		private static void TestHasBothRemoveFromRight(ActiveListJoinBehaviour joinBehaviour, int[] initial, int[] result)
		{
			var sut = CreateHasBoth(joinBehaviour, out var left, out var right);

			Assert.True(sut.SequenceEqual(initial));

			right.Remove(1);
			right.Remove(0);

			Assert.True(sut.SequenceEqual(result));
		}

		private static void TestHasLeftMoveLeft(ActiveListJoinBehaviour joinBehaviour, int[] initial, int[] result)
		{
			var sut = CreateHasLeft(joinBehaviour, out var left, out var right);

			Assert.True(sut.SequenceEqual(initial));

			left.Move(2, 0);

			Assert.True(sut.SequenceEqual(result));
		}

		private static void TestHasRightMoveRight(ActiveListJoinBehaviour joinBehaviour, int[] initial, int[] result)
		{
			var sut = CreateHasRight(joinBehaviour, out var left, out var right);

			Assert.True(sut.SequenceEqual(initial));

			right.Move(1, 0);

			Assert.True(sut.SequenceEqual(result));
		}

		private static void TestHasBothMoveLeft(ActiveListJoinBehaviour joinBehaviour, int[] initial, int[] result)
		{
			var sut = CreateHasBoth(joinBehaviour, out var left, out var right);

			Assert.True(sut.SequenceEqual(initial));

			left.Move(2, 0);

			Assert.True(sut.SequenceEqual(result));
		}

		private static void TestHasBothMoveRight(ActiveListJoinBehaviour joinBehaviour, int[] initial, int[] result)
		{
			var sut = CreateHasBoth(joinBehaviour, out var left, out var right);

			Assert.True(sut.SequenceEqual(initial));

			right.Move(1, 0);

			Assert.True(sut.SequenceEqual(result));
		}

		public class InnerJoin
		{
			[Fact]
			public void EmptyAddToLeft()
				=> TestEmptyAddToLeft(ActiveListJoinBehaviour.Inner, new int[0], new int[0]);

			[Fact]
			public void EmptyAddToRight()
				=> TestEmptyAddToRight(ActiveListJoinBehaviour.Inner, new int[0], new int[0]);

			[Fact]
			public void HasLeftReplaceInLeft()
				=> TestHasLeftReplaceInLeft(ActiveListJoinBehaviour.Inner, new int[0], new int[0]);

			[Fact]
			public void HasLeftAddToRight()
				=> TestHasLeftAddToRight(ActiveListJoinBehaviour.Inner, new int[0], new[] { 21, 201, 22, 202, 23, 203 });

			[Fact]
			public void HasRightAddToLeft()
				=> TestHasRightAddToLeft(ActiveListJoinBehaviour.Inner, new int[0], new[] { 13, 103, 16, 106, 19, 109 });

			[Fact]
			public void HasRightReplaceInRight()
				=> TestHasRightReplaceInRight(ActiveListJoinBehaviour.Inner, new int[0], new int[0]);

			[Fact]
			public void HasBothReplaceInLeft()
				=> TestHasBothReplaceInLeft(ActiveListJoinBehaviour.Inner, new[] { 11, 101, 12, 102, 13, 103 }, new[] { 13, 103, 16, 106, 19, 109 });

			[Fact]
			public void HasBothReplaceInRight()
				=> TestHasBothReplaceInRight(ActiveListJoinBehaviour.Inner, new[] { 11, 101, 12, 102, 13, 103 }, new[] { 21, 201, 22, 202, 23, 203 });

			[Fact]
			public void HasLeftRemoveFromLeft()
				=> TestHasLeftRemoveFromLeft(ActiveListJoinBehaviour.Inner, new int[0], new int[0]);

			[Fact]
			public void HasRightRemoveFromRight()
				=> TestHasRightRemoveFromRight(ActiveListJoinBehaviour.Inner, new int[0], new int[0]);

			[Fact]
			public void HasBothRemoveFromLeft()
				=> TestHasBothRemoveFromLeft(ActiveListJoinBehaviour.Inner, new[] { 11, 101, 12, 102, 13, 103 }, new int[0]);

			[Fact]
			public void HasBothRemoveFromRight()
				=> TestHasBothRemoveFromRight(ActiveListJoinBehaviour.Inner, new[] { 11, 101, 12, 102, 13, 103 }, new int[0]);

			[Fact]
			public void HasLeftMoveLeft()
				=> TestHasLeftMoveLeft(ActiveListJoinBehaviour.Inner, new int[0], new int[0]);

			[Fact]
			public void HasRightMoveRight()
				=> TestHasRightMoveRight(ActiveListJoinBehaviour.Inner, new int[0], new int[0]);

			[Fact]
			public void HasBothMoveLeft()
				=> TestHasBothMoveLeft(ActiveListJoinBehaviour.Inner, new[] { 11, 101, 12, 102, 13, 103 }, new[] { 13, 103, 11, 101, 12, 102 });

			[Fact]
			public void HasBothMoveRight()
				=> TestHasBothMoveRight(ActiveListJoinBehaviour.Inner, new[] { 11, 101, 12, 102, 13, 103 }, new[] { 101, 11, 102, 12, 103, 13 });

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
				=> TestHasLeftSetRight(ActiveListJoinBehaviour.Inner, new int[0], new[] { 21, 201, 22, 202, 23, 203 });

			[Fact]
			public void HasRightSetLeft()
				=> TestHasRightSetLeft(ActiveListJoinBehaviour.Inner, new int[0], new[] { 13, 103, 16, 106, 19, 109 });

			[Fact]
			public void HasRightSetRight()
				=> TestHasRightSetRight(ActiveListJoinBehaviour.Inner, new int[0], new int[0]);

			[Fact]
			public void HasBothSetLeft()
				=> TestHasBothSetLeft(ActiveListJoinBehaviour.Inner, new[] { 11, 101, 12, 102, 13, 103 }, new[] { 13, 103, 16, 106, 19, 109 });

			[Fact]
			public void HasBothSetRight()
				=> TestHasBothSetRight(ActiveListJoinBehaviour.Inner, new[] { 11, 101, 12, 102, 13, 103 }, new[] { 21, 201, 22, 202, 23, 203 });

			[Fact]
			public void HasLeftClearLeft()
				=> TestHasLeftClearLeft(ActiveListJoinBehaviour.Inner, new int[0], new int[0]);

			[Fact]
			public void HasRightClearRight()
				=> TestHasRightClearRight(ActiveListJoinBehaviour.Inner, new int[0], new int[0]);

			[Fact]
			public void HasBothClearLeft()
				=> TestHasBothClearLeft(ActiveListJoinBehaviour.Inner, new[] { 11, 101, 12, 102, 13, 103 }, new int[0]);

			[Fact]
			public void HasBothClearRight()
				=> TestHasBothClearRight(ActiveListJoinBehaviour.Inner, new[] { 11, 101, 12, 102, 13, 103 }, new int[0]);
		}

		public class LeftJoin
		{
			[Fact]
			public void EmptyAddToLeft()
				=> TestEmptyAddToLeft(ActiveListJoinBehaviour.Left, new int[0], new[] { 3, 6, 9 });

			[Fact]
			public void EmptyAddToRight()
				=> TestEmptyAddToRight(ActiveListJoinBehaviour.Left, new int[0], new int[0]);

			[Fact]
			public void HasLeftReplaceInLeft()
				=> TestHasLeftReplaceInLeft(ActiveListJoinBehaviour.Left, new[] { 1, 2, 3 }, new[] { 3, 6, 9 });

			[Fact]
			public void HasLeftAddToRight()
				=> TestHasLeftAddToRight(ActiveListJoinBehaviour.Left, new[] { 1, 2, 3 }, new[] { 21, 201, 22, 202, 23, 203 });

			[Fact]
			public void HasRightAddToLeft()
				=> TestHasRightAddToLeft(ActiveListJoinBehaviour.Left, new int[0], new[] { 13, 103, 16, 106, 19, 109 });

			[Fact]
			public void HasRightReplaceInRight()
				=> TestHasRightReplaceInRight(ActiveListJoinBehaviour.Left, new int[0], new int[0]);

			[Fact]
			public void HasBothReplaceInLeft()
				=> TestHasBothReplaceInLeft(ActiveListJoinBehaviour.Left, new[] { 11, 101, 12, 102, 13, 103 }, new[] { 13, 103, 16, 106, 19, 109 });

			[Fact]
			public void HasBothReplaceInRight()
				=> TestHasBothReplaceInRight(ActiveListJoinBehaviour.Left, new[] { 11, 101, 12, 102, 13, 103 }, new[] { 21, 201, 22, 202, 23, 203 });

			[Fact]
			public void HasLeftRemoveFromLeft()
				=> TestHasLeftRemoveFromLeft(ActiveListJoinBehaviour.Left, new[] { 1, 2, 3 }, new int[0]);

			[Fact]
			public void HasRightRemoveFromRight()
				=> TestHasRightRemoveFromRight(ActiveListJoinBehaviour.Left, new int[0], new int[0]);

			[Fact]
			public void HasBothRemoveFromLeft()
				=> TestHasBothRemoveFromLeft(ActiveListJoinBehaviour.Left, new[] { 11, 101, 12, 102, 13, 103 }, new int[0]);

			[Fact]
			public void HasBothRemoveFromRight()
				=> TestHasBothRemoveFromRight(ActiveListJoinBehaviour.Left, new[] { 11, 101, 12, 102, 13, 103 }, new[] { 1, 2, 3 });

			[Fact]
			public void HasLeftMoveLeft()
				=> TestHasLeftMoveLeft(ActiveListJoinBehaviour.Left, new[] { 1, 2, 3 }, new[] { 3, 1, 2 });

			[Fact]
			public void HasRightMoveRight()
				=> TestHasRightMoveRight(ActiveListJoinBehaviour.Left, new int[0], new int[0]);

			[Fact]
			public void HasBothMoveLeft()
				=> TestHasBothMoveLeft(ActiveListJoinBehaviour.Left, new[] { 11, 101, 12, 102, 13, 103 }, new[] { 13, 103, 11, 101, 12, 102 });

			[Fact]
			public void HasBothMoveRight()
				=> TestHasBothMoveRight(ActiveListJoinBehaviour.Left, new[] { 11, 101, 12, 102, 13, 103 }, new[] { 101, 11, 102, 12, 103, 13 });

			[Fact]
			public void EmptySetLeft()
				=> TestEmptySetLeft(ActiveListJoinBehaviour.Left, new int[0], new[] { 3, 6, 9 });

			[Fact]
			public void EmptySetRight()
				=> TestEmptySetRight(ActiveListJoinBehaviour.Left, new int[0], new int[0]);

			[Fact]
			public void HasLeftSetLeft()
				=> TestHasLeftSetLeft(ActiveListJoinBehaviour.Left, new[] { 1, 2, 3 }, new[] { 3, 6, 9 });

			[Fact]
			public void HasLeftSetRight()
				=> TestHasLeftSetRight(ActiveListJoinBehaviour.Left, new[] { 1, 2, 3 }, new[] { 21, 201, 22, 202, 23, 203 });

			[Fact]
			public void HasRightSetLeft()
				=> TestHasRightSetLeft(ActiveListJoinBehaviour.Left, new int[0], new[] { 13, 103, 16, 106, 19, 109 });

			[Fact]
			public void HasRightSetRight()
				=> TestHasRightSetRight(ActiveListJoinBehaviour.Left, new int[0], new int[0]);

			[Fact]
			public void HasBothSetLeft()
				=> TestHasBothSetLeft(ActiveListJoinBehaviour.Left, new[] { 11, 101, 12, 102, 13, 103 }, new[] { 13, 103, 16, 106, 19, 109 });

			[Fact]
			public void HasBothSetRight()
				=> TestHasBothSetRight(ActiveListJoinBehaviour.Left, new[] { 11, 101, 12, 102, 13, 103 }, new[] { 21, 201, 22, 202, 23, 203 });

			[Fact]
			public void HasLeftClearLeft()
				=> TestHasLeftClearLeft(ActiveListJoinBehaviour.Left, new[] { 1, 2, 3 }, new int[0]);

			[Fact]
			public void HasRightClearRight()
				=> TestHasRightClearRight(ActiveListJoinBehaviour.Left, new int[0], new int[0]);

			[Fact]
			public void HasBothClearLeft()
				=> TestHasBothClearLeft(ActiveListJoinBehaviour.Left, new[] { 11, 101, 12, 102, 13, 103 }, new int[0]);

			[Fact]
			public void HasBothClearRight()
				=> TestHasBothClearRight(ActiveListJoinBehaviour.Left, new[] { 11, 101, 12, 102, 13, 103 }, new[] { 1, 2, 3 });
		}

		public class LeftExcludingJoin
		{
			[Fact]
			public void EmptyAddToLeft()
				=> TestEmptyAddToLeft(ActiveListJoinBehaviour.LeftExcluding, new int[0], new[] { 3, 6, 9 });

			[Fact]
			public void EmptyAddToRight()
				=> TestEmptyAddToRight(ActiveListJoinBehaviour.LeftExcluding, new int[0], new int[0]);

			[Fact]
			public void HasLeftReplaceInLeft()
				=> TestHasLeftReplaceInLeft(ActiveListJoinBehaviour.LeftExcluding, new[] { 1, 2, 3 }, new[] { 3, 6, 9 });

			[Fact]
			public void HasLeftAddToRight()
				=> TestHasLeftAddToRight(ActiveListJoinBehaviour.LeftExcluding, new[] { 1, 2, 3 }, new int[0]);

			[Fact]
			public void HasRightAddToLeft()
				=> TestHasRightAddToLeft(ActiveListJoinBehaviour.LeftExcluding, new int[0], new int[0]);

			[Fact]
			public void HasRightReplaceInRight()
				=> TestHasRightReplaceInRight(ActiveListJoinBehaviour.LeftExcluding, new int[0], new int[0]);

			[Fact]
			public void HasBothReplaceInLeft()
				=> TestHasBothReplaceInLeft(ActiveListJoinBehaviour.LeftExcluding, new int[0], new int[0]);

			[Fact]
			public void HasBothReplaceInRight()
				=> TestHasBothReplaceInRight(ActiveListJoinBehaviour.LeftExcluding, new int[0], new int[0]);

			[Fact]
			public void HasLeftRemoveFromLeft()
				=> TestHasLeftRemoveFromLeft(ActiveListJoinBehaviour.LeftExcluding, new[] { 1, 2, 3 }, new int[0]);

			[Fact]
			public void HasRightRemoveFromRight()
				=> TestHasRightRemoveFromRight(ActiveListJoinBehaviour.LeftExcluding, new int[0], new int[0]);

			[Fact]
			public void HasBothRemoveFromLeft()
				=> TestHasBothRemoveFromLeft(ActiveListJoinBehaviour.LeftExcluding, new int[0], new int[0]);

			[Fact]
			public void HasBothRemoveFromRight()
				=> TestHasBothRemoveFromRight(ActiveListJoinBehaviour.LeftExcluding, new int[0], new[] { 1, 2, 3 });

			[Fact]
			public void HasLeftMoveLeft()
				=> TestHasLeftMoveLeft(ActiveListJoinBehaviour.LeftExcluding, new[] { 1, 2, 3 }, new[] { 3, 1, 2 });

			[Fact]
			public void HasRightMoveRight()
				=> TestHasRightMoveRight(ActiveListJoinBehaviour.LeftExcluding, new int[0], new int[0]);

			[Fact]
			public void HasBothMoveLeft()
				=> TestHasBothMoveLeft(ActiveListJoinBehaviour.LeftExcluding, new int[0], new int[0]);

			[Fact]
			public void HasBothMoveRight()
				=> TestHasBothMoveRight(ActiveListJoinBehaviour.LeftExcluding, new int[0], new int[0]);

			[Fact]
			public void EmptySetLeft()
				=> TestEmptySetLeft(ActiveListJoinBehaviour.LeftExcluding, new int[0], new[] { 3, 6, 9 });

			[Fact]
			public void EmptySetRight()
				=> TestEmptySetRight(ActiveListJoinBehaviour.LeftExcluding, new int[0], new int[0]);

			[Fact]
			public void HasLeftSetLeft()
				=> TestHasLeftSetLeft(ActiveListJoinBehaviour.LeftExcluding, new[] { 1, 2, 3 }, new[] { 3, 6, 9 });

			[Fact]
			public void HasLeftSetRight()
				=> TestHasLeftSetRight(ActiveListJoinBehaviour.LeftExcluding, new[] { 1, 2, 3 }, new int[0]);

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
				=> TestHasLeftClearLeft(ActiveListJoinBehaviour.LeftExcluding, new[] { 1, 2, 3 }, new int[0]);

			[Fact]
			public void HasRightClearRight()
				=> TestHasRightClearRight(ActiveListJoinBehaviour.LeftExcluding, new int[0], new int[0]);

			[Fact]
			public void HasBothClearLeft()
				=> TestHasBothClearLeft(ActiveListJoinBehaviour.LeftExcluding, new int[0], new int[0]);

			[Fact]
			public void HasBothClearRight()
				=> TestHasBothClearRight(ActiveListJoinBehaviour.LeftExcluding, new int[0], new[] { 1, 2, 3 });
		}

		public class RightJoin
		{
			[Fact]
			public void EmptyAddToLeft()
				=> TestEmptyAddToLeft(ActiveListJoinBehaviour.Right, new int[0], new int[0]);

			[Fact]
			public void EmptyAddToRight()
				=> TestEmptyAddToRight(ActiveListJoinBehaviour.Right, new int[0], new[] { 20, 200 });

			[Fact]
			public void HasLeftReplaceInLeft()
				=> TestHasLeftReplaceInLeft(ActiveListJoinBehaviour.Right, new int[0], new int[0]);

			[Fact]
			public void HasLeftAddToRight()
				=> TestHasLeftAddToRight(ActiveListJoinBehaviour.Right, new int[0], new[] { 21, 201, 22, 202, 23, 203 });

			[Fact]
			public void HasRightAddToLeft()
				=> TestHasRightAddToLeft(ActiveListJoinBehaviour.Right, new[] { 10, 100 }, new[] { 13, 103, 16, 106, 19, 109 });

			[Fact]
			public void HasRightReplaceInRight()
				=> TestHasRightReplaceInRight(ActiveListJoinBehaviour.Right, new[] { 10, 100 }, new[] { 20, 200 });

			[Fact]
			public void HasBothReplaceInLeft()
				=> TestHasBothReplaceInLeft(ActiveListJoinBehaviour.Right, new[] { 11, 101, 12, 102, 13, 103 }, new[] { 13, 103, 16, 106, 19, 109 });

			[Fact]
			public void HasBothReplaceInRight()
				=> TestHasBothReplaceInRight(ActiveListJoinBehaviour.Right, new[] { 11, 101, 12, 102, 13, 103 }, new[] { 21, 201, 22, 202, 23, 203 });

			[Fact]
			public void HasLeftRemoveFromLeft()
				=> TestHasLeftRemoveFromLeft(ActiveListJoinBehaviour.Right, new int[0], new int[0]);

			[Fact]
			public void HasRightRemoveFromRight()
				=> TestHasRightRemoveFromRight(ActiveListJoinBehaviour.Right, new[] { 10, 100 }, new int[0]);

			[Fact]
			public void HasBothRemoveFromLeft()
				=> TestHasBothRemoveFromLeft(ActiveListJoinBehaviour.Right, new[] { 11, 101, 12, 102, 13, 103 }, new[] { 10, 100 });

			[Fact]
			public void HasBothRemoveFromRight()
				=> TestHasBothRemoveFromRight(ActiveListJoinBehaviour.Right, new[] { 11, 101, 12, 102, 13, 103 }, new int[0]);

			[Fact]
			public void HasLeftMoveLeft()
				=> TestHasLeftMoveLeft(ActiveListJoinBehaviour.Right, new int[0], new int[0]);

			[Fact]
			public void HasRightMoveRight()
				=> TestHasRightMoveRight(ActiveListJoinBehaviour.Right, new[] { 10, 100 }, new[] { 100, 10 });

			[Fact]
			public void HasBothMoveLeft()
				=> TestHasBothMoveLeft(ActiveListJoinBehaviour.Right, new[] { 11, 101, 12, 102, 13, 103 }, new[] { 13, 103, 11, 101, 12, 102 });

			[Fact]
			public void HasBothMoveRight()
				=> TestHasBothMoveRight(ActiveListJoinBehaviour.Right, new[] { 11, 101, 12, 102, 13, 103 }, new[] { 101, 11, 102, 12, 103, 13 });

			[Fact]
			public void EmptySetLeft()
				=> TestEmptySetLeft(ActiveListJoinBehaviour.Right, new int[0], new int[0]);

			[Fact]
			public void EmptySetRight()
				=> TestEmptySetRight(ActiveListJoinBehaviour.Right, new int[0], new[] { 20, 200 });

			[Fact]
			public void HasLeftSetLeft()
				=> TestHasLeftSetLeft(ActiveListJoinBehaviour.Right, new int[0], new int[0]);

			[Fact]
			public void HasLeftSetRight()
				=> TestHasLeftSetRight(ActiveListJoinBehaviour.Right, new int[0], new[] { 21, 201, 22, 202, 23, 203 });

			[Fact]
			public void HasRightSetLeft()
				=> TestHasRightSetLeft(ActiveListJoinBehaviour.Right, new[] { 10, 100 }, new[] { 13, 103, 16, 106, 19, 109 });

			[Fact]
			public void HasRightSetRight()
				=> TestHasRightSetRight(ActiveListJoinBehaviour.Right, new[] { 10, 100 }, new[] { 20, 200 });

			[Fact]
			public void HasBothSetLeft()
				=> TestHasBothSetLeft(ActiveListJoinBehaviour.Right, new[] { 11, 101, 12, 102, 13, 103 }, new[] { 13, 103, 16, 106, 19, 109 });

			[Fact]
			public void HasBothSetRight()
				=> TestHasBothSetRight(ActiveListJoinBehaviour.Right, new[] { 11, 101, 12, 102, 13, 103 }, new[] { 21, 201, 22, 202, 23, 203 });

			[Fact]
			public void HasLeftClearLeft()
				=> TestHasLeftClearLeft(ActiveListJoinBehaviour.Right, new int[0], new int[0]);

			[Fact]
			public void HasRightClearRight()
				=> TestHasRightClearRight(ActiveListJoinBehaviour.Right, new[] { 10, 100 }, new int[0]);

			[Fact]
			public void HasBothClearLeft()
				=> TestHasBothClearLeft(ActiveListJoinBehaviour.Right, new[] { 11, 101, 12, 102, 13, 103 }, new[] { 10, 100 });

			[Fact]
			public void HasBothClearRight()
				=> TestHasBothClearRight(ActiveListJoinBehaviour.Right, new[] { 11, 101, 12, 102, 13, 103 }, new int[0]);
		}

		public class RightExcludingJoin
		{
			[Fact]
			public void EmptyAddToLeft()
				=> TestEmptyAddToLeft(ActiveListJoinBehaviour.RightExcluding, new int[0], new int[0]);

			[Fact]
			public void EmptyAddToRight()
				=> TestEmptyAddToRight(ActiveListJoinBehaviour.RightExcluding, new int[0], new[] { 20, 200 });

			[Fact]
			public void HasLeftReplaceInLeft()
				=> TestHasLeftReplaceInLeft(ActiveListJoinBehaviour.RightExcluding, new int[0], new int[0]);

			[Fact]
			public void HasLeftAddToRight()
				=> TestHasLeftAddToRight(ActiveListJoinBehaviour.RightExcluding, new int[0], new int[0]);

			[Fact]
			public void HasRightAddToLeft()
				=> TestHasRightAddToLeft(ActiveListJoinBehaviour.RightExcluding, new[] { 10, 100 }, new int[0]);

			[Fact]
			public void HasRightReplaceInRight()
				=> TestHasRightReplaceInRight(ActiveListJoinBehaviour.RightExcluding, new[] { 10, 100 }, new[] { 20, 200 });

			[Fact]
			public void HasBothReplaceInLeft()
				=> TestHasBothReplaceInLeft(ActiveListJoinBehaviour.RightExcluding, new int[0], new int[0]);

			[Fact]
			public void HasBothReplaceInRight()
				=> TestHasBothReplaceInRight(ActiveListJoinBehaviour.RightExcluding, new int[0], new int[0]);

			[Fact]
			public void HasLeftRemoveFromLeft()
				=> TestHasLeftRemoveFromLeft(ActiveListJoinBehaviour.RightExcluding, new int[0], new int[0]);

			[Fact]
			public void HasRightRemoveFromRight()
				=> TestHasRightRemoveFromRight(ActiveListJoinBehaviour.RightExcluding, new[] { 10, 100 }, new int[0]);

			[Fact]
			public void HasBothRemoveFromLeft()
				=> TestHasBothRemoveFromLeft(ActiveListJoinBehaviour.RightExcluding, new int[0], new[] { 10, 100 });

			[Fact]
			public void HasBothRemoveFromRight()
				=> TestHasBothRemoveFromRight(ActiveListJoinBehaviour.RightExcluding, new int[0], new int[0]);

			[Fact]
			public void HasLeftMoveLeft()
				=> TestHasLeftMoveLeft(ActiveListJoinBehaviour.RightExcluding, new int[0], new int[0]);

			[Fact]
			public void HasRightMoveRight()
				=> TestHasRightMoveRight(ActiveListJoinBehaviour.RightExcluding, new[] { 10, 100 }, new[] { 100, 10 });

			[Fact]
			public void HasBothMoveLeft()
				=> TestHasBothMoveLeft(ActiveListJoinBehaviour.RightExcluding, new int[0], new int[0]);

			[Fact]
			public void HasBothMoveRight()
				=> TestHasBothMoveRight(ActiveListJoinBehaviour.RightExcluding, new int[0], new int[0]);

			[Fact]
			public void EmptySetLeft()
				=> TestEmptySetLeft(ActiveListJoinBehaviour.RightExcluding, new int[0], new int[0]);

			[Fact]
			public void EmptySetRight()
				=> TestEmptySetRight(ActiveListJoinBehaviour.RightExcluding, new int[0], new[] { 20, 200 });

			[Fact]
			public void HasLeftSetLeft()
				=> TestHasLeftSetLeft(ActiveListJoinBehaviour.RightExcluding, new int[0], new int[0]);

			[Fact]
			public void HasLeftSetRight()
				=> TestHasLeftSetRight(ActiveListJoinBehaviour.RightExcluding, new int[0], new int[0]);

			[Fact]
			public void HasRightSetLeft()
				=> TestHasRightSetLeft(ActiveListJoinBehaviour.RightExcluding, new[] { 10, 100 }, new int[0]);

			[Fact]
			public void HasRightSetRight()
				=> TestHasRightSetRight(ActiveListJoinBehaviour.RightExcluding, new[] { 10, 100 }, new[] { 20, 200 });

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
				=> TestHasRightClearRight(ActiveListJoinBehaviour.RightExcluding, new[] { 10, 100 }, new int[0]);

			[Fact]
			public void HasBothClearLeft()
				=> TestHasBothClearLeft(ActiveListJoinBehaviour.RightExcluding, new int[0], new[] { 10, 100 });

			[Fact]
			public void HasBothClearRight()
				=> TestHasBothClearRight(ActiveListJoinBehaviour.RightExcluding, new int[0], new int[0]);
		}

		public class OuterJoin
		{
			[Fact]
			public void EmptyAddToLeft()
				=> TestEmptyAddToLeft(ActiveListJoinBehaviour.Outer, new int[0], new[] { 3, 6, 9 });

			[Fact]
			public void EmptyAddToRight()
				=> TestEmptyAddToRight(ActiveListJoinBehaviour.Outer, new int[0], new[] { 20, 200 });

			[Fact]
			public void HasLeftReplaceInLeft()
				=> TestHasLeftReplaceInLeft(ActiveListJoinBehaviour.Outer, new[] { 1, 2, 3 }, new[] { 3, 6, 9 });

			[Fact]
			public void HasLeftAddToRight()
				=> TestHasLeftAddToRight(ActiveListJoinBehaviour.Outer, new[] { 1, 2, 3 }, new[] { 21, 201, 22, 202, 23, 203 });

			[Fact]
			public void HasRightAddToLeft()
				=> TestHasRightAddToLeft(ActiveListJoinBehaviour.Outer, new[] { 10, 100 }, new[] { 13, 103, 16, 106, 19, 109 });

			[Fact]
			public void HasRightReplaceInRight()
				=> TestHasRightReplaceInRight(ActiveListJoinBehaviour.Outer, new[] { 10, 100 }, new[] { 20, 200 });

			[Fact]
			public void HasBothReplaceInLeft()
				=> TestHasBothReplaceInLeft(ActiveListJoinBehaviour.Outer, new[] { 11, 101, 12, 102, 13, 103 }, new[] { 13, 103, 16, 106, 19, 109 });

			[Fact]
			public void HasBothReplaceInRight()
				=> TestHasBothReplaceInRight(ActiveListJoinBehaviour.Outer, new[] { 11, 101, 12, 102, 13, 103 }, new[] { 21, 201, 22, 202, 23, 203 });

			[Fact]
			public void HasLeftRemoveFromLeft()
				=> TestHasLeftRemoveFromLeft(ActiveListJoinBehaviour.Outer, new[] { 1, 2, 3 }, new int[0]);

			[Fact]
			public void HasRightRemoveFromRight()
				=> TestHasRightRemoveFromRight(ActiveListJoinBehaviour.Outer, new[] { 10, 100 }, new int[0]);

			[Fact]
			public void HasBothRemoveFromLeft()
				=> TestHasBothRemoveFromLeft(ActiveListJoinBehaviour.Outer, new[] { 11, 101, 12, 102, 13, 103 }, new[] { 10, 100 });

			[Fact]
			public void HasBothRemoveFromRight()
				=> TestHasBothRemoveFromRight(ActiveListJoinBehaviour.Outer, new[] { 11, 101, 12, 102, 13, 103 }, new[] { 1, 2, 3 });

			[Fact]
			public void HasLeftMoveLeft()
				=> TestHasLeftMoveLeft(ActiveListJoinBehaviour.Outer, new[] { 1, 2, 3 }, new[] { 3, 1, 2 });

			[Fact]
			public void HasRightMoveRight()
				=> TestHasRightMoveRight(ActiveListJoinBehaviour.Outer, new[] { 10, 100 }, new[] { 100, 10 });

			[Fact]
			public void HasBothMoveLeft()
				=> TestHasBothMoveLeft(ActiveListJoinBehaviour.Outer, new[] { 11, 101, 12, 102, 13, 103 }, new[] { 13, 103, 11, 101, 12, 102 });

			[Fact]
			public void HasBothMoveRight()
				=> TestHasBothMoveRight(ActiveListJoinBehaviour.Outer, new[] { 11, 101, 12, 102, 13, 103 }, new[] { 101, 11, 102, 12, 103, 13 });

			[Fact]
			public void EmptySetLeft()
				=> TestEmptySetLeft(ActiveListJoinBehaviour.Outer, new int[0], new[] { 3, 6, 9 });

			[Fact]
			public void EmptySetRight()
				=> TestEmptySetRight(ActiveListJoinBehaviour.Outer, new int[0], new[] { 20, 200 });

			[Fact]
			public void HasLeftSetLeft()
				=> TestHasLeftSetLeft(ActiveListJoinBehaviour.Outer, new[] { 1, 2, 3 }, new[] { 3, 6, 9 });

			[Fact]
			public void HasLeftSetRight()
				=> TestHasLeftSetRight(ActiveListJoinBehaviour.Outer, new[] { 1, 2, 3 }, new[] { 21, 201, 22, 202, 23, 203 });

			[Fact]
			public void HasRightSetLeft()
				=> TestHasRightSetLeft(ActiveListJoinBehaviour.Outer, new[] { 10, 100 }, new[] { 13, 103, 16, 106, 19, 109 });

			[Fact]
			public void HasRightSetRight()
				=> TestHasRightSetRight(ActiveListJoinBehaviour.Outer, new[] { 10, 100 }, new[] { 20, 200 });

			[Fact]
			public void HasBothSetLeft()
				=> TestHasBothSetLeft(ActiveListJoinBehaviour.Outer, new[] { 11, 101, 12, 102, 13, 103 }, new[] { 13, 103, 16, 106, 19, 109 });

			[Fact]
			public void HasBothSetRight()
				=> TestHasBothSetRight(ActiveListJoinBehaviour.Outer, new[] { 11, 101, 12, 102, 13, 103 }, new[] { 21, 201, 22, 202, 23, 203 });

			[Fact]
			public void HasLeftClearLeft()
				=> TestHasLeftClearLeft(ActiveListJoinBehaviour.Outer, new[] { 1, 2, 3 }, new int[0]);

			[Fact]
			public void HasRightClearRight()
				=> TestHasRightClearRight(ActiveListJoinBehaviour.Outer, new[] { 10, 100 }, new int[0]);

			[Fact]
			public void HasBothClearLeft()
				=> TestHasBothClearLeft(ActiveListJoinBehaviour.Outer, new[] { 11, 101, 12, 102, 13, 103 }, new[] { 10, 100 });

			[Fact]
			public void HasBothClearRight()
				=> TestHasBothClearRight(ActiveListJoinBehaviour.Outer, new[] { 11, 101, 12, 102, 13, 103 }, new[] { 1, 2, 3 });
		}

		public class OuterExcludingJoin
		{
			[Fact]
			public void EmptyAddToLeft()
				=> TestEmptyAddToLeft(ActiveListJoinBehaviour.OuterExcluding, new int[0], new[] { 3, 6, 9 });

			[Fact]
			public void EmptyAddToRight()
				=> TestEmptyAddToRight(ActiveListJoinBehaviour.OuterExcluding, new int[0], new[] { 20, 200 });

			[Fact]
			public void HasLeftReplaceInLeft()
				=> TestHasLeftReplaceInLeft(ActiveListJoinBehaviour.OuterExcluding, new[] { 1, 2, 3 }, new[] { 3, 6, 9 });

			[Fact]
			public void HasLeftAddToRight()
				=> TestHasLeftAddToRight(ActiveListJoinBehaviour.OuterExcluding, new[] { 1, 2, 3 }, new int[0]);

			[Fact]
			public void HasRightAddToLeft()
				=> TestHasRightAddToLeft(ActiveListJoinBehaviour.OuterExcluding, new[] { 10, 100 }, new int[0]);

			[Fact]
			public void HasRightReplaceInRight()
				=> TestHasRightReplaceInRight(ActiveListJoinBehaviour.OuterExcluding, new[] { 10, 100 }, new[] { 20, 200 });

			[Fact]
			public void HasBothReplaceInLeft()
				=> TestHasBothReplaceInLeft(ActiveListJoinBehaviour.OuterExcluding, new int[0], new int[0]);

			[Fact]
			public void HasBothReplaceInRight()
				=> TestHasBothReplaceInRight(ActiveListJoinBehaviour.OuterExcluding, new int[0], new int[0]);

			[Fact]
			public void HasLeftRemoveFromLeft()
				=> TestHasLeftRemoveFromLeft(ActiveListJoinBehaviour.OuterExcluding, new[] { 1, 2, 3 }, new int[0]);

			[Fact]
			public void HasRightRemoveFromRight()
				=> TestHasRightRemoveFromRight(ActiveListJoinBehaviour.OuterExcluding, new[] { 10, 100 }, new int[0]);

			[Fact]
			public void HasBothRemoveFromLeft()
				=> TestHasBothRemoveFromLeft(ActiveListJoinBehaviour.OuterExcluding, new int[0], new[] { 10, 100 });

			[Fact]
			public void HasBothRemoveFromRight()
				=> TestHasBothRemoveFromRight(ActiveListJoinBehaviour.OuterExcluding, new int[0], new[] { 1, 2, 3 });

			[Fact]
			public void HasLeftMoveLeft()
				=> TestHasLeftMoveLeft(ActiveListJoinBehaviour.OuterExcluding, new[] { 1, 2, 3 }, new[] { 3, 1, 2 });

			[Fact]
			public void HasRightMoveRight()
				=> TestHasRightMoveRight(ActiveListJoinBehaviour.OuterExcluding, new[] { 10, 100 }, new[] { 100, 10 });

			[Fact]
			public void HasBothMoveLeft()
				=> TestHasBothMoveLeft(ActiveListJoinBehaviour.OuterExcluding, new int[0], new int[0]);

			[Fact]
			public void HasBothMoveRight()
				=> TestHasBothMoveRight(ActiveListJoinBehaviour.OuterExcluding, new int[0], new int[0]);

			[Fact]
			public void EmptySetLeft()
				=> TestEmptySetLeft(ActiveListJoinBehaviour.OuterExcluding, new int[0], new[] { 3, 6, 9 });

			[Fact]
			public void EmptySetRight()
				=> TestEmptySetRight(ActiveListJoinBehaviour.OuterExcluding, new int[0], new[] { 20, 200 });

			[Fact]
			public void HasLeftSetLeft()
				=> TestHasLeftSetLeft(ActiveListJoinBehaviour.OuterExcluding, new[] { 1, 2, 3 }, new[] { 3, 6, 9 });

			[Fact]
			public void HasLeftSetRight()
				=> TestHasLeftSetRight(ActiveListJoinBehaviour.OuterExcluding, new[] { 1, 2, 3 }, new int[0]);

			[Fact]
			public void HasRightSetLeft()
				=> TestHasRightSetLeft(ActiveListJoinBehaviour.OuterExcluding, new[] { 10, 100 }, new int[0]);

			[Fact]
			public void HasRightSetRight()
				=> TestHasRightSetRight(ActiveListJoinBehaviour.OuterExcluding, new[] { 10, 100 }, new[] { 20, 200 });

			[Fact]
			public void HasBothSetLeft()
				=> TestHasBothSetLeft(ActiveListJoinBehaviour.OuterExcluding, new int[0], new int[0]);

			[Fact]
			public void HasBothSetRight()
				=> TestHasBothSetRight(ActiveListJoinBehaviour.OuterExcluding, new int[0], new int[0]);

			[Fact]
			public void HasLeftClearLeft()
				=> TestHasLeftClearLeft(ActiveListJoinBehaviour.OuterExcluding, new[] { 1, 2, 3 }, new int[0]);

			[Fact]
			public void HasRightClearRight()
				=> TestHasRightClearRight(ActiveListJoinBehaviour.OuterExcluding, new[] { 10, 100 }, new int[0]);

			[Fact]
			public void HasBothClearLeft()
				=> TestHasBothClearLeft(ActiveListJoinBehaviour.OuterExcluding, new int[0], new[] { 10, 100 });

			[Fact]
			public void HasBothClearRight()
				=> TestHasBothClearRight(ActiveListJoinBehaviour.OuterExcluding, new int[0], new[] { 1, 2, 3 });
		}
	}
}
