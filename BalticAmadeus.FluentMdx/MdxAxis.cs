using System.Collections.Generic;
using System.Linq;

namespace BalticAmadeus.FluentMdx
{
    public class MdxAxis : IMdxExpression
    {
        private string _title;
        private MdxTuple _axisParameters;
        private readonly IList<string> _properties;
        private bool _isNonEmpty;

        public MdxAxis() : this(null)
        {
        }

        public MdxAxis(string title) : this(title, null, new List<string>()) { }

        internal MdxAxis(string title, MdxTuple axisParameters, IList<string> properties)
        {
            _axisParameters = axisParameters;
            _properties = properties;
            _title = title;
            _isNonEmpty = false;
        }

        public string Title
        {
            get { return _title; }
        }

        public MdxTuple AxisParameters
        {
            get { return _axisParameters; }
        }

        public IEnumerable<string> Properties
        {
            get { return _properties; }
        }

        public string GetStringExpression()
        {
            if (!_isNonEmpty)
            {
                if (!Properties.Any())
                    return string.Format(@"{0} ON {1}",
                        AxisParameters,
                        Title);

                return string.Format(@"{0} DIMENSION PROPERTIES {1} ON {2}",
                    AxisParameters,
                    string.Join(", ", Properties),
                    Title);
            }

            if (!Properties.Any())
                return string.Format(@"NON EMPTY {0} ON {1}",
                    AxisParameters, 
                    Title);

            return string.Format(@"NON EMPTY {0} DIMENSION PROPERTIES {1} ON {2}",
                AxisParameters,
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

        public MdxAxis With(MdxTuple parameter)
        {
            _axisParameters = parameter;
            return this;
        }

        public MdxAxis Named(string title)
        {
            _title = title;
            return this;
        }

        public MdxAxis NonEmpty()
        {
            _isNonEmpty = true;
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