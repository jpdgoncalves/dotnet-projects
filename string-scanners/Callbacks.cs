
namespace StringScanners
{
    public static partial class Scanners
    {
        public delegate void ScanFunctionCallback(string source, int start, int offset);

        /// <summary>
        /// Creates a ScanFunction which when scanner is successful, invokes the callback
        /// with the result.
        /// </summary>
        public static ScanFunction WithSuccess(this ScanFunction scanner, ScanFunctionCallback success)
        {
            return (string source, int start) =>
            {
                var result = scanner(source, start);
                if (result.success) success(source, start, result.offset);
                return result;
            };
        }

        /// <summary>
        /// Creates a ScanFunction which when scanner fails, invokes the callback
        /// with the failure result.
        /// </summary>
        /// <param name="scanner"></param>
        /// <param name="failure"></param>
        /// <returns></returns>
        public static ScanFunction WithFailure(this ScanFunction scanner, ScanFunctionCallback failure)
        {
            return (string source, int start) =>
            {
                var result = scanner(source, start);
                if (!result.success) failure(source, start, result.offset);
                return result;
            };
        }

        /// <summary>
        /// Creates a ScanFunction which when the scanner succeeds it invokes the
        /// success callback, and when it fails it invokes the failure callback.
        /// </summary>
        public static ScanFunction With(this ScanFunction scanner, ScanFunctionCallback success, ScanFunctionCallback failure)
        {
            return (string source, int start) =>
            {
                var result = scanner(source, start);
                if (result.success) success(source, start, result.offset);
                else failure(source, start, result.offset);
                return result;
            };
        }
    }
}