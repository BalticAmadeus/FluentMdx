using System;
using System.Collections.Generic;
using System.Linq;

namespace BalticAmadeus.FluentMdx
{
    public class MdxAxis : IMdxExpression
    {
        private readonly List<MdxAxisParameter> _parameters;
        private readonly string _name;

        internal MdxAxis(string name, List<MdxAxisParameter> parameters)
        {
            _parameters = parameters;
            _name = name;
        }

        public MdxAxis(string name) : this(name, new List<MdxAxisParameter>()) { }

        public string Name
        {
            get { return _name; }
        }

        public IEnumerable<MdxAxisParameter> Parameters
        {
            get { return _parameters; }
        }

        public MdxAxis With(MdxAxisParameter axisParameter)
        {
            if (axisParameter == null)
                throw new ArgumentNullException("axisParameter");

            _parameters.Add(axisParameter);
            return this;
        }

        public MdxAxis Without(MdxAxisParameter axisParameter)
        {
            if (axisParameter == null)
                throw new ArgumentNullException("axisParameter");

            _parameters.Remove(axisParameter);
            return this;
        }

        public string GetStringExpression()
        {
            if (!_parameters.Any())
                throw new ArgumentException("There are no axis parameters in axis!");

            return string.Format(@"NON EMPTY {{ {0} }} ON {1}",
                string.Join(", ", Parameters),
                Name);
        }

        public override string ToString()
        {
            return GetStringExpression();
        }

        public override int GetHashCode()
        {
            return Name.GetHashCode();
        }

        public override bool Equals(object obj)
        {
            var axis = obj as MdxAxis;
            if (axis == null)
                return false;

            return Name == axis.Name;
        }
    }
}