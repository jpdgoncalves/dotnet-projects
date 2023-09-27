// See https://aka.ms/new-console-template for more information

using System.Linq.Expressions;
using System.Reflection.Metadata.Ecma335;
using System.Text;

namespace HTMLToJS
{

    public class Program
    {
        public static void Main(string[] args)
        {
            string filename = "example.html";
            string filecontent;

            using (var sr = new StreamReader(filename))
            {
                filecontent = sr.ReadToEnd();
            }


        }
    }

    public interface ITokenizerState
    {
        public void Run(Tokenizer tokenizer);
    }

    public class Tokenizer
    {
        public ITokenizerState State { get; set; }
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

        public Tokenizer(ITokenizerState startingState, string source)
        {
            Source = source;
            _charPosition = 0;
            State = startingState;
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
            if (CurrentChar != '"' || CurrentChar != '\'') return null;
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

    public class SearchState : ITokenizerState
    {
        public void Run(Tokenizer tokenizer)
        {
            while (tokenizer.CharPosition <= tokenizer.Source.Length)
            {
                // For now ignore raw text in the uppermost level
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
    }

    public class ClosingTagState : ITokenizerState
    {
        public void Run(Tokenizer tokenizer)
        {
            throw new NotImplementedException();
        }
    }

    public class TagState : ITokenizerState
    {
        public void Run(Tokenizer tokenizer)
        {
            var tagName = tokenizer.ConsumeTagName();
            if (tagName == null) {
                tokenizer.Empty();
                return;
            }

            Token tag = new Token(TokenType.TAG, tagName);
            tokenizer.ConsumeWhitespaces();
            tokenizer.ConsumeChar('/');

            if (!tokenizer.CurrentChar.HasValue) return;
            if (tokenizer.CurrentChar == '>') {
                tokenizer.State = new SearchState();
                return;
            }

            // Transition to a state where we search for attributes
        }
    }

    
}