using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BalticAmadeus.FluentMdx.Lexer;
using NUnit.Framework;

namespace BalticAmadeus.FluentMdx.Tests
{
    [TestFixture]
    public class LexerTests
    {
        private Lexer.Lexer _lexer;

        [TestFixtureSetUp]
        public void TestFixtureSetUp()
        {
            _lexer = new Lexer.Lexer();
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
                new Token(TokenType.NumberExpression, "1"),
                new Token(TokenType.RightSquareBracket, "]"),
                new Token(TokenType.LastToken, ""),
            };

            //ACT
            var tokens = _lexer.Tokenize(source).ToList();
            
            //ASSERT
            Assert.That(tokens.Count, Is.EqualTo(expectedTokens.Count));
            for (int i = 0; i < expectedTokens.Count; i++)
            {
                Assert.That(tokens[i].Value, Is.EqualTo(expectedTokens[i].Value));
                Assert.That(tokens[i].Type, Is.EqualTo(expectedTokens[i].Type));
            }
        }

    }
}
