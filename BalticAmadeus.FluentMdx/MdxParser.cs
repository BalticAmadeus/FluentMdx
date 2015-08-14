using System;
using System.Collections.Generic;
using BalticAmadeus.FluentMdx.EnumerableExtensions;
using BalticAmadeus.FluentMdx.Lexer;

namespace BalticAmadeus.FluentMdx
{
    /// <summary>
    /// Represents a machine that performs syntactical analysis.
    /// </summary>
    public sealed class MdxParser : IMdxParser
    {
        private readonly ILexer _lexer;

        /// <summary>
        /// Initializes a new instance of <see cref="MdxParser"/> with default lexical analysis machine.
        /// </summary>
        public MdxParser()
        {
            _lexer = new Lexer.Lexer();
        }

        /// <summary>
        /// Performs lexical and syntactical analyses over specified query string and returns the parsed components combined into <see cref="MdxQuery"/>.
        /// </summary>
        /// <param name="source">Mdx query string.</param>
        /// <returns>Returns the parsed Mdx components combined into <see cref="MdxQuery"/>.</returns>
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
       
        /// <summary>
        /// Performs query syntactical analysis over collection of <see cref="Token"/> objects using <see cref="IStatedTwoWayEnumerator{T}"/>.
        /// </summary>
        /// <param name="enumerator">Extended enumerator of collection of <see cref="Token"/> objects.</param>
        /// <param name="expression">Output parsed query if syntactic analysis was succeeded.</param>
        /// <returns><value>True</value> if succeeded. <value>False</value> if failed.</returns>
        internal static bool TryParseQuery(IStatedTwoWayEnumerator<Token> enumerator, out MdxExpressionBase expression)
        {
            enumerator.SavePosition();
            expression = null;
            
            var query = Mdx.Query();
            
            if (IsNextTokenValid(enumerator, TokenType.With))
            {
                MdxExpressionBase declaration;
                if (!TryParseWithDeclaration(enumerator, out declaration))
                {
                    enumerator.RestoreLastSavedPosition();
                    return false;
                }

                query.With((MdxDeclaration)declaration);

                while (TryParseWithDeclaration(enumerator, out declaration))
                {
                    query.With((MdxDeclaration)declaration);
                }
            }

            if (!IsNextTokenValid(enumerator, TokenType.Select))
            {
                enumerator.RestoreLastSavedPosition();
                return false;
            }

            do
            {
                MdxExpressionBase childExpression;
                if (!TryParseAxis(enumerator, out childExpression))
                {
                    enumerator.RestoreLastSavedPosition();
                    return false;
                }

                query.On((MdxAxis)childExpression);
            } while (IsNextTokenValid(enumerator, TokenType.MemberSeparator));

            if (!IsNextTokenValid(enumerator, TokenType.From))
            {
                enumerator.RestoreLastSavedPosition();
                return false;
            }

            if (IsNextTokenValid(enumerator, TokenType.LeftRoundBracket))
            {
                MdxExpressionBase innerQuery;
                if (!TryParseQuery(enumerator, out innerQuery))
                {
                    enumerator.RestoreLastSavedPosition();
                    return false;
                }

                query.From((MdxQuery) innerQuery);

                if (!IsNextTokenValid(enumerator, TokenType.RightRoundBracket))
                {
                    enumerator.RestoreLastSavedPosition();
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
                        enumerator.RestoreLastSavedPosition();
                        return false;
                    }

                    query.From((MdxCube)childExpression);
                } while (IsNextTokenValid(enumerator, TokenType.MemberSeparator));
            }

            if (!IsNextTokenValid(enumerator, TokenType.Where))
            {
                expression = query;
                enumerator.RemoveLastSavedState();
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
                enumerator.RestoreLastSavedPosition();
                return false;
            }

            expression = query;
            enumerator.RemoveLastSavedState();
            return true;
        }

