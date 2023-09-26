// See https://aka.ms/new-console-template for more information

namespace HTMLToJS {

    public class Program {
        public static void Main(string[] args) {
            string filename = "example.html";
            string filecontent;

            using (var sr = new StreamReader(filename)) {
                filecontent = sr.ReadToEnd();
            }


        }
    }

    public enum TokenType {

        // Used for tags
        LEFT_ARROW,
        LEFT_SLASH_ARROW,
        RIGHT_ARROW,

        // Control
        SPACE,
        DOUBLE_COLLOM,
        EQUAL_SIGN,
        EOF,

        // Strings and text
        ASCII_ALPHA_STRING,
        ASCII_ALPHANUMERIC_STRING,
        TEXT,
    }

    public class Token {
        public TokenType Type {get;}
        public string Value {get;}

        public Token(TokenType type, string value) {
            Type = type;
            Value = value;
        }

        public override string ToString()
        {
            return $"Token {{Type={Type}, Value={Value}}}";
        }
    }

    public class Tokenizer {
        public Tokenizer() {
            
        }
    }
}