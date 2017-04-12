using ActiveListExtensions.Tests.Helpers;
using ActiveListExtensions.Utilities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xunit;

namespace ActiveListExtensions.Tests.ValueModifiers
{
	public class ActiveSequenceEqualTests
	{
		[Fact]
		public void AddToMakeNotEqual()
		{
			CreateLists(out IActiveValue<bool> sut, out Func<bool> linq, out ObservableList<IntegerTestClass> listOne, out ObservableList<IntegerTestClass> listTwo, new[] { 1, 2, 3 }, new[] { 1, 2, 3 });

			Assert.Equal(sut.Value, linq.Invoke());

			listOne.Add(0, new IntegerTestClass());

			Assert.Equal(sut.Value, linq.Invoke());
		}

		[Fact]
		public void AddToMakeEqual()
		{
			CreateLists(out IActiveValue<bool> sut, out Func<bool> linq, out ObservableList<IntegerTestClass> listOne, out ObservableList<IntegerTestClass> listTwo, new[] { 1, 2, 3 }, new[] { 1, 3 });

			Assert.Equal(sut.Value, linq.Invoke());

			listTwo.Add(1, new IntegerTestClass() { Property = 2 });

			Assert.Equal(sut.Value, linq.Invoke());
		}

		[Fact]
		public void RemoveToMakeNotEqual()
		{
			CreateLists(out IActiveValue<bool> sut, out Func<bool> linq, out ObservableList<IntegerTestClass> listOne, out ObservableList<IntegerTestClass> listTwo, new[] { 1, 2, 3 }, new[] { 1, 2, 3 });

			Assert.Equal(sut.Value, linq.Invoke());

			listOne.Remove(1);

			Assert.Equal(sut.Value, linq.Invoke());
		}

		[Fact]
		public void RemoveToMakeEqual()
		{
			CreateLists(out IActiveValue<bool> sut, out Func<bool> linq, out ObservableList<IntegerTestClass> listOne, out ObservableList<IntegerTestClass> listTwo, new[] { 1, 2 }, new[] { 1, 2, 3 });

			Assert.Equal(sut.Value, linq.Invoke());

			listTwo.Remove(2);

			Assert.Equal(sut.Value, linq.Invoke());
		}

		[Fact]
		public void MoveToMakeNotEqual()
		{
			CreateLists(out IActiveValue<bool> sut, out Func<bool> linq, out ObservableList<IntegerTestClass> listOne, out ObservableList<IntegerTestClass> listTwo, new[] { 1, 2, 3 }, new[] { 1, 2, 3 });

			Assert.Equal(sut.Value, linq.Invoke());

			listOne.Move(0, 2);

			Assert.Equal(sut.Value, linq.Invoke());
		}

		[Fact]
		public void MoveToMakeEqual()
		{
			CreateLists(out IActiveValue<bool> sut, out Func<bool> linq, out ObservableList<IntegerTestClass> listOne, out ObservableList<IntegerTestClass> listTwo, new[] { 1, 2, 3 }, new[] { 2, 1, 3 });

			Assert.Equal(sut.Value, linq.Invoke());

			listTwo.Move(1, 0);

			Assert.Equal(sut.Value, linq.Invoke());
		}

		[Fact]
		public void ReplaceToMakeNotEqual()
		{
			CreateLists(out IActiveValue<bool> sut, out Func<bool> linq, out ObservableList<IntegerTestClass> listOne, out ObservableList<IntegerTestClass> listTwo, new[] { 1, 2, 3 }, new[] { 1, 2, 3 });

			Assert.Equal(sut.Value, linq.Invoke());

			listOne.Replace(0, new IntegerTestClass() { Property = 0 });

			Assert.Equal(sut.Value, linq.Invoke());
		}

		[Fact]
		public void ReplaceToMakeEqual()
		{
			CreateLists(out IActiveValue<bool> sut, out Func<bool> linq, out ObservableList<IntegerTestClass> listOne, out ObservableList<IntegerTestClass> listTwo, new[] { 1, 2, 3 }, new[] { 1, 2, 4 });

			Assert.Equal(sut.Value, linq.Invoke());

			listTwo.Replace(2, new IntegerTestClass() { Property = 3 });

			Assert.Equal(sut.Value, linq.Invoke());
		}

		[Fact]
		public void ResetToMakeNotEqual()
		{
			CreateLists(out IActiveValue<bool> sut, out Func<bool> linq, out ObservableList<IntegerTestClass> listOne, out ObservableList<IntegerTestClass> listTwo, new[] { 1, 2, 3 }, new[] { 1, 2, 3 });

			Assert.Equal(sut.Value, linq.Invoke());

			listOne.Reset(new[] { new IntegerTestClass() });

			Assert.Equal(sut.Value, linq.Invoke());
		}

		[Fact]
		public void ResetToMakeEqual()
		{
			CreateLists(out IActiveValue<bool> sut, out Func<bool> linq, out ObservableList<IntegerTestClass> listOne, out ObservableList<IntegerTestClass> listTwo, new[] { 1, 2, 3 }, new[] { 1 });

			Assert.Equal(sut.Value, linq.Invoke());

			listTwo.Reset(new[] { 1, 2, 3 }.Select(i => new IntegerTestClass() { Property = i }));

			Assert.Equal(sut.Value, linq.Invoke());
		}

