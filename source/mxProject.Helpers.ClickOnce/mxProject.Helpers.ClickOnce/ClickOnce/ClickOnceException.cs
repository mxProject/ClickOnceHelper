using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace mxProject.ClickOnce
{

    /// <summary>
    /// 
    /// </summary>
    public class ClickOnceException : Exception
    {

        /// <summary>
        /// Create a new instance.
        /// </summary>
        /// <param name="message">The error message that explains the reason for the exception.</param>
        /// <param name="innerException">The exception that is the cause of the current exception.</param>
        public ClickOnceException(string message, Exception innerException) : base(message, innerException)
        {
        }

    }

}
