using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;

namespace BalticAmadeus.FluentMdx.Lexer
{
    internal class Lexer : ILexer
    {
        private static readonly IEnumerable<TokenDefinition> TokenDefinitions = new List<TokenDefinition>
        {
            new TokenDefinition(TokenType.Select, "SELECT"),
            new TokenDefinition(TokenType.Non, "NON"),
            new TokenDefinition(TokenType.Empty, "EMPTY"),
            new TokenDefinition(TokenType.On, "ON"),
            new TokenDefinition(TokenType.From, "FROM"),
            new TokenDefinition(TokenType.Where, "WHERE"),
            new TokenDefinition(TokenType.Comma, "\\,"),
            new TokenDefinition(TokenType.LeftCurlyBracket, "\\{"),
            new TokenDefinition(TokenType.RightCurlyBracket, "\\}"),
            new TokenDefinition(TokenType.LeftRoundBracket, "\\("),
            new TokenDefinition(TokenType.RightRoundBracket, "\\)"),
            new TokenDefinition(TokenType.LeftSquareBracket, "\\["),
            new TokenDefinition(TokenType.RightSquareBracket, "\\]"),
            new TokenDefinition(TokenType.RangeSeparator, "\\:"),
            new TokenDefinition(TokenType.ValueSeparator, "\\.\\&"),
            new TokenDefinition(TokenType.IdentifierSeparator, "\\."),
            
            new TokenDefinition(TokenType.AxisName, "(COLUMNS)|(ROWS)|(PAGES)|(CHAPTERS)|(SECTIONS)|(AXIS\\([0-9]+\\))"),
            new TokenDefinition(TokenType.NumberExpression, "[0-9]+"),
            new TokenDefinition(TokenType.IdentifierExpression, "[a-zA-Z_][a-zA-Z0-9 \\/\\\\\\-\\:]*[a-zA-Z0-9_]"),
        };

        private readonly IEnumerable<TokenDefinition> _tokenDefinitions;

        public Lexer()
        {
            _tokenDefinitions = TokenDefinitions;
        }

        public IEnumerable<Token> Tokenize(string source)
        {
            var endOfLineRegex = new Regex("[\\n|\\r]");

            int currentIndex = 0;
            
            while (currentIndex < source.Length)
            {
                if (source[currentIndex] == ' ')
                {
                    currentIndex++;
                    continue;
                }

                TokenDefinition matchedDefinition = null;
                int matchLength = 0;

                foreach (var rule in _tokenDefinitions)
                {
                    var match = rule.Pattern.Match(source, currentIndex);

                    if (!match.Success || (match.Index - currentIndex) != 0) 
                        continue;
                    
                    matchedDefinition = rule;
                    matchLength = match.Length;
                    break;
                }

                if (matchedDefinition == null)
                    throw new Exception(string.Format("Unrecognized symbol '{0}'.", source[currentIndex]));
                
                var value = source.Substring(currentIndex, matchLength);

                yield return
                    new Token(matchedDefinition.Type, value);

                endOfLineRegex.Match(value);
                currentIndex += matchLength;
            }

            yield return new Token(TokenType.LastToken, string.Empty);
        }
    }
}