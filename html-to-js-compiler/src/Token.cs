// See https://aka.ms/new-console-template for more information

namespace HTMLToJS
{
    public enum TokenType
    {
        TAG,
        END_TAG,
        TEXT
    }
    
    public class Token
    {
        public TokenType Type {get;}

        public string Name {get;}
        public Dictionary<string, string> Attributes {get;} = new Dictionary<string, string>();

        public Token(TokenType type) {
            Type = type;
            Name = "";
        }

        public Token(TokenType type, string name) {
            Type = type;
            Name = name;
        }
    }
}