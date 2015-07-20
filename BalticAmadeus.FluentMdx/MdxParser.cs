using System;
using System.Collections.Generic;
using System.Linq;
using BalticAmadeus.FluentMdx.Extensions;
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
                throw new ArgumentException("Cannot parse the expression. There are no such rules.");
        
            if (!IsNextTokenValid(enumerator, TokenType.LastToken))
                throw new ArgumentException("There are tokens left in expression.");

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

            var queryAxes = new List<MdxAxis>();
            do
            {
                IMdxExpression childExpression;
                if (!TryParseAxis(enumerator, out childExpression))
                    return false;

                queryAxes.Add((MdxAxis)childExpression);
            } while (IsNextTokenValid(enumerator, TokenType.Comma));

            if (!IsNextTokenValid(enumerator, TokenType.From))
                return false;

            var queryCubes = new List<MdxCube>();
            do
            {
                IMdxExpression childExpression;
                if (!TryParseCube(enumerator, out childExpression))
                    return false;

                queryCubes.Add((MdxCube)childExpression);
            } while (IsNextTokenValid(enumerator, TokenType.Comma));

            if (!IsNextTokenValid(enumerator, TokenType.Where))
            {
                //Parsing for query without WHERE clause is finished here.

                expression = new MdxQuery(queryAxes, queryCubes);
                return true;
            }
            
            IMdxExpression member;
            if (TryParseMember(enumerator, out member))
            {
                var memberTuple = new MdxMemberTuple().With((MdxMember) member);

                expression = new MdxQuery(queryAxes, queryCubes, memberTuple);
                return true;
            }

            IMdxExpression set;
            if (TryParseSet(enumerator, out set))
            {
                var setTuple = new MdxSetTuple().With((MdxSet) set);

                expression = new MdxQuery(queryAxes, queryCubes, setTuple);
                return true;
            }

            IMdxExpression tuple;
            if (TryParseTuple(enumerator, out tuple))
            {
                expression = new MdxQuery(queryAxes, queryCubes, (MdxTuple)tuple);
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

        private static bool TryParseMember(ITwoWayEnumerator<Token> enumerator, out IMdxExpression expression)
        {
            expression = null;

            if (!IsNextTokenValid(enumerator, TokenType.Member))
                return false;

            var memberName = enumerator.Current.Value;

            if (!IsNextTokenValid(enumerator, TokenType.Value))
                return false;

            var memberValue = enumerator.Current.Value;

            if (!IsNextTokenValid(enumerator, TokenType.Colon))
            {
                expression = new MdxValueMember(memberName, memberValue);
                return true;
            }

            if (!IsNextTokenValid(enumerator, TokenType.Member))
                return false;

            if (memberName != enumerator.Current.Value)
                return false; //MEMBER NAMES MISMATCH

            if (!IsNextTokenValid(enumerator, TokenType.Value))
                return true;

            var rangeValue = enumerator.Current.Value;

            expression = new MdxRangeMember(memberName, memberValue, rangeValue);

            return true;
        }

        private static bool TryParseAxis(ITwoWayEnumerator<Token> enumerator, out IMdxExpression expression)
        {
            expression = null;

            if (!IsNextTokenValid(enumerator, TokenType.Non))
                return false;
            
            if (!IsNextTokenValid(enumerator, TokenType.Empty))
                return false;
            
            if (!IsNextTokenValid(enumerator, TokenType.LeftCurlyBracket))
                return false;

            var axisParameters = new List<MdxAxisParameter>();
            do
            {
                IMdxExpression childExpression;
                if (!TryParseAxisParameter(enumerator, out childExpression))
                    return false;

                axisParameters.Add((MdxAxisParameter)childExpression);
            } while (IsNextTokenValid(enumerator, TokenType.Comma));

            if (!IsNextTokenValid(enumerator, TokenType.RightCurlyBracket))
                return false;
            
            if (!IsNextTokenValid(enumerator, TokenType.On))
                return false;

            if (!IsNextTokenValid(enumerator, TokenType.AxisName))
                return false;

            string axisName = enumerator.Current.Value;

            expression = new MdxAxis(axisName, axisParameters);
            return true;
        }

        private static bool TryParseAxisParameter(ITwoWayEnumerator<Token> enumerator, out IMdxExpression expression)
        {
            expression = null;

            if (!IsNextTokenValid(enumerator, TokenType.Member))
                return false;

            string axisParameterName = enumerator.Current.Value;
            
            expression = new MdxAxisParameter(axisParameterName);
            return true;
        }

        private static bool TryParseCube(ITwoWayEnumerator<Token> enumerator, out IMdxExpression expression)
        {
            expression = null;

            if (!IsNextTokenValid(enumerator, TokenType.Member))
                return false;
            
            string cubeName = enumerator.Current.Value;

            expression = new MdxCube(cubeName);
            return true;
        }
    }
}