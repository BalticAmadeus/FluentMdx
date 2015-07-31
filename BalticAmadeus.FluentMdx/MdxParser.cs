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

            } while (IsNextTokenValid(enumerator, TokenType.Comma));

            if (!IsNextTokenValid(enumerator, TokenType.RightCurlyBracket))
                return false;

            if (members.Any() && sets.Any())
                return false;

            if (!members.Any() && !sets.Any())
                expression = new MdxTuple();

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

            } while (IsNextTokenValid(enumerator, TokenType.Comma));

            if (!IsNextTokenValid(enumerator, TokenType.RightRoundBracket))
                return false;

            if (members.Any() && tuples.Any())
                return false;

            if (!members.Any() && !tuples.Any())
                expression = new MdxSet();

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

            if (!IsNextTokenValid(enumerator, TokenType.LeftSquareBracket))
                return false;

            if (!IsNextTokenValid(enumerator, TokenType.IdentifierExpression))
                return false;

            identifiers.Add(enumerator.Current.Value);

            if (!IsNextTokenValid(enumerator, TokenType.RightSquareBracket))
                return false;

            var appliedFunctions = new List<MdxNavigationFunction>();
            while (IsNextTokenValid(enumerator, TokenType.IdentifierSeparator))
            {
                IMdxExpression function;
                if (TryParseNavigationFunction(enumerator, out function))
                {
                    appliedFunctions.Add((MdxNavigationFunction)function);
                    continue;
                }

                if (!IsNextTokenValid(enumerator, TokenType.LeftSquareBracket))
                    return false;

                if (!IsNextTokenValid(enumerator, TokenType.IdentifierExpression))
                    return false;

                identifiers.Add(enumerator.Current.Value);

                if (!IsNextTokenValid(enumerator, TokenType.RightSquareBracket))
                    return false;
            } 

            if (!IsNextTokenValid(enumerator, TokenType.ValueSeparator))
            {
                expression = new MdxMember(identifiers, appliedFunctions);
                return true;
            }

            if (!IsNextTokenValid(enumerator, TokenType.LeftSquareBracket))
                return false;

            if (!IsNextTokenValid(enumerator, TokenType.IdentifierExpression) &&
                !IsNextTokenValid(enumerator, TokenType.NumberExpression) &&
                !IsNextTokenValid(enumerator, TokenType.DateExpression))
                return false;

            var memberValue = enumerator.Current.Value;

            if (IsNextTokenValid(enumerator, TokenType.IdentifierExpression))
                memberValue += enumerator.Current.Value;

            if (!IsNextTokenValid(enumerator, TokenType.RightSquareBracket))
                return false;
            
            while (IsNextTokenValid(enumerator, TokenType.IdentifierSeparator))
            {
                IMdxExpression function;
                if (!TryParseNavigationFunction(enumerator, out function))
                    return false;

                appliedFunctions.Add((MdxNavigationFunction)function);
            }

            if (!IsNextTokenValid(enumerator, TokenType.RangeSeparator))
            {
                expression = new MdxValueMember(identifiers, memberValue, appliedFunctions);
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

            if (!IsNextTokenValid(enumerator, TokenType.IdentifierExpression) &&
                !IsNextTokenValid(enumerator, TokenType.NumberExpression) &&
                !IsNextTokenValid(enumerator, TokenType.DateExpression))
                return false;

            var rangeValue = enumerator.Current.Value;

            if (IsNextTokenValid(enumerator, TokenType.IdentifierExpression))
                rangeValue += enumerator.Current.Value;

            if (!IsNextTokenValid(enumerator, TokenType.RightSquareBracket))
                return false;

            expression = new MdxRangeMember(identifiers, memberValue, rangeValue);
            return true;
        }

        internal static bool TryParseAxis(ITwoWayEnumerator<Token> enumerator, out IMdxExpression expression)
        {
            expression = null;

            var axis = new MdxAxis();

            if (IsNextTokenValid(enumerator, TokenType.Non))
            {
                if (!IsNextTokenValid(enumerator, TokenType.Empty))
                    return false;

                axis.NonEmpty();
            }

            IMdxExpression axisParameter;
            if (TryParseTuple(enumerator, out axisParameter))
            {
                axis.With((MdxTuple)axisParameter);
            } 
            else if (TryParseSet(enumerator, out axisParameter))
            {
                axis.With(new MdxSetTuple().With((MdxSet)axisParameter));
            } 
            else if (TryParseMember(enumerator, out axisParameter))
            {
                axis.With(new MdxMemberTuple().With((MdxMember)axisParameter));
            }
            else
            {
                return false;
            }

            if (IsNextTokenValid(enumerator, TokenType.Dimension))
            {
                if (!IsNextTokenValid(enumerator, TokenType.Properties))
                    return false;

                do
                {
                    if (!IsNextTokenValid(enumerator, TokenType.DimensionProperty))
                        return false;

                    axis.WithProperties(enumerator.Current.Value);
                } while (IsNextTokenValid(enumerator, TokenType.Comma));
            }
            else if (IsNextTokenValid(enumerator, TokenType.Properties))
            {
                do
                {
                    if (!IsNextTokenValid(enumerator, TokenType.DimensionProperty))
                        return false;

                    axis.WithProperties(enumerator.Current.Value);
                } while (IsNextTokenValid(enumerator, TokenType.Comma));
            }

            if (!IsNextTokenValid(enumerator, TokenType.On))
                return false;

            if (!IsNextTokenValid(enumerator, TokenType.AxisName) &&
                !IsNextTokenValid(enumerator, TokenType.NumberExpression))
                return false;

            string axisName = enumerator.Current.Value;
            axis.Named(axisName);

            expression = axis;
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

        internal static bool TryParseNavigationFunction(ITwoWayEnumerator<Token> enumerator, out IMdxExpression expression)
        {
            expression = null;

            if (!IsNextTokenValid(enumerator, TokenType.IdentifierExpression) &&
                !IsNextTokenValid(enumerator, TokenType.DimensionProperty))
                return false;

            var functionTitle = enumerator.Current.Value;

            if (!IsNextTokenValid(enumerator, TokenType.LeftRoundBracket))
            {
                expression = new MdxNavigationFunction(functionTitle);
                return true;
            }
            
            var functionParameters = new List<string>();
            do
            {
                if (!IsNextTokenValid(enumerator, TokenType.IdentifierExpression) &&
                    !IsNextTokenValid(enumerator, TokenType.NumberExpression) && 
                    !IsNextTokenValid(enumerator, TokenType.DateExpression))
                    return false;

                functionParameters.Add(enumerator.Current.Value);

            } while (IsNextTokenValid(enumerator, TokenType.Comma));

            if (!IsNextTokenValid(enumerator, TokenType.RightRoundBracket))
                return false;

            expression = new MdxNavigationFunction(functionTitle, functionParameters);
            return true;
        }

        internal static bool TryParseFunction(ITwoWayEnumerator<Token> enumerator, out IMdxExpression expression)
        {
            expression = null;

            var function = new MdxFunction();
            do
            {
                if (!IsNextTokenValid(enumerator, TokenType.IdentifierExpression))
                    return false;

                function.Titled(enumerator.Current.Value);

            } while (IsNextTokenValid(enumerator, TokenType.IdentifierSeparator));

            if (!IsNextTokenValid(enumerator, TokenType.LeftRoundBracket))
            {
                enumerator.MovePrevious();
                return false;
            }

            do
            {
                IMdxExpression childExpression;
                if (TryParseExpression(enumerator, out childExpression))
                {
                    function.WithParameter((MdxExpression)childExpression);
                    continue;
                }
                
            } while (IsNextTokenValid(enumerator, TokenType.Comma));

            if (!IsNextTokenValid(enumerator, TokenType.RightRoundBracket))
                return false;

            expression = function;
            return true;
        }

        internal static bool TryParseExpression(ITwoWayEnumerator<Token> enumerator, out IMdxExpression expression)
        {
            expression = null;

            var mdxExpression = new MdxExpression();

            do
            {
                IMdxExpression childExpression;
                if (TryParseFunction(enumerator, out childExpression) ||
                    TryParseTuple(enumerator, out childExpression) ||
                    TryParseSet(enumerator, out childExpression) ||
                    TryParseMember(enumerator, out childExpression))
                {
                    mdxExpression.WithOperand(childExpression.ToString());
                }
                else if (IsNextTokenValid(enumerator, TokenType.IdentifierExpression) ||
                         IsNextTokenValid(enumerator, TokenType.DateExpression) ||
                         IsNextTokenValid(enumerator, TokenType.NumberExpression))
                {
                    mdxExpression.WithOperand(enumerator.Current.Value);
                }
                else
                {
                    return false;
                }
                
                if (!IsNextTokenValid(enumerator, TokenType.OperationExpression))
                    break;

                mdxExpression.WithOperation(enumerator.Current.Value);

            } while (true);

            expression = mdxExpression;
            return true;
        }
    }
}