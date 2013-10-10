using System;
using System.Text;
using System.Collections.Generic;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace RuleEngine.Tests
{
    [TestClass]
    public class ConditionNodeTests
    {
        #region ParentNode

        [TestMethod]
        public void ParentNodeFromLeftConditionTest()
        {
            // Arrange :
            var leftNode = new ConditionNode();
            var parentNode = new ConditionNode();
            Assert.IsNull(leftNode.ParentNode);

            // Act :
            parentNode.LeftCondition = leftNode;

            // Assert :
            Assert.IsNotNull(leftNode.ParentNode);
            Assert.AreEqual(parentNode, leftNode.ParentNode);
        }

        [TestMethod]
        public void ParentNodeFromRightConditionTest()
        {
            // Arrange :
            var rightNode = new ConditionNode();
            var parentNode = new ConditionNode();
            Assert.IsNull(rightNode.ParentNode);

            // Act :
            parentNode.RightCondition = rightNode;

            // Assert :
            Assert.IsNotNull(rightNode.ParentNode);
            Assert.AreEqual(parentNode, rightNode.ParentNode);
        }

        #endregion



        #region IsTrue

        [TestMethod]
        [ExpectedException(typeof(InvalidOperationException))]
        public void IsTrueWithNoAssociationTest()
        {
            // Arrange :
            var node = new ConditionNode()
            {
                LeftCondition = ConditionTests.ATrueCondition(),
                Association = null,
                RightCondition = ConditionTests.ATrueCondition()
            };

            // Act :
            node.IsTrue();

            // Assert :
            Assert.Fail("Ce test doit lever une exception.");
        }

        [TestMethod]
        [ExpectedException(typeof(InvalidOperationException))]
        public void IsTrueWithNoConditionNorAssociationTest()
        {
            // Arrange :
            var node = new ConditionNode()
            {
                LeftCondition = null,
                Association = null,
                RightCondition = null
            };

            // Act :
            node.IsTrue();

            // Assert :
            Assert.Fail("Ce test doit lever une exception.");
        }

        [TestMethod]
        [ExpectedException(typeof(InvalidOperationException))]
        public void IsTrueWithNoConditionTest()
        {
            // Arrange :
            var node = new ConditionNode()
            {
                LeftCondition = null,
                Association = Association.And,
                RightCondition = null
            };

            // Act :
            node.IsTrue();

            // Assert :
            Assert.Fail("Ce test doit lever une exception.");
        }

        #endregion
    }
}