		[Fact]
		public void ChangePropertyToMakeNotEqual()
		{
			CreateLists(out IActiveValue<bool> sut, out Func<bool> linq, out ObservableList<IntegerTestClass> listOne, out ObservableList<IntegerTestClass> listTwo, new[] { 1, 2, 3 }, new[] { 1, 2, 3 });

			Assert.Equal(sut.Value, linq.Invoke());

			listOne[1].Property = 0;

			Assert.Equal(sut.Value, linq.Invoke());
		}

		[Fact]
		public void ChangePropertyToMakeEqual()
		{
			CreateLists(out IActiveValue<bool> sut, out Func<bool> linq, out ObservableList<IntegerTestClass> listOne, out ObservableList<IntegerTestClass> listTwo, new[] { 1, 2, 3 }, new[] { 1, 2, 0 });

			Assert.Equal(sut.Value, linq.Invoke());

			listTwo[2].Property = 3;

			Assert.Equal(sut.Value, linq.Invoke());
		}

		[Fact]
		public void ChangeParameterToMakeNotEqual()
		{
			var parameter = new ActiveValue<IntegerTestClass>(new IntegerTestClass() { Property = 1 });

			CreateLists(out IActiveValue<bool> sut, out Func<bool> linq, out ObservableList<IntegerTestClass> listOne, out ObservableList<IntegerTestClass> listTwo, new[] { 1, 2, 3 }, new[] { 1, 2, 3 }, parameter);

			Assert.Equal(sut.Value, linq.Invoke());

			parameter.Value = new IntegerTestClass() { Property = 2 };

			Assert.Equal(sut.Value, linq.Invoke());
		}

		[Fact]
		public void ChangeParameterToMakeEqual()
		{
			var parameter = new ActiveValue<IntegerTestClass>(new IntegerTestClass() { Property = 1 });

			CreateLists(out IActiveValue<bool> sut, out Func<bool> linq, out ObservableList<IntegerTestClass> listOne, out ObservableList<IntegerTestClass> listTwo, new[] { 2, 4, 6 }, new[] { 1, 2, 3 }, parameter);

			Assert.Equal(sut.Value, linq.Invoke());

			parameter.Value = new IntegerTestClass() { Property = 2 };

			Assert.Equal(sut.Value, linq.Invoke());
		}

		[Fact]
		public void ChangeParameterValueToMakeNotEqual()
		{
			var parameter = new ActiveValue<IntegerTestClass>(new IntegerTestClass() { Property = 1 });

			CreateLists(out IActiveValue<bool> sut, out Func<bool> linq, out ObservableList<IntegerTestClass> listOne, out ObservableList<IntegerTestClass> listTwo, new[] { 1, 2, 3 }, new[] { 1, 2, 3 }, parameter);

			Assert.Equal(sut.Value, linq.Invoke());

			parameter.Value.Property = 2;

			Assert.Equal(sut.Value, linq.Invoke());
		}

		[Fact]
		public void ChangeParameterValueToMakeEqual()
		{
			var parameter = new ActiveValue<IntegerTestClass>(new IntegerTestClass() { Property = 1 });

			CreateLists(out IActiveValue<bool> sut, out Func<bool> linq, out ObservableList<IntegerTestClass> listOne, out ObservableList<IntegerTestClass> listTwo, new[] { 2, 4, 6 }, new[] { 1, 2, 3 }, parameter);

			Assert.Equal(sut.Value, linq.Invoke());

			parameter.Value.Property = 2;

			Assert.Equal(sut.Value, linq.Invoke());
		}

		private void CreateLists(out IActiveValue<bool> sut, out Func<bool> linq, out ObservableList<IntegerTestClass> listOne, out ObservableList<IntegerTestClass> listTwo, int[] numbersOne, int[] numbersTwo, IActiveValue<IntegerTestClass> parameter = null)
		{
			var left = listOne = new ObservableList<IntegerTestClass>();
			var right = listTwo = new ObservableList<IntegerTestClass>();

			foreach (var value in Enumerable.Range(0, Math.Max(numbersOne.Length, numbersTwo.Length)))
			{
				if (value < numbersOne.Length)
					listOne.Add(value, new IntegerTestClass() { Property = numbersOne[value] });
				if (value < numbersTwo.Length)
					listTwo.Add(value, new IntegerTestClass() { Property = numbersTwo[value] });
			}

			if (parameter != null)
				sut = left.ActiveSequenceEqual(right, parameter, (i1, i2, p) => i1.Property == i2.Property * p.Property);
			else
				sut = left.ActiveSequenceEqual(right, (i1, i2) => i1.Property == i2.Property);
			linq = () => left.Select(i => i.Property).SequenceEqual(right.Select(i => i.Property * (parameter?.Value.Property ?? 1)), new KeyEqualityComparer<int>(i => i, null));
		}
	}
}
