using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using BalticAmadeus.FluentMdx.Lexer;
using NUnit.Framework;

namespace BalticAmadeus.FluentMdx.Tests
{
    [TestFixture, ExcludeFromCodeCoverage]
    public class MdxParserTests
    {
        private MdxParser _parserSut;

        [TestFixtureSetUp]
        public void TestFixtureSetUp()
        {
            _parserSut = new MdxParser();
        }

        [Test]
        public void ParseQuery_WithSingleAxisParameter_ReturnsParsedQuery()
        {
            //ARRANGE   
            const string queryString = "SELECT " +
                                       "NON EMPTY { [Measures].[Measure] } ON COLUMNS " +
                                       "FROM [Cube]";

            //ACT
            var query = _parserSut.ParseQuery(queryString);

            //ASSERT
            Assert.That(query, Is.Not.Null);
            Assert.That(query.GetStringExpression(), Is.EqualTo(queryString));
        }

        [Test]
        public void ParseQuery_WithMultipleAxisParameters_ReturnsParsedQuery()
        {
            //ARRANGE   
            const string queryString = "SELECT " +
                                       "NON EMPTY { [Measures].[Measure1], [Measures].[Measure2], [Measures].[Measure3] } ON COLUMNS " +
                                       "FROM [Cube]";

            //ACT
            var query = _parserSut.ParseQuery(queryString);

            //ASSERT
            Assert.That(query, Is.Not.Null);
            Assert.That(query.GetStringExpression(), Is.EqualTo(queryString));
        }

        [Test]
        public void ParserQuery_WithMultipleAxes_ReturnsParsedQuery()
        {
            //ARRANGE   
            const string queryString = "SELECT " +
                                       "NON EMPTY { [Measures].[Measure] } ON COLUMNS, " +
                                       "NON EMPTY { [Dim Hierarchy].[Dim].ALLMEMBERS } ON ROWS " +
                                       "FROM [Cube]";

            //ACT
            var query = _parserSut.ParseQuery(queryString);

            //ASSERT
            Assert.That(query, Is.Not.Null);
            Assert.That(query.GetStringExpression(), Is.EqualTo(queryString));
        }

        [Test]
        public void ParseQuery_WithSingleWhereValueMember_ReturnsParsedQuery()
        {
            //ARRANGE   
            const string queryString = "SELECT " +
                                       "NON EMPTY { [Measures].[Measure] } ON COLUMNS " +
                                       "FROM [Cube] " +
                                       "WHERE [Dim Hierarchy].[Dim].[Dim Key].&[1]";

            const string expectedString = "SELECT " +
                                       "NON EMPTY { [Measures].[Measure] } ON COLUMNS " +
                                       "FROM [Cube] " +
                                       "WHERE { [Dim Hierarchy].[Dim].[Dim Key].&[1] }";

            //ACT
            var query = _parserSut.ParseQuery(queryString);

            //ASSERT
            Assert.That(query, Is.Not.Null);
            Assert.That(query.GetStringExpression(), Is.EqualTo(expectedString));
        }

        [Test]
        public void ParseQuery_WithSingleWhereRangeMember_ReturnsParsedQuery()
        {
            //ARRANGE   
            const string queryString = "SELECT " +
                                       "NON EMPTY { [Measures].[Measure] } ON COLUMNS " +
                                       "FROM [Cube] " +
                                       "WHERE [Dim Hierarchy].[Dim].[Dim Key].&[1]:[Dim Hierarchy].[Dim].[Dim Key].&[2]";
            
            const string expectedString = "SELECT " +
                                       "NON EMPTY { [Measures].[Measure] } ON COLUMNS " +
                                       "FROM [Cube] " +
                                       "WHERE { [Dim Hierarchy].[Dim].[Dim Key].&[1]:[Dim Hierarchy].[Dim].[Dim Key].&[2] }";

            //ACT
            var query = _parserSut.ParseQuery(queryString);

            //ASSERT
            Assert.That(query, Is.Not.Null);
            Assert.That(query.GetStringExpression(), Is.EqualTo(expectedString));
        }

