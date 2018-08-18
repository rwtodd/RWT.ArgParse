using System;

namespace RWT.ArgParse
{
    public class ArgParseException : ArgumentException
    {
        internal ArgParseException(String txt) : base(txt)
        {

        }
    }
}
