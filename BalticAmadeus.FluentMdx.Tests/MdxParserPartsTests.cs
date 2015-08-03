using System.Diagnostics.CodeAnalysis;
using BalticAmadeus.FluentMdx.EnumerableExtensions;
using NUnit.Framework;

namespace BalticAmadeus.FluentMdx.Tests
{
    [TestFixture, ExcludeFromCodeCoverage]
    public class MdxParserPartsTests
    {
        private Lexer.Lexer _lexer;

        [TestFixtureSetUp]
        public void TestFixtureSetUp()
        {
            _lexer = new Lexer.Lexer();
        }

        [Test]
        public void ParseAxisParameter_WithSubsequentIdentifiers_SucceedsReturnsAxisParameter()
        {
            //ARRANGE   
            const string queryString = "[Aaa].[Bbb].[Ccc].FUNCTION(1, 2).FUNCTION";

            const string expectedString = "[Aaa].[Bbb].[Ccc].FUNCTION(1, 2).FUNCTION";

            //ACT
            MdxExpressionBase expression;
            bool isSucceeded = MdxParser.TryParseMember(_lexer.Tokenize(queryString).GetTwoWayEnumerator(), out expression);

            //ASSERT
            Assert.That(isSucceeded, Is.True);
            Assert.That(expression, Is.InstanceOf<MdxMember>());
            Assert.That(expression.ToString(), Is.EqualTo(expectedString));
        }

        [Test]
        public void ParseNavigationFunction_WithSubsequentFunctions_SuccedsAndReturnsFunction()
        {
            //ARRANGE   
            const string queryString = "FUNCTION(1, 2)";

            const string expectedString = "FUNCTION(1, 2)";

            //ACT
            MdxExpressionBase expression;
            bool isSucceeded = MdxParser.TryParseNavigationFunction(_lexer.Tokenize(queryString).GetTwoWayEnumerator(), out expression);

            //ASSERT
            Assert.That(isSucceeded, Is.True);
            Assert.That(expression, Is.InstanceOf<MdxNavigationFunction>());
            Assert.That(expression.ToString(), Is.EqualTo(expectedString));
        }

        [Test]
        public void ParseAxis_WithParameters_SecceedsAndReturnsAxis()
        {
            //ARRANGE   
            const string queryString = "NON EMPTY { [Aaa].[Bbb].[Ccc].FUNCTION(1, 2).FUNCTION, [Aaa] } ON COLUMNS";

            const string expectedString = "NON EMPTY { [Aaa].[Bbb].[Ccc].FUNCTION(1, 2).FUNCTION, [Aaa] } ON COLUMNS";

            //ACT
            MdxExpressionBase expression;
            bool isSucceeded = MdxParser.TryParseAxis(_lexer.Tokenize(queryString).GetTwoWayEnumerator(), out expression);

            //ASSERT
            Assert.That(isSucceeded, Is.True);
            Assert.That(expression, Is.InstanceOf<MdxAxis>());
            Assert.That(expression.ToString(), Is.EqualTo(expectedString));
        }

        [Test]
        public void ParseAxis_WithParameterAndDimensionProperties_SecceedsAndReturnsAxis()
        {
            //ARRANGE   
            const string queryString = "NON EMPTY { [Aaa] } DIMENSION PROPERTIES CATALOG_NAME, CUSTOM_ROLLUP ON COLUMNS";

            const string expectedString = "NON EMPTY { [Aaa] } DIMENSION PROPERTIES CATALOG_NAME, CUSTOM_ROLLUP ON COLUMNS";

            //ACT
            MdxExpressionBase expression;
            bool isSucceeded = MdxParser.TryParseAxis(_lexer.Tokenize(queryString).GetTwoWayEnumerator(), out expression);

            //ASSERT
            Assert.That(isSucceeded, Is.True);
            Assert.That(expression, Is.InstanceOf<MdxAxis>());
            Assert.That(expression.ToString(), Is.EqualTo(expectedString));
        }

        [Test]
        public void ParseCube_WithParameters_SecceedsAndReturnsCube()
        {
            //ARRANGE   
            const string queryString = "[Aaa].[Bbb].[Ccc]";

            const string expectedString = "[Aaa].[Bbb].[Ccc]";

            //ACT
            MdxExpressionBase expression;
            bool isSucceeded = MdxParser.TryParseCube(_lexer.Tokenize(queryString).GetTwoWayEnumerator(), out expression);

            //ASSERT
            Assert.That(isSucceeded, Is.True);
            Assert.That(expression, Is.InstanceOf<MdxCube>());
            Assert.That(expression.ToString(), Is.EqualTo(expectedString));
        }

        [Test]
        public void ParseMember_WithSubsequentIdentifiersAndValue_SuceeedsAndReturnsValueMember()
        {
            //ARRANGE   
            const string queryString = "[Aaa].[Bbb].[Ccc].&[1]";

            const string expectedString = "[Aaa].[Bbb].[Ccc].&[1]";

            //ACT
            MdxExpressionBase expression;
            bool isSucceeded = MdxParser.TryParseMember(_lexer.Tokenize(queryString).GetTwoWayEnumerator(), out expression);

            //ASSERT
            Assert.That(isSucceeded, Is.True);
            Assert.That(expression, Is.InstanceOf<MdxMember>());
            Assert.That(expression.ToString(), Is.EqualTo(expectedString));
        }