        [Test]
        public void ParseQuery_WithSingleWhereTuple_ReturnsParsedQuery()
        {
            //ARRANGE   
            const string queryString = "SELECT " +
                                       "NON EMPTY { [Measures].[Measure] } ON COLUMNS " +
                                       "FROM [Cube] " +
                                       "WHERE { [Dim Hierarchy].[Dim].[Dim Key].&[1], [Dim Hierarchy].[Dim].[Dim Key].&[3] }";

            //ACT
            var query = _parserSut.ParseQuery(queryString);

            //ASSERT        
            Assert.That(query, Is.Not.Null);
            Assert.That(query.GetStringExpression(), Is.EqualTo(queryString));
        }

        [Test]
        public void ParseQuery_WithSingleWhereSet_ReturnsParsedQuery()
        {
            //ARRANGE   
            const string queryString = "SELECT " +
                                       "NON EMPTY { [Measures].[Measure] } ON COLUMNS " +
                                       "FROM [Cube] " +
                                       "WHERE ( [Dim1 Hierarchy].[Dim1].[Dim1 Key].&[1], [Dim2 Hierarchy].[Dim2].[Dim2 Key].&[2] )";

            const string expectedString = "SELECT " +
                                       "NON EMPTY { [Measures].[Measure] } ON COLUMNS " +
                                       "FROM [Cube] " +
                                       "WHERE { ( [Dim1 Hierarchy].[Dim1].[Dim1 Key].&[1], [Dim2 Hierarchy].[Dim2].[Dim2 Key].&[2] ) }";

            //ACT
            var query = _parserSut.ParseQuery(queryString);

            //ASSERT
            Assert.That(query, Is.Not.Null);
            Assert.That(query.GetStringExpression(), Is.EqualTo(expectedString));
        }

        [Test]
        public void ParseQuery_WithMultipleSetsInTuple_ReturnsParsedQuery()
        {
            //ARRANGE   
            const string queryString = "SELECT " +
                                       "NON EMPTY { [Measures].[Measure] } ON COLUMNS " +
                                       "FROM [Cube] " +
                                       "WHERE ( [Dim1 Hierarchy].[Dim1].[Dim1 Key].&[1], [Dim2 Hierarchy].[Dim2].[Dim2 Key].&[2] )";

            const string expectedString = "SELECT " +
                                          "NON EMPTY { [Measures].[Measure] } ON COLUMNS " +
                                          "FROM [Cube] " +
                                          "WHERE { ( [Dim1 Hierarchy].[Dim1].[Dim1 Key].&[1], [Dim2 Hierarchy].[Dim2].[Dim2 Key].&[2] ) }";

            //ACT
            var query = _parserSut.ParseQuery(queryString);

            //ASSERT
            Assert.That(query, Is.Not.Null);
            Assert.That(query.GetStringExpression(), Is.EqualTo(expectedString));
        }

        [Test]
        public void ParseQuery_WithMultipleTuplesInSet_ReturnsParsedQuery()
        {
            //ARRANGE   
            const string queryString = "SELECT " +
                                       "NON EMPTY { [Measures].[Measure] } ON COLUMNS " +
                                       "FROM [Cube] " +
                                       "WHERE ( " +
                                       "{ [Dim1 Hierarchy].[Dim1].[Dim1 Key].&[1], [Dim1 Hierarchy].[Dim1].[Dim1 Key].&[2] }, " +
                                       "{ [Dim2 Hierarchy].[Dim2].[Dim2 Key].&[1], [Dim2 Hierarchy].[Dim2].[Dim2 Key].&[2] } " +
                                       ")";

            const string expectedString = "SELECT " +
                                       "NON EMPTY { [Measures].[Measure] } ON COLUMNS " +
                                       "FROM [Cube] " +
                                       "WHERE { ( " +
                                       "{ [Dim1 Hierarchy].[Dim1].[Dim1 Key].&[1], [Dim1 Hierarchy].[Dim1].[Dim1 Key].&[2] }, " +
                                       "{ [Dim2 Hierarchy].[Dim2].[Dim2 Key].&[1], [Dim2 Hierarchy].[Dim2].[Dim2 Key].&[2] } " +
                                       ") }";

            //ACT
            var query = _parserSut.ParseQuery(queryString);

            //ASSERT
            Assert.That(query, Is.Not.Null);
            Assert.That(query.GetStringExpression(), Is.EqualTo(expectedString));
        }
    }
}
