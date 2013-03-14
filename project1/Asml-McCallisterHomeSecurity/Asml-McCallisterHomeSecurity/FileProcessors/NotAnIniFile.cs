/*
 * NotAnIniFile.cs
 * implements an exception for the IniProcessor class.
 * Written for CptS323 at Washington State University, Spring 2013
 * Written By: Chris Walters
 * Last Modified By: Chris Walters
 * Last Modified: March 14, 2013
 */

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Asml_McCallisterHomeSecurity.FileProcessors
{
    /// <summary>
    /// NotAnIniFile, a standard exception to clarify why it is being thrown.
    /// </summary>
    [Serializable]
    public class NotAnIniFile : Exception
    {
        public NotAnIniFile()
        {
        }

        public NotAnIniFile(string message)
            : base(message)
        {
        }

        public NotAnIniFile(string message, Exception inner)
            : base(message, inner)
        {
        }

        protected NotAnIniFile(System.Runtime.Serialization.SerializationInfo info, System.Runtime.Serialization.StreamingContext context)
            : base(info, context)
        {
        }
    }
}
