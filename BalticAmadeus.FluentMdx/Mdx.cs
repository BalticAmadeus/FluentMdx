namespace BalticAmadeus.FluentMdx
{
    public static class Mdx
    {
        public static MdxQuery Query()
        {
            return new MdxQuery();
        }

        internal static MdxCube Cube()
        {
            return new MdxCube();
        }

        public static MdxCube Cube(params string[] titles)
        {
            return Cube().Titled(titles);
        }

        internal static MdxAxis Axis()
        {
            return new MdxAxis();
        }

        public static MdxAxis Axis(string title)
        {
            return Axis().Titled(title);
        }

        public static MdxTuple Tuple()
        {
            return new MdxTuple();
        }

        public static MdxSet Set()
        {
            return new MdxSet();
        }

        public static MdxMember Member()
        {
            return new MdxMember();
        }

        public static MdxMember Member(params string[] titles)
        {
            return Member().Titled(titles);
        }

        public static MdxRange Range()
        {
            return new MdxRange();
        }

        public static MdxFunction Function()
        {
            return new MdxFunction();
        }

        public static MdxFunction Function(params string[] titles)
        {
            return Function().Titled(titles);
        }

        public static MdxNavigationFunction NavigationFunction()
        {
            return new MdxNavigationFunction();
        }

        public static MdxNavigationFunction NavigationFunction(string title)
        {
            return NavigationFunction().Titled(title);
        }

        public static MdxExpression Expression()
        {
            return new MdxExpression();
        }

        public static MdxConstantExpression ConstantValue()
        {
            return new MdxConstantExpression();
        }

        public static MdxConstantExpression ConstantValue(string value)
        {
            return new MdxConstantExpression().WithValue(value);
        }
    }
}
