namespace BalticAmadeus.FluentMdx
{
    public static class Mdx
    {
        public static MdxQuery Query()
        {
            return new MdxQuery();
        }

        public static MdxCube Cube()
        {
            return new MdxCube();
        }

        public static MdxAxis Axis()
        {
            return new MdxAxis();
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

        public static MdxRange Range()
        {
            return new MdxRange();
        }
    }
}
