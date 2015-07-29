using System;
using System.Collections.Generic;
using System.Linq;
using BalticAmadeus.FluentMdx.EnumerableExtensions;
using BalticAmadeus.FluentMdx.Lexer;

namespace BalticAmadeus.FluentMdx
{
    public class MdxParser : IMdxParser
    {
        private readonly ILexer _lexer;

        public MdxParser() : this(new Lexer.Lexer()) { }

        internal MdxParser(ILexer lexer)
        {
            _lexer = lexer;
        }

        public MdxQuery ParseQuery(string source)
        {
            string trimmedSource = source.Trim();

            var tokens = _lexer.Tokenize(trimmedSource);

            var enumerator = tokens.GetTwoWayEnumerator();
            
            IMdxExpression expression;
            if (!TryParseQuery(enumerator, out expression))
            {
                var tokensLeft = new List<Token>();
                while (enumerator.MoveNext())
                    tokensLeft.Add(enumerator.Current);

                throw new ArgumentException(string.Format("Cannot parse the expression. There are no such rules. {0}.", string.Join(", ", tokensLeft)));
            }

            if (!IsNextTokenValid(enumerator, TokenType.LastToken))
            {
                var tokensLeft = new List<Token>();
                while (enumerator.MoveNext())
                    tokensLeft.Add(enumerator.Current);

                throw new ArgumentException(string.Format("There are tokens left in expression. {0}.", string.Join(", ", tokensLeft)));
            }

            return (MdxQuery) expression;
        }

        private static bool IsNextTokenValid(ITwoWayEnumerator<Token> enumerator, TokenType expectedTokenType)
        {
            if (!enumerator.MoveNext())
                return false;

            if (enumerator.Current.Type == expectedTokenType)
                return true;

            enumerator.MovePrevious();
            return false;
        }

        private static bool TryParseQuery(ITwoWayEnumerator<Token> enumerator, out IMdxExpression expression)
        {
            expression = null;

            if (!IsNextTokenValid(enumerator, TokenType.Select))
                return false;

            var query = new MdxQuery();

            do
            {
                IMdxExpression childExpression;
                if (!TryParseAxis(enumerator, out childExpression))
                    return false;

                query.On((MdxAxis)childExpression);
            } while (IsNextTokenValid(enumerator, TokenType.Comma));

            if (!IsNextTokenValid(enumerator, TokenType.From))
                return false;

            if (IsNextTokenValid(enumerator, TokenType.LeftRoundBracket))
            {
                IMdxExpression innerQuery;
                if (!TryParseQuery(enumerator, out innerQuery))
                    return false;

                query.From((MdxQuery) innerQuery);

                if (!IsNextTokenValid(enumerator, TokenType.RightRoundBracket))
                    return false;
            }
            else
            {
                do
                {
                    IMdxExpression childExpression;
                    if (!TryParseCube(enumerator, out childExpression))
                        return false;

                    query.From((MdxCube)childExpression);
                } while (IsNextTokenValid(enumerator, TokenType.Comma));
            }

            if (!IsNextTokenValid(enumerator, TokenType.Where))
            {
                expression = query;
                return true;
            }

            IMdxExpression member;
            if (TryParseMember(enumerator, out member))
            {
                var memberTuple = new MdxMemberTuple().With((MdxMember) member);
                query.Where(memberTuple);

                expression = query;
                return true;
            }

            IMdxExpression set;
            if (TryParseSet(enumerator, out set))
            {
                var setTuple = new MdxSetTuple().With((MdxSet)set);
                query.Where(setTuple);

                expression = query;
                return true;
            }

            IMdxExpression tuple;
            if (TryParseTuple(enumerator, out tuple))
            {
                query.Where((MdxTuple)tuple);

                expression = query;
                return true;
            }
           
            return false;
        }

        private static bool TryParseTuple(ITwoWayEnumerator<Token> enumerator, out IMdxExpression expression)
        {
            expression = null;

            if (!IsNextTokenValid(enumerator, TokenType.LeftCurlyBracket))
                return false;

            var members = new List<MdxMember>();
            var sets = new List<MdxSet>();
            do
            {
                IMdxExpression childExpression;
                if (TryParseMember(enumerator, out childExpression))
                {
                    members.Add((MdxMember)childExpression);
                    continue;
                }

                if (TryParseSet(enumerator, out childExpression))
                {
                    sets.Add((MdxSet)childExpression);
                    continue;
                }

                return false;

            } while (IsNextTokenValid(enumerator, TokenType.Comma));

            if (!IsNextTokenValid(enumerator, TokenType.RightCurlyBracket))
                return false;

            if (members.Any() && sets.Any())
                return false;

            if (!members.Any() && !sets.Any())
                return false;

            if (members.Any())
                expression = new MdxMemberTuple(members);

            if (sets.Any())
                expression = new MdxSetTuple(sets);

            return true;
        }

