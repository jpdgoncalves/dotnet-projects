
using static StringScanners.Scanners;
using static StringScanners.ScannerCallbacks;

namespace StringScanners
{
    public class ScannerCallbacks {
        public delegate void ScanFunctionCallback(string source, int start, int offset);

        private ScanFunction _scanFunction;
        public ScanFunctionCallback? OnSuccess;
        public ScanFunctionCallback? OnFailure;

        public ScannerCallbacks(ScanFunction scanFunction) {
            _scanFunction = scanFunction;
        }

        public static implicit operator ScanFunction(ScannerCallbacks callbacks) {
            if (callbacks.OnSuccess is not null && callbacks.OnFailure is not null) {
                return (source, start) => {
                    var (success, offset) = callbacks._scanFunction(source, start);
                    if (success) callbacks.OnSuccess(source, start, offset);
                    else callbacks.OnFailure(source, start, offset);
                    return (success, offset);
                };
            } else if (callbacks.OnSuccess is not null) {
                return (source, start) => {
                    var (success, offset) = callbacks._scanFunction(source, start);
                    if (success) callbacks.OnSuccess(source, start, offset);
                    return (success, offset);
                };
            } else if (callbacks.OnFailure is not null) {
                return (source, start) => {
                    var (success, offset) = callbacks._scanFunction(source, start);
                    if (!success) callbacks.OnFailure(source, start, offset);
                    return (success, offset);
                };
            } else {
                return callbacks._scanFunction;
            }
        }
    }

    public static partial class Scanners
    {
        /// <summary>
        /// Creates a ScannerCallbacks objects that allows to associate
        /// delegates as callbacks for success andor failure cases
        /// </summary>
        /// <param name="scanner"></param>
        public static ScannerCallbacks WithCallbacks(this ScanFunction scanner) {
            return new ScannerCallbacks(scanner);
        }
    }
}