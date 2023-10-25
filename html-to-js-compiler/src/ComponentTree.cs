
using System.Text;
using System.Text.RegularExpressions;
using static HtmlToJs.HtmlTree;

namespace HtmlToJs
{
    public class ComponentTree
    {
        private static int _nextId = 0;
        private static readonly string COMPONENT_KEY = "data-component";
        private static readonly string GETTER_KEY = "data-getter";
        /// <summary>
        /// Matches expressions like Hello, Hello() and Hello(arg1, arg2)
        /// </summary>
        private static readonly Regex COMP_DATA_REGEX = new(@"([^\(]+)(?:\(([^\)]*)\))?");

        public readonly ComponentTree Root;
        public readonly bool IsRoot;
        public readonly string ComponentName;
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
        public readonly List<ComponentTree> Children = new();

        private ComponentTree(
            HtmlTree node, ComponentTree? parent,
            int? childIndex = null
        )
        {
            Root = parent == null ? this : parent.Root;
            IsRoot = Root == this;

            if (IsRoot) {
                var groups = COMP_DATA_REGEX.Match(node.Attributes[COMPONENT_KEY]).Groups;
                ComponentName = groups[1].Value.Trim();
                ComponentNameLower = ComponentName.ToLower();
                ComponentArguments = new();
                if (groups.Count > 2) {
                    foreach (var arg in groups[2].Value.Split(",")) {
                        ComponentArguments.Add(arg.Trim());
                    }
                }
            } else {
                ComponentName = Root.ComponentName;
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
            return html.Attributes.ContainsKey(COMPONENT_KEY) && html.Attributes[COMPONENT_KEY].Length > 0;
        }

        public static ComponentTree Make(HtmlTree root)
        {
            if (!HasComponentName(root))
                throw new ArgumentException($"Provided HtmlTree root doesn't have a non empty '{COMPONENT_KEY}' attribute");

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
                component.Children.Add(MakeInternal(children[i], component, i));
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

            foreach (var child in Children)
            {
                builder.Append(child.InternalToString(indent + "  "));
            }

            return builder.ToString();
        }
    }
}