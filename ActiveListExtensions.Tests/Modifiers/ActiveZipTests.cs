﻿using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ActiveListExtensions.Tests.Helpers;
using Xunit;

namespace ActiveListExtensions.Tests.Modifiers
{
    public class ActiveZipTests
    {
        [Fact]
        public void RandomlyInsertItems() => CollectionTestHelpers.RandomlyInsertItemsIntoTwoCollections((l1, l2) => l1.ActiveZip(l2, (i1, i2) => i1 + i2), (l1, l2) => l1.Zip(l2, (i1, i2) => i1 + i2), () => RandomGenerator.GenerateRandomInteger(0, 10), () => RandomGenerator.GenerateRandomInteger(0, 10));

        [Fact]
        public void RandomlyRemoveItems() => CollectionTestHelpers.RandomlyRemoveItemsFromTwoCollections((l1, l2) => l1.ActiveZip(l2, (i1, i2) => i1 + i2), (l1, l2) => l1.Zip(l2, (i1, i2) => i1 + i2), () => RandomGenerator.GenerateRandomInteger(0, 10), () => RandomGenerator.GenerateRandomInteger(0, 10));

        [Fact]
        public void RandomlyReplaceItems() => CollectionTestHelpers.RandomlyReplaceItemsInTwoCollections((l1, l2) => l1.ActiveZip(l2, (i1, i2) => i1 + i2), (l1, l2) => l1.Zip(l2, (i1, i2) => i1 + i2), () => RandomGenerator.GenerateRandomInteger(0, 10), () => RandomGenerator.GenerateRandomInteger(0, 10));

        [Fact]
        public void RandomlyMoveItems() => CollectionTestHelpers.RandomlyMoveItemsWithinTwoCollections((l1, l2) => l1.ActiveZip(l2, (i1, i2) => i1 + i2), (l1, l2) => l1.Zip(l2, (i1, i2) => i1 + i2), () => RandomGenerator.GenerateRandomInteger(0, 10), () => RandomGenerator.GenerateRandomInteger(0, 10));

        [Fact]
        public void ResetWithRandomItems() => CollectionTestHelpers.ResetTwoCollectionsWithRandomItems((l1, l2) => l1.ActiveZip(l2, (i1, i2) => i1 + i2), (l1, l2) => l1.Zip(l2, (i1, i2) => i1 + i2), () => RandomGenerator.GenerateRandomInteger(0, 10), () => RandomGenerator.GenerateRandomInteger(0, 10));

        [Fact]
        public void RandomlyChangePropertyValues() => CollectionTestHelpers.RandomlyChangePropertyValuesInTwoCollections((l1, l2) => l1.ActiveZip(l2, (i1, i2) => i1.Property + i2.OtherProperty), (l1, l2) => l1.Zip(l2, (i1, i2) => i1.Property + i2.OtherProperty), () => new ActiveZipTestClassOne() { Property = RandomGenerator.GenerateRandomInteger() }, () => new ActiveZipTestClassTwo() { OtherProperty = RandomGenerator.GenerateRandomInteger() }, o => o.Property = RandomGenerator.GenerateRandomInteger(), o => o.OtherProperty = RandomGenerator.GenerateRandomInteger());
    }

    public class ActiveZipTestClassOne : INotifyPropertyChanged
    {
        private int _property;
        public int Property
        {
            get { return _property; }
            set
            {
                if (_property == value)
                    return;
                _property = value;
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(Property)));
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;
    }

    public class ActiveZipTestClassTwo : INotifyPropertyChanged
    {
        private int _otherProperty;
        public int OtherProperty
        {
            get { return _otherProperty; }
            set
            {
                if (_otherProperty == value)
                    return;
                _otherProperty = value;
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(OtherProperty)));
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;
    }
}
