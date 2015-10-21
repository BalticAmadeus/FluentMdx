using System;
using System.Collections.Generic;
using System.Text;

namespace BalticAmadeus.FluentMdx
{
    /// <summary>
    /// Represents arithmetics, logics or equality expressions used in Mdx query.
    /// </summary>
    public sealed class MdxExpression : MdxExpressionBase, IMdxExpression
    {
        private readonly IList<IMdxExpression> _operands; 
        private readonly IList<string> _operators;
        private bool _isNegative;
        private bool _isNot;

        /// <summary>
        /// List of MdxExpression Operands
        /// </summary>
        public IEnumerable<IMdxExpression> Operands {
            get { return _operands; }
        }

        /// <summary>
        /// Initializes a new instance of <see cref="MdxExpression"/>.
        /// </summary>
        public MdxExpression()
        {
            _isNegative = false;
            _isNot = false;

            _operands = new List<IMdxExpression>();
            _operators = new List<string>();
        }

        /// <summary>
        /// Appends the operation to the end of expression and returns the updated current <see cref="MdxExpression"/>.
        /// </summary>
        /// <param name="operationOperator">Appended operation operator.</param>
        /// <returns>Returns the updated current <see cref="MdxExpression"/>.</returns>
        public MdxExpression WithOperator(string operationOperator)
        {
            _operators.Add(operationOperator);
            return this;
        }

        /// <summary>
        /// Appends the <see cref="IMdxExpression"/> to the end of expression and 
        /// returns the updated current <see cref="MdxExpression"/>.
        /// </summary>
        /// <param name="operand">Appended <see cref="IMdxExpression"/>.</param>
        /// <returns>Returns the updated current <see cref="MdxExpression"/>.</returns>
        public MdxExpression WithOperand(IMdxExpression operand)
        {
            _operands.Add(operand);
            return this;
        }


        /// <summary>
        /// Appends the specified operator and <see cref="IMdxExpression"/> to the 
        /// end of expression and returns the updated current <see cref="MdxExpression"/>.
        /// </summary>
        /// <param name="operationOperator">Appended operation operator.</param>
        /// <param name="operand">Appended <see cref="IMdxExpression"/>.</param>
        /// <returns>Returns the updated current <see cref="MdxExpression"/>.</returns>
        public MdxExpression WithOperation(string operationOperator, IMdxExpression operand)
        {
            _operators.Add(operationOperator);
            _operands.Add(operand);
            
            return this;
        }

        /// <summary>
        /// Marks the current expression as negative and returns the updated current <see cref="MdxExpression"/>.
        /// </summary>
        /// <returns>Returns the updated current <see cref="MdxExpression"/>.</returns>
        public MdxExpression AsNegative()
        {
            _isNegative = true;
            return this;
        }

        /// <summary>
        /// Marks the current expression as negative and returns the updated current <see cref="MdxExpression"/>.
        /// </summary>
        /// <returns>Returns the updated current <see cref="MdxExpression"/>.</returns>
        public MdxExpression AsNegated()
        {
            _isNot = true;
            return this;
        }

        protected override string GetStringExpression()
        {
            var sb = new StringBuilder();

            if (_isNegative)
                sb.Append("-(");

            if (_isNot)
                sb.Append("NOT (");

            var operandsEnumerator = _operands.GetEnumerator();
            if (!operandsEnumerator.MoveNext())
                throw new InvalidOperationException("Expression must have at least one operand!");

            if (operandsEnumerator.Current is MdxExpression)
                sb.Append(string.Format("({0})", operandsEnumerator.Current));
            else
                sb.Append(operandsEnumerator.Current);

            foreach (var op in _operators)
            {
                sb.Append(" ");
                sb.Append(op);
                sb.Append(" ");

                if (!operandsEnumerator.MoveNext())
                    throw new InvalidOperationException("Expression expects more operands that specified!");

                if (operandsEnumerator.Current is MdxExpression)
                    sb.Append(string.Format("({0})", operandsEnumerator.Current));
                else
                    sb.Append(operandsEnumerator.Current);
            }

            if (_isNegative)
                sb.Append(")");

            if (_isNot)
                sb.Append(")");

            return sb.ToString();
        }
    }
}