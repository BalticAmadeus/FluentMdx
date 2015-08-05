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
                .On(Mdx.Axis("COLUMNS").WithSlicer(Mdx.Tuple().With(Mdx.Member("Dim Hierarchy", "Dim"))).AsNonEmpty())
                .From(Mdx.Cube("Cube"))
                .Where(Mdx.Tuple().With(Mdx.Set().With(Mdx.Member("Dim Hierarchy", "Dim", "Dim Key").WithValue("1"))));

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
                .On(Mdx.Axis("COLUMNS").AsEmpty().WithSlicer(Mdx.Tuple().With(Mdx.Member("Dim1 Hierarchy", "Dim1"))))
                .On(Mdx.Axis("ROWS").AsEmpty().WithSlicer(Mdx.Tuple().With(Mdx.Member("Dim2 Hierarchy", "Dim2"))).WithProperties("CHILDREN_CARDINALITY"))
                .From(Mdx.Cube("Cube"))
                .Where(Mdx.Tuple().With(Mdx.Set().With(Mdx.Member("Dim2 Hierarchy", "Dim2", "Dim2 Key").WithValue("1"))));

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
                .On(Mdx.Axis("ROWS").AsNonEmpty().WithSlicer(Mdx.Tuple().With(Mdx.Member("Dim Hierarchy", "Dim"))))
                .From(Mdx.Cube("Cube1"))
                .From(Mdx.Cube("Cube2"))
                .From(Mdx.Cube("Cube3"))
                .Where(Mdx.Tuple().With(Mdx.Set().With(
                    Mdx.Range()
                        .From(Mdx.Member("Dim Hierarchy", "Dim", "Dim Key").WithValue("1"))
                        .To(Mdx.Member("Dim Hierarchy", "Dim", "Dim Key").WithValue("4")))));
            
            //ASSERT
            Assert.That(query.ToString(), Is.EqualTo(expectedQueryString));
        }
    }
}
