using System.Diagnostics.CodeAnalysis;
using NUnit.Framework;

namespace BalticAmadeus.FluentMdx.Tests
{
    [TestFixture, ExcludeFromCodeCoverage]
    public class MdxQueryTests
    {
        [Test]
        public void CreateQuery_WithSimpleWhereClause_QueryCreatedAsExpected()
        {
            //ARRANGE
            const string expectedQueryString = "SELECT " +
                                               "NON EMPTY { [Dim Hierarchy].[Dim] } ON COLUMNS " +
                                               "FROM [Cube] " +
                                               "WHERE { ( { ( [Dim Hierarchy].[Dim].[Dim Key].&[1] ) } ) }";

            //ACT
            var query = new MdxQuery()
                .On(new MdxAxis("COLUMNS").NonEmpty().With(new MdxMemberTuple().With(new MdxMember("Dim Hierarchy").AppendNames("Dim"))))
                .From(new MdxCube("Cube"))
                .Where(new MdxSetTuple().With(new MdxMemberSet().With(new MdxValueMember("1", "Dim Hierarchy", "Dim", "Dim Key"))));

            //ASSERT
            Assert.That(query.ToString(), Is.EqualTo(expectedQueryString));
        }

        [Test]
        public void CreateQuery_WithMultipleAxesAndWhereClause_QueryCreatedAsExpected()
        {
            //ARRANGE
            const string expectedQueryString = "SELECT " +
                                               "NON EMPTY { [Dim1 Hierarchy].[Dim1] } ON COLUMNS, " +
                                               "NON EMPTY { [Dim2 Hierarchy].[Dim2] } ON ROWS " +
                                               "FROM [Cube] " +
                                               "WHERE { ( { ( [Dim2 Hierarchy].[Dim2].[Dim2 Key].&[1] ) } ) }";

            //ACT
            var query = new MdxQuery()
                .On(new MdxAxis("COLUMNS").NonEmpty().With(new MdxMemberTuple().With(new MdxMember("Dim1 Hierarchy", "Dim1"))))
                .On(new MdxAxis("ROWS").NonEmpty().With(new MdxMemberTuple().With(new MdxMember("Dim2 Hierarchy", "Dim2"))))
                .From(new MdxCube("Cube"))
                .Where(new MdxSetTuple().With(new MdxMemberSet().With(new MdxValueMember("1", "Dim2 Hierarchy", "Dim2", "Dim2 Key"))));

            //ASSERT
            Assert.That(query.ToString(), Is.EqualTo(expectedQueryString));
        }

        [Test]
        public void CreateQuery_WithMultipleCubesAndRangeWhereClause_QueryCreatedAsExpected()
        {
            //ARRANGE
            const string expectedQueryString = "SELECT " +
                                               "NON EMPTY { [Dim Hierarchy].[Dim] } ON ROWS " +
                                               "FROM [Cube1], [Cube2], [Cube3] " +
                                               "WHERE { ( { ( [Dim Hierarchy].[Dim].[Dim Key].&[1]:[Dim Hierarchy].[Dim].[Dim Key].&[4] ) } ) }";

            //ACT
            var query = new MdxQuery()
                .On(new MdxAxis("ROWS").NonEmpty().With(new MdxMemberTuple().With(new MdxMember("Dim Hierarchy", "Dim"))))
                .From(new MdxCube("Cube1"))
                .From(new MdxCube("Cube2"))
                .From(new MdxCube("Cube3"))
                .Where(new MdxSetTuple().With(new MdxMemberSet().With(new MdxRangeMember("1", "4", "Dim Hierarchy", "Dim", "Dim Key"))));
            
            //ASSERT
            Assert.That(query.ToString(), Is.EqualTo(expectedQueryString));
        }
    }
}
