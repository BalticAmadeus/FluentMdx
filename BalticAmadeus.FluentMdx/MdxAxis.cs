using System.Collections.Generic;
using System.Linq;

namespace BalticAmadeus.FluentMdx
{
    public class MdxAxis : IMdxExpression
    {
        private readonly string _title;
        private readonly IList<MdxMember> _axisParameters;
        private readonly IList<string> _properties;

        public MdxAxis(string title) : this(title, new List<MdxMember>(), new List<string>()) { }

        internal MdxAxis(string title, IList<MdxMember> axisParameters, IList<string> properties)
        {
            _axisParameters = axisParameters;
            _properties = properties;
            _title = title;
        }

        public string Title
        {
            get { return _title; }
        }

        public IEnumerable<MdxMember> AxisParameters
        {
            get { return _axisParameters; }
        }

        public IEnumerable<string> Properties
        {
            get { return _properties; }
        }

        public string GetStringExpression()
        {
            if (!Properties.Any())
                return string.Format(@"NON EMPTY {{ {0} }} ON {1}",
                    string.Join(", ", AxisParameters),
                    Title);

            return string.Format(@"NON EMPTY {{ {0} }} DIMENSION PROPERTIES {1} ON {2}",
                string.Join(", ", AxisParameters),
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

        public MdxAxis With(MdxMember parameter)
        {
            _axisParameters.Add(parameter);
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