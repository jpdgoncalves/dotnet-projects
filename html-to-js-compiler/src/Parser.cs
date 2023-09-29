
namespace HTMLToJS
{

    public class Parser
    {
        private Node? _root;

        public Node? Root { get { return _root; } }

        public Parser() { }
    }

    public enum NodeType { ELEMENT, TEXT }

    public class Node
    {
        public Node? Parent { get; set; }
        public readonly NodeType Type;
        public readonly string TagName;
        public readonly string InnerText;
        public readonly List<Node> children = new List<Node>();

        /// <summary>
        /// Creates a node of the specified type
        /// and tagName.
        /// </summary>
        /// <param name="type">The type of the node</param>
        /// <param name="tagName">The tag name of the node</param>
        public Node(NodeType type, string tagName)
        {
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
            TagName = "Text";
            InnerText = innerText;
        }

        public Node? FindAncestorByName(string name)
        {
            if (Parent == null) return null;
            if (Parent.TagName.Equals(name)) return Parent;
            return Parent.FindAncestorByName(name);
        }
    }
}