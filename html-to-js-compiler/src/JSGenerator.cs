
using static HtmlToJs.HtmlNode;

namespace HtmlToJs {

    public class JsGenerator {
        private string _componentName = "";
        private List<string> _observedAttributes = new();
        public JsGenerator() {

        }
    }

    public class ComponentTree {

        public readonly ComponentTree? Parent;
        public readonly int? ChildIndex;
        public readonly HTMLNodeType Type;
        public readonly string? Name;
        public readonly string? InnerText;
        public readonly Dictionary<string, string> Attributes = new();
        public readonly List<ComponentTree> Children = new();

        private ComponentTree(
            HtmlNode node, ComponentTree? parent,
            int? childIndex = null
        ) {
            Parent = parent;
            ChildIndex = childIndex;
            Type = node.Type;
            Name = node.Name;
            InnerText = node.InnerText;
        }

        public List<int> GetPath() {
            if (ChildIndex == null || Parent == null) return new();
            var path = Parent.GetPath();
            path.Add(ChildIndex.Value);
            return path;
        }

        public static ComponentTree Make(HtmlNode root) {
            return MakeInternal(root);
        }

        private static ComponentTree MakeInternal(HtmlNode node, ComponentTree? parent = null, int? childIndex = null) {
            var component = new ComponentTree(node, parent, childIndex);

            foreach (var (attr, value) in node.Attributes) {
                component.Attributes.Add(attr, value);
            }

            var children = node.Children;
            var childrenCount = children.Count;
            for (var i = 0; i < childrenCount; i++) {
                component.Children.Add(MakeInternal(children[i], component, i));
            }

            return component;
        }
    }
}