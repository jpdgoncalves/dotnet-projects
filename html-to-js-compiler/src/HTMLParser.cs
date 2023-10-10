
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
            var anyChar = CharScanner.Any;

            var tagNameChar = CharScanner.AsciiLettersDigits.And('-');
            var attrNameChar = CharScanner.AsciiLettersDigits.And('-');
            var attrValueChar = CharScanner.AsciiLettersDigits;
            var whitespaceChar = CharScanner.AsciiWhitespaces;

            var attrValueUnquoted = OneOrMore(attrValueChar).WithSuccess(walker.WalkAttrValue);
            var attrValueSingeQuote = Sequence(
                singleQuoteChar,
                ZeroOrMore(anyChar.Except(singleQuoteChar)).WithSuccess(walker.WalkAttrValue),
                singleQuoteChar
            );
            var attrValueDoubleQuote = Sequence(
                doubleQuoteChar,
                ZeroOrMore(anyChar.Except(doubleQuoteChar)).WithSuccess(walker.WalkAttrValue),
                doubleQuoteChar
            );
            
            var tagName = OneOrMore(tagNameChar);
            var attrName = OneOrMore(attrNameChar).WithSuccess(walker.WalkAttrName);
            var attrValue = FirstOf(
                attrValueUnquoted,
                attrValueSingeQuote,
                attrValueDoubleQuote
            );

            var attr = FirstOf(
                Sequence(attrName, CharScanner.Of('='), attrValue),
                attrName
            );

            var tag = Sequence(
                CharScanner.Of('<'),
                tagName.WithSuccess(walker.WalkTag),
                Maybe(OneOrMore(Sequence(
                    OneOrMore(whitespaceChar),
                    attr
                ))),
                ZeroOrMore(whitespaceChar),
                ZeroOrMore(CharScanner.Of('/')),
                CharScanner.Of('>')
            );
            var endTag = Sequence(
                Str("</"),
                tagName.WithSuccess(walker.WalkEndTag),
                ZeroOrMore(whitespaceChar),
                CharScanner.Of('>')
            );
            var text = Sequence(anyChar.Except('<'));
            var comment = Sequence(
                Str("<!--"),
                ZeroOrMore(
                    Sequence(
                        Not(Str("-->")),
                        anyChar
                    )
                ).WithSuccess(walker.WalkComment),
                Str("-->")
            );

            parser = ZeroOrMore(FirstOf(
                comment,
                endTag,
                tag,
                text
            ));
        }

        public void Parse(string source) {
            parser(source, 0);
        }
    }

    public class HTMLWalker {
        public void WalkTag(string source, int start, int offset) {
            Console.WriteLine("TAG " + source.Substring(start, offset - start));
        }

        public void WalkAttrName(string source, int start, int offset) {
            Console.WriteLine("ATTR NAME " + source.Substring(start, offset - start));
        }

        public void WalkAttrValue(string source, int start, int offset) {
            Console.WriteLine("ATTR VALUE " + source.Substring(start, offset - start));
        }

        public void WalkEndTag(string source, int start, int offset) {
            Console.WriteLine("END TAG " + source.Substring(start, offset - start));
        }

        public void WalkComment(string source, int start, int offset) {
            Console.WriteLine("COMMENT " + source.Substring(start, offset - start));
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