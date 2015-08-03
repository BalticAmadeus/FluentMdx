using System.Diagnostics.CodeAnalysis;
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
            Assert.That(query.ToString(), Is.EqualTo(queryString));
        }

        [Test]
        public void ParseQuery_WithSingleAxisParameterAndDimensionProperties_ReturnsParsedQuery()
        {
            //ARRANGE   
            const string queryString = "SELECT " +
                                       "NON EMPTY { [Measures].[Measure] } " +
                                       "DIMENSION PROPERTIES CATALOG_NAME, CHILDREN_CARDINALITY, CUSTOM_ROLLUP, CUSTOM_ROLLUP_PROPERTIES, DESCRIPTION, " +
                                       "DIMENSION_UNIQUE_NAME, HIERARCHY_UNIQUE_NAME, IS_DATAMEMBER, IS_PLACEHOLDERMEMBER, KEY0, LCID, LEVEL_NUMBER, LEVEL_UNIQUE_NAME, " +
                                       "MEMBER_CAPTION, MEMBER_KEY, MEMBER_NAME, MEMBER_TYPE, MEMBER_UNIQUE_NAME, MEMBER_VALUE, PARENT_COUNT, PARENT_LEVEL, " +
                                       "PARENT_UNIQUE_NAME, SKIPPED_LEVELS, UNARY_OPERATOR, UNIQUE_NAME ON COLUMNS " +
                                       "FROM [Cube]";

            //ACT
            var query = _parserSut.ParseQuery(queryString);

            //ASSERT
            Assert.That(query, Is.Not.Null);
            Assert.That(query.ToString(), Is.EqualTo(queryString));
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
            Assert.That(query.ToString(), Is.EqualTo(queryString));
        }

        [Test]
        public void ParseQuery_WithNoAxisParameters_ReturnsParsedQuery()
        {
            //ARRANGE   
            const string queryString = "SELECT " +
                                       "NON EMPTY {  } ON COLUMNS " +
                                       "FROM [Cube]";

            //ACT
            var query = _parserSut.ParseQuery(queryString);

            //ASSERT
            Assert.That(query, Is.Not.Null);
            Assert.That(query.ToString(), Is.EqualTo(queryString));
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
            Assert.That(query.ToString(), Is.EqualTo(queryString));
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
                                       "WHERE { ( { [Dim Hierarchy].[Dim].[Dim Key].&[1] } ) }";

            //ACT
            var query = _parserSut.ParseQuery(queryString);

            //ASSERT
            Assert.That(query, Is.Not.Null);
            Assert.That(query.ToString(), Is.EqualTo(expectedString));
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
                                       "WHERE { ( { [Dim Hierarchy].[Dim].[Dim Key].&[1]:[Dim Hierarchy].[Dim].[Dim Key].&[2] } ) }";

            //ACT
            var query = _parserSut.ParseQuery(queryString);

            //ASSERT
            Assert.That(query, Is.Not.Null);
            Assert.That(query.ToString(), Is.EqualTo(expectedString));
        }

        [Test]
        public void ParseQuery_WithSingleWhereRangeMemberDate_ReturnsParsedQuery()
        {
            //ARRANGE   
            const string queryString = "SELECT " +
                                       "NON EMPTY { [Measures].[Measure] } ON COLUMNS " +
                                       "FROM [Cube] " +
                                       "WHERE [Dim Hierarchy].[Date].[Date].&[2010-10-10T00:00:00]:[Dim Hierarchy].[Date].[Date].&[2011-10-10T00:00:00]";

            const string expectedString = "SELECT " +
                                       "NON EMPTY { [Measures].[Measure] } ON COLUMNS " +
                                       "FROM [Cube] " +
                                       "WHERE { ( { [Dim Hierarchy].[Date].[Date].&[2010-10-10T00:00:00]:[Dim Hierarchy].[Date].[Date].&[2011-10-10T00:00:00] } ) }";

            //ACT
            var query = _parserSut.ParseQuery(queryString);

            //ASSERT
            Assert.That(query, Is.Not.Null);
            Assert.That(query.ToString(), Is.EqualTo(expectedString));
        }

        [Test]
        public void ParseQuery_WithSingleWhereTuple_ReturnsParsedQuery()
        {
            //ARRANGE   
            const string queryString = "SELECT " +
                                       "NON EMPTY { [Measures].[Measure] } ON COLUMNS " +
                                       "FROM [Cube] " +
                                       "WHERE { [Dim Hierarchy].[Dim].[Dim Key].&[1], [Dim Hierarchy].[Dim].[Dim Key].&[3] }";

            const string expectedString = "SELECT " +
                                       "NON EMPTY { [Measures].[Measure] } ON COLUMNS " +
                                       "FROM [Cube] " +
                                       "WHERE { ( { [Dim Hierarchy].[Dim].[Dim Key].&[1], [Dim Hierarchy].[Dim].[Dim Key].&[3] } ) }";

            //ACT
            var query = _parserSut.ParseQuery(queryString);

            //ASSERT        
            Assert.That(query, Is.Not.Null);
            Assert.That(query.ToString(), Is.EqualTo(expectedString));
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
                                       "WHERE { ( { ( [Dim1 Hierarchy].[Dim1].[Dim1 Key].&[1], [Dim2 Hierarchy].[Dim2].[Dim2 Key].&[2] ) } ) }";

            //ACT
            var query = _parserSut.ParseQuery(queryString);

            //ASSERT
            Assert.That(query, Is.Not.Null);
            Assert.That(query.ToString(), Is.EqualTo(expectedString));
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
                                          "WHERE { ( { ( [Dim1 Hierarchy].[Dim1].[Dim1 Key].&[1], [Dim2 Hierarchy].[Dim2].[Dim2 Key].&[2] ) } ) }";

            //ACT
            var query = _parserSut.ParseQuery(queryString);

            //ASSERT
            Assert.That(query, Is.Not.Null);
            Assert.That(query.ToString(), Is.EqualTo(expectedString));
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
                                       "WHERE { ( { ( " +
                                       "{ [Dim1 Hierarchy].[Dim1].[Dim1 Key].&[1], [Dim1 Hierarchy].[Dim1].[Dim1 Key].&[2] }, " +
                                       "{ [Dim2 Hierarchy].[Dim2].[Dim2 Key].&[1], [Dim2 Hierarchy].[Dim2].[Dim2 Key].&[2] } " +
                                       ") } ) }";

            //ACT
            var query = _parserSut.ParseQuery(queryString);

            //ASSERT
            Assert.That(query, Is.Not.Null);
            Assert.That(query.ToString(), Is.EqualTo(expectedString));
        }

        [Test]
        public void ParseQuery_WithInnerQuery_ReturnsParsedQuery()
        {
            //ARRANGE   
            const string queryString = "SELECT " +
                                       "NON EMPTY { [Measures].[Measure] } ON COLUMNS " +
                                       "FROM ( " +
                                           "SELECT " +
                                           "NON EMPTY { [Dim Hierarchy].[Dim].&[1], [Dim Hierarchy].[Dim].&[2] } ON COLUMNS " +
                                           "FROM [Cube] " +
                                       ") WHERE ( " +
                                       "{ [Dim1 Hierarchy].[Dim1].[Dim1 Key].&[1], [Dim1 Hierarchy].[Dim1].[Dim1 Key].&[2] }, " +
                                       "{ [Dim2 Hierarchy].[Dim2].[Dim2 Key].&[1], [Dim2 Hierarchy].[Dim2].[Dim2 Key].&[2] } " +
                                       ")";

            const string expectedString = "SELECT " +
                                       "NON EMPTY { [Measures].[Measure] } ON COLUMNS " +
                                       "FROM ( " +
                                           "SELECT " +
                                           "NON EMPTY { [Dim Hierarchy].[Dim].&[1], [Dim Hierarchy].[Dim].&[2] } ON COLUMNS " +
                                           "FROM [Cube] " +
                                       ") WHERE { ( { ( " +
                                       "{ [Dim1 Hierarchy].[Dim1].[Dim1 Key].&[1], [Dim1 Hierarchy].[Dim1].[Dim1 Key].&[2] }, " +
                                       "{ [Dim2 Hierarchy].[Dim2].[Dim2 Key].&[1], [Dim2 Hierarchy].[Dim2].[Dim2 Key].&[2] } " +
                                       ") } ) }";

            //ACT
            var query = _parserSut.ParseQuery(queryString);

            //ASSERT
            Assert.That(query, Is.Not.Null);
            Assert.That(query.ToString(), Is.EqualTo(expectedString));
        }

        [Test]
        public void ParseQuery_WithFunctions_ReturnsParsedQuery()
        {
            //ARRANGE   
            const string queryString = "SELECT " +
                                       "NON EMPTY { [Dim Hierarchy1].[Dim1], [Dim Hierarchy1].[Dim2], [Dim Hierarchy1].[Dim3] } DIMENSION PROPERTIES CHILDREN_CARDINALITY, PARENT_UNIQUE_NAME ON COLUMNS, " +
                                       "NON EMPTY { [Dim Hierarchy2].[Dim1], ORDER([Dim Hierarchy2].[Dim2].Children, [Dim Hierarchy2].[Dim2].CurrentMember.MEMBER_CAPTION, asc) } DIMENSION PROPERTIES CHILDREN_CARDINALITY, PARENT_UNIQUE_NAME ON ROWS " +
                                       "FROM [Cube]";

            const string expectedString = "SELECT " +
                                          "NON EMPTY { [Dim Hierarchy1].[Dim1], [Dim Hierarchy1].[Dim2], [Dim Hierarchy1].[Dim3] } DIMENSION PROPERTIES CHILDREN_CARDINALITY, PARENT_UNIQUE_NAME ON COLUMNS, " +
                                          "NON EMPTY { [Dim Hierarchy2].[Dim1], ORDER([Dim Hierarchy2].[Dim2].Children, [Dim Hierarchy2].[Dim2].CurrentMember.MEMBER_CAPTION, asc) } DIMENSION PROPERTIES CHILDREN_CARDINALITY, PARENT_UNIQUE_NAME ON ROWS " +
                                          "FROM [Cube]";

            //ACT
            var query = _parserSut.ParseQuery(queryString);

            //ASSERT
            Assert.That(query, Is.Not.Null);
            Assert.That(query.ToString(), Is.EqualTo(expectedString));
        }
    }
}
