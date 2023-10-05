
namespace HTMLToJS {

    public static partial class Scanners {
        public delegate (bool success, int offset) ScanFunction(string source, int start);
        public delegate bool ScanCharCondition(char c);

        public static ScanFunction Char() {
            return (string source, int start) => start < source.Length ? (true, start + 1) : (false, start);
        }

        public static ScanFunction Char(char c) {
            return (string source, int start) => {
                return start < source.Length && source[start] == c ? (true, start + 1) : (false, start);
            };
        }

        public static ScanFunction Char(ScanCharCondition condition) {
            return (string source, int start) => {
                return start < source.Length && condition(source[start]) ? (true, start + 1) : (false, start);
            };
        }

        public static ScanFunction Str(string str) {
            return (string source, int start) => {
                int i = 0;
                while (i < str.Length) {
                    if (start + i >= source.Length || str[i] != source[start + i]) return (false, start);
                    i++;
                }
                return (true, start + i);
            };
        }
    }
}