        internal static bool TryParseWithDeclaration(IStatedTwoWayEnumerator<Token> enumerator, out MdxExpressionBase expression)
        {
            enumerator.SavePosition();
            expression = null;

            var declaration = Mdx.Declaration();

            if (!IsNextTokenValid(enumerator, TokenType.Member) && 
                !IsNextTokenValid(enumerator, TokenType.Set))
            {
                enumerator.RestoreLastSavedPosition();
                return false;
            }

            if (!IsNextTokenValid(enumerator, TokenType.LeftSquareBracket))
            {
                enumerator.RestoreLastSavedPosition();
                return false;
            }

            if (!IsNextTokenValid(enumerator, TokenType.IdentifierExpression))
            {
                enumerator.RestoreLastSavedPosition();
                return false;
            }

            declaration.Titled(enumerator.Current.Value);

            if (!IsNextTokenValid(enumerator, TokenType.RightSquareBracket))
            {
                enumerator.RestoreLastSavedPosition();
                return false;
            }

            while (IsNextTokenValid(enumerator, TokenType.IdentifierSeparator))
            {
                if (!IsNextTokenValid(enumerator, TokenType.LeftSquareBracket))
                {
                    enumerator.RestoreLastSavedPosition();
                    return false;
                }

                if (!IsNextTokenValid(enumerator, TokenType.IdentifierExpression))
                {
                    enumerator.RestoreLastSavedPosition();
                    return false;
                }

                declaration.Titled(enumerator.Current.Value);

                if (!IsNextTokenValid(enumerator, TokenType.RightSquareBracket))
                {
                    enumerator.RestoreLastSavedPosition();
                    return false;
                }
            }

            if (!IsNextTokenValid(enumerator, TokenType.As))
            {
                enumerator.RestoreLastSavedPosition();
                return false;
            }


            MdxExpressionBase declarationExpression;
            if (TryParseTuple(enumerator, out declarationExpression))
            {
                declaration.As((MdxTuple) declarationExpression);
            }
            else if (TryParseExpression(enumerator, out declarationExpression))
            {
                declaration.As((MdxExpression) declarationExpression);
            }
            else
            {
                enumerator.RestoreLastSavedPosition();
                return false;
            }

            expression = declaration;
            enumerator.RemoveLastSavedState();
            return true;
        }

        /// <summary>
        /// Performs tuple syntactical analysis over collection of <see cref="Token"/> objects using <see cref="IStatedTwoWayEnumerator{T}"/>.
        /// </summary>
        /// <param name="enumerator">Extended enumerator of collection of <see cref="Token"/> objects.</param>
        /// <param name="expression">Output parsed tuple if syntactic analysis was succeeded.</param>
        /// <returns><value>True</value> if succeeded. <value>False</value> if failed.</returns>
        internal static bool TryParseTuple(IStatedTwoWayEnumerator<Token> enumerator, out MdxExpressionBase expression)
        {
            enumerator.SavePosition();
            expression = null;

            if (!IsNextTokenValid(enumerator, TokenType.LeftCurlyBracket))
            {
                enumerator.RestoreLastSavedPosition();
                return false;
            }

            var tuple = Mdx.Tuple();

            if (IsNextTokenValid(enumerator, TokenType.RightCurlyBracket))
            {
                expression = tuple;

                enumerator.RemoveLastSavedState();
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
                else if (TryParseTuple(enumerator, out childExpression))
                {
                    tuple.With((MdxTuple)childExpression);
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
                    enumerator.RestoreLastSavedPosition();
                    return false;
                }

            } while (IsNextTokenValid(enumerator, TokenType.MemberSeparator));

            if (!IsNextTokenValid(enumerator, TokenType.RightCurlyBracket))
            {
                enumerator.RestoreLastSavedPosition();
                return false;
            }

            expression = tuple;

            enumerator.RemoveLastSavedState();
            return true;
        }

