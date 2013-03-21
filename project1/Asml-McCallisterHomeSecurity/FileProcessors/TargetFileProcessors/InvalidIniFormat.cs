/*
 * InvalidIniFormat.cs
 * implements an exception for the IniProcessor class.
 * Written for CptS323 at Washington State University, Spring 2013
 * Written By: Chris Walters
 * Last Modified By: Chris Walters
 * Date Modified: March 14, 2013
 */
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TargetFileProcessors
{
    /// <summary>
    /// InvalidIniFormat, a standard exception to clarify why it is being thrown. 
    /// </summary>
    [Serializable]
    public class InvalidIniFormat : Exception
    {
        public InvalidIniFormat()
        {
        }

        public InvalidIniFormat(string message)
            : base(message)
        {
        }

        public InvalidIniFormat(string message, Exception inner)
            : base(message, inner)
        {
        }

        protected InvalidIniFormat(System.Runtime.Serialization.SerializationInfo info, System.Runtime.Serialization.StreamingContext context)
            : base(info, context)
        {
        }
    }
}
