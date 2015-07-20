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
        Member,
        AxisName,
        LeftRoundBracket,
        RightRoundBracket,
        Colon,
        Where,
        Value,
        Number
    }
}