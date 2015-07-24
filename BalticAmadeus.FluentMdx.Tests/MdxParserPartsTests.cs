using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using BalticAmadeus.FluentMdx.EnumerableExtensions;
using BalticAmadeus.FluentMdx.Lexer;
using NUnit.Framework;

namespace BalticAmadeus.FluentMdx.Tests
{
    [TestFixture, ExcludeFromCodeCoverage]
    public class MdxParserPartsTests
    {
        [Test]
        public void ParseIdentifier_WithSubsequentIdentifiers_SucceedsReturnsIdentifier()
        {
            //ARRANGE
            var list = new List<Token>
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
                new Token(TokenType.RightSquareBracket, "]")
            };

            //ACT
            IMdxExpression expression;
            bool isSucceeded = MdxParser.TryParseIdentifier(list.GetTwoWayEnumerator(), out expression);

            //ASSERT
            Assert.That(isSucceeded, Is.True);
            Assert.That(expression, Is.InstanceOf<MdxIdentifier>());
            Assert.That(expression.GetStringExpression(), Is.EqualTo("[Aaa].[Bbb].[Ccc]"));
        }

        [Test]
        public void ParseIdentifier_WithSubsequentIdentifiersAndFunctions_SucceedsReturnsIdentifier()
        {
            //ARRANGE
            var list = new List<Token>
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
                new Token(TokenType.IdentifierSeparator, "."),
                new Token(TokenType.IdentifierExpression, "FUNCTION"),
                new Token(TokenType.LeftRoundBracket, "("),
                new Token(TokenType.IdentifierExpression, "1"),
                new Token(TokenType.Comma, ","),
                new Token(TokenType.IdentifierExpression, "2"),
                new Token(TokenType.RightRoundBracket, ")"),
                new Token(TokenType.IdentifierSeparator, "."),
                new Token(TokenType.IdentifierExpression, "FUNCTION"),
            };

            //ACT
            IMdxExpression expression;
            bool isSucceeded = MdxParser.TryParseIdentifier(list.GetTwoWayEnumerator(), out expression);

