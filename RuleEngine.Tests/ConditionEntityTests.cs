using System;
using System.Text;
using System.Collections.Generic;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace RuleEngine.Tests
{
    [TestClass]
    public class ConditionEntityTests
    {
        #region Exceptions

        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException))]
        public void ParseNullConditionListTest()
        {
            // Arrange :
            IEnumerable<ConditionEntity> nullConditionList = null;

            // Act :
            ConditionEntity.Parse(nullConditionList);

            // Assert :
            Assert.Fail("Ce test doit lever une exception.");
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException))]
        public void ParseEmptyConditionListTest()
        {
            // Arrange :
            IEnumerable<ConditionEntity> emptyConditionList = new List<ConditionEntity>(0);

            // Act :
            ConditionEntity.Parse(emptyConditionList);

            // Assert :
            Assert.Fail("Ce test doit lever une exception.");
        }

        [TestMethod]
        [ExpectedException(typeof(InvalidOperationException))]
        public void ParseConditionListWithNullConditionTest()
        {
            // Arrange :
            IEnumerable<ConditionEntity> conditionListWithNullItem = new List<ConditionEntity>(new ConditionEntity[1] { null });

            // Act :
            ConditionEntity.Parse(conditionListWithNullItem);

            // Assert :
            Assert.Fail("Ce test doit lever une exception.");
        }

        [TestMethod]
        [ExpectedException(typeof(InvalidOperationException))]
        public void ParseWithTooManyOpeningBracketsTest()
        {
            // Arrange :
            IList<ConditionEntity> conditionList = new List<ConditionEntity>(1);
            conditionList.Add(new ConditionEntity()
            {
                Association = null,
                NombreParenthesesOuvrantes = 1,
                OperandeGaucheSaisieLibre = null,
                Operateur = Operator.Equal,
                OperandeDroitSaisieLibre = null,
                NombreParenthesesFermantes = 0
            });

            // Act :
            ConditionEntity.Parse(conditionList);

            // Assert :
            Assert.Fail("Ce test doit lever une exception.");
        }

        [TestMethod]
        [ExpectedException(typeof(InvalidOperationException))]
        public void ParseWithTooManyClosingBracketsTest()
        {
            // Arrange :
            IList<ConditionEntity> conditionList = new List<ConditionEntity>(1);
            conditionList.Add(new ConditionEntity()
            {
                Association = null,
                NombreParenthesesOuvrantes = 0,
                OperandeGaucheSaisieLibre = null,
                Operateur = Operator.Equal,
                OperandeDroitSaisieLibre = null,
                NombreParenthesesFermantes = 1
            });

            // Act :
            ConditionEntity.Parse(conditionList);

            // Assert :
            Assert.Fail("Ce test doit lever une exception.");
        }

        [TestMethod]
        [ExpectedException(typeof(InvalidOperationException))]
        public void ParseWithUnmatchedBracketsTest()
        {
            // Arrange :
            IList<ConditionEntity> conditionList = new List<ConditionEntity>(2);
            conditionList.Add(new ConditionEntity()
            {
                Association = null,
                NombreParenthesesOuvrantes = 0,
                OperandeGaucheSaisieLibre = null,
                Operateur = Operator.Equal,
                OperandeDroitSaisieLibre = null,
                NombreParenthesesFermantes = 1
            });
            conditionList.Add(new ConditionEntity()
            {
                Association = null,
                NombreParenthesesOuvrantes = 1,
                OperandeGaucheSaisieLibre = null,
                Operateur = Operator.Equal,
                OperandeDroitSaisieLibre = null,
                NombreParenthesesFermantes = 0
            });

            // Act :
            ConditionEntity.Parse(conditionList);

            // Assert :
            Assert.Fail("Ce test doit lever une exception.");
        }

        [TestMethod]
        [ExpectedException(typeof(InvalidOperationException))]
        public void ParseWithFirstElementWithAssociationTest()
        {
            // Arrange :
            IList<ConditionEntity> conditionList = new List<ConditionEntity>(1);
            conditionList.Add(new ConditionEntity()
            {
                Association = Association.And,
                NombreParenthesesOuvrantes = 0,
                OperandeGaucheSaisieLibre = null,
                Operateur = Operator.Equal,
                OperandeDroitSaisieLibre = null,
                NombreParenthesesFermantes = 0
            });

            // Act :
            ConditionEntity.Parse(conditionList);

            // Assert :
            Assert.Fail("Ce test doit lever une exception.");
        }

        #endregion



        #region 1 condition

        [TestMethod]
        public void ParseSimpleConditionTest()
        {
            // Arrange :
            var conditions = this.GetSimpleConditionEntities();

            // Act :
            var tree = ConditionEntity.Parse(conditions);

            // Assert :
            Assert.IsTrue(tree.IsTrue());
            Assert.AreEqual("(A Equal A)", tree.Root.ToString());
        }

        private IEnumerable<ConditionEntity> GetSimpleConditionEntities()
        {
            var conditions = new List<ConditionEntity>(1);

            var cA = new ConditionEntity()
            {
                Association = null,
                NombreParenthesesOuvrantes = 0,
                OperandeGaucheSaisieLibre = "A",
                Operateur = Operator.Equal,
                OperandeDroitSaisieLibre = "A",
                NombreParenthesesFermantes = 0
            };
            conditions.Add(cA);

            return conditions;
        }

        #endregion



        #region 3 conditions sans priorité

        [TestMethod]
        public void ParseThreeConditionEntitiesWithoutPrioritiesTest()
        {
            // Arrange :
            var conditions = this.GetThreeConditionEntitiesWithoutPriorities(true, Association.And, true, Association.And, true);

            // Act :
            var tree = ConditionEntity.Parse(conditions);

            // Assert :
            Assert.IsTrue(tree.IsTrue());
            Assert.AreEqual("(A Equal A And (B Equal B And C Equal C))", tree.Root.ToString());

            // Arrange :
            conditions = this.GetThreeConditionEntitiesWithoutPriorities(true, Association.And, true, Association.And, false);

            // Act :
            tree = ConditionEntity.Parse(conditions);

            // Assert :
            Assert.IsFalse(tree.IsTrue());
            Assert.AreEqual("(A Equal A And (B Equal B And C Equal #))", tree.Root.ToString());

            // Arrange :
            conditions = this.GetThreeConditionEntitiesWithoutPriorities(false, Association.Or, false, Association.Or, false);

            // Act :
            tree = ConditionEntity.Parse(conditions);

            // Assert :
            Assert.IsFalse(tree.IsTrue());
            Assert.AreEqual("(A Equal # Or (B Equal # Or C Equal #))", tree.Root.ToString());

            // Arrange :
            conditions = this.GetThreeConditionEntitiesWithoutPriorities(false, Association.Or, false, Association.Or, true);

            // Act :
            tree = ConditionEntity.Parse(conditions);

            // Assert :
            Assert.IsTrue(tree.IsTrue());
            Assert.AreEqual("(A Equal # Or (B Equal # Or C Equal C))", tree.Root.ToString());
        }

        [TestMethod]
        [ExpectedException(typeof(InvalidOperationException))]
        [Ignore()] // En attente de l'implémentation de la vérification...
        public void ParseThreeConditionEntitiesWithoutPrioritiesButWithDifferentAssociationsTest()
        {
            // Arrange :
            var conditions = this.GetThreeConditionEntitiesWithoutPriorities(true, Association.And, true, Association.Or, true);

            // Act :
            var tree = ConditionEntity.Parse(conditions);

            // Assert :
            Assert.Fail("Ce test doit lever une exception");
        }

        private IEnumerable<ConditionEntity> GetThreeConditionEntitiesWithoutPriorities(bool aIsTrue, Association aAndBAssociation, bool bIsTrue, Association bAndCAssociation, bool cIsTrue)
        {
            var conditions = new List<ConditionEntity>(3);

            var cA = new ConditionEntity()
            {
                Association = null,
                NombreParenthesesOuvrantes = 0,
                OperandeGaucheSaisieLibre = "A",
                Operateur = Operator.Equal,
                OperandeDroitSaisieLibre = aIsTrue ? "A" : "#",
                NombreParenthesesFermantes = 0
            };
            conditions.Add(cA);

            var cB = new ConditionEntity()
            {
                Association = aAndBAssociation,
                NombreParenthesesOuvrantes = 0,
                OperandeGaucheSaisieLibre = "B",
                Operateur = Operator.Equal,
                OperandeDroitSaisieLibre = bIsTrue ? "B" : "#",
                NombreParenthesesFermantes = 0
            };
            conditions.Add(cB);

            var cC = new ConditionEntity()
            {
                Association = bAndCAssociation,
                NombreParenthesesOuvrantes = 0,
                OperandeGaucheSaisieLibre = "C",
                Operateur = Operator.Equal,
                OperandeDroitSaisieLibre = cIsTrue ? "C" : "#",
                NombreParenthesesFermantes = 0
            };
            conditions.Add(cC);

            return conditions;
        }

        #endregion



        #region 3 conditions avec priorités

        [TestMethod]
        public void ParseThreeConditionEntitiesWithPrioritiesTest()
        {
            this.ParseActAssertHelper
            (
                inputConditionEntities: this.GetThreeConditionEntitiesWithPriorities("(", true, Association.Or, "", true, ")", Association.And, false, ""),
                expectedResult: false,
                expectedConditionTree: "((A Equal A Or B Equal B) And C Equal #)"
            );

            this.ParseActAssertHelper
            (
                inputConditionEntities: this.GetThreeConditionEntitiesWithPriorities("(", true, Association.Or, "", false, ")", Association.And, false, ""),
                expectedResult: false,
                expectedConditionTree: "((A Equal A Or B Equal #) And C Equal #)"
            );

            this.ParseActAssertHelper
            (
                inputConditionEntities: this.GetThreeConditionEntitiesWithPriorities("", true, Association.Or, "(", true, "", Association.And, false, ")"),
                expectedResult: true,
                expectedConditionTree: "(A Equal A Or (B Equal B And C Equal #))"
            );

            this.ParseActAssertHelper
            (
                inputConditionEntities: this.GetThreeConditionEntitiesWithPriorities("", true, Association.Or, "(", false, "", Association.And, false, ")"),
                expectedResult: true,
                expectedConditionTree: "(A Equal A Or (B Equal # And C Equal #))"
            );
        }

        private IEnumerable<ConditionEntity> GetThreeConditionEntitiesWithPriorities(string aOpeningBrackets, bool aConditionResult, Association aAndBAssociation, string bOpeningBrackets, bool bConditionResult, string bClosingBrackets, Association bAndCAssociation, bool cConditionResult, string cClosingBrackets)
        {
            var conditions = new List<ConditionEntity>(3);

            var cA = new ConditionEntity()
            {
                Association = null,
                NombreParenthesesOuvrantes = aOpeningBrackets.Length,
                OperandeGaucheSaisieLibre = "A",
                Operateur = Operator.Equal,
                OperandeDroitSaisieLibre = aConditionResult ? "A" : "#",
                NombreParenthesesFermantes = 0
            };
            conditions.Add(cA);

            var cB = new ConditionEntity()
            {
                Association = aAndBAssociation,
                NombreParenthesesOuvrantes = bOpeningBrackets.Length,
                OperandeGaucheSaisieLibre = "B",
                Operateur = Operator.Equal,
                OperandeDroitSaisieLibre = bConditionResult ? "B" : "#",
                NombreParenthesesFermantes = bClosingBrackets.Length
            };
            conditions.Add(cB);

            var cC = new ConditionEntity()
            {
                Association = bAndCAssociation,
                NombreParenthesesOuvrantes = 0,
                OperandeGaucheSaisieLibre = "C",
                Operateur = Operator.Equal,
                OperandeDroitSaisieLibre = cConditionResult ? "C" : "#",
                NombreParenthesesFermantes = cClosingBrackets.Length
            };
            conditions.Add(cC);

            return conditions;
        }

        #endregion



        #region 4 conditions sans priorité

        [TestMethod]
        public void ParseFourConditionEntitiesWithoutPrioritiesTest()
        {
            this.ParseActAssertHelper
            (
                inputConditionEntities: this.GetFourConditionEntitiesWithoutPriorities(true, Association.And, true, Association.And, true, Association.And, true),
                expectedResult: true,
                expectedConditionTree: "(A Equal A And (B Equal B And (C Equal C And D Equal D)))"
            );

            this.ParseActAssertHelper
            (
                inputConditionEntities: this.GetFourConditionEntitiesWithoutPriorities(false, Association.And, true, Association.And, true, Association.And, true),
                expectedResult: false,
                expectedConditionTree: "(A Equal # And (B Equal B And (C Equal C And D Equal D)))"
            );

            this.ParseActAssertHelper
            (
                inputConditionEntities: this.GetFourConditionEntitiesWithoutPriorities(true, Association.Or, true, Association.Or, true, Association.Or, true),
                expectedResult: true,
                expectedConditionTree: "(A Equal A Or (B Equal B Or (C Equal C Or D Equal D)))"
            );

            this.ParseActAssertHelper
            (
                inputConditionEntities: this.GetFourConditionEntitiesWithoutPriorities(false, Association.Or, true, Association.Or, true, Association.Or, true),
                expectedResult: true,
                expectedConditionTree: "(A Equal # Or (B Equal B Or (C Equal C Or D Equal D)))"
            );
        }

        private IEnumerable<ConditionEntity> GetFourConditionEntitiesWithoutPriorities(bool aConditionResult, Association aAndBAssociation, bool bConditionResult, Association bAndCAssociation, bool cConditionResult, Association cAndDAssociation, bool dConditionResult)
        {
            return this.GetFourConditionEntitiesWithPriorities("", aConditionResult, aAndBAssociation, "", bConditionResult, "", bAndCAssociation, "", cConditionResult, "", cAndDAssociation, "", dConditionResult, "");
        }

        #endregion



        #region 4 conditions avec priorités

        [TestMethod]
        public void ParseFourConditionEntitiesWithPrioritiesTest()
        {
            this.ParseActAssertHelper
            (
                inputConditionEntities: this.GetFourConditionEntitiesWithPriorities("((", true, Association.Or, "", true, ")", Association.And, "", true, ")", Association.Or, "", true, ""),
                expectedResult: true,
                expectedConditionTree: "(((A Equal A Or B Equal B) And C Equal C) Or D Equal D)"
            );

            this.ParseActAssertHelper
            (
                inputConditionEntities: this.GetFourConditionEntitiesWithPriorities("((", false, Association.Or, "", false, ")", Association.And, "", true, ")", Association.Or, "", false, ""),
                expectedResult: false,
                expectedConditionTree: "(((A Equal # Or B Equal #) And C Equal C) Or D Equal #)"
            );

            this.ParseActAssertHelper
            (
                inputConditionEntities: this.GetFourConditionEntitiesWithPriorities("((", false, Association.Or, "", false, ")", Association.And, "", false, ")", Association.Or, "", true, ""),
                expectedResult: true,
                expectedConditionTree: "(((A Equal # Or B Equal #) And C Equal #) Or D Equal D)"
            );

            this.ParseActAssertHelper
            (
                inputConditionEntities: this.GetFourConditionEntitiesWithPriorities("((", false, Association.Or, "", false, ")", Association.And, "", false, ")", Association.Or, "", false, ""),
                expectedResult: false,
                expectedConditionTree: "(((A Equal # Or B Equal #) And C Equal #) Or D Equal #)"
            );

            this.ParseActAssertHelper
            (
                inputConditionEntities: this.GetFourConditionEntitiesWithPriorities("(", true, Association.Or, "", true, ")", Association.And, "(", true, "", Association.Or, "", true, ")"),
                expectedResult: true,
                expectedConditionTree: "((A Equal A Or B Equal B) And (C Equal C Or D Equal D))"
            );

            this.ParseActAssertHelper
            (
                inputConditionEntities: this.GetFourConditionEntitiesWithPriorities("(", false, Association.Or, "", true, ")", Association.And, "(", false, "", Association.Or, "", true, ")"),
                expectedResult: true,
                expectedConditionTree: "((A Equal # Or B Equal B) And (C Equal # Or D Equal D))"
            );

            this.ParseActAssertHelper
            (
                inputConditionEntities: this.GetFourConditionEntitiesWithPriorities("(", false, Association.Or, "", false, ")", Association.And, "(", true, "", Association.Or, "", true, ")"),
                expectedResult: false,
                expectedConditionTree: "((A Equal # Or B Equal #) And (C Equal C Or D Equal D))"
            );

            this.ParseActAssertHelper
            (
                inputConditionEntities: this.GetFourConditionEntitiesWithPriorities("(", true, Association.Or, "", true, ")", Association.And, "(", false, "", Association.Or, "", false, ")"),
                expectedResult: false,
                expectedConditionTree: "((A Equal A Or B Equal B) And (C Equal # Or D Equal #))"
            );
        }

        private IEnumerable<ConditionEntity> GetFourConditionEntitiesWithPriorities(string aOpeningBrackets, bool aConditionResult, Association aAndBAssociation, string bOpeningBrackets, bool bConditionResult, string bClosingBrackets, Association bAndCAssociation, string cOpeningBrackets, bool cConditionResult, string cClosingBrackets, Association cAndDAssociation, string dOpeningBrackets, bool dConditionResult, string dClosingBrackets)
        {
            var conditions = new List<ConditionEntity>(4);

            var cA = new ConditionEntity()
            {
                Association = null,
                NombreParenthesesOuvrantes = aOpeningBrackets.Length,
                OperandeGaucheSaisieLibre = "A",
                Operateur = Operator.Equal,
                OperandeDroitSaisieLibre = aConditionResult ? "A" : "#",
                NombreParenthesesFermantes = 0
            };
            conditions.Add(cA);

            var cB = new ConditionEntity()
            {
                Association = aAndBAssociation,
                NombreParenthesesOuvrantes = bOpeningBrackets.Length,
                OperandeGaucheSaisieLibre = "B",
                Operateur = Operator.Equal,
                OperandeDroitSaisieLibre = bConditionResult ? "B" : "#",
                NombreParenthesesFermantes = bClosingBrackets.Length
            };
            conditions.Add(cB);

            var cC = new ConditionEntity()
            {
                Association = bAndCAssociation,
                NombreParenthesesOuvrantes = cOpeningBrackets.Length,
                OperandeGaucheSaisieLibre = "C",
                Operateur = Operator.Equal,
                OperandeDroitSaisieLibre = cConditionResult ? "C" : "#",
                NombreParenthesesFermantes = cClosingBrackets.Length
            };
            conditions.Add(cC);

            var cD = new ConditionEntity()
            {
                Association = cAndDAssociation,
                NombreParenthesesOuvrantes = dOpeningBrackets.Length,
                OperandeGaucheSaisieLibre = "D",
                Operateur = Operator.Equal,
                OperandeDroitSaisieLibre = dConditionResult ? "D" : "#",
                NombreParenthesesFermantes = dClosingBrackets.Length
            };
            conditions.Add(cD);

            return conditions;
        }

        #endregion



        #region Helper

        /// <summary>
        /// Utilitaire d'assertion de la méthode Parse.
        /// </summary>
        /// <param name="inputConditionEntities">Condition(s) passée(s) en entrée de la méthode.</param>
        /// <param name="expectedResult">Résultat attendu (true/false).</param>
        /// <param name="expectedConditionTree">Chaîne représentant l'arbre de condition (<see cref="ConditionTree"/>) attendu.</param>
        private void ParseActAssertHelper(IEnumerable<ConditionEntity> inputConditionEntities, bool expectedResult, string expectedConditionTree)
        {
            // Act :
            var tree = ConditionEntity.Parse(inputConditionEntities);

            // Assert :
            if (expectedResult)
                Assert.IsTrue(tree.IsTrue());
            else
                Assert.IsFalse(tree.IsTrue());
            Assert.AreEqual(expectedConditionTree, tree.Root.ToString());
        }

        #endregion
    }
}
