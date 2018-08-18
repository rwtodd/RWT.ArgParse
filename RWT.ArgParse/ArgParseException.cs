using System;

namespace RWT.ArgParse
{
    /// <summary>
    /// thrown when there is a problem parsing cmdline arguments.
    /// </summary>
    public class ArgParseException : Exception
    {
        public ArgParseException(String txt) : base(txt)
        {

        }
    }
}
