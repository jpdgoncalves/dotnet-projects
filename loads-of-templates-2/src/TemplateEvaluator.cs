
using System.Text;
using StringScanners;
using static StringScanners.Scanners;

namespace LoadOfTemplates {

    public class TemplateEvaluator {

        private ScanFunction scanFunction;
        private ScannerCallbacks textCallbacks;
        private ScannerCallbacks templateTextCallbacks;
        
        public TemplateEvaluator() {
            var text = Expression(@"(.*){{");
            var templateText = Expression(@"(:*)}}");
            var templateTag = SequenceOf(
                Str("{{"),
                templateText,
                Str("}}")
            );

            textCallbacks = text.WithCallbacks();
            templateTextCallbacks = templateText.WithCallbacks();

            scanFunction = ZeroOrMore(FirstOf(
                templateTag,
                text
            ));
        }

        public string Evaluate(string input) {
            StringBuilder output = new();

            textCallbacks.OnSuccess = (source, start, offset) => TextCallback(output, source, start, offset);
            templateTextCallbacks.OnSuccess = (source, start, offset) => TemplateTextCallback(output, source, start, offset);
            scanFunction(input, 0);

            return output.ToString();
        }

        private void TextCallback(StringBuilder output, string source, int start, int offset) {

        }

        private void TemplateTextCallback(StringBuilder output, string source, int start, int offset) {

        }
    }
}