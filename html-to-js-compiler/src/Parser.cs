
namespace HTMLToJS
{
    public class Parser
    {
        private Node? _root;
        public Node? Root { get { return _root; } }
        private Node? current;
        private readonly List<Token> _tokens;
        public bool _wasSuccessful = true;

        public bool WasSuccessful { get { return _wasSuccessful; } }

        public Parser(List<Token> tokens) {
            _tokens = tokens;
            Parse();
        }

        private void Parse() {
            foreach (var token in _tokens) {
                switch(token.Type) {
                    case TokenType.TAG:
                        ParseTag(token);
                        break;
                    case TokenType.END_TAG:
                        ParseEndTag(token);
                        break;
                    case TokenType.TEXT:
                        ParseText(token);
                        break;
                }
            }
        }

        private void ParseTag(Token token) {

        }

        private void ParseEndTag(Token token) {

        }

        private void ParseText(Token token) {

        }
    }

    public enum NodeType { ELEMENT, TEXT }

    public class Node
    {
        public Node? Parent { get; set; }
        public readonly NodeType Type;
        public readonly string TagName;
        public readonly Dictionary<string, string> Attributes;
        public readonly List<NodeArgument> AttrArgs = new();
        public readonly string InnerText;
        public readonly List<NodeArgument> InnerTextArgs = new();
        public readonly List<Node> Children = new();

        /// <summary>
        /// Creates a node of the specified type
        /// and tagName.
        /// </summary>
        /// <param name="type">The type of the node</param>
        /// <param name="tagName">The tag name of the node</param>
        public Node(NodeType type, string tagName, Dictionary<string, string>? attrs)
        {
            Attributes = attrs == null ? new() : attrs;
            Type = type;
            TagName = tagName;
            InnerText = "";
        }

        /// <summary>
        /// Creates a Node of type TEXT
        /// </summary>
        /// <param name="innerText">The text to the node contains</param>
        public Node(string innerText)
        {
            Type = NodeType.TEXT;
            Attributes = new();
            TagName = "Text";
            InnerText = innerText;
        }

        /// <summary>
        /// Finds the closest ancestor that has
        /// specified tag name.
        /// </summary>
        /// <param name="tagName">The tagName to search for.</param>
        /// <returns>The closest ancestor or null.</returns>
        public Node? FindAncestorByName(string tagName)
        {
            if (Parent == null) return null;
            if (Parent.TagName.Equals(tagName)) return Parent;
            return Parent.FindAncestorByName(tagName);
        }

        public List<NodeArgument> CollectArguments() {
            return CollectArguments(new List<NodeArgument>());
        }

        public List<NodeArgument> CollectArguments(List<NodeArgument> collected) {
            collected.AddRange(AttrArgs);
            collected.AddRange(InnerTextArgs);
            Children.ForEach((child) => child.CollectArguments(collected));
            return collected;
        }
    }

    public record NodeArgument(string Name, string Type) {}
}