using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OperationsManager
{
    /// <summary>
    /// Operations Manager error, a standard exception to clarify why it is being thrown. 
    /// </summary>
    [Serializable]
    public class OperationsError : Exception
    {
        public OperationsError()
        {
        }

        public OperationsError(string message)
            : base(message)
        {
        }

        public OperationsError(string message, Exception inner)
            : base(message, inner)
        {
        }

        protected OperationsError(System.Runtime.Serialization.SerializationInfo info, System.Runtime.Serialization.StreamingContext context)
            : base(info, context)
        {
        }
    }
}
