namespace BalticAmadeus.FluentMdx.Lexer
{
    /// <summary>
    /// Represents an immutable lexical analysis result as chunk of recognized text.
    /// </summary>
    internal struct Token
    {
        /// <summary>
        /// Represents the <see cref="TokenType"/> of <see cref="Token"/>. This field is read-only.
        /// </summary>
        public readonly TokenType Type;

        /// <summary>
        /// Represents the recognized value of <see cref="Token"/>. This field is read-only.
        /// </summary>
        public readonly string Value;

        /// <summary>
        /// Initializes a new instance of <see cref="Token"/> with provided <paramref name="type"/> and <paramref name="value"/>.
        /// </summary>
        /// <param name="type">Type of token.</param>
        /// <param name="value">Recognized token value.</param>
        public Token(TokenType type, string value)
        {
            Value = value;
            Type = type;
        }

        /// <summary>
        /// Converts the value of the current <see cref="Token"/> object to its equivalent string representation.
        /// </summary>
        /// <returns>A string representation of the value of the current <see cref="Token"/> object.</returns>
        public override string ToString()
        {
            return string.Format("{0}[{1}]", Type, Value);
        }
    }
}