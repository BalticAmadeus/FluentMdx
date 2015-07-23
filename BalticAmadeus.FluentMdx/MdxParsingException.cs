using System;

namespace BalticAmadeus.FluentMdx
{
    public class MdxParsingException : Exception
    {
        public MdxParsingException(string message) : base(message)
        {
        }
    }
}