            //ASSERT
            Assert.That(isSucceeded, Is.True);
            Assert.That(expression, Is.InstanceOf<MdxIdentifier>());
            Assert.That(expression.GetStringExpression(), Is.EqualTo("[Aaa].[Bbb].[Ccc].FUNCTION(1, 2).FUNCTION"));
        }

        [Test]
        public void ParseAxisParameter_WithSubsequentIdentifiers_SucceedsReturnsAxisParameter()
        {
            //ARRANGE
            var list = new List<Token>
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
                new Token(TokenType.IdentifierSeparator, "."),
                new Token(TokenType.IdentifierExpression, "FUNCTION"),
                new Token(TokenType.LeftRoundBracket, "("),
                new Token(TokenType.IdentifierExpression, "1"),
                new Token(TokenType.Comma, ","),
                new Token(TokenType.IdentifierExpression, "2"),
                new Token(TokenType.RightRoundBracket, ")"),
                new Token(TokenType.IdentifierSeparator, "."),
                new Token(TokenType.IdentifierExpression, "FUNCTION"),
            };

            //ACT
            IMdxExpression expression;
            bool isSucceeded = MdxParser.TryParseAxisParameter(list.GetTwoWayEnumerator(), out expression);

            //ASSERT
            Assert.That(isSucceeded, Is.True);
            Assert.That(expression, Is.InstanceOf<MdxAxisParameter>());
            Assert.That(expression.GetStringExpression(), Is.EqualTo("[Aaa].[Bbb].[Ccc].FUNCTION(1, 2).FUNCTION"));
        }

        [Test]
        public void ParseFunction_WithSubsequentFunctions_SuccedsAndReturnsFunction()
        {
            //ARRANGE
            var list = new List<Token>
            {
                new Token(TokenType.IdentifierExpression, "FUNCTION"),
                new Token(TokenType.LeftRoundBracket, "("),
                new Token(TokenType.IdentifierExpression, "1"),
                new Token(TokenType.Comma, ","),
                new Token(TokenType.IdentifierExpression, "2"),
                new Token(TokenType.RightRoundBracket, ")")
            };

            //ACT
            IMdxExpression expression;
            bool isSucceeded = MdxParser.TryParseFunction(list.GetTwoWayEnumerator(), out expression);

            //ASSERT
            Assert.That(isSucceeded, Is.True);
            Assert.That(expression, Is.InstanceOf<MdxFunction>());
            Assert.That(expression.GetStringExpression(), Is.EqualTo("FUNCTION(1, 2)"));
        }

        [Test]
        public void ParseAxis_WithParameters_SecceedsAndReturnsAxis()
        {
            //ARRANGE
            var list = new List<Token>
            {
                new Token(TokenType.Non, "NON"),
                new Token(TokenType.Empty, "EMPTY"),
                new Token(TokenType.LeftCurlyBracket, "{"),
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
                new Token(TokenType.IdentifierSeparator, "."),
                new Token(TokenType.IdentifierExpression, "FUNCTION"),
                new Token(TokenType.LeftRoundBracket, "("),
                new Token(TokenType.IdentifierExpression, "1"),
                new Token(TokenType.Comma, ","),
                new Token(TokenType.IdentifierExpression, "2"),
                new Token(TokenType.RightRoundBracket, ")"),
                new Token(TokenType.IdentifierSeparator, "."),
                new Token(TokenType.IdentifierExpression, "FUNCTION"),
                new Token(TokenType.Comma, ","),
                new Token(TokenType.LeftSquareBracket, "["),
                new Token(TokenType.IdentifierExpression, "Aaa"),
                new Token(TokenType.RightSquareBracket, "]"),
                new Token(TokenType.RightCurlyBracket, "}"),
                new Token(TokenType.On, "ON"),
                new Token(TokenType.AxisName, "COLUMNS"),
            };

            //ACT
            IMdxExpression expression;
            bool isSucceeded = MdxParser.TryParseAxis(list.GetTwoWayEnumerator(), out expression);

            //ASSERT
            Assert.That(isSucceeded, Is.True);
            Assert.That(expression, Is.InstanceOf<MdxAxis>());
            Assert.That(expression.GetStringExpression(), Is.EqualTo("NON EMPTY { [Aaa].[Bbb].[Ccc].FUNCTION(1, 2).FUNCTION, [Aaa] } ON COLUMNS"));
        }

        [Test]
        public void ParseCube_WithParameters_SecceedsAndReturnsCube()
        {
            //ARRANGE
            var list = new List<Token>
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
                new Token(TokenType.RightSquareBracket, "]")
            };

            //ACT
            IMdxExpression expression;
            bool isSucceeded = MdxParser.TryParseCube(list.GetTwoWayEnumerator(), out expression);

            //ASSERT
            Assert.That(isSucceeded, Is.True);
            Assert.That(expression, Is.InstanceOf<MdxCube>());
            Assert.That(expression.GetStringExpression(), Is.EqualTo("[Aaa].[Bbb].[Ccc]"));
        }

        [Test]
        public void ParseMember_WithSubsequentIdentifiersAndValue_SuceeedsAndReturnsValueMember()
        {
            //ARRANGE
            var list = new List<Token>
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
                new Token(TokenType.RightSquareBracket, "]")
            };

            //ACT
            IMdxExpression expression;
            bool isSucceeded = MdxParser.TryParseMember(list.GetTwoWayEnumerator(), out expression);

            //ASSERT
            Assert.That(isSucceeded, Is.True);
            Assert.That(expression, Is.InstanceOf<MdxMember>());
            Assert.That(expression.GetStringExpression(), Is.EqualTo("[Aaa].[Bbb].[Ccc].&[1]"));
        }
    }
}
