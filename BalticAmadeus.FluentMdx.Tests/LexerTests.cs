using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using BalticAmadeus.FluentMdx.Lexer;
using NUnit.Framework;

namespace BalticAmadeus.FluentMdx.Tests
{
    [TestFixture, ExcludeFromCodeCoverage]
    public class LexerTests
    {
        private Lexer.Lexer _lexer;

        [TestFixtureSetUp]
        public void TestFixtureSetUp()
        {
            _lexer = new Lexer.Lexer();
        }

        [Test]
        public void Tokenize_WithUndefinedSymbol_ThrowsExceptionWithMessage()
        {
            //ARRANGE
            string source = "[$]";

            //ACT
            //ASSERT
            var ex = Assert.Throws<Exception>(() => _lexer.Tokenize(source).ToList());
            Assert.That(ex.Message, Is.EqualTo("Unrecognized symbol '$'."));
        }

        [Test]
        public void Tokenize_WithMemberString_TokensAreCorrect()
        {
            //ARRANGE
            string source = "[Aaa].[Bbb].[Ccc].&[1]";

            var expectedTokens = new List<Token>
            {
                new Token(TokenType.LeftSquareBracket, "["),
                new Token(TokenType.IdentifierExpression, "Aaa"),
                new Token(TokenType.RightSquareBracket, "]"),
                new Token(TokenType.IdentifierSeparator, "."),
                new Token(TokenType.LeftSquareBracket, "["),
                new Token(TokenType.IdentifierExpression, "Bbb"),
                new Token(TokenType.RightSquareBracket, "]"),
                new Token(TokenType.IdentifierSeparator, "."),
                new Token(TokenType.LeftSquareBracket, "["),
                new Token(TokenType.IdentifierExpression, "Ccc"),
                new Token(TokenType.RightSquareBracket, "]"),
                new Token(TokenType.ValueSeparator, ".&"),
                new Token(TokenType.LeftSquareBracket, "["),
                new Token(TokenType.IdentifierExpression, "1"),
                new Token(TokenType.RightSquareBracket, "]"),
                new Token(TokenType.LastToken, ""),
            };

            //ACT
            var tokens = _lexer.Tokenize(source).ToList();
            
            //ASSERT
            Assert.That(tokens.Count, Is.EqualTo(expectedTokens.Count));
            for (int i = 0; i < expectedTokens.Count; i++)
                Assert.That(tokens[i].ToString(), Is.EqualTo(expectedTokens[i].ToString()));
        }

        [Test]
        public void Tokenize_WithDimensionPropertiesString_TokensAreCorrect()
        {
            //ARRANGE
            string source =
                "CATALOG_NAME CHILDREN_CARDINALITY CUSTOM_ROLLUP CUSTOM_ROLLUP_PROPERTIES DESCRIPTION " +
                "DIMENSION_UNIQUE_NAME HIERARCHY_UNIQUE_NAME IS_DATAMEMBER IS_PLACEHOLDERMEMBER KEY0 LCID " +
                "LEVEL_NUMBER LEVEL_UNIQUE_NAME MEMBER_CAPTION MEMBER_KEY MEMBER_NAME MEMBER_TYPE " +
                "MEMBER_UNIQUE_NAME MEMBER_VALUE PARENT_COUNT PARENT_LEVEL PARENT_UNIQUE_NAME SKIPPED_LEVELS " +
                "UNARY_OPERATOR UNIQUE_NAME";

            var expectedTokens = new List<Token>
            {
                new Token(TokenType.DimensionProperty, "CATALOG_NAME"),
                new Token(TokenType.DimensionProperty, "CHILDREN_CARDINALITY"),
                new Token(TokenType.DimensionProperty, "CUSTOM_ROLLUP"),
                new Token(TokenType.DimensionProperty, "CUSTOM_ROLLUP_PROPERTIES"),
                new Token(TokenType.DimensionProperty, "DESCRIPTION"),
                new Token(TokenType.DimensionProperty, "DIMENSION_UNIQUE_NAME"),
                new Token(TokenType.DimensionProperty, "HIERARCHY_UNIQUE_NAME"),
                new Token(TokenType.DimensionProperty, "IS_DATAMEMBER"),
                new Token(TokenType.DimensionProperty, "IS_PLACEHOLDERMEMBER"),
                new Token(TokenType.DimensionProperty, "KEY0"),
                new Token(TokenType.DimensionProperty, "LCID"),
                new Token(TokenType.DimensionProperty, "LEVEL_NUMBER"),
                new Token(TokenType.DimensionProperty, "LEVEL_UNIQUE_NAME"),
                new Token(TokenType.DimensionProperty, "MEMBER_CAPTION"),
                new Token(TokenType.DimensionProperty, "MEMBER_KEY"),
                new Token(TokenType.DimensionProperty, "MEMBER_NAME"),
                new Token(TokenType.DimensionProperty, "MEMBER_TYPE"),
                new Token(TokenType.DimensionProperty, "MEMBER_UNIQUE_NAME"),
                new Token(TokenType.DimensionProperty, "MEMBER_VALUE"),
                new Token(TokenType.DimensionProperty, "PARENT_COUNT"),
                new Token(TokenType.DimensionProperty, "PARENT_LEVEL"),
                new Token(TokenType.DimensionProperty, "PARENT_UNIQUE_NAME"),
                new Token(TokenType.DimensionProperty, "SKIPPED_LEVELS"),
                new Token(TokenType.DimensionProperty, "UNARY_OPERATOR"),
                new Token(TokenType.DimensionProperty, "UNIQUE_NAME"),
                new Token(TokenType.LastToken, "")
            };

            //ACT
            var tokens = _lexer.Tokenize(source).ToList();

            //ASSERT
            Assert.That(tokens.Count, Is.EqualTo(expectedTokens.Count));
            for (int i = 0; i < expectedTokens.Count; i++)
                Assert.That(tokens[i].ToString(), Is.EqualTo(expectedTokens[i].ToString()));
        }

    }
}
