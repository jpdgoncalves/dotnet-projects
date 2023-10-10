
namespace HTMLToJS.Scanners
{
    public static class CharSets
    {
        public static IEnumerable<char> List(params char[] chars) => chars.AsEnumerable();

        public static IEnumerable<char> Range(char start, char end)
        {
            List<char> chars = new();
            for (var c = start; c <= end; c++) chars.Add(c);
            return chars;
        }

        public static IEnumerable<char> Combine(params IEnumerable<char>[] charSets)
        {
            List<char> temp = new();
            foreach (var set in charSets) temp.AddRange(set);

            return new HashSet<char>(temp);
        }

        public static IEnumerable<char> AsciiLowerLetters = Range('a', 'z');
        public static IEnumerable<char> AsciiUpperLetters = Range('A', 'Z');
        public static IEnumerable<char> AsciiDigits = Range('0', '9');
        public static IEnumerable<char> AsciiWhitespaces = List(' ', '\t', '\r', '\n');
        public static IEnumerable<char> AsciiLetters = Combine(AsciiLowerLetters, AsciiUpperLetters);
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

        public (bool success, int offset) Scan(string source, int start)
        {
            return start < source.Length && _expected.Contains(source[start]) ? (true, start + 1) : (false, start);
        }

        public CharScanner And(params char[] chars) => And(chars.AsEnumerable());

        public CharScanner And(params IEnumerable<char>[] charSets)
        {
            var combined = new CharScanner();
            combined._anyChar = _anyChar;
            combined.Expect(_expected);
            combined.Forbid(_forbidden);
            foreach (var set in charSets) combined.Expect(set);
            return combined;
        }

        public CharScanner And(params CharScanner[] others) {
            var combined = new CharScanner();
            combined._anyChar = _anyChar;
            combined.Expect(_expected);
            combined.Forbid(_forbidden);

            foreach (var other in others) {
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

        public CharScanner Except(params CharScanner[] others) {
            var combined = new CharScanner();
            combined._anyChar = _anyChar;
            combined.Expect(_expected);
            combined.Forbid(_forbidden);

            foreach (var other in others) {
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

        public static CharScanner NotOf(params char[] chars) => NotOf(chars.AsEnumerable());

        public static CharScanner NotOf(params IEnumerable<char>[] charSets)
        {
            CharScanner scanner = new();
            scanner._anyChar = true;
            foreach (var set in charSets) scanner.Forbid(set);
            return scanner;
        }

        public static CharScanner Any = new CharScanner{_anyChar = true};
        public static CharScanner AsciiLowerLetters = Of(CharSets.AsciiLowerLetters);
        public static CharScanner AsciiUpperLetters = Of(CharSets.AsciiUpperLetters);
        public static CharScanner AsciiDigits = Of(CharSets.AsciiDigits);
        public static CharScanner AsciiWhitespaces = Of(CharSets.AsciiWhitespaces);
        public static CharScanner AsciiLetters = Of(CharSets.AsciiLetters);
        public static CharScanner AsciiLettersDigits = Of(CharSets.AsciiLettersDigits);

        public static implicit operator Scanners.ScanFunction(CharScanner scanner) {
            return (string source, int start) => {
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

        public static ScanFunction Str(string pattern) {
            return (string source, int start) => {
                var i = 0;

                return (i == pattern.Length, start + i);
            };
        }
    }
}