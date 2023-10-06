
namespace HTMLToJS
{
    public class HTMLParser {

        private Scanners.ScanFunction parser;
        private HTMLWalker walker = new HTMLWalker();
        public HTMLParser() {
            var tagNameChar = Scanners.Char(Scanners.OneOf(
                Scanners.CharSet(
                    Scanners.CharRange('a', 'z'),
                    Scanners.CharRange('A', 'Z'),
                    Scanners.CharRange('0', '9'),
                    Scanners.CharList('-')
                )
            ));
            var tagName = Scanners.OneOrMore(tagNameChar).SuccessCallback(walker.WalkTagName);
            parser = Scanners.SequenceOf(
                Scanners.Char('<'),
                tagName,
                Scanners.Char('>')
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