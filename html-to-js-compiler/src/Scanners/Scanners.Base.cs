
namespace HtmlToJs.Scanners
{
    public static class CharSets
    {
        /// <summary>
        /// Creates a IEnumerable from the provided list of chars;
        /// </summary>
        /// <param name="chars">The chars inserted into the list</param>
        /// <returns>An INEnumerable containing the chars</returns>
        public static IEnumerable<char> List(params char[] chars) => chars.AsEnumerable();
        /// <summary>
        /// Creates an IEnumerable between a start and end char
        /// including the start and end themselves
        /// </summary>
        /// <param name="start">The starting character</param>
        /// <param name="end">The ending character</param>
        /// <returns>An IEnumerable containing the specified range of characters</returns>
        public static IEnumerable<char> Range(char start, char end)
        {
            List<char> chars = new();
            for (var c = start; c <= end; c++) chars.Add(c);
            return chars;
        }

        /// <summary>
        /// Utility to combine different lists of characters into one
        /// that only contains unique ones
        /// </summary>
        /// <param name="charSets">The lists of characters to combine</param>
        /// <returns>An IEnumerable containing all the unique characters</returns>
        public static IEnumerable<char> Combine(params IEnumerable<char>[] charSets)
        {
            List<char> temp = new();
            foreach (var set in charSets) temp.AddRange(set);

            return new HashSet<char>(temp);
        }

        /// <summary>
        /// List of characters from a to z (lowercase)
        /// </summary>
        public static IEnumerable<char> AsciiLowerLetters = Range('a', 'z');
        /// <summary>
        /// A list of characters from A to Z (uppercase)
        /// </summary>
        public static IEnumerable<char> AsciiUpperLetters = Range('A', 'Z');
        /// <summary>
        /// A list containing the digits from 0 to 9
        /// </summary>
        public static IEnumerable<char> AsciiDigits = Range('0', '9');
        /// <summary>
        /// A list containing all the whitespace characters
        /// </summary>
        public static IEnumerable<char> AsciiWhitespaces = List(' ', '\t', '\r', '\n');
        /// <summary>
        /// A list containing the letters from a to z (lowercase and uppercase)
        /// </summary>
        public static IEnumerable<char> AsciiLetters = Combine(AsciiLowerLetters, AsciiUpperLetters);
        /// <summary>
        /// A list containing the letters from a to z (lower and uppercase) and the
        /// digits from 0 to 9
        /// </summary>
        public static IEnumerable<char> AsciiLettersDigits = Combine(AsciiLetters, AsciiDigits);
    }

    public class CharScanner
    {
        private bool _anyChar = false;
        private HashSet<char> _expected = new();
        private HashSet<char> _forbidden = new();

        private CharScanner() { }

        private void Expect(IEnumerable<char> chars)
        {
            foreach (var c in chars)
            {
                _expected.Add(c);
                _forbidden.Remove(c);
            }
        }

        private void Forbid(IEnumerable<char> chars)
        {
            foreach (var c in chars)
            {
                _forbidden.Add(c);
                _expected.Remove(c);
            }
        }

        /// <summary>
        /// Creates a CharScanner that extends the currently
        /// expected characters with the ones provided
        /// </summary>
        /// <param name="chars">The extension characters</param>
        /// <returns>
        /// A CharScanner that expects all the characters it was searching
        /// before and the ones provided. Characters that were not supposed
        /// to appear before will be lost if they coincide with provided
        /// characters
        /// </returns>
        public CharScanner And(params char[] chars) => And(chars.AsEnumerable());

        /// <summary>
        /// Creates a CharScanner that extends the currently
        /// expected characters with the ones provided
        /// </summary>
        /// <param name="charSets">The extension characters</param>
        /// <returns>
        /// A CharScanner that expects all the characters it was searching
        /// before and the ones provided. Characters that were not supposed
        /// to appear before will be lost if they coincide with provided
        /// characters
        /// </returns>
        public CharScanner And(params IEnumerable<char>[] charSets)
        {
            var combined = new CharScanner();
            combined._anyChar = _anyChar;
            combined.Expect(_expected);
            combined.Forbid(_forbidden);
            foreach (var set in charSets) combined.Expect(set);
            return combined;
        }

