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
        public string OperandeGaucheSaisieLibre { get; set; }
        public TypeSource OperandeGaucheTypeSource { get; set; }
        public Question OperandeGaucheQuestion { get; set; }
        public Operator Operateur { get; set; }
        public string OperandeDroitSaisieLibre { get; set; }
        public TypeSource OperandeDroitTypeSource { get; set; }
        public Question OperandeDroitQuestion { get; set; }
        public int NombreParenthesesFermantes { get; set; }


        public override string ToString()
        {
            return string.Format
            (
                "{0} {1} {2} {3} {4} {5}",
                this.Association.HasValue ? this.Association.Value.ToString() : string.Empty,
                new string('(', this.NombreParenthesesOuvrantes),
                this.OperandeGaucheSaisieLibre,
                this.Operateur,
                this.OperandeDroitSaisieLibre,
                new string(')', this.NombreParenthesesFermantes)
            ).Trim();
        }


        public bool OperandsAreCompatible()
        {
            // TODO : vérifier la compatibilité des opérandes gauche et droite
            return true;
        }


        private ICondition BuildCondition()
        {
            ICondition condition;
            var operandsType = this.GetOperandsType();

            if (operandsType.Equals(typeof(int)))
                condition = new Condition<int>(this.GetOperandeGaucheAsInt(), this.Operateur, this.GetOperandeDroitAsInt());
            else if (operandsType.Equals(typeof(DateTime)))
                condition = new Condition<DateTime>(this.GetOperandeGaucheAsDateTime(), this.Operateur, this.GetOperandeDroitAsDateTime());
            else
                condition = new Condition<string>(this.GetOperandeGaucheAsString(), this.Operateur, this.GetOperandeDroitAsString());

            return condition;
        }

        private DateTime GetOperandeDroitAsDateTime()
        {
            throw new NotImplementedException();
        }

        private DateTime GetOperandeGaucheAsDateTime()
        {
            throw new NotImplementedException();
        }

        private int GetOperandeDroitAsInt()
        {
            throw new NotImplementedException();
        }

        private int GetOperandeGaucheAsInt()
        {
            throw new NotImplementedException();
        }

        public Type GetOperandsType()
        {
            if (!this.OperandsAreCompatible())
                throw new InvalidOperationException();

            if (this.OperandeGaucheTypeSource == TypeSource.Question && this.OperandeGaucheQuestion.FormatSaisie == FormatSaisie.Entier)
                return typeof(int);
            if (this.OperandeGaucheTypeSource == TypeSource.Question && this.OperandeGaucheQuestion.FormatSaisie == FormatSaisie.Date)
                return typeof(DateTime);
            if (this.OperandeDroitTypeSource == TypeSource.Question && this.OperandeDroitQuestion.FormatSaisie == FormatSaisie.Entier)
                return typeof(int);
            if (this.OperandeDroitTypeSource == TypeSource.Question && this.OperandeDroitQuestion.FormatSaisie == FormatSaisie.Date)
                return typeof(DateTime);

            return typeof(string);
        }

        private string GetOperandeGaucheAsString()
        {
            // TODO
            switch (this.OperandeGaucheTypeSource)
            {
                case TypeSource.SaisieLibre:
                    return this.OperandeGaucheSaisieLibre;
                case TypeSource.Enumeration:
                    break;
                case TypeSource.BaseDeDonnees:
                    break;
                case TypeSource.Question:
                    break;
                case TypeSource.Regle:
                    throw new InvalidOperationException("Une règle ne peut pas être utilisée pour une condition.");
                default:
                    throw new NotSupportedException();
            }

            return string.Empty;
        }

        private string GetOperandeDroitAsString()
        {
            // TODO
            switch (this.OperandeDroitTypeSource)
            {
                case TypeSource.SaisieLibre:
                    return this.OperandeDroitSaisieLibre;
                case TypeSource.Enumeration:
                    break;
                case TypeSource.BaseDeDonnees:
                    break;
                case TypeSource.Question:
                    break;
                case TypeSource.Regle:
                    throw new InvalidOperationException("Une règle ne peut pas être utilisée pour une condition.");
                default:
                    throw new NotSupportedException();
            }

            return string.Empty;
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
                currentNode.LeftCondition = currentConditionEntity.BuildCondition();
            }
            else if (currentNode.RightCondition != null)
            {
                currentNode.RightCondition = new ConditionNode()
                {
                    LeftCondition = currentNode.RightCondition,
                    RightCondition = currentConditionEntity.BuildCondition()
                };
                currentNode = currentNode.RightCondition as ConditionNode;
                if (currentConditionEntity.Association.HasValue) currentNode.Association = currentConditionEntity.Association;
            }
            else
            {
                currentNode.RightCondition = currentConditionEntity.BuildCondition();
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
