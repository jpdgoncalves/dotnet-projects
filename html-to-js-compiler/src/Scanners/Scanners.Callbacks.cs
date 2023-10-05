
namespace HTMLToJS
{

    public static partial class Scanners
    {
        public delegate void ScanFunctionCallback(string source, int start, int offset);

        public static ScanFunction SuccessCallback(this ScanFunction scanner, ScanFunctionCallback success)
        {
            return (string source, int start) =>
            {
                var result = scanner(source, start);
                if (result.success) success(source, start, result.offset);
                return result;
            };
        }

        public static ScanFunction FailureCallback(this ScanFunction scanner, ScanFunctionCallback failure)
        {
            return (string source, int start) =>
            {
                var result = scanner(source, start);
                if (!result.success) failure(source, start, result.offset);
                return result;
            };
        }

        public static ScanFunction Callbacks(this ScanFunction scanner, ScanFunctionCallback success, ScanFunctionCallback failure)
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