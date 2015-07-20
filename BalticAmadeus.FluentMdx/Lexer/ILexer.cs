using System.Collections.Generic;

namespace BalticAmadeus.FluentMdx.Lexer
{
    internal interface ILexer
    {
        IEnumerable<Token> Tokenize(string source);
    }
}