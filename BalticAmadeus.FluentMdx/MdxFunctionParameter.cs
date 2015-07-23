namespace BalticAmadeus.FluentMdx
{
    public class MdxFunctionParameter : IMdxExpression
    {
        private readonly string _parameterValue;

        public MdxFunctionParameter(string parameterValue)
        {
            _parameterValue = parameterValue;
        }

        public string ParameterValue
        {
            get { return _parameterValue; }
        }

        public string GetStringExpression()
        {
            return ParameterValue;
        }

        public override int GetHashCode()
        {
            return ParameterValue.GetHashCode();
        }

        public override string ToString()
        {
            return GetStringExpression();
        }
    }
}
