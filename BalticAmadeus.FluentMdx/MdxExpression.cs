using System;
using System.Collections.Generic;
using System.Text;

namespace BalticAmadeus.FluentMdx
{
    public class MdxExpression : IMdxExpression
    {
        private readonly IList<string> _operations;
        private readonly IList<string> _expressions;

        public MdxExpression()
        {
            _operations = new List<string>();
            _expressions = new List<string>();
        }

        public MdxExpression WithOperand(string operand)
        {
            _expressions.Add(operand);
            return this;
        }

        public MdxExpression WithOperation(string operation)
        {
            _operations.Add(operation);
            return this;
        }

        public string GetStringExpression()
        {
            var sb = new StringBuilder();

            var operandsEnumerator = _expressions.GetEnumerator();
            if (!operandsEnumerator.MoveNext())
                throw new InvalidOperationException("Expression must have at least one operand!");

            sb.Append(operandsEnumerator.Current);

            foreach (var op in _operations)
            {
                sb.Append(" ");
                sb.Append(op);
                sb.Append(" ");

                if (!operandsEnumerator.MoveNext())
                    throw new InvalidOperationException("Expression expects more operands that specified!");

                sb.Append(operandsEnumerator.Current);
            }

            return sb.ToString();
        }

        public override string ToString()
        {
            return GetStringExpression();
        }
    }
}