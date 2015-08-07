using System;
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
            Assert.Throws<ArgumentNullException>(() => { new StatedTwoWayEnumerator<string>(null); });
        }

        [Test]
        public void TwoWayEnumerator_MoveForwardAndBack_WithFewElements_Succeeded()
        {
            //ARRANGE
            var elements = new List<string> {"FirstItem", "SecondItem"};

            //ACT
            //ASSERT
            using (var twoWayEnumerator = elements.GetStatedTwoWayEnumerator())
            {
                Assert.Throws<InvalidOperationException>(() => { var c = twoWayEnumerator.Current; });
                twoWayEnumerator.Reset();
                Assert.Throws<InvalidOperationException>(() => { var c = twoWayEnumerator.Current; });
                Assert.Throws<InvalidOperationException>(() => { twoWayEnumerator.MovePrevious(); });
                Assert.That(twoWayEnumerator.MoveNext(), Is.True);
                Assert.That(twoWayEnumerator.Current, Is.EqualTo("FirstItem"));
                Assert.That(twoWayEnumerator.MoveNext(), Is.True);
                Assert.That(twoWayEnumerator.Current, Is.EqualTo("SecondItem"));
                Assert.That(twoWayEnumerator.MoveNext(), Is.False);
                Assert.That(twoWayEnumerator.MovePrevious(), Is.True);
                Assert.That(twoWayEnumerator.Current, Is.EqualTo("FirstItem"));
                Assert.That(((IEnumerator) twoWayEnumerator).Current, Is.EqualTo(twoWayEnumerator.Current));
            }
        }

        [Test]
        public void TwoWayEnumerator_Rollback_Ok()
        {
            var elements = new List<string>{"A", "B", "C", "D"};

            using (var enumerator = elements.GetStatedTwoWayEnumerator())
            {
                enumerator.MoveNext();
                enumerator.MoveNext();
                enumerator.MoveNext();
                enumerator.SavePosition();
                enumerator.MoveNext();
                enumerator.RestoreLastSavedPosition();
                enumerator.SavePosition();
                enumerator.MoveNext();
                enumerator.RemoveLastSavedState();
                enumerator.Reset();


            }
        }
    }
}
