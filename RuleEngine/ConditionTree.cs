using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace RuleEngine
{
    public class ConditionTree : ICondition
    {
        public ICondition Root { get; set; }

        public ConditionTree(ConditionNode node)
        {
            this.Root = node;
        }

        public bool IsTrue()
        {
            return this.Root.IsTrue();
        }
    }
}