        /// <summary>
        /// Performs set syntactical analysis over collection of <see cref="Token"/> objects using <see cref="IStatedTwoWayEnumerator{T}"/>.
        /// </summary>
        /// <param name="enumerator">Extended enumerator of collection of <see cref="Token"/> objects.</param>
        /// <param name="expression">Output parsed set if syntactic analysis was succeeded.</param>
        /// <returns><value>True</value> if succeeded. <value>False</value> if failed.</returns>
        internal static bool TryParseSet(IStatedTwoWayEnumerator<Token> enumerator, out MdxExpressionBase expression)
        {
            enumerator.SavePosition();
            expression = null;

            if (!IsNextTokenValid(enumerator, TokenType.LeftRoundBracket))
            {
                enumerator.RestoreLastSavedPosition();
                return false;
            }

            var set = Mdx.Set();

            if (IsNextTokenValid(enumerator, TokenType.RightRoundBracket))
            {
                expression = set;

                enumerator.RemoveLastSavedState();
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
                else if (TryParseSet(enumerator, out childExpression))
                {
                    set.With((MdxSet)childExpression);
                }
                else if (TryParseFunction(enumerator, out childExpression))
                {
                    set.With((MdxFunction)childExpression);
                }
                else
                {
                    enumerator.RestoreLastSavedPosition();
                    return false;
                }

            } while (IsNextTokenValid(enumerator, TokenType.MemberSeparator));

            if (!IsNextTokenValid(enumerator, TokenType.RightRoundBracket))
            {
                enumerator.RestoreLastSavedPosition();
                return false;
            }

            expression = set;

            enumerator.RemoveLastSavedState();
            return true;
        }

        /// <summary>
        /// Performs range syntactical analysis over collection of <see cref="Token"/> objects using <see cref="IStatedTwoWayEnumerator{T}"/>.
        /// </summary>
        /// <param name="enumerator">Extended enumerator of collection of <see cref="Token"/> objects.</param>
        /// <param name="expression">Output parsed range if syntactic analysis was succeeded.</param>
        /// <returns><value>True</value> if succeeded. <value>False</value> if failed.</returns>
        internal static bool TryParseRange(IStatedTwoWayEnumerator<Token> enumerator, out MdxExpressionBase expression)
        {
            enumerator.SavePosition();
            expression = null;

            var range = new MdxRange();

            MdxExpressionBase fromMember;
            if (!TryParseMember(enumerator, out fromMember))
            {
                enumerator.RestoreLastSavedPosition();
                return false;
            }

            if (!IsNextTokenValid(enumerator, TokenType.RangeSeparator))
            {
                enumerator.RestoreLastSavedPosition();
                return false;
            }

            range.From((MdxMember)fromMember);

            MdxExpressionBase toMember;
            if (!TryParseMember(enumerator, out toMember))
            {
                enumerator.RestoreLastSavedPosition();
                return false;
            }

            range.To((MdxMember) toMember);

            expression = range;

            enumerator.RemoveLastSavedState();
            return true;
        }
        
        /// <summary>
        /// Performs member syntactical analysis over collection of <see cref="Token"/> objects using <see cref="IStatedTwoWayEnumerator{T}"/>.
        /// </summary>
        /// <param name="enumerator">Extended enumerator of collection of <see cref="Token"/> objects.</param>
        /// <param name="expression">Output parsed member if syntactic analysis was succeeded.</param>
        /// <returns><value>True</value> if succeeded. <value>False</value> if failed.</returns>
        internal static bool TryParseMember(IStatedTwoWayEnumerator<Token> enumerator, out MdxExpressionBase expression)
        {
            enumerator.SavePosition();
            expression = null;

            if (!IsNextTokenValid(enumerator, TokenType.LeftSquareBracket))
            {
                enumerator.RestoreLastSavedPosition();
                return false;
            }

            var member = Mdx.Member();

            if (!IsNextTokenValid(enumerator, TokenType.IdentifierExpression))
            {
                enumerator.RestoreLastSavedPosition();
                return false;
            }

            member.Titled(enumerator.Current.Value);

            if (!IsNextTokenValid(enumerator, TokenType.RightSquareBracket))
            {
                enumerator.RestoreLastSavedPosition();
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
                    enumerator.RestoreLastSavedPosition();
                    return false;
                }

                if (!IsNextTokenValid(enumerator, TokenType.IdentifierExpression))
                {
                    enumerator.RestoreLastSavedPosition();
                    return false;
                }

                member.Titled(enumerator.Current.Value);

                if (!IsNextTokenValid(enumerator, TokenType.RightSquareBracket))
                {
                    enumerator.RestoreLastSavedPosition();
                    return false;
                }
            }

