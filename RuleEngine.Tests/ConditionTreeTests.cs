using System;
using System.Text;
using System.Collections.Generic;
using System.Linq;
using NUnit.Framework;

namespace RuleEngine.Tests
{
    [TestFixture]
    public class ConditionTreeTests
    {
        [Test]
        public void IsTrueTest()
        {
            // Exemple simple :
            // Association | ( | Gauche | Opérateur | Droite | )
            //                   FormeS      =          SA

            // Exemple avec association :
            // Association | ( | Gauche | Opérateur | Droite | )
            //                   FormeS      =          SA
            //     OU            FormeS      =          SAS

            // Exemple avec association et priorité :
            // Association | ( | Gauche | Opérateur | Droite | )
            //               (   FormeS      =          SA          => c1
            //     ET            Capital     >          50000  )    => c2
            //     OU            FormeS      =          SAS         => c3

            // Arrange
            var tree = this.BuildTree("SA", 100000);

            // Act - Assert :
            Assert.IsTrue(tree.IsTrue());

            // Arrange
            tree = this.BuildTree("SAS", 1000);

            // Act - Assert :
            Assert.IsTrue(tree.IsTrue());

            // Arrange
            tree = this.BuildTree("SA", 1000);

            // Act - Assert :
            Assert.IsFalse(tree.IsTrue());
        }


        private ConditionTree BuildTree(string formeSociale, int capital)
        {
            var c1 = new Condition<string>(formeSociale, Operator.Equal, "SA");
            var c2 = new Condition<int>(capital, Operator.GreaterThan, 50000);
            var c3 = new Condition<string>(formeSociale, Operator.Equal, "SAS");

            var c1c2Node = new ConditionNode()
            {
                LeftCondition = c1,
                Association = Association.And,
                RightCondition = c2
            };
            var c1c2c3Node = new ConditionNode()
            {
                LeftCondition = c1c2Node,
                Association = Association.Or,
                RightCondition = c3
            };

            return new ConditionTree(c1c2c3Node);
        }
    }
}
