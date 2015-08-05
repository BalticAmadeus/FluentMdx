namespace BalticAmadeus.FluentMdx
{
    public class MdxRange : MdxExpressionBase, IMdxMember, IMdxExpressionOperand
    {
        public MdxMember FromMember { get; private set; }
        public MdxMember ToMember { get; private set; }

        public MdxRange From(MdxMember member)
        {
            FromMember = member;

            return this;
        }

        public MdxRange To(MdxMember member)
        {
            ToMember = member;

            return this;
        }

        protected override string GetStringExpression()
        {
            return string.Format("{0}:{1}", FromMember, ToMember);
        }
    }
}