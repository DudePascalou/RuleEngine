using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace RuleEngine
{
    public class ConditionNode : ICondition
    {
        /// <summary>
        /// Noeud de condition parent, s'il existe. Null, sinon.
        /// </summary>
        /// <remarks>
        /// Cette propriété est renseignée automatiquement lors de l'affectation
        /// de LeftCondition ou RightCondition.
        /// </remarks>
        public ConditionNode ParentNode { get; private set; }

        /// <summary>
        /// Association reliant les deux conditions du noeud.
        /// </summary>
        public Association? Association { get; set; }

        private ICondition _LeftCondition;
        /// <summary>
        /// Condition de gauche.
        /// </summary>
        public ICondition LeftCondition
        {
            get { return this._LeftCondition; }
            set
            {
                this._LeftCondition = value;

                var node = value as ConditionNode;
                if (node != null)
                    node.ParentNode = this;
            }
        }

        private ICondition _RightCondition;
        /// <summary>
        /// Condition de droite.
        /// </summary>
        public ICondition RightCondition
        {
            get { return this._RightCondition; }
            set
            {
                this._RightCondition = value;

                var node = value as ConditionNode;
                if (node != null)
                    node.ParentNode = this;
            }
        }

        public bool IsTrue()
        {
            if (!this.Association.HasValue)
            {
                if (this.LeftCondition != null && this.RightCondition != null)
                    throw new InvalidOperationException("L'association est obligatoire lorsque les deux conditions du noeud sont renseignées.");
                else if (this.LeftCondition != null)
                    return this.LeftCondition.IsTrue();
                else if (this.RightCondition != null)
                    return this.RightCondition.IsTrue();
                else
                    throw new InvalidOperationException("Au moins une condition doit être renseignée");
            }
            else
            {
                if (this.LeftCondition == null || this.RightCondition == null)
                    throw new InvalidOperationException("Lorsque l'association est renseignée, les deux conditions sont obligatoires.");

                switch (this.Association.Value)
                {
                    case RuleEngine.Association.And:
                        return this.LeftCondition.IsTrue() && this.RightCondition.IsTrue();
                    case RuleEngine.Association.Or:
                        return this.LeftCondition.IsTrue() || this.RightCondition.IsTrue();
                    default:
                        throw new NotSupportedException();
                }
            }
        }


        public override string ToString()
        {
            if (!this.Association.HasValue)
            {
                if (this.RightCondition == null)
                    return string.Format
                    (
                        "({0})",
                        this.LeftCondition
                    );
                else
                    return string.Format
                    (
                        "({0} - {1})",
                        this.LeftCondition,
                        this.RightCondition
                    );
            }
            else
            {
                return string.Format
                (
                    "({0} {1} {2})",
                    this.LeftCondition,
                    this.Association.Value,
                    this.RightCondition
                );
            }
        }
    }
}
