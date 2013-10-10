using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace RuleEngine
{
    public class Condition<T> : ICondition where T : IComparable<T>
    {
        public T LeftOperand { get; set; }
        public Operator Operator { get; set; }
        public T RightOperand { get; set; }


        public Condition(T leftOperand, Operator conditionOperator, T rightOperand)
        {
            this.LeftOperand = leftOperand;
            this.Operator = conditionOperator;
            this.RightOperand = rightOperand;
        }


        public bool IsTrue()
        {
            int comparisonResult = this.LeftOperand.CompareTo(this.RightOperand);

            switch (this.Operator)
            {
                case Operator.Equal:
                    return comparisonResult == 0;
                case Operator.NotEqual:
                    return comparisonResult != 0;
                case Operator.GreaterThan:
                    return comparisonResult > 0;
                case Operator.LessThan:
                    return comparisonResult < 0;
                case Operator.GreaterThanOrEqual:
                    return comparisonResult >= 0;
                case Operator.LessThanOrEqual:
                    return comparisonResult <= 0;
                default:
                    throw new NotSupportedException();
            }
        }

        public override string ToString()
        {
            return string.Format
            (
                "{0} {1} {2}",
                this.LeftOperand,
                this.Operator,
                this.RightOperand
            ).Trim();
        }
    }
}
