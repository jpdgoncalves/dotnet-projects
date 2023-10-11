
using System.Text;
using static HtmlToJs.HtmlNode;

namespace HtmlToJs
{

    public class JsGenerator
    {
        private StringBuilder _observedAttr = new();
        private StringBuilder _attrGetters = new();
        private StringBuilder _constructor = new();
        private StringBuilder _nodeGetters = new();

        public string GenerateComponent(string name, ComponentTree tree)
        {
            StringBuilder sourceCode = new();
            ComponentTree root = tree;

            Reset();
            GenerateObservedAttr(root);
            GenerateAttrGetters(root);
            GenerateConstructor(root);
            GenerateNodeGetters(root);

            sourceCode.AppendLine();
            sourceCode.AppendLine($"export default class {name} {{");
            sourceCode.AppendLine();
            sourceCode.AppendLine($"    static observedAttributes = [{_observedAttr.ToString()}];");
            sourceCode.AppendLine();

            sourceCode.AppendLine("    #attrListeners = new Map();");
            sourceCode.AppendLine("    #root;");
            sourceCode.AppendLine();

            sourceCode.AppendLine(_attrGetters.ToString());
            sourceCode.AppendLine();

            sourceCode.AppendLine("    constructor() {");
            sourceCode.Append(_constructor.ToString());
            sourceCode.AppendLine("    }");
            sourceCode.AppendLine();

            sourceCode.AppendLine("    watch(attr, fn) {");
            sourceCode.AppendLine("        if (!this.#attrListeners.has(attr)) this.#attrListeners.set(attr, []);");
            sourceCode.AppendLine("        this.#attrListeners.get(attr).push(fn);");
            sourceCode.AppendLine("    }");
            sourceCode.AppendLine();

            sourceCode.Append(_nodeGetters.ToString());
            sourceCode.AppendLine();
            sourceCode.AppendLine("}");

            sourceCode.AppendLine();
            //TODO user defined name
            sourceCode.Append($"customElements.define('place-holder', {name})");

            return sourceCode.ToString();
        }

        private void GenerateNodeGetters(ComponentTree root, bool isRoot = true)
        {
            //TODO
        }

        private void GenerateConstructor(ComponentTree node, bool isRoot = true)
        {
            if (isRoot)
            {

                _constructor.AppendLine($"        let root = document.createElement('div');");
                _constructor.AppendLine($"        this.#root = root;");
                _constructor.AppendLine();

                foreach (var (attr, value) in node.Attributes)
                {
                    _constructor.AppendLine($"        root.setAttribute('{attr}', '{value}');");
                }

                _constructor.AppendLine();
                foreach (var child in node.Children) GenerateConstructor(child, isRoot: false);
                _constructor.AppendLine();
                foreach (var child in node.Children) _constructor.AppendLine($"        root.appendChild({child.Id});");

            }
            else if (node.Type == HTMLNodeType.TAG)
            {

                _constructor.AppendLine($"        let {node.Id} = document.createElement('{node.Name}');");
                _constructor.AppendLine();

                foreach (var (attr, value) in node.Attributes)
                {
                    _constructor.AppendLine($"        {node.Id}.setAttribute('{attr}', '{value}');");
                }

                _constructor.AppendLine();
                foreach (var child in node.Children) GenerateConstructor(child, isRoot: false);
                _constructor.AppendLine();
                foreach (var child in node.Children) _constructor.AppendLine($"        {node.Id}.appendChild({child.Id});");

            }
            else
            {
                var text = node.InnerText != null ? node.InnerText : "";
                _constructor.AppendLine($"        let {node.Id} = document.createTextNode({ToLiteral(text)});");
            }
        }

        private void GenerateAttrGetters(ComponentTree root, bool isRoot = true)
        {
            //TODO
        }

        private void GenerateObservedAttr(ComponentTree root, bool isRoot = true)
        {
            //TODO
        }

        private void Reset()
        {
            _observedAttr.Clear();
            _attrGetters.Clear();
            _constructor.Clear();
            _nodeGetters.Clear();
        }

        private static string ToLiteral(string input)
        {
            StringBuilder literal = new StringBuilder(input.Length + 2);
            literal.Append("\"");
            foreach (var c in input)
            {
                switch (c)
                {
                    case '\"': literal.Append("\\\""); break;
                    case '\\': literal.Append(@"\\"); break;
                    case '\0': literal.Append(@"\0"); break;
                    case '\a': literal.Append(@"\a"); break;
                    case '\b': literal.Append(@"\b"); break;
                    case '\f': literal.Append(@"\f"); break;
                    case '\n': literal.Append(@"\n"); break;
                    case '\r': literal.Append(@"\r"); break;
                    case '\t': literal.Append(@"\t"); break;
                    case '\v': literal.Append(@"\v"); break;
                    default:
                        // ASCII printable character
                        if (c >= 0x20 && c <= 0x7e)
                        {
                            literal.Append(c);
                            // As UTF16 escaped character
                        }
                        else
                        {
                            literal.Append(@"\u");
                            literal.Append(((int)c).ToString("x4"));
                        }
                        break;
                }
            }
            literal.Append("\"");
            return literal.ToString();
        }
    }

    public class ComponentTree
    {
        private static int _nextId = 0;
        public readonly ComponentTree? Parent;
        public readonly int? ChildIndex;
        public readonly HTMLNodeType Type;
        public readonly string? Name;
        public readonly string Id;
        public readonly string? InnerText;
        public readonly Dictionary<string, string> Attributes = new();
        public readonly List<ComponentTree> Children = new();

        private ComponentTree(
            HtmlNode node, ComponentTree? parent,
            int? childIndex = null
        )
        {
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

        public override string ToString()
        {
            return InternalToString();
        }

        private string InternalToString(string indent = "")
        {
            StringBuilder builder = new();
            builder.Append($"{indent}ComponentTree: Type {Type}, Name {Name}, Id {Id}, Parent {(Parent != null ? Parent.Name : null)}\n");
            builder.Append($"{indent}               Id {Id}, Innertext {InnerText}\n");
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

        public static ComponentTree Make(HtmlNode root)
        {
            _nextId = 0;
            return MakeInternal(root);
        }

        private static ComponentTree MakeInternal(HtmlNode node, ComponentTree? parent = null, int? childIndex = null)
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
    }
}