        private static bool TryParseSet(ITwoWayEnumerator<Token> enumerator, out IMdxExpression expression)
        {
            expression = null;

            if (!IsNextTokenValid(enumerator, TokenType.LeftRoundBracket))
                return false;

            var members = new List<MdxMember>();
            var tuples = new List<MdxTuple>();
            do
            {
                IMdxExpression childExpression;
                if (TryParseMember(enumerator, out childExpression))
                {
                    members.Add((MdxMember)childExpression);   
                    continue;
                }

                if (TryParseTuple(enumerator, out childExpression))
                {
                    tuples.Add((MdxTuple)childExpression);
                    continue;                    
                }

                return false;

            } while (IsNextTokenValid(enumerator, TokenType.Comma));

            if (!IsNextTokenValid(enumerator, TokenType.RightRoundBracket))
                return false;

            if (members.Any() && tuples.Any())
                return false;

            if (!members.Any() && !tuples.Any())
                return false;

            if (members.Any())
                expression = new MdxMemberSet(members);

            if (tuples.Any())
                expression = new MdxTupleSet(tuples);

            return true;
        }

        internal static bool TryParseMember(ITwoWayEnumerator<Token> enumerator, out IMdxExpression expression)
        {
            expression = null;

            var identifiers = new List<string>();
            var appliedFunctions = new List<MdxFunction>();
            do
            {
                IMdxExpression function;
                if (TryParseFunction(enumerator, out function))
                {
                    appliedFunctions.Add((MdxFunction)function);
                    continue;
                }

                if (!IsNextTokenValid(enumerator, TokenType.LeftSquareBracket))
                    return false;

                if (!IsNextTokenValid(enumerator, TokenType.IdentifierExpression))
                    return false;

                identifiers.Add(enumerator.Current.Value);

                if (!IsNextTokenValid(enumerator, TokenType.RightSquareBracket))
                    return false;

            } while (IsNextTokenValid(enumerator, TokenType.IdentifierSeparator));

            if (!IsNextTokenValid(enumerator, TokenType.ValueSeparator))
            {
                expression = new MdxMember(identifiers, appliedFunctions);
                return true;
            }

            if (!IsNextTokenValid(enumerator, TokenType.LeftSquareBracket))
                return false;

            if (!IsNextTokenValid(enumerator, TokenType.IdentifierExpression))
                return false;

            var memberValue = enumerator.Current.Value;

            if (!IsNextTokenValid(enumerator, TokenType.RightSquareBracket))
                return false;

            if (!IsNextTokenValid(enumerator, TokenType.RangeSeparator))
            {
                expression = new MdxValueMember(identifiers, memberValue);
                return true;
            }

            var otherIdentifiers = new List<string>();
            do
            {
                if (!IsNextTokenValid(enumerator, TokenType.LeftSquareBracket))
                    return false;

                if (!IsNextTokenValid(enumerator, TokenType.IdentifierExpression))
                    return false;

                otherIdentifiers.Add(enumerator.Current.Value);

                if (!IsNextTokenValid(enumerator, TokenType.RightSquareBracket))
                    return false;

            } while (IsNextTokenValid(enumerator, TokenType.IdentifierSeparator));

            if (identifiers.Where((t, i) => t != otherIdentifiers[i]).Any())
                throw new ArgumentException("Identifiers for range members must match!");

            if (!IsNextTokenValid(enumerator, TokenType.ValueSeparator))
                return false;

            if (!IsNextTokenValid(enumerator, TokenType.LeftSquareBracket))
                return false;

            if (!IsNextTokenValid(enumerator, TokenType.IdentifierExpression))
                return false;

            var rangeValue = enumerator.Current.Value;

            if (!IsNextTokenValid(enumerator, TokenType.RightSquareBracket))
                return false;

            expression = new MdxRangeMember(identifiers, memberValue, rangeValue);
            return true;
        }

