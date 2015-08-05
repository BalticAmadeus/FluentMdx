using System.Collections.Generic;
using System.Linq;

namespace BalticAmadeus.FluentMdx
{
    public class MdxAxis : MdxExpressionBase
    {
        private readonly IList<string> _properties;

        public MdxAxis()
        {
            _properties = new List<string>();

            AxisSlicer = null;
            Title = null;
            IsNonEmpty = false;
        }

        public string Title { get; private set; }
        public IMdxMember AxisSlicer { get; private set; }
        public bool IsNonEmpty { get; private set; }

        public IEnumerable<string> Properties
        {
            get { return _properties; }
        }


        protected override string GetStringExpression()
        {
            if (!IsNonEmpty)
            {
                if (!Properties.Any())
                    return string.Format(@"{0} ON {1}",
                        AxisSlicer,
                        Title);

                return string.Format(@"{0} DIMENSION PROPERTIES {1} ON {2}",
                    AxisSlicer,
                    string.Join(", ", Properties),
                    Title);
            }

            if (!Properties.Any())
                return string.Format(@"NON EMPTY {0} ON {1}",
                    AxisSlicer, 
                    Title);

            return string.Format(@"NON EMPTY {0} DIMENSION PROPERTIES {1} ON {2}",
                AxisSlicer,
                    string.Join(", ", Properties),
                    Title);
        }

        public MdxAxis WithSlicer(MdxTuple parameter)
        {
            AxisSlicer = parameter;
            return this;
        }

        public MdxAxis Titled(string title)
        {
            Title = title;
            return this;
        }

        public MdxAxis AsNonEmpty()
        {
            IsNonEmpty = true;
            return this;
        }

        public MdxAxis AsEmpty()
        {
            IsNonEmpty = false;
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