        /// <summary>
        /// Creates a CharScanner that combines all of the
        /// previous ones in sequence.
        /// </summary>
        /// <param name="others">CharScanners to combine with.</param>
        /// <returns></returns>
        public CharScanner And(params CharScanner[] others)
        {
            var combined = new CharScanner();
            combined._anyChar = _anyChar;
            combined.Expect(_expected);
            combined.Forbid(_forbidden);

            foreach (var other in others)
            {
                combined._anyChar |= other._anyChar;
                combined.Expect(other._expected);
                combined.Forbid(other._forbidden);
            }

            return combined;
        }

        public CharScanner Except(params char[] chars) => Except(chars.AsEnumerable());

        public CharScanner Except(params IEnumerable<char>[] charSets)
        {
            var combined = new CharScanner();
            combined._anyChar = _anyChar;
            combined.Expect(_expected);
            combined.Forbid(_forbidden);
            foreach (var set in charSets) combined.Forbid(set);
            return combined;
        }

        public CharScanner Except(params CharScanner[] others)
        {
            var combined = new CharScanner();
            combined._anyChar = _anyChar;
            combined.Expect(_expected);
            combined.Forbid(_forbidden);

            foreach (var other in others)
            {
                combined._anyChar |= other._anyChar;
                combined.Expect(other._forbidden);
                combined.Forbid(other._expected);
            }

            return combined;
        }

        public static CharScanner Of(params char[] chars) => Of(chars.AsEnumerable());

        public static CharScanner Of(params IEnumerable<char>[] charSets)
        {
            CharScanner scanner = new();
            foreach (var set in charSets) scanner.Expect(set);
            return scanner;
        }

        /// <summary>
        /// Creates a CharScanner that expects any character except
        /// the ones provided.
        /// </summary>
        /// <param name="chars">The characters that are not expected.</param>
        public static CharScanner NotOf(params char[] chars) => NotOf(chars.AsEnumerable());

        /// <summary>
        /// Creates a CharScanner that expects any character except
        /// the ones provided.
        /// </summary>
        /// <param name="chars">The characters that are not expected.</param>
        public static CharScanner NotOf(params IEnumerable<char>[] charSets)
        {
            CharScanner scanner = new();
            scanner._anyChar = true;
            foreach (var set in charSets) scanner.Forbid(set);
            return scanner;
        }

        /// <summary>
        /// A CharScanner that accepts any character.
        /// </summary>
        public static CharScanner Any = new CharScanner { _anyChar = true };
        public static CharScanner AsciiLowerLetters = Of(CharSets.AsciiLowerLetters);
        public static CharScanner AsciiUpperLetters = Of(CharSets.AsciiUpperLetters);
        public static CharScanner AsciiDigits = Of(CharSets.AsciiDigits);
        public static CharScanner AsciiWhitespaces = Of(CharSets.AsciiWhitespaces);
        public static CharScanner AsciiLetters = Of(CharSets.AsciiLetters);
        public static CharScanner AsciiLettersDigits = Of(CharSets.AsciiLettersDigits);

        public static implicit operator Scanners.ScanFunction(CharScanner scanner)
        {
            return (string source, int start) =>
            {
                if (start >= source.Length) return (false, start);
                if (!scanner._anyChar && !scanner._expected.Contains(source[start])) return (false, start);
                if (scanner._forbidden.Contains(source[start])) return (false, start);
                return (true, start + 1);
            };
        }
    }

    public static partial class Scanners
    {
        public delegate (bool success, int offset) ScanFunction(string source, int start);

        /// <summary>
        /// A Scan Function that expects the exact string that was provided.
        /// </summary>
        /// <param name="pattern">The expected string</param>
        /// <returns></returns>
        public static ScanFunction Str(string pattern)
        {
            return (string source, int start) =>
            {
                var i = 0;
                for (; i < pattern.Length && start + i < source.Length; i++)
                {
                    if (pattern[i] != source[start + i]) return (false, start + i);
                }
                return (i == pattern.Length, start + i);
            };
        }
    }
}