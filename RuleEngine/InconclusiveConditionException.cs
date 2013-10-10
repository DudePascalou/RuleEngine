using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace RuleEngine
{
    /// <summary>
    /// Exception levée lorsque la condition ne peut pas être résolue.
    /// </summary>
    public class InconclusiveConditionException : Exception
    {
        private static readonly string _BasicMessage = "Impossible de déterminer le résultat de la condition.";
        private static readonly string _ExtendedMessage = "Impossible de déterminer le résultat de la condition." + Environment.NewLine + "Consulter l'exception interne pour plus de détail.";

        public InconclusiveConditionException()
            : base(InconclusiveConditionException._BasicMessage)
        { }

        public InconclusiveConditionException(string message)
            : base(InconclusiveConditionException._ExtendedMessage, new Exception(message))
        { }

        public InconclusiveConditionException(Exception innerException)
            : base(InconclusiveConditionException._ExtendedMessage, innerException)
        { }
    }
}
