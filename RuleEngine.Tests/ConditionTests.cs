﻿using System;
using System.Text;
using System.Collections.Generic;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace RuleEngine.Tests
{
    [TestClass]
    public class ConditionTests
    {
        [TestMethod]
        public void IsTrueWithEqualMethod()
        {
            // Arrange :
            var c1 = new Condition<string>(string.Empty, Operator.Equal, string.Empty);

            // Act - Assert :
            Assert.IsTrue(c1.IsTrue());
        }

        [TestMethod]
        public void IsTrueWithNotEqualMethod()
        {
            // Arrange :
            var c1 = new Condition<string>(string.Empty, Operator.NotEqual, "test");

            // Act - Assert :
            Assert.IsTrue(c1.IsTrue());
        }

        [TestMethod]
        public void IsTrueWithGreaterMethod()
        {
            // Arrange :
            var c1 = new Condition<int>(2, Operator.GreaterThan, 1);

            // Act - Assert :
            Assert.IsTrue(c1.IsTrue());
        }

        [TestMethod]
        public void IsTrueWithLessMethod()
        {
            // Arrange :
            var c1 = new Condition<int>(1, Operator.LessThan, 2);

            // Act - Assert :
            Assert.IsTrue(c1.IsTrue());
        }

        [TestMethod]
        public void IsTrueWithGreaterOrEqualMethod()
        {
            // Arrange :
            var c1 = new Condition<int>(2, Operator.GreaterThan, 1);

            // Act - Assert :
            Assert.IsTrue(c1.IsTrue());

            // Arrange :
            var c2 = new Condition<int>(1, Operator.GreaterThanOrEqual, 1);

            // Act - Assert :
            Assert.IsTrue(c2.IsTrue());
        }

        [TestMethod]
        public void IsTrueWithLessOrEqualMethod()
        {
            // Arrange :
            var c1 = new Condition<int>(1, Operator.LessThanOrEqual, 2);

            // Act - Assert :
            Assert.IsTrue(c1.IsTrue());

            // Arrange :
            var c2 = new Condition<int>(1, Operator.LessThanOrEqual, 1);

            // Act - Assert :
            Assert.IsTrue(c2.IsTrue());
        }



        #region Helpers

        public static Condition<string> ATrueCondition()
        {
            return new Condition<string>(string.Empty, Operator.Equal, string.Empty);
        }

        public static Condition<string> AFalseCondition()
        {
            return new Condition<string>(string.Empty, Operator.Equal, "#");
        }

        #endregion
    }
}
