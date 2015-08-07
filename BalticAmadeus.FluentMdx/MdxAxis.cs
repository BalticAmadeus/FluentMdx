using System.Collections.Generic;
using System.Linq;

namespace BalticAmadeus.FluentMdx
{
    /// <summary>
    /// Represents Mdx query axis specification.
    /// </summary>
    public sealed class MdxAxis : MdxExpressionBase
    {
        private readonly IList<string> _properties;

        /// <summary>
        /// Initializes a new instance of <see cref="MdxAxis"/>.
        /// </summary>
        public MdxAxis()
        {
            _properties = new List<string>();

            AxisSlicer = null;
            Title = null;
            IsNonEmpty = false;
        }

        /// <summary>
        /// Gets the axis title.
        /// </summary>
        public string Title { get; private set; }

        /// <summary>
        /// Gets the axis slicer.
        /// </summary>
        public MdxTuple AxisSlicer { get; private set; }

        /// <summary>
        /// Gets the value if the axis is specified as non-empty.
        /// </summary>
        public bool IsNonEmpty { get; private set; }

        /// <summary>
        /// Gets the collection of axis properties.
        /// </summary>
        public IEnumerable<string> Properties
        {
            get { return _properties; }
        }

        /// <summary>
        /// Sets the slicer for axis and returns the updated current instance of <see cref="MdxAxis"/>.
        /// </summary>
        /// <param name="slicer">Axis slicer.</param>
        /// <returns>Returns the updated current instance of <see cref="MdxAxis"/>.</returns>
        public MdxAxis WithSlicer(MdxTuple slicer)
        {
            AxisSlicer = slicer;
            return this;
        }

        /// <summary>
        /// Sets the title for axis and returns the updated current instance of <see cref="MdxAxis"/>.
        /// </summary>
        /// <param name="title">Axis title.</param>
        /// <returns>Returns the updated current instance of <see cref="MdxAxis"/>.</returns>
        public MdxAxis Titled(string title)
        {
            Title = title;
            return this;
        }

        /// <summary>
        /// Marks axis as non-empty and returns the updated current instance of <see cref="MdxAxis"/>.
        /// </summary>
        /// <returns>Returns the updated current instance of <see cref="MdxAxis"/>.</returns>
        public MdxAxis AsNonEmpty()
        {
            IsNonEmpty = true;
            return this;
        }

        /// <summary>
        /// Marks axis as empty and returns the updated current instance of <see cref="MdxAxis"/>.
        /// </summary>
        /// <returns>Returns the updated current instance of <see cref="MdxAxis"/>.</returns>
        public MdxAxis AsEmpty()
        {
            IsNonEmpty = false;
            return this;
        }

        /// <summary>
        /// Applies axis properties and returns the updated current instance of <see cref="MdxAxis"/>.
        /// </summary>
        /// <param name="properties">Collection of axis properties.</param>
        /// <returns>Returns the updated current instance of <see cref="MdxAxis"/>.</returns>
        public MdxAxis WithProperties(params string[] properties)
        {
            foreach (var property in properties)
                _properties.Add(property);
            return this;
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
    }
}