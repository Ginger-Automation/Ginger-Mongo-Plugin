using GingerMongoDB.SyntaxParser;
using MongoDB.Driver;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace GingerMongoDB
{
    public class SyntaxParser1
    {        
        static List<TokenDefinition> _tokenDefinitions;
        

        public SyntaxParser1()
        {
            if (_tokenDefinitions != null) return;

            _tokenDefinitions = new List<TokenDefinition>();

            _tokenDefinitions.Add(new TokenDefinition(TokenType.CollectionName, "dbcol.", "db\\.([a-zA-Z0-9]*)\\.", 1)); // db.cutomsers.   contains letters and numbers  TODO: add _ and check what chars for collection name are valid            
            _tokenDefinitions.Add(new TokenDefinition(TokenType.find, "find", "find|Find", 1));             
            _tokenDefinitions.Add(new TokenDefinition(TokenType.Comma, ",", ",", 1));            
            // _tokenDefinitions.Add(new TokenDefinition(TokenType.JSON, "json", "{([^']*)}", 1));   // Start with { and end with }

            _tokenDefinitions.Add(new TokenDefinition(TokenType.JSON, "json", @"{(?:[^{}]|(?R))*}", 1));   // Start with { and end with }

            _tokenDefinitions.Add(new TokenDefinition(TokenType.OpenParenthesis, "(", "\\(", 1));
            _tokenDefinitions.Add(new TokenDefinition(TokenType.CloseParenthesis, ")", "\\)", 1));            
            _tokenDefinitions.Add(new TokenDefinition(TokenType.StringValue, "string", "\"([^']*)\"", 1));   //  "abc"            
            _tokenDefinitions.Add(new TokenDefinition(TokenType.NumberValue, "number", "\\d+", 2));

            // Add sort, limit, insertMany

        }



        public IEnumerable<DslToken> Tokenize(string text)
        {
            var tokenMatches = FindTokenMatches(text);

            var groupedByIndex = tokenMatches.GroupBy(x => x.StartIndex)
                .OrderBy(x => x.Key)
                .ToList();

            TokenMatch lastMatch = null;
            for (int i = 0; i < groupedByIndex.Count; i++)
            {
                var bestMatch = groupedByIndex[i].OrderBy(x => x.Precedence).First();
                if (lastMatch != null && bestMatch.StartIndex < lastMatch.EndIndex)
                    continue;

                yield return new DslToken(bestMatch.TokenType, bestMatch.Value);

                lastMatch = bestMatch;
            }

            yield return new DslToken(TokenType.SequenceTerminator);
        }

        private List<TokenMatch> FindTokenMatches(string text)
        {
            var tokenMatches = new List<TokenMatch>();

            foreach (var tokenDefinition in _tokenDefinitions)
                tokenMatches.AddRange(tokenDefinition.FindMatches(text).ToList());

            return tokenMatches;
        }

        public static string GetKey(TokenType tokenType)
        {
            string key = (from x in _tokenDefinitions where x.tokenType == tokenType select x.key).SingleOrDefault();
            return key;
        }





    }
}
