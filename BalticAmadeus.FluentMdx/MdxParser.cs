using System;
using System.Collections.Generic;
using BalticAmadeus.FluentMdx.EnumerableExtensions;
using BalticAmadeus.FluentMdx.Lexer;

namespace BalticAmadeus.FluentMdx
{
    public class MdxParser : IMdxParser
    {
        private readonly ILexer _lexer;

        public MdxParser()
        {
            _lexer = new Lexer.Lexer();
        }

        public MdxQuery ParseQuery(string source)
        {
            string trimmedSource = source.Trim();

            var tokens = _lexer.Tokenize(trimmedSource);

            var enumerator = tokens.GetTwoWayEnumerator();
            
            MdxExpressionBase expression;
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

        private static bool TryParseQuery(ITwoWayEnumerator<Token> enumerator, out MdxExpressionBase expression)
        {
            expression = null;

            if (!IsNextTokenValid(enumerator, TokenType.Select))
                return false;

            var query = Mdx.Query();

            do
            {
                MdxExpressionBase childExpression;
                if (!TryParseAxis(enumerator, out childExpression))
                    return false;

                query.On((MdxAxis)childExpression);
            } while (IsNextTokenValid(enumerator, TokenType.Comma));

            if (!IsNextTokenValid(enumerator, TokenType.From))
                return false;

            if (IsNextTokenValid(enumerator, TokenType.LeftRoundBracket))
            {
                MdxExpressionBase innerQuery;
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
                    MdxExpressionBase childExpression;
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

            MdxExpressionBase slicer;
            if (TryParseMember(enumerator, out slicer))
            {
                if (slicer is MdxMember)
                {
                    var memberTuple = Mdx.Tuple().With((MdxMember) slicer);
                    query.Where(memberTuple);
                }
                else
                {
                    var memberTuple = Mdx.Tuple().With((MdxRange)slicer);
                    query.Where(memberTuple);
                }
            }
            else if (TryParseSet(enumerator, out slicer))
            {
                var setTuple = Mdx.Tuple().With((MdxSet)slicer);
                query.Where(setTuple);
            }
            else if (TryParseTuple(enumerator, out slicer))
            {
                query.Where((MdxTuple)slicer);
            }
            else if (TryParseFunction(enumerator, out slicer))
            {
                query.Where((MdxTuple) slicer);

            }
            else
            {
                return false;
            }

            expression = query;
            return true;
        }

        private static bool TryParseTuple(ITwoWayEnumerator<Token> enumerator, out MdxExpressionBase expression)
        {
            expression = null;

            if (!IsNextTokenValid(enumerator, TokenType.LeftCurlyBracket))
                return false;

            var tuple = Mdx.Tuple();

            if (IsNextTokenValid(enumerator, TokenType.RightCurlyBracket))
            {
                expression = tuple;

                return true;
            }

            do
            {
                MdxExpressionBase childExpression;
                if (TryParseMember(enumerator, out childExpression))
                {
                    if (childExpression is MdxMember)
                    {
                        tuple.With((MdxMember)childExpression);
                    }
                    else
                    {
                        tuple.With((MdxRange)childExpression);
                    }
                } 
                else if (TryParseSet(enumerator, out childExpression))
                {
                    tuple.With((MdxSet)childExpression);
                }
                else if (TryParseFunction(enumerator, out childExpression))
                {
                    tuple.With((MdxFunction) childExpression);
                }
                else
                {
                    return false;
                }

            } while (IsNextTokenValid(enumerator, TokenType.Comma));

            if (!IsNextTokenValid(enumerator, TokenType.RightCurlyBracket))
                return false;

            expression = tuple;

            return true;
        }

        private static bool TryParseSet(ITwoWayEnumerator<Token> enumerator, out MdxExpressionBase expression)
        {
            expression = null;

            if (!IsNextTokenValid(enumerator, TokenType.LeftRoundBracket))
                return false;

            var set = Mdx.Set();

            if (IsNextTokenValid(enumerator, TokenType.RightRoundBracket))
            {
                expression = set;

                return true;
            }

            do
            {
                MdxExpressionBase childExpression;
                if (TryParseMember(enumerator, out childExpression))
                {
                    if (childExpression is MdxMember)
                    {
                        set.With((MdxMember)childExpression);
                    }
                    else
                    {
                        set.With((MdxRange)childExpression);
                    }
                }
                else if (TryParseTuple(enumerator, out childExpression))
                {
                    set.With((MdxTuple)childExpression);
                }
                else if (TryParseFunction(enumerator, out childExpression))
                {
                    set.With((MdxFunction)childExpression);
                }
                else
                {
                    return false;
                }

            } while (IsNextTokenValid(enumerator, TokenType.Comma));

            if (!IsNextTokenValid(enumerator, TokenType.RightRoundBracket))
                return false;

            expression = set;

            return true;
        }

        internal static bool TryParseMember(ITwoWayEnumerator<Token> enumerator, out MdxExpressionBase expression)
        {
            expression = null;

            if (!IsNextTokenValid(enumerator, TokenType.LeftSquareBracket))
                return false;

            var member = Mdx.Member();

            if (!IsNextTokenValid(enumerator, TokenType.IdentifierExpression))
                return false;

            member.Titled(enumerator.Current.Value);

            if (!IsNextTokenValid(enumerator, TokenType.RightSquareBracket))
                return false;

            while (IsNextTokenValid(enumerator, TokenType.IdentifierSeparator))
            {
                MdxExpressionBase function;
                if (TryParseNavigationFunction(enumerator, out function))
                {
                    member.WithFunction((MdxNavigationFunction) function);
                    continue;
                }

                if (!IsNextTokenValid(enumerator, TokenType.LeftSquareBracket))
                    return false;

                if (!IsNextTokenValid(enumerator, TokenType.IdentifierExpression))
                    return false;

                member.Titled(enumerator.Current.Value);

                if (!IsNextTokenValid(enumerator, TokenType.RightSquareBracket))
                    return false;
            }

            if (!IsNextTokenValid(enumerator, TokenType.ValueSeparator))
            {
                expression = member;

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

            member.WithValue(memberValue);

            if (!IsNextTokenValid(enumerator, TokenType.RightSquareBracket))
                return false;

            while (IsNextTokenValid(enumerator, TokenType.IdentifierSeparator))
            {
                MdxExpressionBase function;
                if (!TryParseNavigationFunction(enumerator, out function))
                    return false;

                member.WithFunction((MdxNavigationFunction) function);
            }


            if (!IsNextTokenValid(enumerator, TokenType.RangeSeparator))
            {
                expression = member;
                return true;
            }

            var toMember = Mdx.Member();
            do
            {
                if (!IsNextTokenValid(enumerator, TokenType.LeftSquareBracket))
                    return false;

                if (!IsNextTokenValid(enumerator, TokenType.IdentifierExpression))
                    return false;

                toMember.Titled(enumerator.Current.Value);

                if (!IsNextTokenValid(enumerator, TokenType.RightSquareBracket))
                    return false;

            } while (IsNextTokenValid(enumerator, TokenType.IdentifierSeparator));

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

            toMember.WithValue(rangeValue);

            if (!IsNextTokenValid(enumerator, TokenType.RightSquareBracket))
                return false;

            expression = Mdx.Range().From(member).To(toMember);
            return true;
        }

        internal static bool TryParseAxis(ITwoWayEnumerator<Token> enumerator, out MdxExpressionBase expression)
        {
            expression = null;

            var axis = Mdx.Axis();

            if (IsNextTokenValid(enumerator, TokenType.Non))
            {
                if (!IsNextTokenValid(enumerator, TokenType.Empty))
                    return false;

                axis.NonEmpty();
            }

            MdxExpressionBase axisParameter;
            if (TryParseTuple(enumerator, out axisParameter))
            {
                axis.With((MdxTuple)axisParameter);
            } 
            else if (TryParseSet(enumerator, out axisParameter))
            {
                axis.With(Mdx.Tuple().With((MdxSet)axisParameter));
            } 
            else if (TryParseMember(enumerator, out axisParameter))
            {
                if (axisParameter is MdxMember)
                {
                    axis.With(Mdx.Tuple().With((MdxMember)axisParameter));
                }
                else
                {
                    axis.With(Mdx.Tuple().With((MdxRange)axisParameter));
                }
            }
            else if (TryParseFunction(enumerator, out axisParameter))
            {
                axis.With(Mdx.Tuple().With((MdxMember)axisParameter));
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
            axis.Titled(axisName);

            expression = axis;
            return true;
        }

        internal static bool TryParseCube(ITwoWayEnumerator<Token> enumerator, out MdxExpressionBase expression)
        {
            expression = null;

            var cube = Mdx.Cube();

            do
            {
                if (!IsNextTokenValid(enumerator, TokenType.LeftSquareBracket))
                    return false;

                if (!IsNextTokenValid(enumerator, TokenType.IdentifierExpression))
                    return false;

                cube.Titled(enumerator.Current.Value);

                if (!IsNextTokenValid(enumerator, TokenType.RightSquareBracket))
                    return false;
                
            } while (IsNextTokenValid(enumerator, TokenType.IdentifierSeparator));

            expression = cube;
            return true;
        }

        internal static bool TryParseNavigationFunction(ITwoWayEnumerator<Token> enumerator, out MdxExpressionBase expression)
        {
            expression = null;

            if (!IsNextTokenValid(enumerator, TokenType.IdentifierExpression) &&
                !IsNextTokenValid(enumerator, TokenType.DimensionProperty))
                return false;

            var functionTitle = enumerator.Current.Value;

            if (!IsNextTokenValid(enumerator, TokenType.LeftRoundBracket))
            {
                expression = Mdx.NavigationFunction().Titled(functionTitle);
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

            expression = Mdx.NavigationFunction().Titled(functionTitle).WithParameters(functionParameters.ToArray());
            return true;
        }

        internal static bool TryParseFunction(ITwoWayEnumerator<Token> enumerator, out MdxExpressionBase expression)
        {
            expression = null;

            var function = Mdx.Function();
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
                MdxExpressionBase childExpression;
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

        internal static bool TryParseExpression(ITwoWayEnumerator<Token> enumerator, out MdxExpressionBase expression)
        {
            expression = null;

            var mdxExpression = Mdx.Expression();

            do
            {
                MdxExpressionBase childExpression;
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