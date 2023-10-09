
namespace HTMLToJS.Scanners {

    public static partial class Scanners {

        /// <summary>
        /// Combines multiple scanners into one
        /// that tries to match one of the multiple
        /// scanners. It tries all of them one by one
        /// returning the result of the first one to be
        /// successful.
        /// </summary>
        /// <param name="scanners">The ScanFunctions to match</param>
        /// <returns>
        /// A ScanFunction that returns the offset of the first scanner
        /// that matched or failure if none does.
        /// </returns>
        public static ScanFunction FirstOf(params ScanFunction[] scanners) {
            return (string source, int start) => {
                foreach(var scanner in scanners) {
                    var (sucess, offset) = scanner(source, start);
                    if (sucess) return (true, offset);
                }
                return (false, start);
            };
        }

        /// <summary>
        /// Transforms a sequence of ScanFunction into a single
        /// ScanFunction that tries to match all of them in
        /// sequence, applying them one after the other and bringing
        /// the offset of the past one into the next.
        /// </summary>
        /// <param name="scanners">A sequence of ScanFunction to match in the specified order</param>
        /// <returns>
        /// A ScanFunction that is successful if it matches the whole sequence
        /// of scanners, returning the offset of where it stopped. Otherwise
        /// it fails and returns the starting offset.
        /// </returns>
        public static ScanFunction SequenceOf(params ScanFunction[] scanners) {
            return (string source, int start) => {
                var lastOffset = start;
                foreach(var scanner in scanners) {
                    var (sucess, offset) = scanner(source, lastOffset);
                    if (!sucess) return (false, start);
                    lastOffset = offset;
                }
                return (true, lastOffset);
            };
        }
    }
}