            if (!IsNextTokenValid(enumerator, TokenType.ValueSeparator))
            {
                expression = member;

                enumerator.RemoveLastSavedState();
                return true;
            }

            if (!IsNextTokenValid(enumerator, TokenType.LeftSquareBracket))
            {
                enumerator.RestoreLastSavedPosition();
                return false;
            }

            if (!IsNextTokenValid(enumerator, TokenType.LogicalExpression) &&
                !IsNextTokenValid(enumerator, TokenType.DateExpression) &&
                !IsNextTokenValid(enumerator, TokenType.NumberExpression) &&
                !IsNextTokenValid(enumerator, TokenType.IdentifierExpression) &&
                !IsNextTokenValid(enumerator, TokenType.MathsOperator))
            {
                enumerator.RestoreLastSavedPosition();
                return false;
            }

            var memberValue = enumerator.Current.Value;

            if (IsNextTokenValid(enumerator, TokenType.IdentifierExpression))
                memberValue += enumerator.Current.Value;

            member.WithValue(memberValue);

            if (!IsNextTokenValid(enumerator, TokenType.RightSquareBracket))
            {
                enumerator.RestoreLastSavedPosition();
                return false;
            }

            while (IsNextTokenValid(enumerator, TokenType.IdentifierSeparator))
            {
                MdxExpressionBase function;
                if (!TryParseNavigationFunction(enumerator, out function))
                {
                    enumerator.RestoreLastSavedPosition();
                    return false;
                }

                member.WithFunction((MdxNavigationFunction) function);
            }

            expression = member;

