using System;

namespace Isotope.Code.Utils
{
    /// <summary>
    /// Exception for indicating that a resource does not exist.
    /// </summary>
    public class NotFoundException: Exception
    {
        public NotFoundException(string message)
            : base(message)
        {
            
        }
    }
}