using System.Text.RegularExpressions;

namespace BalticAmadeus.FluentMdx.Lexer
{
    internal class TokenDefinition
    {
        public Regex Pattern { get; private set; }
        public TokenType Type {get; private set; }
        
        public TokenDefinition(TokenType type, string pattern)
        {
            Type = type;
            Pattern = new Regex(pattern, RegexOptions.IgnoreCase);
        }
    }
}