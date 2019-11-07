using System.Collections.Generic;
using System.Text.RegularExpressions;

namespace GingerMongoDB.SyntaxParser
{
    public enum TokenType
    {
        NotDefined,        
        find,        
        OpenParenthesis,
        CloseParenthesis,
        Comma,        
        NumberValue,        
        StringValue,
        CollectionName,
        SequenceTerminator,
        JSON,        
    }

    public class TokenDefinition
    {
        private Regex _regex;
        public readonly TokenType tokenType;
        private readonly int _precedence;
        public string key;

        public TokenDefinition(TokenType tokenType, string key,  string regexPattern, int precedence)
        {
            _regex = new Regex(regexPattern, RegexOptions.IgnoreCase | RegexOptions.Compiled);   // Ignore case !!!!!!
            this.tokenType = tokenType;
            _precedence = precedence;
            this.key = key;
        }

        public IEnumerable<TokenMatch> FindMatches(string inputString)
        {
            var matches = _regex.Matches(inputString);
            for (int i = 0; i < matches.Count; i++)
            {
                yield return new TokenMatch()
                {
                    StartIndex = matches[i].Index,
                    EndIndex = matches[i].Index + matches[i].Length,
                    TokenType = tokenType,
                    Value = matches[i].Value,
                    Precedence = _precedence
                };
            }
        }
    }
}
