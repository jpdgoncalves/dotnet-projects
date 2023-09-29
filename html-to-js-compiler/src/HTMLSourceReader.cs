namespace HTMLToJS
{
    /// <summary>
    /// This class contains functionality to read
    /// various types of sequences of characters,
    /// from single characters to sequences of
    /// ASCII letters and digits.
    /// Methods that start with "Consume" return
    /// a bool on whether they were sucessful or not.
    /// Methods that start with "Get" are guaranteed
    /// to either return a result or throw an exception.
    /// Both methods only update the cursor if they
    /// are sucessful.
    /// </summary>
    public class HTMLSourceReader
    {
        private string _source;
        public string Source { get { return _source; } }

        private int _cursor;
        public int Cursor { get { return _cursor; } }

        private char CurrentChar
        {
            get
            {
                return _source[_cursor];
            }
        }

        public bool ReachedEnd
        {
            get
            {
                return _cursor >= _source.Length;
            }
        }

        public HTMLSourceReader(string source)
        {
            _source = source;
            _cursor = 0;
        }

        /// <summary>
        /// Advance the cursor by offset characters.
        /// </summary>
        /// <param name="offset">
        /// The amount of characters to offset.
        /// If the offset is negative or the cursor has reached the end, 
        /// the cursor is not advanced.
        /// </param>
        public void Advance(int offset)
        {
            if (offset < 0) return;
            _cursor += offset;
            _cursor = _cursor >= _source.Length ? _source.Length : _cursor;
        }

        /// <summary>
        /// Consumes the remaining of the source reader.
        /// Used in error situations.
        /// </summary>
        public void ConsumeAll() {
            _cursor = _source.Length;
        }

        /// <summary>
        /// Tries to consume the symbol from the source.
        /// </summary>
        /// <param name="symbol">The character to consume.</param>
        /// <returns>True if successful and false otherwise.</returns>
        public bool ConsumeChar(char symbol)
        {
            return !ReachedEnd && _source[_cursor++] == symbol;
        }

        public char GetChar(char symbol)
        {
            if (ReachedEnd) throw new InvalidOperationException($"Reader has reached its end");
            if (_source[_cursor++] != symbol) throw new InvalidOperationException($"No '{symbol}' at the current position was found");
            return symbol;
        }

        public bool ComsumeOneOf(params char[] symbols)
        {
            if (ReachedEnd) return false;
            foreach (var symbol in symbols)
            {
                if (CurrentChar == symbol)
                {
                    _cursor++;
                    return true;
                }
            }
            return false;
        }

        public char GetOneOf(params char[] symbols)
        {
            if (ReachedEnd) throw new InvalidOperationException($"Reader has reached its end");
            foreach (var symbol in symbols)
            {
                if (CurrentChar == symbol)
                {
                    _cursor++;
                    return symbol;
                }
            }
            throw new InvalidOperationException($"None of '{string.Join(",", _source)}' were found at the current position");
        }

        public bool ConsumeWhile(char symbol)
        {
            if (ReachedEnd) return false;
            var start = _cursor;

            while (!ReachedEnd && CurrentChar == symbol)
            {
                _cursor++;
            }

            return start != _cursor;
        }

        public bool ConsumeWhitespaces()
        {
            if (ReachedEnd) return false;
            var start = _cursor;

            while (!ReachedEnd && char.IsWhiteSpace(CurrentChar))
            {
                _cursor++;
            }

            return start != _cursor;
        }

        /// <summary>
        /// Consumes the source until the symbol
        /// is found.
        /// </summary>
        /// <param name="symbol">
        /// The character to stop consuming at.
        /// </param>
        /// <returns>
        /// If it was able to consume any character from the source
        /// returns true. Returns false if otherwise.
        /// </returns>
        public bool ConsumeUntil(char symbol)
        {
            if (ReachedEnd) return false;
            var start = _cursor;

            while (!ReachedEnd && CurrentChar != symbol)
            {
                _cursor++;
            }

            return start != _cursor;
        }
        
        /// <summary>
        /// Consumes input until the symbol is found in the source
        /// and returns the result.
        /// </summary>
        /// <param name="symbol">The stop character</param>
        /// <returns>
        /// The consumed source between (inclusive) the current position of the cursor
        /// to the position where the symbol was found (exclusive).
        /// </returns>
        /// <exception cref="InvalidOperationException"></exception>
        public string GetUntil(char symbol, bool emptyException = true)
        {
            if (ReachedEnd) throw new InvalidOperationException($"Reader has reached its end");
            var start = _cursor;

            while (!ReachedEnd && CurrentChar != symbol)
            {
                _cursor++;
            }

            if (start == _cursor && emptyException) throw new InvalidOperationException($"No input was read");

            return _source.Substring(start, _cursor - start);
        }

        public string GetASCIILetterDigitSequence(bool emptyException = true) {
            if (ReachedEnd) throw new InvalidOperationException($"Reader has reached its end");
            var start = _cursor;

            while (!ReachedEnd && char.IsAsciiLetterOrDigit(CurrentChar))
            {
                _cursor++;
            }

            if (start == _cursor && emptyException) throw new InvalidOperationException($"No input was read");

            return _source.Substring(start, _cursor - start);
        }

        public string GetASCIILetterSequence(bool emptyException = true) {
            if (ReachedEnd) throw new InvalidOperationException($"Reader has reached its end");
            var start = _cursor;

            while (!ReachedEnd && char.IsAsciiLetter(CurrentChar))
            {
                _cursor++;
            }

            if (start == _cursor && emptyException) throw new InvalidOperationException($"No input was read");

            return _source.Substring(start, _cursor - start);
        }

        public string GetASCIILetterNumberSequenceWith(char symbol, bool emptyException = true) {
            if (ReachedEnd) throw new InvalidOperationException($"Reader has reached its end");
            var start = _cursor;

            while (!ReachedEnd && (char.IsAsciiLetterOrDigit(CurrentChar) || CurrentChar == symbol))
            {
                _cursor++;
            }

            if (start == _cursor && emptyException) throw new InvalidOperationException($"No input was read");

            return _source.Substring(start, _cursor - start);
        }
    }
}