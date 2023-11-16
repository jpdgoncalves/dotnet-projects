
using static StringScanners.Scanners;

namespace StringScanners
{
    /// <summary>
    /// Class used to handle callback functionality
    /// </summary>
    public class ScannerCallbacks
    {
        public delegate void ScanFunctionCallback(string source, int start, int offset);

        private ScanFunction _scanFunction;
        /// <summary>
        /// Callback invoked when the scan function is successful
        /// </summary>
        public ScanFunctionCallback? OnSuccess;
        /// <summary>
        /// Callback invoked when the scan function is not successful
        /// </summary>
        public ScanFunctionCallback? OnFailure;

        public ScannerCallbacks(ScanFunction scanFunction)
        {
            _scanFunction = scanFunction;
        }

        public static implicit operator ScanFunction(ScannerCallbacks callbacks)
        {
            return (source, start) =>
            {
                var (success, offset) = callbacks._scanFunction(source, start);
                if (success && callbacks.OnSuccess is not null) callbacks.OnSuccess(source, start, offset);
                else if (callbacks.OnFailure is not null) callbacks.OnFailure(source, start, offset);
                return (success, offset);
            };
        }
    }

    public static partial class Scanners
    {
        /// <summary>
        /// Creates a ScannerCallbacks objects that allows to associate
        /// delegates as callbacks for success andor failure cases
        /// </summary>
        /// <param name="scanner"></param>
        public static ScannerCallbacks WithCallbacks(this ScanFunction scanner)
        {
            return new ScannerCallbacks(scanner);
        }
    }
}