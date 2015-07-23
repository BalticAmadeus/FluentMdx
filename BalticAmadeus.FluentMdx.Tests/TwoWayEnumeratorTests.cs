﻿using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using BalticAmadeus.FluentMdx.EnumerableExtensions;
using NUnit.Framework;

namespace BalticAmadeus.FluentMdx.Tests
{
    [TestFixture, ExcludeFromCodeCoverage]
    public class TwoWayEnumeratorTests
    {
        [Test]
        public void TwoWayEnumerator_Create_WithNullListEnumerator_ThrowsArgumentNullException()
        {
            //ARRANGE
            //ACT
            //ASSERT
            Assert.Throws<ArgumentNullException>(() => { new TwoWayEnumerator<string>(null); });
        }

        [Test]
        public void TwoWayEnumerator_MoveForwardAndBack_WithFewElements_Succeeded()
        {
            //ARRANGE
            var elements = new List<string> {"FirstItem", "SecondItem", "ThirdItem"};

            //ACT
            //ASSERT
            using (var twoWayEnumerator = elements.GetTwoWayEnumerator())
            {
                Assert.Throws<InvalidOperationException>(() => { var c = twoWayEnumerator.Current; });
                twoWayEnumerator.Reset();
                Assert.Throws<InvalidOperationException>(() => { var c = twoWayEnumerator.Current; });
                Assert.Throws<InvalidOperationException>(() => { twoWayEnumerator.MovePrevious(); });
                Assert.That(twoWayEnumerator.MoveNext(), Is.True);
                Assert.That(twoWayEnumerator.Current, Is.EqualTo("FirstItem"));
                Assert.That(twoWayEnumerator.MoveNext(), Is.True);
                Assert.That(twoWayEnumerator.Current, Is.EqualTo("SecondItem"));
                Assert.That(twoWayEnumerator.MovePrevious(), Is.True);
                Assert.That(twoWayEnumerator.Current, Is.EqualTo("FirstItem"));
                Assert.That(((IEnumerator) twoWayEnumerator).Current, Is.EqualTo(twoWayEnumerator.Current));
            }
        }
    }
}
