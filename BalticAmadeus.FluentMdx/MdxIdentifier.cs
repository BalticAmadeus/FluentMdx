using System.Collections.Generic;
using System.Linq;

namespace BalticAmadeus.FluentMdx
{
    public class MdxIdentifier : IMdxExpression
    {
        private readonly IList<string> _identifiers;
        private readonly IList<MdxNavigationFunction> _appliedFunctions;

        public MdxIdentifier(params string[] titles) 
            : this(new List<string>(titles), new List<MdxNavigationFunction>()) { }

        internal MdxIdentifier(IList<string> identifiers, IList<MdxNavigationFunction> appliedFunctions)
        {
            _identifiers = identifiers;
            _appliedFunctions = appliedFunctions;
        }

        public IEnumerable<string> Identifiers
        {
            get { return _identifiers; }
        }

        internal IList<string> IdentifiersInternal
        {
            get { return _identifiers; }
        }

        public IEnumerable<MdxNavigationFunction> AppliedFunctions
        {
            get { return _appliedFunctions; }
        }

        internal IList<MdxNavigationFunction> AppliedFunctionsInternal
        {
            get { return _appliedFunctions; }
        }

        public virtual string GetStringExpression()
        {
            if (!AppliedFunctions.Any())
                return string.Format("[{0}]", string.Join("].[", Identifiers));

            return 
                string.Format("[{0}].{1}", string.Join("].[", Identifiers), 
                string.Join(".", AppliedFunctions));
        }

        public override string ToString()
        {
            return GetStringExpression();
        }

        public override int GetHashCode()
        {
            return 13 + Identifiers.Sum(identifier => identifier.GetHashCode()*17);
        }
    }

    public static class MdxIdentifierExtensions
    {
        public static T AppendNames<T>(this T identifier, params string[] nameParts) where T : MdxIdentifier
        {
            foreach (var namePart in nameParts)
                identifier.IdentifiersInternal.Add(namePart);               
            
            return identifier;
        }

        public static T ApplyFunction<T>(this T identifier, MdxNavigationFunction function) where T : MdxIdentifier
        {
            identifier.AppliedFunctionsInternal.Add(function);
            return identifier;
        }
    }
}
