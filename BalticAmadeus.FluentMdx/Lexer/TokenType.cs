namespace BalticAmadeus.FluentMdx.Lexer
{
    /// <summary>
    /// Provides a list of possible tokens that can be recognized by <see cref="ILexer"/>.
    /// </summary>
    internal enum TokenType
    {
        AnyExpression,

        As,

        /// <summary>
        /// Denotes an axis name identifier.
        /// </summary>
        AxisNameIdentifier,

        /// <summary>
        /// Denotes a <value>Dimension</value> keyword.
        /// </summary>
        Dimension,

        /// <summary>
        /// Denotes a date expression. 
        /// </summary>
        DateExpression,

        /// <summary>
        /// Denotes a dimension property identifier.
        /// </summary>
        DimensionProperty,

        /// <summary>
        /// Denotes a <value>Empty</value> keyword.
        /// </summary>
        Empty,

        /// <summary>
        /// Denotes an end of line.
        /// </summary>
        EndOfLine,

        /// <summary>
        /// Denotes a <value>From</value> keyword.
        /// </summary>
        From,

        /// <summary>
        /// Denotes an identifier separator (<value>.</value>).
        /// </summary>
        IdentifierSeparator,

        /// <summary>
        /// Denotes the end of tokens list.
        /// </summary>
        LastToken,

        /// <summary>
        /// Denotes the set openning bracket (<value>{</value>).
        /// </summary>
        LeftCurlyBracket,

        /// <summary>
        /// Denotes the function or tuple openning bracket (<value>(</value>).
        /// </summary>
        LeftRoundBracket,

        /// <summary>
        /// Denotes the member or member value openning bracket (<value>[</value>).
        /// </summary>
        //LeftSquareBracket,

        /// <summary>
        /// Denotes the <value>True</value> or <value>False</value> logical constants.
        /// </summary>
        LogicalExpression,

        /// <summary>
        /// Denotes the logical operators.
        /// </summary>
        LogicsOperator,

        /// <summary>
        /// Denotes the arithmetical and equality operators. 
        /// </summary>
        MathsOperator,

        Member,

        /// <summary>
        /// Denotes a member separator (<value>,</value>).
        /// </summary>
        MemberSeparator,

        /// <summary>
        /// Denotes a <value>Non</value> keyword.
        /// </summary>
        Non,

        /// <summary>
        /// Denotes a <value>Not</value> logical operator.
        /// </summary>
        NotOperator,

        /// <summary>
        /// Denotes any sequence of digits as number expression.
        /// </summary>
        NumberExpression,

        /// <summary>
        /// Denotes an <value>On</value> keyword.
        /// </summary>
        On,

        Ordering,

        /// <summary>
        /// Denotes a <value>Properties</value> keyword.
        /// </summary>
        Properties,

        /// <summary>
        /// Denotes a range separator (<value>:</value>).
        /// </summary>
        RangeSeparator,

        /// <summary>
        /// Denotes the set closing bracket (<value>}</value>).
        /// </summary>
        RightCurlyBracket,

        /// <summary>
        /// Denotes the tuple closing bracket (<value>)</value>).
        /// </summary>
        RightRoundBracket,

        /// <summary>
        /// Denotes the identifier closing bracket (<value>]</value>).
        /// </summary>
        //RightSquareBracket,

        /// <summary>
        /// Denotes a <value>Select</value> keyword.
        /// </summary>
        Select,
        
        Set,

        /// <summary>
        /// Denotes a value separator (<value>.&amp;</value>).
        /// </summary>
        ValueSeparator,

        /// <summary>
        /// Denotes a <value>Where</value> keyword.
        /// </summary>
        Where,

        /// <summary>
        /// Denotes a <value>With</value> keyword.
        /// </summary>
        With,

        IdentifierExpression,
        TitleExpression
    }
}