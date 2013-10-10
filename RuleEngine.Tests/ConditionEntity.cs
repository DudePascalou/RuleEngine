using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Collections;

namespace RuleEngine.Tests
{
    public class ConditionEntity
    {
        public Association? Association { get; set; }
        public int NombreParenthesesOuvrantes { get; set; }
        public string OperandeGauche { get; set; }
        public Operator Operateur { get; set; }
        public string OperandeDroit { get; set; }
        public int NombreParenthesesFermantes { get; set; }


        public override string ToString()
        {
            return string.Format
            (
                "{0} {1} {2} {3} {4} {5}",
                this.Association.HasValue ? this.Association.Value.ToString() : string.Empty,
                new string('(', this.NombreParenthesesOuvrantes),
                this.OperandeGauche,
                this.Operateur,
                this.OperandeDroit,
                new string(')', this.NombreParenthesesFermantes)
            ).Trim();
        }


        public static ConditionTree Parse(IEnumerable<ConditionEntity> conditionEntities)
        {
            if (conditionEntities == null) throw new ArgumentNullException("conditionEntities");
            var conditionEntityList = conditionEntities.ToList();
            ConditionEntity.ValidateConditions(conditionEntityList);

            ConditionNode rootNode = new ConditionNode();
            var conditionsEnumerator = conditionEntityList.GetEnumerator();
            conditionsEnumerator.MoveNext();
            ConditionEntity.InnerParse(conditionsEnumerator, rootNode);

            return new ConditionTree(rootNode);
        }


        private static void InnerParse(IEnumerator<ConditionEntity> conditionsEnumerator, ConditionNode currentNode)
        {
            var currentConditionEntity = conditionsEnumerator.Current;

            for (int i = 0; i < currentConditionEntity.NombreParenthesesOuvrantes; i++)
            {
                if (currentNode.LeftCondition == null)
                {
                    currentNode.LeftCondition = new ConditionNode();
                    if (currentConditionEntity.Association.HasValue) currentNode.Association = currentConditionEntity.Association;
                    currentNode = currentNode.LeftCondition as ConditionNode;
                }
                else
                {
                    currentNode.RightCondition = new ConditionNode();
                    if (currentConditionEntity.Association.HasValue) currentNode.Association = currentConditionEntity.Association;
                    currentNode = currentNode.RightCondition as ConditionNode;
                }
            }

            if (currentNode.LeftCondition == null)
            {
                currentNode.LeftCondition = new Condition<string>(currentConditionEntity.OperandeGauche, currentConditionEntity.Operateur, currentConditionEntity.OperandeDroit); // TODO : interpréter les opérandees provenant de la base...
            }
            else if (currentNode.RightCondition != null)
            {
                currentNode.RightCondition = new ConditionNode()
                {
                    LeftCondition = currentNode.RightCondition,
                    RightCondition = new Condition<string>(currentConditionEntity.OperandeGauche, currentConditionEntity.Operateur, currentConditionEntity.OperandeDroit) // TODO : interpréter les opérandees provenant de la base...
                };
                currentNode = currentNode.RightCondition as ConditionNode;
                if (currentConditionEntity.Association.HasValue) currentNode.Association = currentConditionEntity.Association;
            }
            else
            {
                currentNode.RightCondition = new Condition<string>(currentConditionEntity.OperandeGauche, currentConditionEntity.Operateur, currentConditionEntity.OperandeDroit); // TODO : interpréter les opérandees provenant de la base...
                if (currentConditionEntity.Association.HasValue) currentNode.Association = currentConditionEntity.Association;
            }

            for (int i = 0; i < currentConditionEntity.NombreParenthesesFermantes; i++)
            {
                currentNode = currentNode.ParentNode;
            }

            var isEndOfList = !conditionsEnumerator.MoveNext();
            if (!isEndOfList)
                ConditionEntity.InnerParse(conditionsEnumerator, currentNode);
            else
                while (currentNode.ParentNode != null)
                    currentNode = currentNode.ParentNode;
        }


        private static void ValidateConditions(IEnumerable<ConditionEntity> conditionEntities)
        {
            var bracketStack = new Stack();
            bool isFirstItem = true;

            if (conditionEntities == null || conditionEntities.Count() == 0)
                throw new ArgumentNullException("conditionEntities");

            foreach (var item in conditionEntities)
            {
                if (item == null)
                    throw new InvalidOperationException("La liste des conditions ne doit pas contenir d'élément nul.");
                if (isFirstItem)
                {
                    isFirstItem = false;
                    if (item.Association.HasValue)
                        throw new InvalidOperationException("Le premier élément de la liste des conditions ne doit pas avoir d'association.");
                }
                for (int i = 0; i < item.NombreParenthesesOuvrantes; i++)
                    bracketStack.Push(null);
                for (int i = 0; i < item.NombreParenthesesFermantes; i++)
                {
                    try
                    {
                        bracketStack.Pop();
                    }
                    catch (InvalidOperationException ex)
                    {
                        throw new InvalidOperationException("L'imbrication des parenthèses est incorrecte ou il y a trop de parenthèses fermantes.", ex);
                    }
                }
            }

            var stackIsEmpty = true;
            try
            {
                bracketStack.Pop();
                stackIsEmpty = false;
            }
            catch (InvalidOperationException)
            {
                // Normal, la pile est vide.
            }
            if (!stackIsEmpty)
                throw new InvalidOperationException("Il y a trop de parenthèses ouvrantes.");

            // TODO : Vérifier que les associations inclues dans une paire de parenthèses sont les mêmes.
            // Exemple : (A ET B OU C) ne veut rien dire. De plus, dans cet exemple, le résultat peut être faussé
            // car l'opérateur ET est prioritaire par rapport au OU, mais l'arbre des conditions qui sera produit
            // va donner la priorité au OU : (A ET (B OU C)).
        }
    }
}
