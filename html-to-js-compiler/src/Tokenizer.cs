
namespace HTMLToJS
{
    public interface ITokenizer
    {
        public void Run(Tokenizer tokenizer);
    }

    public class Tokenizer
    {
        public ITokenizer State { get; set; }
        public string Source { get; }

        private int _charPosition = 0;
        public int CharPosition { get { return _charPosition; } }

        public char? CurrentChar
        {
            get
            {
                return CharPosition < Source.Length ? Source[_charPosition] : null;
            }
        }

        public List<Token> Tokens;

        public Tokenizer(ITokenizer startingState, string source)
        {
            Source = source;
            _charPosition = 0;
            State = startingState;
            Tokens = new List<Token>();
            Tokenize();
        }

        public Tokenizer(string source) {
            Source = source;
            _charPosition = 0;
            State = new TextState();
            Tokens = new List<Token>();
            Tokenize();
        }

        private void Tokenize()
        {
            while (CharPosition <= Source.Length)
            {
                State.Run(this);
            }
        }

        public string? ConsumeText()
        {
            var start = _charPosition;

            while (CurrentChar != null && CurrentChar != '<')
            {
                _charPosition += 1;
            }

            if (start >= Source.Length) return null;

            var text = Source.Substring(start, _charPosition - start);
            return text.Length > 0 ? text : null;
        }

        public string? ConsumeAsciiLetterDigitString()
        {
            var start = _charPosition;
            while (CurrentChar.HasValue && char.IsAsciiLetterOrDigit(CurrentChar.Value))
            {
                _charPosition++;
            }

            if (start >= Source.Length) return null;

            var str = Source.Substring(start, _charPosition - start);
            return str.Length > 0 ? str : null;
        }

        public void ConsumeWhitespaces()
        {
            while (CurrentChar == ' ') _charPosition++;
        }

        public char? ConsumeChar(char expected)
        {
            var c = CurrentChar == expected ? CurrentChar : null;
            _charPosition += c != null ? 1 : 0;
            return c;
        }

        public string? ConsumeDelimitedText()
        {
            if (CurrentChar != '"' && CurrentChar != '\'') return null;
            var delimiter = CurrentChar;
            _charPosition++;
            var start = _charPosition;

            while (CurrentChar.HasValue && CurrentChar != delimiter && CurrentChar != '<')
            {
                _charPosition++;
            }

            if (start >= Source.Length) return null;

            var text = Source.Substring(start, _charPosition - start);

            if (CurrentChar != '<') _charPosition++;

            return text;
        }

        public string? ConsumeTagName()
        {
            var start = _charPosition;
            while (CurrentChar.HasValue && (char.IsAsciiLetterOrDigit(CurrentChar.Value) || CurrentChar == '-'))
            {
                _charPosition++;
            }

            if (start >= Source.Length) return null;

            var tagName = Source.Substring(start, _charPosition - start);
            return tagName.Length > 0 ? tagName : null;
        }

        public string? ConsumeAttrName()
        {
            var start = _charPosition;
            while (CurrentChar.HasValue && char.IsAsciiLetter(CurrentChar.Value))
            {
                _charPosition++;
            }

            if (start >= Source.Length) return null;

            var attrName = Source.Substring(start, _charPosition - start);
            return attrName.Length > 0 ? attrName : null;
        }

        public string? ConsumeAttrValue()
        {
            if (!CurrentChar.HasValue) return null;
            return char.IsAsciiLetterOrDigit(CurrentChar.Value) ? ConsumeAsciiLetterDigitString() : ConsumeDelimitedText();
        }

        public void Empty()
        {
            _charPosition = Source.Length;
        }
    }

    public class TextState : ITokenizer
    {
        public void Run(Tokenizer tokenizer)
        {
            var text = tokenizer.ConsumeText();

            if (text != null) tokenizer.Tokens.Add(new Token(text));

            var leftArrow = tokenizer.ConsumeChar('<');
            var slash = tokenizer.ConsumeChar('/');
            if (leftArrow.HasValue && slash.HasValue)
            {
                tokenizer.State = new ClosingTagState();
            }
            else if (leftArrow.HasValue)
            {
                tokenizer.State = new TagState();
            }
        }
    }

    public class ClosingTagState : ITokenizer
    {
        public void Run(Tokenizer tokenizer)
        {
            var tagName = tokenizer.ConsumeTagName();

            if (tagName == null)
            {
                tokenizer.Empty();
                return;
            }

            tokenizer.ConsumeWhitespaces();

            if (tokenizer.ConsumeChar('>') == null)
            {
                tokenizer.Empty();
                return;
            }

            tokenizer.Tokens.Add(new Token(TokenType.END_TAG, tagName));
            tokenizer.State = new TextState();
        }
    }

    public class TagState : ITokenizer
    {
        public void Run(Tokenizer tokenizer)
        {
            var tagName = tokenizer.ConsumeTagName();
            if (tagName == null)
            {
                tokenizer.Empty();
                return;
            }

            Token tag = new Token(TokenType.TAG, tagName);
            tokenizer.ConsumeWhitespaces();
            tokenizer.ConsumeChar('/');
            if (!tokenizer.CurrentChar.HasValue) return;

            var rightArrow = tokenizer.ConsumeChar('>');
            if (rightArrow.HasValue)
            {
                tokenizer.Tokens.Add(tag);
                tokenizer.State = new TextState();
                return;
            }

            // Transition to a state where we search for attributes
            tokenizer.State = new AttributesState(tag);
        }
    }

    public class AttributesState : ITokenizer
    {
        private Token _tag;

        public AttributesState(Token tag)
        {
            _tag = tag;
        }
        public void Run(Tokenizer tokenizer)
        {
            var attrName = tokenizer.ConsumeAttrName();
            tokenizer.ConsumeChar('=');
            var attrValue = tokenizer.ConsumeAttrValue();
            tokenizer.ConsumeWhitespaces();

            while (attrName != null && tokenizer.CurrentChar != '>')
            {
                if (attrValue != null)
                {
                    _tag.Attributes.Add(attrName, attrValue);
                }
                else
                {
                    _tag.Attributes.Add(attrName, "");
                }

                attrName = tokenizer.ConsumeAttrName();
                tokenizer.ConsumeChar('=');
                attrValue = tokenizer.ConsumeAttrValue();
                tokenizer.ConsumeWhitespaces();
            }

            if (tokenizer.CurrentChar == '>')
            {
                tokenizer.Tokens.Add(_tag);
                tokenizer.State = new TextState();
            }
        }
    }
}