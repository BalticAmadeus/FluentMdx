using System.Collections.Generic;

namespace BalticAmadeus.FluentMdx.Lexer
{
    /// <summary>
    /// Transforms characters sequence into list of <see cref="Token"/> objects.
    /// </summary>
    internal interface ILexer
    {
        /// <summary>
        /// Transforms text into list of <see cref="Token"/> objects.
        /// </summary>
        /// <param name="source">Text to tokenize.</param>
        /// <returns>Returns list of tokenized items.</returns>
        IEnumerable<Token> Tokenize(string source);
    }
}