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
        public string Value {get;}
        public Dictionary<string, string> Attributes {get;} = new Dictionary<string, string>();

        public Token(TokenType type) {
            Type = type;
            Value = "";
            Name = "";
        }

        public Token(TokenType type, string name) {
            Type = type;
            Value = "";
            Name = name;
        }

        public Token(string value) {
            Type = TokenType.TEXT;
            Name = "Text";
            Value = value;
        }

        public override string ToString()
        {
            return $"Token {{Type={Type}, Name={Name}, Value={Value}}}";
        }
    }
}