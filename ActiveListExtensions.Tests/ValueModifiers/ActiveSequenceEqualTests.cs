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

		private void CreateLists(out IActiveValue<bool> sut, out Func<bool> linq, out ObservableList<IntegerTestClass> listOne, out ObservableList<IntegerTestClass> listTwo, int[] numbersOne, int[] numbersTwo = null)
		{
			if (numbersTwo == null)
				numbersTwo = numbersOne;

			var left = listOne = new ObservableList<IntegerTestClass>();
			var right = listTwo = new ObservableList<IntegerTestClass>();

			foreach (var value in Enumerable.Range(0, Math.Min(numbersOne.Length, numbersTwo.Length)))
			{
				listOne.Add(value, new IntegerTestClass() { Property = numbersOne[value] });
				listTwo.Add(value, new IntegerTestClass() { Property = numbersTwo[value] });
			}

			sut = left.ActiveSequenceEqual(right, (i1, i2) => i1.Property == i2.Property);
			linq = () => left.SequenceEqual(right, new KeyEqualityComparer<IntegerTestClass>(i => i.Property, null));
		}
	}
}
