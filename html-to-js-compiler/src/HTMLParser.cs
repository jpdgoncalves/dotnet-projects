
using System.Text;
using HTMLToJS.Scanners;
using static HTMLToJS.Scanners.Scanners;

namespace HTMLToJS
{
    public class HTMLParser
    {

        private ScanFunction parser;
        private HTMLWalker walker = new HTMLWalker();

        public HTMLParser()
        {
            var singleQuoteChar = CharScanner.Of('\'');
            var doubleQuoteChar = CharScanner.Of('"');
            var anyChar = CharScanner.Any;

            var tagNameChar = CharScanner.AsciiLettersDigits.And('-');
            var attrNameChar = CharScanner.AsciiLettersDigits.And('-');
            var attrValueChar = CharScanner.AsciiLettersDigits;
            var whitespaceChar = CharScanner.AsciiWhitespaces;

            var attrValueUnquoted = OneOrMore(attrValueChar).WithSuccess(walker.WalkAttrValue);
            var attrValueSingleQuote = Sequence(
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
                attrValueSingleQuote,
                attrValueDoubleQuote,
                attrValueUnquoted
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
            var text = OneOrMore(anyChar.Except('<')).WithSuccess(walker.WalkText);
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

        public HTMLNode Parse(string source)
        {
            walker.Root = HTMLNode.MakeTag("root");
            parser(source, 0);
            return walker.Root;
        }
    }

    public class HTMLWalker
    {
        private static HashSet<string> _VoidElements = new HashSet<string>{
            "area", "base", "br", "col",
            "command", "embed", "hr", "img",
            "input", "keygen", "link", "meta",
            "param", "source", "track", "wbr"
        };
        private HTMLNode _root = HTMLNode.MakeTag("root");
        public HTMLNode Root
        {
            get { return _root; }
            set
            {
                _root = value;
                _current = value;
            }
        }
        private HTMLNode _current;

        private string _curAttrName = "";

        public HTMLWalker()
        {
            _current = _root;
        }

        public void WalkTag(string source, int start, int offset)
        {
            var tagName = source.Substring(start, offset - start);
            var tag = HTMLNode.MakeTag(tagName, parent: _current);
            _current.Children.Add(tag);
            if (!_VoidElements.Contains(tagName)) _current = tag;
        }

        public void WalkText(string source, int start, int offset)
        {
            var text = source.Substring(start, offset - start);
            var tag = HTMLNode.MakeText(text, parent: _current);
            _current.Children.Add(tag);
        }

        public void WalkAttrName(string source, int start, int offset)
        {
            _curAttrName = source.Substring(start, offset - start);
            _current.Attributes[_curAttrName] = "";
        }

        public void WalkAttrValue(string source, int start, int offset)
        {
            var value = source.Substring(start, offset - start);
            _current.Attributes[_curAttrName] = value;
        }

        public void WalkEndTag(string source, int start, int offset)
        {
            var tagName = source.Substring(start, offset - start);
            var closestParent = _current.FindClosestAncestorByName(tagName);
            _current = closestParent != null && closestParent.Parent != null ? closestParent.Parent : Root;
        }

        public void WalkComment(string source, int start, int offset)
        {
            Console.WriteLine($"{{{{COMMENT {source.Substring(start, offset - start)}}}}}");
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

        public static HTMLNode MakeTag(string name, HTMLNode? parent = null) => new HTMLNode(
            type: HTMLNodeType.TAG,
            parent: parent,
            name: name
        );
        public static HTMLNode MakeText(string innerText, HTMLNode? parent = null) => new HTMLNode(
            type: HTMLNodeType.TEXT,
            parent: parent,
            innerText: innerText
        );

        public HTMLNode? FindClosestAncestorByName(string name)
        {
            if (Name != null && Name.Equals(name)) return this;
            if (Parent == null) return null;
            return Parent.FindClosestAncestorByName(name);
        }

        public override string ToString()
        {
            return InternalToString();
        }

        private string InternalToString(string indent = "")
        {
            StringBuilder builder = new();
            builder.Append($"{indent}HTMLNode: Type {Type}, Name {Name}, Parent {(Parent != null ? Parent.Name : null)}, Innertext {InnerText}\n");
            foreach (var (key, value) in Attributes)
            {
                builder.Append(indent);
                builder.Append("  ");
                builder.Append($"- AttrName {key}, Value: {value}\n");
            }

            foreach (var child in Children)
            {
                builder.Append(child.InternalToString(indent + "  "));
            }

            return builder.ToString();
        }

        public enum HTMLNodeType { TAG, TEXT }
    }
}