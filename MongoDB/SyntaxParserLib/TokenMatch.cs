namespace GingerMongoDB.SyntaxParser
{
    public class TokenMatch
    {
        public int StartIndex { get; internal set; }
        public int EndIndex { get; internal set; }
        public TokenType TokenType { get; internal set; }
        public string Value { get; internal set; }
        public int Precedence { get; internal set; }
    }
}