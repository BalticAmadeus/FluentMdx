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

            var enumerator = tokens.GetStatedTwoWayEnumerator();
            
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

        private static bool IsNextTokenValid(IStatedTwoWayEnumerator<Token> enumerator, TokenType expectedTokenType)
        {
            if (!enumerator.MoveNext())
                return false;

            if (enumerator.Current.Type == expectedTokenType)
                return true;

            enumerator.MovePrevious();
            return false;
        }
        
        private static bool TryParseQuery(IStatedTwoWayEnumerator<Token> enumerator, out MdxExpressionBase expression)
        {
            enumerator.BackupState();
            expression = null;

            if (!IsNextTokenValid(enumerator, TokenType.Select))
            {
                enumerator.RestoreState();
                return false;
            }

            var query = Mdx.Query();

            do
            {
                MdxExpressionBase childExpression;
                if (!TryParseAxis(enumerator, out childExpression))
                {
                    enumerator.RestoreState();
                    return false;
                }

                query.On((MdxAxis)childExpression);
            } while (IsNextTokenValid(enumerator, TokenType.MemberSeparator));

            if (!IsNextTokenValid(enumerator, TokenType.From))
            {
                enumerator.RestoreState();
                return false;
            }

            if (IsNextTokenValid(enumerator, TokenType.LeftRoundBracket))
            {
                MdxExpressionBase innerQuery;
                if (!TryParseQuery(enumerator, out innerQuery))
                {
                    enumerator.RestoreState();
                    return false;
                }

                query.From((MdxQuery) innerQuery);

                if (!IsNextTokenValid(enumerator, TokenType.RightRoundBracket))
                {
                    enumerator.RestoreState();
                    return false;
                }
            }
            else
            {
                do
                {
                    MdxExpressionBase childExpression;
                    if (!TryParseCube(enumerator, out childExpression))
                    {
                        enumerator.RestoreState();
                        return false;
                    }

                    query.From((MdxCube)childExpression);
                } while (IsNextTokenValid(enumerator, TokenType.MemberSeparator));
            }

            if (!IsNextTokenValid(enumerator, TokenType.Where))
            {
                expression = query;
                enumerator.MergeState();
                return true;
            }

            MdxExpressionBase slicer;
            if (TryParseRange(enumerator, out slicer))
            {
                query.Where(Mdx.Tuple().With((MdxRange)slicer));
            }
            else if (TryParseMember(enumerator, out slicer))
            {
                query.Where(Mdx.Tuple().With((MdxMember)slicer));
            }
            else if (TryParseSet(enumerator, out slicer))
            {
                query.Where(Mdx.Tuple().With((MdxSet)slicer));
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
                enumerator.RestoreState();
                return false;
            }

            expression = query;
            enumerator.MergeState();
            return true;
        }

        private static bool TryParseTuple(IStatedTwoWayEnumerator<Token> enumerator, out MdxExpressionBase expression)
        {
            enumerator.BackupState();
            expression = null;

            if (!IsNextTokenValid(enumerator, TokenType.LeftCurlyBracket))
            {
                enumerator.RestoreState();
                return false;
            }

            var tuple = Mdx.Tuple();

            if (IsNextTokenValid(enumerator, TokenType.RightCurlyBracket))
            {
                expression = tuple;

                enumerator.MergeState();
                return true;
            }

            do
            {
                MdxExpressionBase childExpression;
                if (TryParseRange(enumerator, out childExpression))
                {
                    tuple.With((MdxRange)childExpression);
                }
                else if (TryParseMember(enumerator, out childExpression))
                {
                    tuple.With((MdxMember)childExpression);
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
                    enumerator.RestoreState();
                    return false;
                }

            } while (IsNextTokenValid(enumerator, TokenType.MemberSeparator));

            if (!IsNextTokenValid(enumerator, TokenType.RightCurlyBracket))
            {
                enumerator.RestoreState();
                return false;
            }

            expression = tuple;

            enumerator.MergeState();
            return true;
        }

        private static bool TryParseSet(IStatedTwoWayEnumerator<Token> enumerator, out MdxExpressionBase expression)
        {
            enumerator.BackupState();
            expression = null;

            if (!IsNextTokenValid(enumerator, TokenType.LeftRoundBracket))
            {
                enumerator.RestoreState();
                return false;
            }

            var set = Mdx.Set();

            if (IsNextTokenValid(enumerator, TokenType.RightRoundBracket))
            {
                expression = set;

                enumerator.MergeState();
                return true;
            }

            do
            {
                MdxExpressionBase childExpression;
                if (TryParseRange(enumerator, out childExpression))
                {
                    set.With((MdxRange)childExpression);
                }
                else if (TryParseMember(enumerator, out childExpression))
                {
                    set.With((MdxMember)childExpression);
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
                    enumerator.RestoreState();
                    return false;
                }

            } while (IsNextTokenValid(enumerator, TokenType.MemberSeparator));

            if (!IsNextTokenValid(enumerator, TokenType.RightRoundBracket))
            {
                enumerator.RestoreState();
                return false;
            }

            expression = set;

            enumerator.MergeState();
            return true;
        }

        internal static bool TryParseRange(IStatedTwoWayEnumerator<Token> enumerator, out MdxExpressionBase expression)
        {
            enumerator.BackupState();
            expression = null;

            var range = new MdxRange();

            MdxExpressionBase fromMember;
            if (!TryParseMember(enumerator, out fromMember))
            {
                enumerator.RestoreState();
                return false;
            }

            if (!IsNextTokenValid(enumerator, TokenType.RangeSeparator))
            {
                enumerator.RestoreState();
                return false;
            }

            range.From((MdxMember)fromMember);

            MdxExpressionBase toMember;
            if (!TryParseMember(enumerator, out toMember))
            {
                enumerator.RestoreState();
                return false;
            }

            range.To((MdxMember) toMember);

            expression = range;

            enumerator.MergeState();
            return true;
        }

        internal static bool TryParseMember(IStatedTwoWayEnumerator<Token> enumerator, out MdxExpressionBase expression)
        {
            enumerator.BackupState();
            expression = null;

            if (!IsNextTokenValid(enumerator, TokenType.LeftSquareBracket))
            {
                enumerator.RestoreState();
                return false;
            }

            var member = Mdx.Member();

            if (!IsNextTokenValid(enumerator, TokenType.IdentifierExpression))
            {
                enumerator.RestoreState();
                return false;
            }

            member.Titled(enumerator.Current.Value);

            if (!IsNextTokenValid(enumerator, TokenType.RightSquareBracket))
            {
                enumerator.RestoreState();
                return false;
            }

            while (IsNextTokenValid(enumerator, TokenType.IdentifierSeparator))
            {
                MdxExpressionBase function;
                if (TryParseNavigationFunction(enumerator, out function))
                {
                    member.WithFunction((MdxNavigationFunction) function);
                    continue;
                }

                if (!IsNextTokenValid(enumerator, TokenType.LeftSquareBracket))
                {
                    enumerator.RestoreState();
                    return false;
                }

                if (!IsNextTokenValid(enumerator, TokenType.IdentifierExpression))
                {
                    enumerator.RestoreState();
                    return false;
                }

                member.Titled(enumerator.Current.Value);

                if (!IsNextTokenValid(enumerator, TokenType.RightSquareBracket))
                {
                    enumerator.RestoreState();
                    return false;
                }
            }

            if (!IsNextTokenValid(enumerator, TokenType.ValueSeparator))
            {
                expression = member;

                enumerator.MergeState();
                return true;
            }

            if (!IsNextTokenValid(enumerator, TokenType.LeftSquareBracket))
            {
                enumerator.RestoreState();
                return false;
            }

            if (!IsNextTokenValid(enumerator, TokenType.LogicalExpression) &&
                !IsNextTokenValid(enumerator, TokenType.DateExpression) &&
                !IsNextTokenValid(enumerator, TokenType.NumberExpression) &&
                !IsNextTokenValid(enumerator, TokenType.IdentifierExpression) &&
                !IsNextTokenValid(enumerator, TokenType.MathsOperator))
            {
                enumerator.RestoreState();
                return false;
            }

            var memberValue = enumerator.Current.Value;

            if (IsNextTokenValid(enumerator, TokenType.IdentifierExpression))
                memberValue += enumerator.Current.Value;

            member.WithValue(memberValue);

            if (!IsNextTokenValid(enumerator, TokenType.RightSquareBracket))
            {
                enumerator.RestoreState();
                return false;
            }

            while (IsNextTokenValid(enumerator, TokenType.IdentifierSeparator))
            {
                MdxExpressionBase function;
                if (!TryParseNavigationFunction(enumerator, out function))
                {
                    enumerator.RestoreState();
                    return false;
                }

                member.WithFunction((MdxNavigationFunction) function);
            }

            expression = member;

            enumerator.MergeState();
            return true;
        }

        internal static bool TryParseAxis(IStatedTwoWayEnumerator<Token> enumerator, out MdxExpressionBase expression)
        {
            enumerator.BackupState();
            expression = null;

            var axis = Mdx.Axis();

            if (IsNextTokenValid(enumerator, TokenType.Non))
            {
                if (!IsNextTokenValid(enumerator, TokenType.Empty))
                {
                    enumerator.RestoreState();
                    return false;
                }

                axis.AsNonEmpty();
            }

            MdxExpressionBase slicer;
            if (TryParseTuple(enumerator, out slicer))
            {
                axis.WithSlicer((MdxTuple)slicer);
            } 
            else if (TryParseSet(enumerator, out slicer))
            {
                axis.WithSlicer(Mdx.Tuple().With((MdxSet)slicer));
            }
            else if (TryParseRange(enumerator, out slicer))
            {
                axis.WithSlicer(Mdx.Tuple().With((MdxRange)slicer));
            }
            else if (TryParseMember(enumerator, out slicer))
            {
                axis.WithSlicer(Mdx.Tuple().With((MdxMember)slicer));
            }
            else if (TryParseFunction(enumerator, out slicer))
            {
                axis.WithSlicer(Mdx.Tuple().With((MdxMember)slicer));
            }
            else
            {
                enumerator.RestoreState();
                return false;
            }

            if (IsNextTokenValid(enumerator, TokenType.Dimension))
            {
                if (!IsNextTokenValid(enumerator, TokenType.Properties))
                {
                    enumerator.RestoreState();
                    return false;
                }

                do
                {
                    if (!IsNextTokenValid(enumerator, TokenType.DimensionProperty))
                    {
                        enumerator.RestoreState();
                        return false;
                    }

                    axis.WithProperties(enumerator.Current.Value);
                } while (IsNextTokenValid(enumerator, TokenType.MemberSeparator));
            }
            else if (IsNextTokenValid(enumerator, TokenType.Properties))
            {
                do
                {
                    if (!IsNextTokenValid(enumerator, TokenType.DimensionProperty))
                    {
                        enumerator.RestoreState();
                        return false;
                    }

                    axis.WithProperties(enumerator.Current.Value);
                } while (IsNextTokenValid(enumerator, TokenType.MemberSeparator));
            }

            if (!IsNextTokenValid(enumerator, TokenType.On))
            {
                enumerator.RestoreState();
                return false;
            }

            if (!IsNextTokenValid(enumerator, TokenType.AxisName) &&
                !IsNextTokenValid(enumerator, TokenType.NumberExpression))
            {
                enumerator.RestoreState();
                return false;
            }

            string axisName = enumerator.Current.Value;
            axis.Titled(axisName);

            expression = axis;

            enumerator.MergeState();
            return true;
        }

        internal static bool TryParseCube(IStatedTwoWayEnumerator<Token> enumerator, out MdxExpressionBase expression)
        {
            enumerator.BackupState();
            expression = null;

            var cube = Mdx.Cube();

            do
            {
                if (!IsNextTokenValid(enumerator, TokenType.LeftSquareBracket))
                {
                    enumerator.RestoreState();
                    return false;
                }

                if (!IsNextTokenValid(enumerator, TokenType.IdentifierExpression))
                {
                    enumerator.RestoreState();
                    return false;
                }

                cube.Titled(enumerator.Current.Value);

                if (!IsNextTokenValid(enumerator, TokenType.RightSquareBracket))
                {
                    enumerator.RestoreState();
                    return false;
                }
                
            } while (IsNextTokenValid(enumerator, TokenType.IdentifierSeparator));

            expression = cube;

            enumerator.MergeState();
            return true;
        }

        internal static bool TryParseNavigationFunction(IStatedTwoWayEnumerator<Token> enumerator, out MdxExpressionBase expression)
        {
            enumerator.BackupState();
            expression = null;

            if (!IsNextTokenValid(enumerator, TokenType.IdentifierExpression) &&
                !IsNextTokenValid(enumerator, TokenType.DimensionProperty))
            {
                enumerator.RestoreState();
                return false;
            }

            var functionTitle = enumerator.Current.Value;

            if (!IsNextTokenValid(enumerator, TokenType.LeftRoundBracket))
            {
                expression = Mdx.NavigationFunction().Titled(functionTitle);

                enumerator.MergeState();
                return true;
            }
            
            var functionParameters = new List<string>();
            do
            {
                if (!IsNextTokenValid(enumerator, TokenType.LogicalExpression) &&
                    !IsNextTokenValid(enumerator, TokenType.DateExpression) &&
                    !IsNextTokenValid(enumerator, TokenType.NumberExpression) &&
                    !IsNextTokenValid(enumerator, TokenType.IdentifierExpression) &&
                    !IsNextTokenValid(enumerator, TokenType.MathsOperator))
                {
                    enumerator.RestoreState();
                    return false;
                }

                var functionParametersValue = enumerator.Current.Value;

                if (IsNextTokenValid(enumerator, TokenType.IdentifierExpression))
                    functionParametersValue += enumerator.Current.Value;

                functionParameters.Add(functionParametersValue);

            } while (IsNextTokenValid(enumerator, TokenType.MemberSeparator));

            if (!IsNextTokenValid(enumerator, TokenType.RightRoundBracket))
            {
                enumerator.RestoreState();
                return false;
            }

            expression = Mdx.NavigationFunction().Titled(functionTitle).WithParameters(functionParameters.ToArray());

            enumerator.MergeState();
            return true;
        }

        internal static bool TryParseFunction(IStatedTwoWayEnumerator<Token> enumerator, out MdxExpressionBase expression)
        {
            enumerator.BackupState();
            expression = null;

            var function = Mdx.Function();
            do
            {
                if (!IsNextTokenValid(enumerator, TokenType.IdentifierExpression))
                {
                    enumerator.RestoreState();
                    return false;
                }

                function.Titled(enumerator.Current.Value);

            } while (IsNextTokenValid(enumerator, TokenType.IdentifierSeparator));

            if (!IsNextTokenValid(enumerator, TokenType.LeftRoundBracket))
            {
                enumerator.RestoreState();
                return false;
            }

            if (IsNextTokenValid(enumerator, TokenType.RightRoundBracket))
            {
                expression = function;

                enumerator.MergeState();
                return true;
            }

            do
            {
                MdxExpressionBase childExpression;
                if (!TryParseExpression(enumerator, out childExpression))
                {
                    enumerator.RestoreState();
                    return false;
                } 
                
                function.WithParameter((MdxExpression)childExpression);    
                
            } while (IsNextTokenValid(enumerator, TokenType.MemberSeparator));

            if (!IsNextTokenValid(enumerator, TokenType.RightRoundBracket))
            {
                enumerator.RestoreState();
                return false;
            }

            expression = function;

            enumerator.MergeState();
            return true;
        }

        internal static bool TryParseExpression(IStatedTwoWayEnumerator<Token> enumerator, out MdxExpressionBase expression)
        {
            enumerator.BackupState();
            expression = null;

            var mdxExpression = Mdx.Expression();

            //TODO: Think how to apply NOT and -
            do
            {
                MdxExpressionBase childExpression;
                if (TryParseFunction(enumerator, out childExpression))
                {
                    mdxExpression.WithOperand((MdxFunction) childExpression);
                }
                else if (TryParseTuple(enumerator, out childExpression))
                {
                    mdxExpression.WithOperand((MdxTuple)childExpression);
                }
                else if (TryParseSet(enumerator, out childExpression))
                {
                    mdxExpression.WithOperand((MdxSet)childExpression);
                }
                else if (TryParseRange(enumerator, out childExpression))
                {
                    mdxExpression.WithOperand((MdxRange)childExpression);
                }
                else if (TryParseMember(enumerator, out childExpression))
                {
                    mdxExpression.WithOperand((MdxMember)childExpression);
                }
                else if (TryParseConstantValue(enumerator, out childExpression))
                {
                    mdxExpression.WithOperand((MdxConstantExpression)childExpression);
                }
                else if (IsNextTokenValid(enumerator, TokenType.NotOperator))
                {
                    if (!TryParseExpression(enumerator, out childExpression))
                    {
                        enumerator.RestoreState();
                        return false;
                    }

                    mdxExpression.WithOperand(((MdxExpression)childExpression).AsNegated());
                }
                else if (IsNextTokenValid(enumerator, TokenType.LeftRoundBracket))
                {
                    if (!TryParseExpression(enumerator, out childExpression))
                    {
                        enumerator.RestoreState();
                        return false;
                    }

                    mdxExpression.WithOperand((MdxExpression)childExpression);

                    if (!IsNextTokenValid(enumerator, TokenType.RightRoundBracket))
                    {
                        enumerator.RestoreState();
                        return false;
                    }
                }
                else
                {
                    enumerator.RestoreState();
                    return false;
                }

                if (!IsNextTokenValid(enumerator, TokenType.MathsOperator) &&
                    !IsNextTokenValid(enumerator, TokenType.LogicsOperator))
                    break;

                mdxExpression.WithOperation(enumerator.Current.Value);

            } while (true);

            expression = mdxExpression;

            enumerator.MergeState();
            return true;
        }

        internal static bool TryParseConstantValue(IStatedTwoWayEnumerator<Token> enumerator, out MdxExpressionBase expression)
        {
            enumerator.BackupState();
            expression = null;

            var constantValue = Mdx.ConstantValue();

            if (!IsNextTokenValid(enumerator, TokenType.LogicalExpression) && 
                !IsNextTokenValid(enumerator, TokenType.DateExpression) &&
                !IsNextTokenValid(enumerator, TokenType.NumberExpression) &&
                !IsNextTokenValid(enumerator, TokenType.IdentifierExpression))
            {
                enumerator.RestoreState();
                return false;
            }

            string constantVal = enumerator.Current.Value;
            if (IsNextTokenValid(enumerator, TokenType.IdentifierExpression))
                constantVal += enumerator.Current.Value;

            constantValue.WithValue(constantVal);

            expression = constantValue;

            enumerator.MergeState();
            return true;
        }
    }
}