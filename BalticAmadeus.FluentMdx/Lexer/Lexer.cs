using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;

namespace BalticAmadeus.FluentMdx.Lexer
{
    /// <summary>
    /// Represents a machine that performs lexical analysis based on regex-defined rules.
    /// </summary>
    internal class Lexer : ILexer
    {
        private static readonly IEnumerable<TokenDefinition> TokenDefinitions = new List<TokenDefinition>
        {
            new TokenDefinition(TokenType.EndOfLine, "[\\n|\\r]", true),

            new TokenDefinition(TokenType.Properties, "PROPERTIES"),
            new TokenDefinition(TokenType.DimensionProperty, "(CATALOG_NAME)|(CHILDREN_CARDINALITY)|(CUSTOM_ROLLUP_PROPERTIES)|(DESCRIPTION)|(DIMENSION_UNIQUE_NAME)|(HIERARCHY_UNIQUE_NAME)|(IS_DATAMEMBER)|(IS_PLACEHOLDERMEMBER)|(KEY0)|(LCID)|(LEVEL_NUMBER)|(LEVEL_UNIQUE_NAME)|(MEMBER_CAPTION)|(MEMBER_KEY)|(MEMBER_NAME)|(MEMBER_TYPE)|(MEMBER_UNIQUE_NAME)|(MEMBER_VALUE)|(PARENT_COUNT)|(PARENT_LEVEL)|(PARENT_UNIQUE_NAME)|(SKIPPED_LEVELS)|(UNARY_OPERATOR)|(UNIQUE_NAME)|(CUSTOM_ROLLUP)"),
            new TokenDefinition(TokenType.Dimension, "DIMENSION"),
            new TokenDefinition(TokenType.AxisNameIdentifier, "(COLUMNS)|(ROWS)|(PAGES)|(CHAPTERS)|(SECTIONS)|(AXIS\\([0-9]+\\))"),
            new TokenDefinition(TokenType.Select, "SELECT"),
            new TokenDefinition(TokenType.Non, "NON"),
            new TokenDefinition(TokenType.Empty, "EMPTY"),
            new TokenDefinition(TokenType.On, "ON"),
            new TokenDefinition(TokenType.From, "FROM"),
            new TokenDefinition(TokenType.Where, "WHERE"),

            new TokenDefinition(TokenType.LeftCurlyBracket, "\\{"),
            new TokenDefinition(TokenType.RightCurlyBracket, "\\}"),
            new TokenDefinition(TokenType.LeftRoundBracket, "\\("),
            new TokenDefinition(TokenType.RightRoundBracket, "\\)"),
            new TokenDefinition(TokenType.LeftSquareBracket, "\\["),
            new TokenDefinition(TokenType.RightSquareBracket, "\\]"),

            new TokenDefinition(TokenType.MemberSeparator, "\\,"),
            new TokenDefinition(TokenType.RangeSeparator, "\\:"),
            new TokenDefinition(TokenType.ValueSeparator, "\\.\\&"),
            new TokenDefinition(TokenType.IdentifierSeparator, "\\."),

            new TokenDefinition(TokenType.DateExpression, "[0-9]{4}\\-[0-9]{2}\\-[0-9]{2}T[0-9]{2}\\:[0-9]{2}\\:[0-9]{2}"),
            new TokenDefinition(TokenType.NumberExpression, "[0-9]+(\\.[0-9]+)?"),
            new TokenDefinition(TokenType.LogicalExpression, "TRUE|FALSE"),

            new TokenDefinition(TokenType.NotOperator, "NOT"),
            new TokenDefinition(TokenType.LogicsOperator, "AND"),
            new TokenDefinition(TokenType.MathsOperator, "\\+|\\-|\\*|\\/|<=|>=|<>|<|=|>"),

            new TokenDefinition(TokenType.IdentifierExpression, "[a-zA-Z0-9 \\/\\\\\\-\\:\\'\\\"]*[a-zA-Z0-9\\/\\\\\\-\\:\\'\\\"]"),
        };

        private readonly IEnumerable<TokenDefinition> _tokenDefinitions;

        /// <summary>
        /// Initializes a new instance of <see cref="Lexer"/> and sets the lexical analysis rules.
        /// </summary>
        public Lexer()
        {
            _tokenDefinitions = TokenDefinitions;
        }

        /// <summary>
        /// Transforms text into list of <see cref="Token"/> objects based on rules.
        /// </summary>
        /// <param name="source">Text to tokenize.</param>
        /// <returns>Returns list of tokenized items.</returns>
        public IEnumerable<Token> Tokenize(string source)
        {
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

                if (!matchedDefinition.IsIgnored)
                    yield return new Token(matchedDefinition.Type, value);

                currentIndex += matchLength;
            }

            yield return new Token(TokenType.LastToken, string.Empty);
        }

        private class TokenDefinition
        {
            public Regex Pattern { get; private set; }
            public TokenType Type { get; private set; }
            public bool IsIgnored { get; private set; }

            public TokenDefinition(TokenType type, string pattern, bool isIgnored = false)
            {
                Type = type;
                Pattern = new Regex(pattern, RegexOptions.IgnoreCase);
                IsIgnored = isIgnored;
            }
        }
    }
}