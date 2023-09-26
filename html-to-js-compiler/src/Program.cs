// See https://aka.ms/new-console-template for more information

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

    public class Option<T>
    {
        private readonly T _value;
        private readonly bool _hasValue;


        private Option()
        {
            _hasValue = false;
        }


        private Option(T value)
        {
            _value = value;
            _hasValue = true;
        }


        public static Option<T> None => new Option<T>();


        public static Option<T> Some(T value) => new Option<T>(value);


        public bool HasValue => _hasValue;


        public T Value
        {
            get
            {
                if (!_hasValue)
                {
                    throw new InvalidOperationException("Option has no value.");
                }


                return _value;
            }
        }


        public Option<TResult> Map<TResult>(Func<T, TResult> func)
        {
            if (!_hasValue)
            {
                return Option<TResult>.None;
            }


            return Option<TResult>.Some(func(_value));
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

        public Option<char> Consume()
        {
            if (CharPosition >= Source.Length) return Option<char>.None;
            return Option<char>.Some(Source[_charPosition++]);
        }

        public Option<char> Peek() {
            if (CharPosition >= Source.Length) return Option<char>.None;
            return Option<char>.Some(Source[_charPosition]);
        }
    }

    public class SearchState : ITokenizerState
    {
        public void Run(Tokenizer tokenizer)
        {
            while (tokenizer.CharPosition <= tokenizer.Source.Length)
            {
                var current = tokenizer.Consume();
                var next = tokenizer.Peek();

                // No point in continuing. There is nothing left
                if (!next.HasValue) return;

                // This is normal text
                if (current.Value != '<') {
                    
                    continue;
                }

                // This is the start of a closing tag
                if (next.Value == '\\') {

                    continue;
                }

                // This is the start of a tag
                tokenizer.State = new TagState();
            }
        }
    }

    public class TagState : ITokenizerState
    {
        public void Run(Tokenizer tokenizer)
        {
            // We expect a sequence of at least one alphanumeric characters
            // For the tag name.
            StringBuilder tagName = new StringBuilder();
            var firstChar = tokenizer.Consume();

            // No tag name. We simply return.
            if (!firstChar.HasValue || !Char.IsAsciiLetter(firstChar.Value)) return;

            // We have a tag name. Append its first char and search for the others
            tagName.Append(firstChar);
            var nextChar = tokenizer.Consume();
            while (nextChar.HasValue && Char.IsAsciiLetter(nextChar.Value)) {
                tagName.Append(nextChar.Value);
                nextChar = tokenizer.Consume();
            }

            // If there is no input left we quit
            if (!nextChar.HasValue) return;

            // We skip whitspaces until we find the next char
            while (nextChar.HasValue && Char.IsWhiteSpace(nextChar.Value)) {
                nextChar = tokenizer.Consume();
            }

            // If the character closes the tag we return to Search State
            if (nextChar.Value == '>') {
                tokenizer.State = new SearchState();
            }

            // Otherwise we start searching for the tag attributes
        }
    }
}