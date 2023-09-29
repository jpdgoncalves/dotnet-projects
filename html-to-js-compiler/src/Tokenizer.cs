
namespace HTMLToJS
{
    public interface ITokenizer
    {
        public void Run(Tokenizer tokenizer, HTMLSourceReader reader);
    }

    public class Tokenizer
    {
        public ITokenizer State { get; set; }

        private HTMLSourceReader _reader;
        private bool _wasSucessful = true;
        public bool WasSucessful { get { return _wasSucessful; } }

        private int _charPosition = 0;
        public int CharPosition { get { return _charPosition; } }

        public List<Token> Tokens;

        public Tokenizer(ITokenizer startingState, string source)
        {
            _reader = new HTMLSourceReader(source);
            _charPosition = 0;
            State = startingState;
            Tokens = new List<Token>();
            Tokenize();
        }

        public Tokenizer(string source)
        {
            _reader = new HTMLSourceReader(source);
            _charPosition = 0;
            State = new TextState();
            Tokens = new List<Token>();
            Tokenize();
        }

        private void Tokenize()
        {
            try
            {
                while (!_reader.ReachedEnd)
                {
                    State.Run(this, _reader);
                }
            }
            catch (Exception e)
            {
                Console.Error.WriteLine(e);
                _wasSucessful = false;
            }
        }
    }

    public class TextState : ITokenizer
    {
        public void Run(Tokenizer tokenizer, HTMLSourceReader reader)
        {

            var text = reader.GetUntil('<', emptyException: false);
            tokenizer.Tokens.Add(new Token(text));

            reader.ConsumeChar('<');
            var slash = reader.ConsumeChar('/');
            if (slash)
            {
                tokenizer.State = new ClosingTagState();
                return;
            }

            tokenizer.State = new TagState();
        }
    }

    public class ClosingTagState : ITokenizer
    {
        public void Run(Tokenizer tokenizer, HTMLSourceReader reader)
        {
            var tagName = reader.GetASCIILetterNumberSequenceWith('-');

            reader.ConsumeWhitespaces();
            reader.GetChar('>');

            tokenizer.Tokens.Add(new Token(TokenType.END_TAG, tagName));
            tokenizer.State = new TextState();
        }
    }

    public class TagState : ITokenizer
    {
        public void Run(Tokenizer tokenizer, HTMLSourceReader reader)
        {
            var tagName = reader.GetASCIILetterNumberSequenceWith('-');

            Token tag = new Token(TokenType.TAG, tagName);
            tokenizer.Tokens.Add(tag);
            reader.ConsumeWhitespaces();
            reader.ConsumeChar('/');
            var tagEnded = reader.ConsumeChar('>');

            if (tagEnded) {
                tokenizer.Tokens.Add(tag);
                tokenizer.State = new TextState();
                return;
            }

            // Transition to a state where we search for attributes
            tokenizer.State = new AttributeState(tag);
        }
    }

    public class AttributeState : ITokenizer
    {
        private Token _tag;

        public AttributeState(Token tag)
        {
            _tag = tag;
        }
        public void Run(Tokenizer tokenizer, HTMLSourceReader reader)
        {
            reader.ConsumeWhitespaces();
            var attrName = reader.GetASCIILetterSequence();
            var hasValue = reader.ConsumeChar('=');

            if (!hasValue) {
                _tag.Attributes.Add(attrName, "");
                return;
            }

            var isDelimitedStr = reader.ComsumeOneOf('"', '\'');

            string? attrValue;
            if (isDelimitedStr)
            {
                var delimiter = reader.Source[reader.Cursor - 1];
                attrValue = reader.GetUntil(delimiter);
                reader.GetChar(delimiter);
            }
            else
            {
                attrValue = reader.GetASCIILetterDigitSequence();
            }

            _tag.Attributes.Add(attrName, attrValue);

            reader.ConsumeWhitespaces();

            if (reader.Source[reader.Cursor] == '>') {
                tokenizer.State = new TextState();
            }
        }
    }
}