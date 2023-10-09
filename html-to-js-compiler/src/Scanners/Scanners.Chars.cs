
namespace HTMLToJS.Scanners
{

    public static partial class Scanners
    {
        public delegate bool ScanCharFunction(char c);
        public static class Chars
        {
            public static ScanCharFunction Of(char c)
            {
                return (ch) => c == ch;
            }

            public static ScanCharFunction OneOf(params char[] chars) => OneOf(chars.AsEnumerable());

            public static ScanCharFunction OneOf(params IEnumerable<char>[] charSets)
            {
                HashSet<char> allowed = new(Set(charSets));
                return allowed.Contains;
            }

            public static ScanCharFunction Any() => (ch) => true;

            public static IEnumerable<char> List(params char[] characters) => new List<char>(characters);

            public static IEnumerable<char> Range(char start, char end)
            {
                List<char> chars = new();
                for (var c = start; c <= end; c++) chars.Add(c);

                return chars;
            }

            private static IEnumerable<char> Set(params IEnumerable<char>[] ranges)
            {
                List<char> temp = new();
                foreach (var range in ranges) temp.AddRange(range);
                return new HashSet<char>(temp);
            }

            public static IEnumerable<char> LowerAsciiLetters = Range('a', 'z');
            public static IEnumerable<char> UpperAsciiLetters = Range('A', 'Z');
            public static IEnumerable<char> AsciiDigits = Range('0', '9');
            public static IEnumerable<char> AsciiLettersDigits = Set(LowerAsciiLetters, UpperAsciiLetters, AsciiDigits);
            public static IEnumerable<char> AsciiLetters = Set(LowerAsciiLetters, UpperAsciiLetters);
        }

        public static ScanFunction toScanFunction(this ScanCharFunction scanChar) {
            return (string source, int start) => start < source.Length && scanChar(source[start]) ? (true, start + 1) : (false, start);
        }

        public static ScanCharFunction Except(this ScanCharFunction scan, params char[] chars)
        {
            return scan.Except(chars.AsEnumerable());
        }

        public static ScanCharFunction Except(this ScanCharFunction scan, IEnumerable<char> chars)
        {
            HashSet<char> forbidden = new(chars);
            return (ch) => !forbidden.Contains(ch) && scan(ch);
        }
    }
}