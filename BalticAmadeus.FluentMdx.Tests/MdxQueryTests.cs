using System;
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
                                               "NON EMPTY { [Dimension Hierarchy].[Dimension] } ON COLUMNS " +
                                               "FROM [Cube] " +
                                               "WHERE { ( [Dimension Hierarchy].[Dimension].[Dimension Key].&[1] ) }";

            //ACT
            var query = new MdxQuery()
                .On(new MdxAxis("COLUMNS").With(new MdxAxisParameter("Dimension Hierarchy").WithNameParts("Dimension")))
                .From(new MdxCube("Cube"))
                .Where(new MdxSetTuple().With(new MdxMemberSet().With(new MdxValueMember("Dimension Hierarchy", "1").WithNameParts("Dimension").WithNameParts("Dimension Key"))));

            //ASSERT
            Assert.That(query.ToString(), Is.EqualTo(expectedQueryString));
        }

        [Test]
        public void CreateQuery_WithMultipleAxesAndWhereClause_QueryCreatedAsExpected()
        {
            //ARRANGE
            const string expectedQueryString = "SELECT " +
                                               "NON EMPTY { [Dimension1 Hierarchy].[Dimension1] } ON COLUMNS, " +
                                               "NON EMPTY { [Dimension2 Hierarchy].[Dimension2] } ON ROWS " +
                                               "FROM [Cube] " +
                                               "WHERE { ( [Dimension2 Hierarchy].[Dimension2].[Dimension2 Key].&[1] ) }";

            //ACT
            var query = new MdxQuery()
                .On(new MdxAxis("COLUMNS").With(new MdxAxisParameter("Dimension1 Hierarchy").WithNameParts("Dimension1")))
                .On(new MdxAxis("ROWS").With(new MdxAxisParameter("Dimension2 Hierarchy").WithNameParts("Dimension2")))
                .From(new MdxCube("Cube"))
                .Where(new MdxSetTuple().With(new MdxMemberSet().With(new MdxValueMember("Dimension2 Hierarchy", "1").WithNameParts("Dimension2", "Dimension2 Key"))));

            //ASSERT
            Assert.That(query.ToString(), Is.EqualTo(expectedQueryString));
        }

        [Test]
        public void CreateQuery_WithMultipleCubesAndRangeWhereClause_QueryCreatedAsExpected()
        {
            //ARRANGE
            const string expectedQueryString = "SELECT " +
                                               "NON EMPTY { [Dimension Hierarchy].[Dimension] } ON ROWS " +
                                               "FROM [Cube1], [Cube2], [Cube3] " +
                                               "WHERE { ( [Dimension Hierarchy].[Dimension].[Dimension Key].&[1]:[Dimension Hierarchy].[Dimension].[Dimension Key].&[4] ) }";

            //ACT
            var query = new MdxQuery()
                .On(new MdxAxis("ROWS").With(new MdxAxisParameter("Dimension Hierarchy").WithNameParts("Dimension")))
                .From(new MdxCube("Cube1"))
                .From(new MdxCube("Cube2"))
                .From(new MdxCube("Cube3"))
                .Where(new MdxSetTuple().With(new MdxMemberSet().With(new MdxRangeMember("Dimension Hierarchy", "1", "4").WithNameParts("Dimension", "Dimension Key"))));
            
            //ASSERT
            Assert.That(query.ToString(), Is.EqualTo(expectedQueryString));
        }

        [Test]
        public void CreateQuery_WithNoAxes_ThrowsException()
        {
            //ARRANGE
            //ACT
            var query = new MdxQuery()
                .From(new MdxCube("Cube"))
                .Where(new MdxSetTuple().With(new MdxMemberSet().With(new MdxRangeMember("Dimension Hierarchy", "1", "4").WithNameParts("Dimension", "Dimension Key"))));
            

            //ASSERT
            Assert.Throws<ArgumentException>(() => { query.ToString(); }, "There are no axes in query!");
        }

        [Test]
        public void CreateQuery_WithNoAxisParameters_ThrowsException()
        {
            //ARRANGE
            //ACT
            var query = new MdxQuery()
                .On(new MdxAxis("ROWS"))
                .Where(new MdxSetTuple().With(new MdxMemberSet().With(new MdxRangeMember("Dimension Hierarchy", "1", "4").WithNameParts("Dimension", "Dimension Key"))));
            
            //ASSERT
            Assert.Throws<ArgumentException>(() => { query.ToString(); }, "There are no axis parameters in axis!");
        }

        [Test]
        public void CreateQuery_WithNoCubes_ThrowsException()
        {
            //ARRANGE
            //ACT
            var query = new MdxQuery()
                .On(new MdxAxis("ROWS").With(new MdxAxisParameter("Dimension Hierarchy").WithNameParts("Dimension")))
                .Where(new MdxSetTuple().With(new MdxMemberSet().With(new MdxRangeMember("Dimension Hierarchy", "1", "4").WithNameParts("Dimension", "Dimension Key"))));
            

            //ASSERT
            Assert.Throws<ArgumentException>(() => { query.ToString(); }, "There are no cubes in query!");
        }
    }
}