        [Test]
        public void ParseMember_WithFunctionAfterValueMember_SuceeedsAndReturnsValueMemberWithFunction()
        {
            //ARRANGE   
            const string queryString = "[Dim1 Hierarchy].[Dim1].[Dim1 Key].&[1].AllMembers";

            const string expectedString = "[Dim1 Hierarchy].[Dim1].[Dim1 Key].&[1].AllMembers";

            //ACT
            MdxExpressionBase expression;
            bool isSucceeded = MdxParser.TryParseMember(_lexer.Tokenize(queryString).GetTwoWayEnumerator(), out expression);

            //ASSERT
            Assert.That(isSucceeded, Is.True);
            Assert.That(expression, Is.InstanceOf<MdxMember>());
            Assert.That(expression.ToString(), Is.EqualTo(expectedString));
        }

        [Test]
        public void ParseFunction_WithNoParameters_SuceeedsAndReturnsFunction()
        {
            //ARRANGE
            const string queryString = "MYFUNCTION()";

            const string expectedString = "MYFUNCTION()";

            //ACT
            MdxExpressionBase expression;
            bool isSucceeded = MdxParser.TryParseFunction(_lexer.Tokenize(queryString).GetTwoWayEnumerator(), out expression);

            //ASSERT
            Assert.That(isSucceeded, Is.True);
            Assert.That(expression, Is.InstanceOf<MdxFunction>());
            Assert.That(expression.ToString(), Is.EqualTo(expectedString));
        }

        [Test]
        public void ParseFunction_WithSingleFunctionParameter_SuceeedsAndReturnsFunction()
        {
            //ARRANGE
            const string queryString = "MYFUNCTION(MYOTHERFUNCTION())";

            const string expectedString = "MYFUNCTION(MYOTHERFUNCTION())";

            //ACT
            MdxExpressionBase expression;
            bool isSucceeded = MdxParser.TryParseFunction(_lexer.Tokenize(queryString).GetTwoWayEnumerator(), out expression);

            //ASSERT
            Assert.That(isSucceeded, Is.True);
            Assert.That(expression, Is.InstanceOf<MdxFunction>());
            Assert.That(expression.ToString(), Is.EqualTo(expectedString));
        }

        [Test]
        public void ParseFunction_WithSingleSetParameter_SuceeedsAndReturnsFunction()
        {
            //ARRANGE
            const string queryString = "MYFUNCTION(())";

            const string expectedString = "MYFUNCTION((  ))";

            //ACT
            MdxExpressionBase expression;
            bool isSucceeded = MdxParser.TryParseFunction(_lexer.Tokenize(queryString).GetTwoWayEnumerator(), out expression);

            //ASSERT
            Assert.That(isSucceeded, Is.True);
            Assert.That(expression, Is.InstanceOf<MdxFunction>());
            Assert.That(expression.ToString(), Is.EqualTo(expectedString));
        }

        [Test]
        public void ParseFunction_WithSingleTupleParameter_SuceeedsAndReturnsFunction()
        {
            //ARRANGE
            const string queryString = "MYFUNCTION({ })";

            const string expectedString = "MYFUNCTION({  })";

            //ACT
            MdxExpressionBase expression;
            bool isSucceeded = MdxParser.TryParseFunction(_lexer.Tokenize(queryString).GetTwoWayEnumerator(), out expression);

            //ASSERT
            Assert.That(isSucceeded, Is.True);
            Assert.That(expression, Is.InstanceOf<MdxFunction>());
            Assert.That(expression.ToString(), Is.EqualTo(expectedString));
        }

        [Test]
        public void ParseFunction_WithSingleMemberParameter_SuceeedsAndReturnsFunction()
        {
            //ARRANGE
            const string queryString = "MYFUNCTION([Id])";

            const string expectedString = "MYFUNCTION([Id])";

            //ACT
            MdxExpressionBase expression;
            bool isSucceeded = MdxParser.TryParseFunction(_lexer.Tokenize(queryString).GetTwoWayEnumerator(), out expression);

            //ASSERT
            Assert.That(isSucceeded, Is.True);
            Assert.That(expression, Is.InstanceOf<MdxFunction>());
            Assert.That(expression.ToString(), Is.EqualTo(expectedString));
        }

        [Test]
        public void ParseFunction_WithSingleExpressionParameter_SuceeedsAndReturnsFunction()
        {
            //ARRANGE
            const string queryString = "MYFUNCTION([Id] > 1)";

            const string expectedString = "MYFUNCTION([Id] > 1)";

            //ACT
            MdxExpressionBase expression;
            bool isSucceeded = MdxParser.TryParseFunction(_lexer.Tokenize(queryString).GetTwoWayEnumerator(), out expression);

            //ASSERT
            Assert.That(isSucceeded, Is.True);
            Assert.That(expression, Is.InstanceOf<MdxFunction>());
            Assert.That(expression.ToString(), Is.EqualTo(expectedString));
        }

        [Test]
        public void ParseFunction_WithSingleNumberAndTextExpressionParameter_SuceeedsAndReturnsFunction()
        {
            //ARRANGE
            const string queryString = "MYFUNCTION.Func(1 + 'asd')";

            const string expectedString = "MYFUNCTION.Func(1 + 'asd')";

            //ACT
            MdxExpressionBase expression;
            bool isSucceeded = MdxParser.TryParseFunction(_lexer.Tokenize(queryString).GetTwoWayEnumerator(), out expression);

            //ASSERT
            Assert.That(isSucceeded, Is.True);
            Assert.That(expression, Is.InstanceOf<MdxFunction>());
            Assert.That(expression.ToString(), Is.EqualTo(expectedString));
        }
    }
}
