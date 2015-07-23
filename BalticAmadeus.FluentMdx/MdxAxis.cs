using System;
using System.Collections.Generic;
using System.Linq;

namespace BalticAmadeus.FluentMdx
{
    public class MdxAxis : IMdxExpression
    {
        private readonly string _title;
        private readonly IList<MdxAxisParameter> _parameters;

        public MdxAxis(string title) : this(title, new List<MdxAxisParameter>()) { }

        internal MdxAxis(string title, IList<MdxAxisParameter> parameters)
        {
            _parameters = parameters;
            _title = title;
        }

        public string Title
        {
            get { return _title; }
        }

        public IEnumerable<MdxAxisParameter> Parameters
        {
            get { return _parameters; }
        }

        public string GetStringExpression()
        {
            if (!_parameters.Any())
                throw new ArgumentException("There are no axis parameters in axis!");

            return string.Format(@"NON EMPTY {{ {0} }} ON {1}",
                string.Join(", ", Parameters),
                Title);
        }

        public override string ToString()
        {
            return GetStringExpression();
        }

        public override int GetHashCode()
        {
            return Title.GetHashCode();
        }

        public MdxAxis With(MdxAxisParameter parameter)
        {
            _parameters.Add(parameter);
            return this;
        }
    }
}