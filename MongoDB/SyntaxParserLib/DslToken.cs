namespace GingerMongoDB.SyntaxParser
{
    public class DslToken
    {
        public TokenType tokenType;
        public string value;
        public TokenType sequenceTerminator;
        

        public DslToken(TokenType sequenceTerminator)
        {
            this.sequenceTerminator = sequenceTerminator;
        }

        public DslToken(TokenType tokenType, string value)
        {
            this.tokenType = tokenType;
            this.value = value;
        }
    }
}