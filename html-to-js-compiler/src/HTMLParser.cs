using HtmlToJs.Scanners;
using static HtmlToJs.Scanners.Scanners;

namespace HtmlToJs
{
    public class HtmlParser
    {

        private ScanFunction parser;
        private HtmlWalker walker = new HtmlWalker();

        public HtmlParser()
        {
            var anyChar = CharScanner.Any;
            var slashChar = CharScanner.Of('/');
            var singleQuoteChar = CharScanner.Of('\'');
            var doubleQuoteChar = CharScanner.Of('"');
            var whitespaceChar = CharScanner.AsciiWhitespaces;
            var lowerThanChar = CharScanner.Of('<');
            var greaterThanChar = CharScanner.Of('>');
            var exclamationSign = CharScanner.Of('!');
            var equalChar = CharScanner.Of('=');

            var endTagStart = Str("</");

            var tagNameChar = anyChar.Except(
                whitespaceChar, lowerThanChar, greaterThanChar,
                equalChar, singleQuoteChar, doubleQuoteChar,
                exclamationSign
            );
            var attrNameChar = tagNameChar;
            var attrValueChar = tagNameChar;


            var attrValueUnquoted = OneOrMore(attrValueChar).WithSuccess(walker.WalkAttrValue);
            var attrValueSingleQuote = SequenceOf(
                singleQuoteChar,
                ZeroOrMore(anyChar.Except(singleQuoteChar)).WithSuccess(walker.WalkAttrValue),
                singleQuoteChar
            );
            var attrValueDoubleQuote = SequenceOf(
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
                SequenceOf(attrName, equalChar, attrValue),
                attrName
            );

            var attrSequence = OneOrMore(
                SequenceOf(
                    OneOrMore(whitespaceChar),
                    attr
                )
            );

            var tag = SequenceOf(
                lowerThanChar,
                tagName.WithSuccess(walker.WalkTag),
                Maybe(attrSequence),
                ZeroOrMore(whitespaceChar),
                ZeroOrMore(slashChar),
                greaterThanChar
            );
            var endTag = SequenceOf(
                endTagStart,
                tagName.WithSuccess(walker.WalkEndTag),
                ZeroOrMore(whitespaceChar),
                greaterThanChar
            );

            var text = OneOrMore(anyChar.Except('<')).WithSuccess(walker.WalkText);

            var commentText = Expression(@"(.*?)(?:-->)");
            var comment = SequenceOf(
                Str("<!--"),
                commentText.WithSuccess(walker.WalkComment),
                Str("-->")
            );

            parser = ZeroOrMore(FirstOf(
                comment,
                endTag,
                tag,
                text
            ));
        }

        public HtmlTree Parse(string filepath)
        {
            string source;
            using (var sr = new StreamReader(filepath))
            {
                source = sr.ReadToEnd();
            }

            walker.Root = HtmlTree.MakeTag("root");
            parser(source, 0);
            return walker.Root;
        }
    }

    public class HtmlWalker
    {
        private static HashSet<string> _VoidElements = new HashSet<string>{
            "area", "base", "br", "col",
            "command", "embed", "hr", "img",
            "input", "keygen", "link", "meta",
            "param", "source", "track", "wbr"
        };
        private HtmlTree _root = HtmlTree.MakeTag("root");
        public HtmlTree Root
        {
            get { return _root; }
            set
            {
                _root = value;
                _parent = value;
                _current = value;
            }
        }
        private HtmlTree _parent;
        private HtmlTree _current;

        private string _curAttrName = "";

        public HtmlWalker()
        {
            _parent = _root;
            _current = _root;
        }

        public void WalkTag(string source, int start, int offset)
        {
            var tagName = source.Substring(start, offset - start);
            var tag = HtmlTree.MakeTag(tagName, parent: _parent);
            _parent.Children.Add(tag);
            _current = tag;
            if (!_VoidElements.Contains(tagName)) _parent = tag;
        }

        public void WalkText(string source, int start, int offset)
        {
            var text = source.Substring(start, offset - start);
            var tag = HtmlTree.MakeText(text, parent: _parent);
            _parent.Children.Add(tag);
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
            var closestParent = _parent.FindClosestAncestorByName(tagName);
            _parent = closestParent != null && closestParent.Parent != null ? closestParent.Parent : Root;
        }

        public void WalkComment(string source, int start, int offset)
        {
            // Console.WriteLine($"{{{{COMMENT {source.Substring(start, offset - start)}}}}}");
        }
    }
}