
namespace HTMLToJS.Scanners {
    
    public static partial class Scanners {

        public static ScanFunction Maybe(ScanCharFunction scanChar) {
            return (source, start) => start < source.Length && scanChar(source[start]) ? (true, start + 1) : (false, start);
        }

        /// <summary>
        /// Turns a scanFunction into one that
        /// is always successful.
        /// </summary>
        /// <param name="scanner">The ScanFunction to transform</param>
        /// <returns>
        /// A ScanFunction that is always successful. It returns a tuple
        /// that contains the end offset if scanner is successful or the
        /// start offset if it fails
        /// </returns>
        public static ScanFunction Maybe(ScanFunction scanner) {
            return (string source, int start) => {
                var (sucess, offset) = scanner(source, start);
                return sucess ? (true, offset) : (true, start);
            };
        }

        public static ScanFunction Not(ScanCharFunction scanChar) {
            return (source, start) => start < source.Length && !scanChar(source[start]) ? (true, start + 1) : (false, start);
        }

        /// <summary>
        /// Transforms a scanner into one that
        /// tries to not match the source string.
        /// </summary>
        /// <param name="scanner">The ScanFunction that must not be matched</param>
        /// <returns>
        /// A ScanFunction whose success is inverse to the success of the
        /// scanner. The offset returned is always the start.
        /// </returns>
        public static ScanFunction Not(ScanFunction scanner) {
            return (string source, int start) => {
                var (sucess, offset) = scanner(source, start);
                return sucess ? (false, start) : (true, start);
            };
        }

        public static ScanFunction ZeroOrMore(ScanCharFunction scanChar) {
            return (string source, int start) => {
                var lastOffset = start;
                while (lastOffset < source.Length) {
                    var success = scanChar(source[lastOffset]);
                    if (!success) break;
                    lastOffset += 1;
                }
                return (true, lastOffset);
            };
        }

        /// <summary>
        /// Turns a ScanFunction that matches only once into one
        /// that matches 0 or more times.
        /// </summary>
        /// <param name="scanner">The ScanFunction to be matched</param>
        /// <returns>
        /// A function is always successful. The offset it returns
        /// marks the place where the last match ends.
        /// </returns>
        public static ScanFunction ZeroOrMore(ScanFunction scanner) {
            return (string source, int start) => {
                var lastOffset = start;
                while (lastOffset < source.Length) {
                    var (success, offset) = scanner(source, lastOffset);
                    if (!success) break;
                    lastOffset = offset;
                }
                return (true, lastOffset);
            };
        }

        public static ScanFunction OneOrMore(ScanCharFunction scanChar) {
            return (string source, int start) => {
                var lastOffset = start;
                while (lastOffset < source.Length) {
                    var success = scanChar(source[lastOffset]);
                    if (!success) break;
                    lastOffset += 1;
                }
                return (lastOffset != start, lastOffset);
            };
        }

        /// <summary>
        /// Turns a ScanFunction that matches only once into one
        /// that matches 1 or more times.
        /// </summary>
        /// <param name="scanner">The ScanFunction to be matched</param>
        /// <returns>
        /// A function is always successful. The offset it returns
        /// marks the place where the last match ends.
        /// </returns>
        public static ScanFunction OneOrMore(ScanFunction scanner) {
            return (string source, int start) => {
                var lastOffset = start;
                while (lastOffset < source.Length) {
                    var (success, offset) = scanner(source, lastOffset);
                    if (!success) break;
                    lastOffset = offset;
                }
                return (lastOffset != start, lastOffset);
            };
        }
    }
}