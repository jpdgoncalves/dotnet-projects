
namespace StringScanners
{

    public abstract class BaseScanner
    {
        public delegate void StringScannerCallback(string source, int start, int length);
        public readonly StringScannerCallback successDelegate = (source, start, length) => { };
        public readonly StringScannerCallback failureDelegate = (source, start, length) => { };
        public abstract (bool success, int length) scan(string source, int start);
    }

    internal class InternalCharScanner : BaseScanner
    {
        public override (bool success, int length) scan(string source, int start)
        {
            throw new NotImplementedException();
        }
    }
}