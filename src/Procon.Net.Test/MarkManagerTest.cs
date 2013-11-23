using System;
using NUnit.Framework;

namespace Procon.Net.Test {
    [TestFixture]
    public class MarkManagerTest {

        /// <summary>
        /// Tests the window is open if the mark list is empty.
        /// </summary>
        [Test]
        public void TestEmptyMarkListOpenWindow() {
            MarkManager marks = new MarkManager();

            Assert.IsTrue(marks.IsValidMarkWindow());
        }

        /// <summary>
        /// Tests throwing a simple 20 marks in quick succession will close the window.
        /// </summary>
        [Test]
        public void TestTwentyMarkListClosedWindow() {
            MarkManager marks = new MarkManager();

            for (var count = 0; count < 20; count++) {
                marks.Mark();
            }

            Assert.IsFalse(marks.IsValidMarkWindow());
        }

        /// <summary>
        /// Tests removing expires marks on an empty list results in no change.
        /// </summary>
        [Test]
        public void TestEmptyMarkListRemoveExpiredNoChange() {
            MarkManager marks = new MarkManager();

            marks.RemoveExpiredMarks();

            Assert.AreEqual(0, marks.Marks.Count);
        }

        /// <summary>
        /// Tests that removing expired marks will remove a mark one hour old.
        /// </summary>
        [Test]
        public void TestTwoMarkListRemoveExpiredOneOld() {
            MarkManager marks = new MarkManager();

            marks.Mark();
            marks.Mark(DateTime.Now.AddHours(-1));

            marks.RemoveExpiredMarks();

            Assert.AreEqual(1, marks.Marks.Count);
        }
    }
}
