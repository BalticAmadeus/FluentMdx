using System;

namespace BalticAmadeus.FluentMdx
{
    /// <summary>
    /// Mdx components factory, that can be used as a more fluent way of creating queries.
    /// </summary>
    public static class Mdx
    {
        /// <summary>
        /// Creates a new instance of <see cref="MdxQuery"/>.
        /// </summary>
        /// <returns>Returns instance of <see cref="MdxQuery"/>.</returns>
        public static MdxQuery Query()
        {
            return new MdxQuery();
        }

        /// <summary>
        /// Creates a new instance of <see cref="MdxCube"/>.
        /// </summary>
        /// <returns>Returns instance of <see cref="MdxCube"/>.</returns>
        internal static MdxCube Cube()
        {
            return new MdxCube();
        }

        /// <summary>
        /// Creates a new instance of <see cref="MdxCube"/> with specified titles.
        /// </summary>
        /// <param name="titles">A collection of cube titles.</param>
        /// <returns>Returns instance of <see cref="MdxCube"/>.</returns>
        public static MdxCube Cube(params string[] titles)
        {
            return Cube().Titled(titles);
        }

        /// <summary>
        /// Creates a new instance of <see cref="MdxAxis"/>.
        /// </summary>
        /// <returns>Returns instance of <see cref="MdxAxis"/>.</returns>
        internal static MdxAxis Axis()
        {
            return new MdxAxis();
        }

        /// <summary>
        /// Creates a new instance of <see cref="MdxAxis"/> with specified title.
        /// </summary>
        /// <param name="title">Axis title.</param>
        /// <returns>Returns instance of <see cref="MdxAxis"/>.</returns>
        [Obsolete("Use Axis(MdxAxis.MdxAxisType type) or Axis(int id) instead.")]
        public static MdxAxis Axis(string title)
        {
            return Axis().Titled(title);
        }

        /// <summary>
        /// Creates a new instance of <see cref="MdxAxis"/> with specified typed-title.
        /// </summary>
        /// <param name="type">Axis title-type.</param>
        /// <returns>Returns instance of <see cref="MdxAxis"/>.</returns>
        public static MdxAxis Axis(MdxAxis.MdxAxisType type)
        {
            return Axis().Titled(type);
        }

        /// <summary>
        /// Creates a new instance of <see cref="MdxAxis"/> with specified id-title.
        /// </summary>
        /// <param name="id">Axis id-title.</param>
        /// <returns>Returns instance of <see cref="MdxAxis"/>.</returns>
        public static MdxAxis Axis(int id)
        {
            return Axis().Titled(id);
        }
        
        /// <summary>
        /// Creates a new instance of empty <see cref="MdxTuple"/>.
        /// </summary>
        /// <returns>Returns instance of <see cref="MdxTuple"/>.</returns>
        public static MdxTuple Tuple()
        {
            return new MdxTuple();
        }

        /// <summary>
        /// Creates a new instance of empty <see cref="MdxSet"/>.
        /// </summary>
        /// <returns>Returns instance of <see cref="MdxSet"/>.</returns>
        public static MdxSet Set()
        {
            return new MdxSet();
        }

        /// <summary>
        /// Creates a new instance of <see cref="MdxMember"/>.
        /// </summary>
        /// <returns>Returns instance of <see cref="MdxMember"/>.</returns>
        internal static MdxMember Member()
        {
            return new MdxMember();
        }

        /// <summary>
        /// Creates a new instance of <see cref="MdxMember"/> with specified titles.
        /// </summary>
        /// <param name="titles">A collection of member titles.</param>
        /// <returns>Returns instance of <see cref="MdxMember"/>.</returns>
        public static MdxMember Member(params string[] titles)
        {
            return Member().Titled(titles);
        }

        /// <summary>
        /// Creates a new instance of <see cref="MdxRange"/>.
        /// </summary>
        /// <returns>Returns instance of <see cref="MdxRange"/>.</returns>
        public static MdxRange Range()
        {
            return new MdxRange();
        }

        /// <summary>
        /// Creates a new instance of <see cref="MdxFunction"/>.
        /// </summary>
        /// <returns>Returns instance of <see cref="MdxFunction"/>.</returns>
        internal static MdxFunction Function()
        {
            return new MdxFunction();
        }

        /// <summary>
        /// Creates a new instance of <see cref="MdxFunction"/> with specified titles.
        /// </summary>
        /// <param name="titles">A collection of function titles.</param>
        /// <returns>Returns instance of <see cref="MdxFunction"/>.</returns>
        public static MdxFunction Function(params string[] titles)
        {
            return Function().Titled(titles);
        }

        /// <summary>
        /// Creates a new instance of <see cref="MdxNavigationFunction"/>.
        /// </summary>
        /// <returns>Returns instance of <see cref="MdxNavigationFunction"/>.</returns>
        internal static MdxNavigationFunction NavigationFunction()
        {
            return new MdxNavigationFunction();
        }

        /// <summary>
        /// Creates a new instance of <see cref="MdxNavigationFunction"/> with specified title.
        /// </summary>
        /// <param name="title">Navigation function title.</param>
        /// <returns>Returns instance of <see cref="MdxNavigationFunction"/>.</returns>
        public static MdxNavigationFunction NavigationFunction(string title)
        {
            return NavigationFunction().Titled(title);
        }

        /// <summary>
        /// Creates a new instance of <see cref="MdxExpression"/>.
        /// </summary>
        /// <returns>Returns instance of <see cref="MdxExpression"/>.</returns>
        public static MdxExpression Expression()
        {
            return new MdxExpression();
        }

        /// <summary>
        /// Creates a new instance of <see cref="MdxConstantExpression"/>.
        /// </summary>
        /// <returns>Returns instance of <see cref="MdxConstantExpression"/>.</returns>
        internal static MdxConstantExpression ConstantValue()
        {
            return new MdxConstantExpression();
        }

        /// <summary>
        /// Creates a new instance of <see cref="MdxConstantExpression"/> with specified value.
        /// </summary>
        /// <param name="value">Constant value string.</param>
        /// <returns>Returns instance of <see cref="MdxConstantExpression"/>.</returns>
        public static MdxConstantExpression ConstantValue(string value)
        {
            return ConstantValue().WithValue(value);
        }

        internal static MdxDeclaration Declaration()
        {
            return new MdxDeclaration();
        }

        public static MdxDeclaration Declaration(params string[] titles)
        {
            return Declaration().Titled(titles);
        }
    }
}
