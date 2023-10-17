
using System.Text;

namespace HtmlToJs
{
    public class HtmlTree
    {
        public readonly HtmlTree? Parent;
        public readonly HTMLNodeType Type;
        public readonly string Name = "";
        public readonly string InnerText = "";
        public readonly Dictionary<string, string> Attributes = new();
        public readonly List<HtmlTree> Children = new();

        public HtmlTree(HtmlTree other) : this(
            other.Type, other.Parent, other.Name, 
            other.InnerText
        ) {}

        private HtmlTree(
            HTMLNodeType type, HtmlTree? parent = null,
            string name = "", string innerText = ""
        )
        {
            Type = type;
            Parent = parent;
            Name = name;
            InnerText = innerText;
        }

        public static HtmlTree MakeTag(string name, HtmlTree? parent = null) => new HtmlTree(
            type: HTMLNodeType.TAG,
            parent: parent,
            name: name
        );
        public static HtmlTree MakeText(string innerText, HtmlTree? parent = null) => new HtmlTree(
            type: HTMLNodeType.TEXT,
            parent: parent,
            innerText: innerText
        );

        public HtmlTree? FindClosestAncestorByName(string name)
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