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
                                               "NON EMPTY { [Dim Hierarchy].[Dim] } ON Columns " +
                                               "FROM [Cube] " +
                                               "WHERE { ( { ( [Dim Hierarchy].[Dim].[Dim Key].&[1] ) } ) }";

            //ACT
            var query = Mdx.Query()
                .On(Mdx.Axis(0).WithSlicer(Mdx.Tuple().With(Mdx.Member("Dim Hierarchy", "Dim"))).AsNonEmpty())
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
                                               "{ [Dim1 Hierarchy].[Dim1] } ON Columns, " +
                                               "{ [Dim2 Hierarchy].[Dim2] } DIMENSION PROPERTIES CHILDREN_CARDINALITY ON Rows " +
                                               "FROM [Cube] " +
                                               "WHERE { ( { ( [Dim2 Hierarchy].[Dim2].[Dim2 Key].&[1] ) } ) }";

            //ACT
            var query = Mdx.Query()
                .On(Mdx.Axis(0).AsEmpty().WithSlicer(Mdx.Tuple().With(Mdx.Member("Dim1 Hierarchy", "Dim1"))))
                .On(Mdx.Axis(1).AsEmpty().WithSlicer(Mdx.Tuple().With(Mdx.Member("Dim2 Hierarchy", "Dim2"))).WithProperties("CHILDREN_CARDINALITY"))
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
                                               "NON EMPTY { [Dim Hierarchy].[Dim] } ON Columns " +
                                               "FROM [Cube1], [Cube2], [Cube3] " +
                                               "WHERE { ( { ( [Dim Hierarchy].[Dim].[Dim Key].&[1]:[Dim Hierarchy].[Dim].[Dim Key].&[4] ) } ) }";

            //ACT
            var query = Mdx.Query()
                .On(Mdx.Axis(0).AsNonEmpty().WithSlicer(Mdx.Tuple().With(Mdx.Member("Dim Hierarchy", "Dim"))))
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

        [Test]
        public void CreateQuery_WithFunctionAndDifferentExpressions_QueryCreatedAsExpected()
        {
            //ARRANGE
            const string expectedQueryString = "SELECT { [Dim Hierarchy].[Dim] } ON Columns " +
                                               "FROM [Cube] " +
                                               "WHERE { ( { FILTER([Dim Hierarchy].[Dim].[Dim Key], { [Dim Hierarchy].[Dim].[Dim Key] }) } ) }";

            //ACT
            var query = Mdx.Query()
                .On(Mdx.Axis().WithSlicer(Mdx.Tuple().With(Mdx.Member("Dim Hierarchy", "Dim"))))
                .From(Mdx.Cube("Cube"))
                .Where(Mdx.Tuple().With(
                    Mdx.Function("FILTER")
                        .WithParameters(Mdx.Member("Dim Hierarchy", "Dim", "Dim Key"))
                        .WithParameters(Mdx.Tuple().With(Mdx.Member("Dim Hierarchy", "Dim", "Dim Key")))));

            //ASSERT
            Assert.That(query.ToString(), Is.EqualTo(expectedQueryString));
        }

        [Test]
        public void CreateQuery_WithDeclaredMemberAndSet_QueryCreatedAsExpected()
        {
            //ARRANGE
            const string expectedQueryString = "WITH " +
                                               "MEMBER [MyMember] AS [Dim Hierarchy].[Dim] " +
                                               "SET [MySet] AS { FILTER([Dim Hierarchy].[Dim].[Dim Key], { [Dim Hierarchy].[Dim].[Dim Key] }) } " +
                                               "SELECT { [MyMember] } ON Columns " +
                                               "FROM [Cube] " +
                                               "WHERE { ( { ( [MySet] ) } ) }";

            //ACT
            var query = Mdx.Query()
                .With(Mdx.DeclaredMember("MyMember").As(Mdx.Expression().WithOperand(Mdx.Member("Dim Hierarchy", "Dim"))))
                .With(Mdx.DeclaredSet("MySet").As(Mdx.Tuple().With(Mdx.Function("FILTER")
                    .WithParameters(Mdx.Member("Dim Hierarchy", "Dim", "Dim Key"))
                    .WithParameters(Mdx.Tuple().With(Mdx.Member("Dim Hierarchy", "Dim", "Dim Key"))))))
                .On(Mdx.Axis(0).WithSlicer(Mdx.Tuple().With(Mdx.Member("MyMember"))))
                .From(Mdx.Cube("Cube"))
                .Where(Mdx.Tuple().With(Mdx.Set().With(Mdx.Member("MySet"))));

            //ASSERT
            Assert.That(query.ToString(), Is.EqualTo(expectedQueryString));
        }
    }
}