            enumerator.RemoveLastSavedState();
            return true;
        }
        
        /// <summary>
        /// Performs axis syntactical analysis over collection of <see cref="Token"/> objects using <see cref="IStatedTwoWayEnumerator{T}"/>.
        /// </summary>
        /// <param name="enumerator">Extended enumerator of collection of <see cref="Token"/> objects.</param>
        /// <param name="expression">Output parsed axis if syntactic analysis was succeeded.</param>
        /// <returns><value>True</value> if succeeded. <value>False</value> if failed.</returns>
        internal static bool TryParseAxis(IStatedTwoWayEnumerator<Token> enumerator, out MdxExpressionBase expression)
        {
            enumerator.SavePosition();
            expression = null;

            var axis = Mdx.Axis();

            if (IsNextTokenValid(enumerator, TokenType.Non))
            {
                if (!IsNextTokenValid(enumerator, TokenType.Empty))
                {
                    enumerator.RestoreLastSavedPosition();
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
                axis.WithSlicer(Mdx.Tuple().With((MdxFunction)slicer));
            }
            else
            {
                enumerator.RestoreLastSavedPosition();
                return false;
            }

            if (IsNextTokenValid(enumerator, TokenType.Dimension))
            {
                if (!IsNextTokenValid(enumerator, TokenType.Properties))
                {
                    enumerator.RestoreLastSavedPosition();
                    return false;
                }

                do
                {
                    if (!IsNextTokenValid(enumerator, TokenType.DimensionProperty))
                    {
                        enumerator.RestoreLastSavedPosition();
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
                        enumerator.RestoreLastSavedPosition();
                        return false;
                    }

                    axis.WithProperties(enumerator.Current.Value);
                } while (IsNextTokenValid(enumerator, TokenType.MemberSeparator));
            }

            if (!IsNextTokenValid(enumerator, TokenType.On))
            {
                enumerator.RestoreLastSavedPosition();
                return false;
            }

            if (!IsNextTokenValid(enumerator, TokenType.AxisNameIdentifier) &&
                !IsNextTokenValid(enumerator, TokenType.NumberExpression))
            {
                enumerator.RestoreLastSavedPosition();
                return false;
            }

            string axisName = enumerator.Current.Value;
            axis.Titled(axisName);

            expression = axis;

            enumerator.RemoveLastSavedState();
            return true;
        }
        
        /// <summary>
        /// Performs cube syntactical analysis over collection of <see cref="Token"/> objects using <see cref="IStatedTwoWayEnumerator{T}"/>.
        /// </summary>
        /// <param name="enumerator">Extended enumerator of collection of <see cref="Token"/> objects.</param>
        /// <param name="expression">Output parsed cube if syntactic analysis was succeeded.</param>
        /// <returns><value>True</value> if succeeded. <value>False</value> if failed.</returns>
        internal static bool TryParseCube(IStatedTwoWayEnumerator<Token> enumerator, out MdxExpressionBase expression)
        {
            enumerator.SavePosition();
            expression = null;

            var cube = Mdx.Cube();

            do
            {
                if (!IsNextTokenValid(enumerator, TokenType.LeftSquareBracket))
                {
                    enumerator.RestoreLastSavedPosition();
                    return false;
                }

                if (!IsNextTokenValid(enumerator, TokenType.IdentifierExpression))
                {
                    enumerator.RestoreLastSavedPosition();
                    return false;
                }

                cube.Titled(enumerator.Current.Value);

                if (!IsNextTokenValid(enumerator, TokenType.RightSquareBracket))
                {
                    enumerator.RestoreLastSavedPosition();
                    return false;
                }
                
            } while (IsNextTokenValid(enumerator, TokenType.IdentifierSeparator));

            expression = cube;

            enumerator.RemoveLastSavedState();
            return true;
        }

        /// <summary>
        /// Performs navigation function syntactical analysis over collection of <see cref="Token"/> objects using <see cref="IStatedTwoWayEnumerator{T}"/>.
        /// </summary>
        /// <param name="enumerator">Extended enumerator of collection of <see cref="Token"/> objects.</param>
        /// <param name="expression">Output parsed navigation function if syntactic analysis was succeeded.</param>
        /// <returns><value>True</value> if succeeded. <value>False</value> if failed.</returns>
        internal static bool TryParseNavigationFunction(IStatedTwoWayEnumerator<Token> enumerator, out MdxExpressionBase expression)
        {
            enumerator.SavePosition();
            expression = null;

            if (!IsNextTokenValid(enumerator, TokenType.IdentifierExpression) &&
                !IsNextTokenValid(enumerator, TokenType.DimensionProperty))
            {
                enumerator.RestoreLastSavedPosition();
                return false;
            }

            var functionTitle = enumerator.Current.Value;

            if (!IsNextTokenValid(enumerator, TokenType.LeftRoundBracket))
            {
                expression = Mdx.NavigationFunction().Titled(functionTitle);

                enumerator.RemoveLastSavedState();
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
                    enumerator.RestoreLastSavedPosition();
                    return false;
                }

                var functionParametersValue = enumerator.Current.Value;

                if (IsNextTokenValid(enumerator, TokenType.IdentifierExpression))
                    functionParametersValue += enumerator.Current.Value;

                functionParameters.Add(functionParametersValue);

            } while (IsNextTokenValid(enumerator, TokenType.MemberSeparator));

            if (!IsNextTokenValid(enumerator, TokenType.RightRoundBracket))
            {
                enumerator.RestoreLastSavedPosition();
                return false;
            }

            expression = Mdx.NavigationFunction().Titled(functionTitle).WithParameters(functionParameters.ToArray());

            enumerator.RemoveLastSavedState();
            return true;
        }
        
        /// <summary>
        /// Performs function syntactical analysis over collection of <see cref="Token"/> objects using <see cref="IStatedTwoWayEnumerator{T}"/>.
        /// </summary>
        /// <param name="enumerator">Extended enumerator of collection of <see cref="Token"/> objects.</param>
        /// <param name="expression">Output parsed function if syntactic analysis was succeeded.</param>
        /// <returns><value>True</value> if succeeded. <value>False</value> if failed.</returns>
        internal static bool TryParseFunction(IStatedTwoWayEnumerator<Token> enumerator, out MdxExpressionBase expression)
        {
            enumerator.SavePosition();
            expression = null;

            var function = Mdx.Function();
            do
            {
                if (!IsNextTokenValid(enumerator, TokenType.IdentifierExpression))
                {
                    enumerator.RestoreLastSavedPosition();
                    return false;
                }

                function.Titled(enumerator.Current.Value);

            } while (IsNextTokenValid(enumerator, TokenType.IdentifierSeparator));

            if (!IsNextTokenValid(enumerator, TokenType.LeftRoundBracket))
            {
                enumerator.RestoreLastSavedPosition();
                return false;
            }

            if (IsNextTokenValid(enumerator, TokenType.RightRoundBracket))
            {
                expression = function;

                enumerator.RemoveLastSavedState();
                return true;
            }

            do
            {
                MdxExpressionBase childExpression;
                if (!TryParseExpression(enumerator, out childExpression))
                {
                    enumerator.RestoreLastSavedPosition();
                    return false;
                } 
                
                function.WithParameters((MdxExpression)childExpression);    
                
            } while (IsNextTokenValid(enumerator, TokenType.MemberSeparator));

            if (!IsNextTokenValid(enumerator, TokenType.RightRoundBracket))
            {
                enumerator.RestoreLastSavedPosition();
                return false;
            }

            expression = function;

            enumerator.RemoveLastSavedState();
            return true;
        }
        
        /// <summary>
        /// Performs expression syntactical analysis over collection of <see cref="Token"/> objects using <see cref="IStatedTwoWayEnumerator{T}"/>.
        /// </summary>
        /// <param name="enumerator">Extended enumerator of collection of <see cref="Token"/> objects.</param>
        /// <param name="expression">Output parsed expression if syntactic analysis was succeeded.</param>
        /// <returns><value>True</value> if succeeded. <value>False</value> if failed.</returns>
        internal static bool TryParseExpression(IStatedTwoWayEnumerator<Token> enumerator, out MdxExpressionBase expression)
        {
            enumerator.SavePosition();
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
                        enumerator.RestoreLastSavedPosition();
                        return false;
                    }

                    mdxExpression.WithOperand(((MdxExpression)childExpression).AsNegated());
                }
                else if (IsNextTokenValid(enumerator, TokenType.LeftRoundBracket))
                {
                    if (!TryParseExpression(enumerator, out childExpression))
                    {
                        enumerator.RestoreLastSavedPosition();
                        return false;
                    }

                    mdxExpression.WithOperand((MdxExpression)childExpression);

                    if (!IsNextTokenValid(enumerator, TokenType.RightRoundBracket))
                    {
                        enumerator.RestoreLastSavedPosition();
                        return false;
                    }
                }
                else
                {
                    enumerator.RestoreLastSavedPosition();
                    return false;
                }

                if (!IsNextTokenValid(enumerator, TokenType.MathsOperator) &&
                    !IsNextTokenValid(enumerator, TokenType.LogicsOperator))
                    break;

                mdxExpression.WithOperator(enumerator.Current.Value);

            } while (true);

            expression = mdxExpression;

            enumerator.RemoveLastSavedState();
            return true;
        }

        /// <summary>
        /// Performs constant value syntactical analysis over collection of <see cref="Token"/> objects using <see cref="IStatedTwoWayEnumerator{T}"/>.
        /// </summary>
        /// <param name="enumerator">Extended enumerator of collection of <see cref="Token"/> objects.</param>
        /// <param name="expression">Output parsed constant value if syntactic analysis was succeeded.</param>
        /// <returns><value>True</value> if succeeded. <value>False</value> if failed.</returns>
        internal static bool TryParseConstantValue(IStatedTwoWayEnumerator<Token> enumerator, out MdxExpressionBase expression)
        {
            enumerator.SavePosition();
            expression = null;

            var constantValue = Mdx.ConstantValue();

            if (!IsNextTokenValid(enumerator, TokenType.LogicalExpression) && 
                !IsNextTokenValid(enumerator, TokenType.DateExpression) &&
                !IsNextTokenValid(enumerator, TokenType.NumberExpression) &&
                !IsNextTokenValid(enumerator, TokenType.IdentifierExpression) && 
                !IsNextTokenValid(enumerator, TokenType.Ordering))
            {
                enumerator.RestoreLastSavedPosition();
                return false;
            }

            string constantVal = enumerator.Current.Value;
            if (IsNextTokenValid(enumerator, TokenType.IdentifierExpression))
                constantVal += enumerator.Current.Value;

            constantValue.WithValue(constantVal);

            expression = constantValue;

            enumerator.RemoveLastSavedState();
            return true;
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
    }
}