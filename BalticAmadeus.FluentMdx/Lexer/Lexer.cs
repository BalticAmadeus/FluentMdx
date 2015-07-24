using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;

namespace BalticAmadeus.FluentMdx.Lexer
{
    internal class Lexer : ILexer
    {
        private static readonly IEnumerable<TokenDefinition> TokenDefinitions;

        static Lexer()
        {
            TokenDefinitions = new List<TokenDefinition>
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

                new TokenDefinition(TokenType.Properties, "PROPERTIES"),
                new TokenDefinition(TokenType.Dimension, "DIMENSION"),
                new TokenDefinition(TokenType.DimensionProperty,
                    "(CATALOG_NAME)|(CHILDREN_CARDINALITY)|(CUSTOM_ROLLUP)|(CUSTOM_ROLLUP_PROPERTIES)|(DESCRIPTION)|(DIMENSION_UNIQUE_NAME)|(HIERARCHY_UNIQUE_NAME)|(IS_DATAMEMBER)|(IS_PLACEHOLDERMEMBER)|(KEY0)|(LCID)|(LEVEL_NUMBER)|(LEVEL_UNIQUE_NAME)|(MEMBER_CAPTION)|(MEMBER_KEY)|(MEMBER_NAME)|(MEMBER_TYPE)|(MEMBER_UNIQUE_NAME)|(MEMBER_VALUE)|(PARENT_COUNT)|(PARENT_LEVEL)|(PARENT_UNIQUE_NAME)|(SKIPPED_LEVELS)|(UNARY_OPERATOR)|(UNIQUE_NAME)"),
                new TokenDefinition(TokenType.AxisName,
                    "(COLUMNS)|(ROWS)|(PAGES)|(CHAPTERS)|(SECTIONS)|(AXIS\\([0-9]+\\))"),
                new TokenDefinition(TokenType.IdentifierExpression,
                    "[a-zA-Z0-9 \\/\\\\\\-\\:]*[a-zA-Z0-9\\/\\\\\\-\\:]"),
            };
        }

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