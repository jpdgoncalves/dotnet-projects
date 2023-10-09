
namespace HTMLToJS.Scanners
{

    public static partial class Scanners
    {
        public delegate void ScanFunctionCallback(string source, int start, int offset);

        public static ScanFunction WithSuccess(this ScanFunction scanner, ScanFunctionCallback success)
        {
            return (string source, int start) =>
            {
                var result = scanner(source, start);
                if (result.success) success(source, start, result.offset);
                return result;
            };
        }

        public static ScanFunction WithFailure(this ScanFunction scanner, ScanFunctionCallback failure)
        {
            return (string source, int start) =>
            {
                var result = scanner(source, start);
                if (!result.success) failure(source, start, result.offset);
                return result;
            };
        }

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