
namespace HTMLToJS.Scanners {

    public static partial class Scanners {
        public delegate (bool success, int offset) ScanFunction(string source, int start);

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