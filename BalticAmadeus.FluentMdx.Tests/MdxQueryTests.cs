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
            var query = Mdx.Query()
                .On(Mdx.Axis().Titled("COLUMNS").With(Mdx.Tuple().With(Mdx.Member().Titled("Dim Hierarchy").Titled("Dim"))).NonEmpty())
                .From(Mdx.Cube().Titled("Cube"))
                .Where(Mdx.Tuple().With(Mdx.Set().With(Mdx.Member().Titled("Dim Hierarchy", "Dim", "Dim Key").WithValue("1"))));

            //ASSERT
            Assert.That(query.ToString(), Is.EqualTo(expectedQueryString));
        }

        [Test]
        public void CreateQuery_WithMultipleAxesAndWhereClause_QueryCreatedAsExpected()
        {
            //ARRANGE
            const string expectedQueryString = "SELECT " +
                                               "{ [Dim1 Hierarchy].[Dim1] } ON COLUMNS, " +
                                               "{ [Dim2 Hierarchy].[Dim2] } DIMENSION PROPERTIES CHILDREN_CARDINALITY ON ROWS " +
                                               "FROM [Cube] " +
                                               "WHERE { ( { ( [Dim2 Hierarchy].[Dim2].[Dim2 Key].&[1] ) } ) }";

            //ACT
            var query = Mdx.Query()
                .On(Mdx.Axis().Titled("COLUMNS").Empty().With(Mdx.Tuple().With(Mdx.Member().Titled("Dim1 Hierarchy", "Dim1"))))
                .On(Mdx.Axis().Titled("ROWS").Empty().With(Mdx.Tuple().With(Mdx.Member().Titled("Dim2 Hierarchy", "Dim2"))).WithProperties("CHILDREN_CARDINALITY"))
                .From(Mdx.Cube().Titled("Cube"))
                .Where(Mdx.Tuple().With(Mdx.Set().With(Mdx.Member().Titled("Dim2 Hierarchy", "Dim2", "Dim2 Key").WithValue("1"))));

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
            var query = Mdx.Query()
                .On(Mdx.Axis().Titled("ROWS").NonEmpty().With(Mdx.Tuple().With(Mdx.Member().Titled("Dim Hierarchy", "Dim"))))
                .From(Mdx.Cube().Titled("Cube1"))
                .From(Mdx.Cube().Titled("Cube2"))
                .From(Mdx.Cube().Titled("Cube3"))
                .Where(Mdx.Tuple().With(Mdx.Set().With(Mdx.Range().From(Mdx.Member().Titled("Dim Hierarchy", "Dim", "Dim Key").WithValue("1")).To(Mdx.Member().Titled("Dim Hierarchy", "Dim", "Dim Key").WithValue("4")))));
            
            //ASSERT
            Assert.That(query.ToString(), Is.EqualTo(expectedQueryString));
        }
    }
}
