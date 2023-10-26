
using System.Text;
using static HtmlToJs.HtmlTree;

namespace HtmlToJs
{
    public class ComponentTree
    {
        private static int _nextId = 0;
        private static readonly string GETTER_KEY = "data-getter";
        private readonly List<ComponentTree> _children = new();

        public readonly ComponentTree Root;
        public readonly bool IsRoot;
        public readonly string ComponentName;
        public readonly string ComponentNameCamel;
        public readonly string ComponentNameLower;
        public readonly HashSet<string> ComponentArguments;
        public readonly bool IsGetter;
        public readonly string GetterName;
        public readonly ComponentTree? Parent;
        public readonly int? ChildIndex;
        public readonly HTMLNodeType Type;
        public readonly string Name;
        public readonly string Id;
        public readonly string InnerText;
        public readonly Dictionary<string, string> Attributes = new();

        public IReadOnlyList<ComponentTree> Children {
            get {
                return _children;
            }
        }

        private ComponentTree(
            HtmlTree node, ComponentTree? parent,
            int? childIndex = null
        )
        {
            Root = parent == null ? this : parent.Root;
            IsRoot = Root == this;

            if (IsRoot) {
                ComponentName = node.Name;
                ComponentNameCamel = string.Join("", node.Name.Split('-').Select(UpperFirst));
                ComponentNameLower = ComponentNameCamel.ToLower();
                ComponentArguments = new(node.Attributes.Keys);
            } else {
                ComponentName = Root.ComponentName;
                ComponentNameCamel = Root.ComponentName;
                ComponentNameLower = Root.ComponentNameLower;
                ComponentArguments = Root.ComponentArguments;
            }
            
            IsGetter = node.Attributes.ContainsKey(GETTER_KEY) && node.Attributes[GETTER_KEY].Length > 0;
            GetterName = IsGetter ? node.Attributes[GETTER_KEY] : "";

            Parent = parent;
            ChildIndex = childIndex;
            Type = node.Type;
            Name = node.Name;
            Id = Type == HTMLNodeType.TAG ? node.Name + _nextId++ : "text" + _nextId++;
            InnerText = node.InnerText;
        }

        public List<int> GetPath()
        {
            if (ChildIndex == null || Parent == null) return new();
            var path = Parent.GetPath();
            path.Add(ChildIndex.Value);
            return path;
        }

        public static bool HasComponentName(HtmlTree html)
        {
            var idx = html.Name.IndexOf('-');
            return idx > 0 && idx < html.Name.Length - 1;
        }

        private static string UpperFirst(string str) {
            if (str.Length == 0) return str;
            if (str.Length == 1) return str.ToUpper();
            return char.ToUpper(str[0]) + str.Substring(1);
        }

        public static ComponentTree Make(HtmlTree root)
        {
            if (!HasComponentName(root))
                throw new ArgumentException($"Provided HtmlTree root doesn't have a valid name.\n It should have at least one letter followed by a dash followed by at least another letter");

            return MakeInternal(root);
        }

        private static ComponentTree MakeInternal(HtmlTree node, ComponentTree? parent = null, int? childIndex = null)
        {
            var component = new ComponentTree(node, parent, childIndex);

            foreach (var (attr, value) in node.Attributes)
            {
                component.Attributes.Add(attr, value);
            }

            var children = node.Children;
            var childrenCount = children.Count;
            for (var i = 0; i < childrenCount; i++)
            {
                component._children.Add(MakeInternal(children[i], component, i));
            }

            return component;
        }

        public override string ToString()
        {
            return InternalToString();
        }

        private string InternalToString(string indent = "")
        {
            StringBuilder builder = new();
            builder.Append($"{indent}ComponentTree: Type {Type}, Name '{Name}', Parent '{(Parent != null ? Parent.Name : null)}'\n");
            builder.Append($"{indent}               ChildIndex {ChildIndex}, Id {Id}, Innertext '{InnerText}'\n");
            builder.Append($"{indent}               IsRoot {IsRoot}, Root '{(IsRoot ? null : Root.Name)}'\n");
            builder.Append($"{indent}               ComponentName {ComponentName}, ArgsCount {ComponentArguments.Count}\n");
            builder.Append($"{indent}               IsGetter {IsGetter}, Root '{(IsGetter ? GetterName : null)}'\n");
            foreach (var (key, value) in Attributes)
            {
                builder.Append($"{indent}               - AttrName {key}, Value: {value}\n");
            }

            foreach (var child in _children)
            {
                builder.Append(child.InternalToString(indent + "  "));
            }

            return builder.ToString();
        }
    }
}