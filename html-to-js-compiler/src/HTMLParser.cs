
using HTMLToJS.Scanners;
using static HTMLToJS.Scanners.Scanners;

namespace HTMLToJS
{
    public class HTMLParser {

        private ScanFunction parser;
        private HTMLWalker walker = new HTMLWalker();
        
        public HTMLParser() {
            var singleQuoteChar = CharScanner.Of('\'');
            var doubleQuoteChar = CharScanner.Of('"');

            var tagNameChar = CharScanner.AsciiLettersDigits.And('-');
            var attrNameChar = CharScanner.AsciiLettersDigits;
            var attrValueChar = CharScanner.AsciiLettersDigits;

            var whitespaces = CharScanner.AsciiWhitespaces;
            var tagName = OneOrMore(tagNameChar);

            parser = SequenceOf(
                CharScanner.Of('<'),
                tagName.WithSuccess(walker.WalkTagName),
                ZeroOrMore(whitespaces),
                CharScanner.Of('>')
            );
        }

        public void Parse(string source) {
            parser(source, 0);
        }
    }

    public class HTMLWalker {
        public void WalkTagName(string source, int start, int offset) {
            Console.WriteLine(source.Substring(start, offset - start));
        }
    }

    public class HTMLNode
    {
        public readonly HTMLNode? Parent;
        public readonly HTMLNodeType Type;
        public readonly string? Name;
        public readonly string? InnerText;
        public readonly Dictionary<string, string> Attributes = new();
        public readonly List<string> Arguments = new();
        public readonly List<HTMLNode> Children = new();

        private HTMLNode(
            HTMLNodeType type, HTMLNode? parent = null,
            string? name = null, string? innerText = null
        )
        {
            Type = type;
            Parent = parent;
            Name = name;
            InnerText = innerText;
        }

        public static HTMLNode MakeTag(string name, HTMLNode? parent) => new HTMLNode(
            type: HTMLNodeType.TAG,
            parent: parent,
            name: name
        );
        public static HTMLNode MakeText(string innerText, HTMLNode? parent) => new HTMLNode(
            type: HTMLNodeType.TEXT,
            parent: parent,
            innerText: innerText
        );

        public static HTMLNode MakeComponent(string name, HTMLNode? parent) => new HTMLNode(
            type: HTMLNodeType.COMPONENT,
            parent: parent,
            name: name
        );

        public HTMLNode? FindClosestAncestorByName(string name)
        {
            if (Name == name) return this;
            if (Parent == null) return null;
            return Parent.FindClosestAncestorByName(name);
        }

        public enum HTMLNodeType { TAG, COMPONENT, TEXT }
    }
}