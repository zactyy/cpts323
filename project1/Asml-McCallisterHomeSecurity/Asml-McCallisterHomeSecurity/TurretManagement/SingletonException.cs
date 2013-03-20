using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Asml_McCallisterHomeSecurity.TurretManagement// singleton exception class
{
    [Serializable]
    class SingletonException : Exception
    {
        public SingletonException(): base()
        {
        }
        public SingletonException(string message): base(message)
        { 
        }
        public SingletonException(string message, Exception inner)
            : base(message,inner)
        {
        }
        protected SingletonException(System.Runtime.Serialization.SerializationInfo info, System.Runtime.Serialization.StreamingContext context)
            : base(info,context)
        {
        }
    }
}
