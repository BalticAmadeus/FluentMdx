namespace BalticAmadeus.FluentMdx.Lexer
{
    internal class Token
    {
        public TokenType Type { get; private set; }
        public string Value { get; private set; }

        public Token(TokenType type, string value)
        {
            Value = value;
            Type = type;
        }

        public override string ToString()
        {
            return string.Format("{0}[{1}]", Type, Value);
        }
    }
}