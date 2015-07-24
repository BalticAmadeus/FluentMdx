using System;
using System.Collections.Generic;
using System.Linq;

namespace BalticAmadeus.FluentMdx
{
    public class MdxAxis : IMdxExpression
    {
        private readonly string _title;
        private readonly IList<MdxAxisParameter> _parameters;
        private readonly IList<string> _properties; 

        public MdxAxis(string title) : this(title, new List<MdxAxisParameter>(), new List<string>()) { }

        internal MdxAxis(string title, IList<MdxAxisParameter> parameters, IList<string> properties)
        {
            _parameters = parameters;
            _properties = properties;
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

        public IEnumerable<string> Properties
        {
            get { return _properties; }
        }

        public string GetStringExpression()
        {
            if (!Parameters.Any())
                throw new ArgumentException("There are no axis parameters in axis!");

            if (!Properties.Any())
                return string.Format(@"NON EMPTY {{ {0} }} ON {1}",
                    string.Join(", ", Parameters),
                    Title);

            return string.Format(@"NON EMPTY {{ {0} }} DIMENSION PROPERTIES {1} ON {2}",
                string.Join(", ", Parameters),
                string.Join(", ", Properties),
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

        public MdxAxis WithProperties(params string[] properties)
        {
            foreach (var property in properties)
                _properties.Add(property);
            return this;
        }
    }
}