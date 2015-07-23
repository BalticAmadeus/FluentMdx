namespace BalticAmadeus.FluentMdx.Lexer
{
    internal enum TokenType
    {
        LastToken,
        Select,
        Non,
        Empty,
        From,
        On,
        Comma,
        LeftCurlyBracket,
        RightCurlyBracket,
        IdentifierExpression,
        AxisName,
        LeftRoundBracket,
        RightRoundBracket,
        RangeSeparator,
        Where,
        ValueSeparator,
        NumberExpression,
        LeftSquareBracket,
        RightSquareBracket,
        IdentifierSeparator,
        AnyExpression
    }
}