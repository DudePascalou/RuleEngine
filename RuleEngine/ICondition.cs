using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace RuleEngine
{
    public interface ICondition
    {
        bool IsTrue();
    }
}