        internal static bool TryParseAxis(ITwoWayEnumerator<Token> enumerator, out IMdxExpression expression)
        {
            expression = null;

            if (!IsNextTokenValid(enumerator, TokenType.Non))
                return false;
            
            if (!IsNextTokenValid(enumerator, TokenType.Empty))
                return false;
            
            if (!IsNextTokenValid(enumerator, TokenType.LeftCurlyBracket))
                return false;

            var axisParameters = new List<MdxMember>();
            IMdxExpression axisParameter;
            while (TryParseMember(enumerator, out axisParameter))
            {
                axisParameters.Add((MdxMember)axisParameter);

                if (!IsNextTokenValid(enumerator, TokenType.Comma))
                    break;
            }

            if (!IsNextTokenValid(enumerator, TokenType.RightCurlyBracket))
                return false;

            var axisProperties = new List<string>();
            if (IsNextTokenValid(enumerator, TokenType.Dimension))
            {
                if (!IsNextTokenValid(enumerator, TokenType.Properties))
                    return false;

                do
                {
                    if (!IsNextTokenValid(enumerator, TokenType.DimensionProperty))
                        return false;

                    axisProperties.Add(enumerator.Current.Value);
                } while (IsNextTokenValid(enumerator, TokenType.Comma));
            }
            else if (IsNextTokenValid(enumerator, TokenType.Properties))
            {
                do
                {
                    if (!IsNextTokenValid(enumerator, TokenType.DimensionProperty))
                        return false;

                    axisProperties.Add(enumerator.Current.Value);
                } while (IsNextTokenValid(enumerator, TokenType.Comma));
            }

            if (!IsNextTokenValid(enumerator, TokenType.On))
                return false;

            if (!IsNextTokenValid(enumerator, TokenType.AxisName) &&
                !IsNextTokenValid(enumerator, TokenType.IdentifierExpression))
                return false;

            string axisName = enumerator.Current.Value;

            expression = new MdxAxis(axisName, axisParameters, axisProperties);
            return true;
        }

        internal static bool TryParseCube(ITwoWayEnumerator<Token> enumerator, out IMdxExpression expression)
        {
            expression = null;

            var identifiers = new List<string>();
            do
            {
                if (!IsNextTokenValid(enumerator, TokenType.LeftSquareBracket))
                    return false;

                if (!IsNextTokenValid(enumerator, TokenType.IdentifierExpression))
                    return false;

                identifiers.Add(enumerator.Current.Value);

                if (!IsNextTokenValid(enumerator, TokenType.RightSquareBracket))
                    return false;
                
            } while (IsNextTokenValid(enumerator, TokenType.IdentifierSeparator));

            expression = new MdxCube(identifiers);
            return true;
        }

        internal static bool TryParseIdentifier(ITwoWayEnumerator<Token> enumerator, out IMdxExpression expression)
        {
            expression = null;

            var identifiers = new List<string>();
            var appliedFunctions = new List<MdxFunction>();
            do
            {
                IMdxExpression function;
                if (TryParseFunction(enumerator, out function))
                {
                    appliedFunctions.Add((MdxFunction) function);
                    continue;
                }
                
                if (!IsNextTokenValid(enumerator, TokenType.LeftSquareBracket))
                    return false;

                if (!IsNextTokenValid(enumerator, TokenType.IdentifierExpression))
                    return false;

                identifiers.Add(enumerator.Current.Value);

                if (!IsNextTokenValid(enumerator, TokenType.RightSquareBracket))
                    return false;

            } while (IsNextTokenValid(enumerator, TokenType.IdentifierSeparator));

            expression = new MdxIdentifier(identifiers, appliedFunctions);
            return true;
        }

        internal static bool TryParseFunction(ITwoWayEnumerator<Token> enumerator, out IMdxExpression expression)
        {
            expression = null;

            if (!IsNextTokenValid(enumerator, TokenType.IdentifierExpression))
                return false;

            var functionTitle = enumerator.Current.Value;

            if (!IsNextTokenValid(enumerator, TokenType.LeftRoundBracket))
            {
                expression = new MdxFunction(functionTitle);
                return true;
            }


            var functionParameters = new List<string>();
            do
            {
                if (!IsNextTokenValid(enumerator, TokenType.IdentifierExpression))
                    return false;

                functionParameters.Add(enumerator.Current.Value);

            } while (IsNextTokenValid(enumerator, TokenType.Comma));

            if (!IsNextTokenValid(enumerator, TokenType.RightRoundBracket))
                return false;

            expression = new MdxFunction(functionTitle, functionParameters);
            return true;
        }
    }
}