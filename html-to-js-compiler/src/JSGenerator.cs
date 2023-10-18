
using System.Text;
using static HtmlToJs.HtmlTree;

namespace HtmlToJs
{

    public class JsGenerator
    {
        public static string GETTER_KEY = "data-getter";
        private StringBuilder _funcBody = new();
        private StringBuilder _getterFuncs = new();

        private JsGenerator() { }

        private string GenerateComponent(ComponentTree node)
        {
            StringBuilder code = new();
            string componentName = node.ComponentName;

            GenerateFuncBody(node);
            GenerateGetterFuncs(node);

            code.AppendLine();
            code.AppendLine($"export function base{componentName}() {{");
            code.Append(_funcBody);
            code.AppendLine("}\n");
            code.Append(_getterFuncs);

            return code.ToString();
        }

        private void GenerateFuncBody(ComponentTree node)
        {
            var vName = node == node.Root ? node.ComponentName : node.Id;

            if (node.Type == HTMLNodeType.TAG)
            {
                _funcBody.AppendLine($"    let {vName} = document.createElement({ToLiteral(node.Name)});");
                foreach (var (attr, value) in node.Attributes)
                {
                    _funcBody.AppendLine($"    {vName}.setAttribute({ToLiteral(attr)}, {ToLiteral(value)});");
                }
                foreach (var child in node.Children)
                {
                    _funcBody.AppendLine();
                    GenerateFuncBody(child);
                }
                if (node.Children.Count > 0) _funcBody.AppendLine();
                foreach (var child in node.Children)
                {
                    _funcBody.AppendLine($"    {vName}.appendChild({child.Id});");
                }
            }
            else
            {
                _funcBody.AppendLine($"    let {vName} = document.createTextNode({ToLiteral(node.InnerText)});");
            }

            if (node == node.Root)
            {
                _funcBody.AppendLine();
                _funcBody.AppendLine($"    return {vName};");
            }
        }

        private void GenerateGetterFuncs(ComponentTree node)
        {
            var isRoot = node.IsRoot;
            var componentName = node.Root.ComponentName.ToLower();

            if (!isRoot && node.Attributes.ContainsKey(GETTER_KEY) && node.Attributes[GETTER_KEY].Length != 0)
            {
                var getterName = node.Attributes[GETTER_KEY];
                var path = node.GetPath();
                _getterFuncs.AppendLine($"export function get{getterName}({componentName}) {{");
                _getterFuncs.Append($"    return {componentName}");
                foreach (var p in path)
                {
                    _getterFuncs.Append($".childNodes[{p}]");
                }
                _getterFuncs.AppendLine(";\n}\n");
            }

            foreach (var child in node.Children) GenerateGetterFuncs(child);
        }

        public static void GenerateComponentCode(string inFilePath, string outFilePath)
        {
            var generator = new JsGenerator();
            var parser = new HtmlParser();
            var htmlRoot = parser.Parse(inFilePath);
            var components = htmlRoot.Children.FindAll(ComponentTree.HasComponentName);

            using (var sw = new StreamWriter(outFilePath))
            {
                foreach (var component in components)
                {
                    var tree = ComponentTree.Make(component);
                    sw.Write(generator.GenerateComponent(tree));
                }
            }
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
}