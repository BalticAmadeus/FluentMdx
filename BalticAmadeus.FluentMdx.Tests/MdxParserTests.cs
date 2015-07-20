using NUnit.Framework;

namespace BalticAmadeus.FluentMdx.Tests
{
    [TestFixture]
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
                                       "NON EMPTY { [Dimension Hierarchy].[Dimension].ALLMEMBERS } ON ROWS " +
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
                                       "WHERE [Dimension Hierarchy].[Dimension].[Dimension Key].&[1]";

            //ACT
            var query = _parserSut.ParseQuery(queryString);

            //ASSERT
            Assert.That(query, Is.Not.Null);
            Assert.That(query.GetStringExpression(), Is.EqualTo(queryString));
        }

        [Test]
        public void ParseQuery_WithSingleWhereRangeMember_ReturnsParsedQuery()
        {
            //ARRANGE   
            const string queryString = "SELECT " +
                                       "NON EMPTY { [Measures].[Measure] } ON COLUMNS " +
                                       "FROM [Cube] " +
                                       "WHERE [Dimension Hierarchy].[Dimension].[Dimension Key].&[1]:[Dimension Hierarchy].[Dimension].[Dimension Key].&[2]";

            //ACT
            var query = _parserSut.ParseQuery(queryString);

            //ASSERT
            Assert.That(query, Is.Not.Null);
            Assert.That(query.GetStringExpression(), Is.EqualTo(queryString));
        }

        [Test]
        public void ParseQuery_WithSingleWhereTuple_ReturnsParsedQuery()
        {
            //ARRANGE   
            const string queryString = "SELECT " +
                                       "NON EMPTY { [Measures].[Measure] } ON COLUMNS " +
                                       "FROM [Cube] " +
                                       "WHERE { [Dimension Hierarchy].[Dimension].[Dimension Key].&[1], [Dimension Hierarchy].[Dimension].[Dimension Key].&[3] }";

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
                                       "WHERE ( [Dimension1 Hierarchy].[Dimension1].[Dimension1 Key].&[1], [Dimension2 Hierarchy].[Dimension2].[Dimension2 Key].&[2] )";

            //ACT
            var query = _parserSut.ParseQuery(queryString);

            //ASSERT
            Assert.That(query, Is.Not.Null);
            Assert.That(query.GetStringExpression(), Is.EqualTo(queryString));
        }

        [Test]
        public void ParseQuery_WithMultipleSetsInTuple_ReturnsParsedQuery()
        {
            //ARRANGE   
            const string queryString = "SELECT " +
                                       "NON EMPTY { [Measures].[Measure] } ON COLUMNS " +
                                       "FROM [Cube] " +
                                       "WHERE { ( [Dimension1 Hierarchy].[Dimension1].[Dimension1 Key].&[1], [Dimension2 Hierarchy].[Dimension2].[Dimension2 Key].&[2] ) }";

            const string expectedString = "SELECT " +
                                          "NON EMPTY { [Measures].[Measure] } ON COLUMNS " +
                                          "FROM [Cube] " +
                                          "WHERE ( [Dimension1 Hierarchy].[Dimension1].[Dimension1 Key].&[1], [Dimension2 Hierarchy].[Dimension2].[Dimension2 Key].&[2] )";

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
                                       "{ [Dimension1 Hierarchy].[Dimension1].[Dimension1 Key].&[1], [Dimension1 Hierarchy].[Dimension1].[Dimension1 Key].&[2] }, " +
                                       "{ [Dimension2 Hierarchy].[Dimension2].[Dimension2 Key].&[1], [Dimension2 Hierarchy].[Dimension2].[Dimension2 Key].&[2] } " +
                                       ")";

            //ACT
            var query = _parserSut.ParseQuery(queryString);

            //ASSERT
            Assert.That(query, Is.Not.Null);
            Assert.That(query.GetStringExpression(), Is.EqualTo(queryString));
        }
    }
}
