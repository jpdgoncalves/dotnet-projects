
using System.Text;
using static HtmlToJs.HtmlNode;

namespace HtmlToJs
{

    public class JsGenerator
    {
        private string _genFuncName = "";
        private string _genCompName = "";
        private StringBuilder _funcBody = new();
        private StringBuilder _getterFuncs = new();

        private JsGenerator(string filepath)
        {
            var filename = Path.GetFileNameWithoutExtension(filepath);
            var temp = filename.Split("-");
            var funcName = new StringBuilder();
            var compName = new StringBuilder();

            for (var i = 0; i < temp.Length; i++)
            {
                var part = temp[i];
                if (part.Length == 0) continue;
                if (!char.IsLetter(part[0]) || part.Length == 1)
                {
                    funcName.Append(part);
                }
                else
                {
                    funcName.Append(char.ToUpper(part[0]) + part.Substring(1));
                }

                compName.Append(part);
            }

            _genFuncName = funcName.ToString();
            _genCompName = compName.ToString();
        }

        private string GenerateCode(ComponentTree node)
        {
            StringBuilder code = new();

            GenerateFuncBody(node);
            GenerateGetterFuncs(node);

            code.AppendLine();
            code.AppendLine($"export default function base{_genFuncName}() {{");
            code.Append(_funcBody);
            code.AppendLine("}\n");
            code.Append(_getterFuncs);

            return code.ToString();
        }

        private void GenerateFuncBody(ComponentTree node, bool isRoot = true)
        {
            var vName = isRoot ? _genCompName : node.Id;

            if (node.Type == HTMLNodeType.TAG) {
                _funcBody.AppendLine($"    let {vName} = document.createElement({ToLiteral(node.Name)});");
                foreach (var (attr, value) in node.Attributes) {
                    _funcBody.AppendLine($"    {vName}.setAttribute({ToLiteral(attr)}, {ToLiteral(value)});");
                }
                foreach (var child in node.Children) {
                    _funcBody.AppendLine();
                    GenerateFuncBody(child, false);
                }
                if (node.Children.Count > 0) _funcBody.AppendLine();
                foreach (var child in node.Children) {
                    _funcBody.AppendLine($"    {vName}.appendChild({child.Id});");
                }
            } else {
                _funcBody.AppendLine($"    let {vName} = document.createTextNode({ToLiteral(node.InnerText)});");
            }

            if (isRoot) {
                _funcBody.AppendLine();
                _funcBody.AppendLine($"    return {vName};");
            }
        }

        private void GenerateGetterFuncs(ComponentTree node, bool isRoot = true)
        {
            if (!isRoot && node.Attributes.ContainsKey("data-getter") && node.Attributes["data-getter"].Length != 0) {
                var getterName = node.Attributes["data-getter"];
                var path = node.GetPath();
                _getterFuncs.AppendLine($"export function get{getterName}() {{");
                _getterFuncs.Append($"    return {_genCompName}");
                foreach (var p in path) {
                    _getterFuncs.Append($".childNodes[{p}]");
                }
                _getterFuncs.AppendLine(";\n}\n");
            }

            foreach (var child in node.Children) GenerateGetterFuncs(child, false);
        }

        public static string GenerateComponentCode(string filepath, ComponentTree tree)
        {
            var generator = new JsGenerator(filepath);
            return generator.GenerateCode(tree);
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
        public readonly string Name;
        public readonly string Id;
        public readonly string InnerText;
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
            builder.Append($"{indent}ComponentTree: Type {Type}, Name {Name}, Parent {(Parent != null ? Parent.Name : null)}\n");
            builder.Append($"{indent}               ChildIndex {ChildIndex}, Id {Id}, Innertext {InnerText}\n");
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