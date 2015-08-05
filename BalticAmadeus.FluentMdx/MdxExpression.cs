using System;
using System.Collections.Generic;
using System.Text;

namespace BalticAmadeus.FluentMdx
{
    public class MdxExpression : MdxExpressionBase, IMdxExpressionOperand
    {
        private readonly IList<IMdxExpressionOperand> _operands; 
        private readonly IList<string> _operations;
        private bool _isNegative;
        private bool _isNot;

        public MdxExpression()
        {
            _isNegative = false;
            _isNot = false;

            _operands = new List<IMdxExpressionOperand>();
            _operations = new List<string>();
        }

        public MdxExpression WithOperation(string operation)
        {
            _operations.Add(operation);
            return this;
        }

        public MdxExpression WithOperand(IMdxExpressionOperand operand)
        {
            _operands.Add(operand);
            return this;
        }

        public MdxExpression AsNegative()
        {
            _isNegative = true;
            return this;
        }

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

            foreach (var op in _operations)
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