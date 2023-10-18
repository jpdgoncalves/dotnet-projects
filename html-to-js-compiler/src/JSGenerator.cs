
using System.Text;
using static HtmlToJs.HtmlTree;

namespace HtmlToJs
{

    public static class JsGenerator
    {

        private static string GenerateComponent(ComponentTree node)
        {
            StringBuilder code = new();

            code.AppendLine();
            code.AppendLine($"export function base{node.ComponentName}() {{");
            GenerateFuncBody(node, code);
            code.AppendLine("}\n");
            GenerateGetterFuncs(node, code);

            return code.ToString();
        }

        private static void GenerateFuncBody(ComponentTree node, StringBuilder code)
        {
            var vName = node == node.Root ? node.ComponentName : node.Id;

            if (node.Type == HTMLNodeType.TAG)
            {
                code.AppendLine($"    let {vName} = document.createElement({ToLiteral(node.Name)});");
                foreach (var (attr, value) in node.Attributes)
                {
                    code.AppendLine($"    {vName}.setAttribute({ToLiteral(attr)}, {ToLiteral(value)});");
                }
                foreach (var child in node.Children)
                {
                    code.AppendLine();
                    GenerateFuncBody(child, code);
                }
                if (node.Children.Count > 0) code.AppendLine();
                foreach (var child in node.Children)
                {
                    code.AppendLine($"    {vName}.appendChild({child.Id});");
                }
            }
            else
            {
                code.AppendLine($"    let {vName} = document.createTextNode({ToLiteral(node.InnerText)});");
            }

            if (node == node.Root)
            {
                code.AppendLine();
                code.AppendLine($"    return {vName};");
            }
        }

        private static void GenerateGetterFuncs(ComponentTree node, StringBuilder code)
        {
            var isRoot = node.IsRoot;
            var componentName = node.Root.ComponentNameLower;

            if (!isRoot && node.IsGetter)
            {
                var getterName = node.GetterName;
                var path = node.GetPath();
                code.AppendLine($"export function get{getterName}({componentName}) {{");
                code.Append($"    return {componentName}");
                foreach (var p in path)
                {
                    code.Append($".childNodes[{p}]");
                }
                code.AppendLine(";\n}\n");
            }

            foreach (var child in node.Children) GenerateGetterFuncs(child, code);
        }

        public static void GenerateComponents(string inFilePath, string outFilePath)
        {
            var parser = new HtmlParser();
            var htmlRoot = parser.Parse(inFilePath);
            var components = htmlRoot.Children.FindAll(ComponentTree.HasComponentName);

            using (var sw = new StreamWriter(outFilePath))
            {
                foreach (var component in components)
                {
                    var tree = ComponentTree.Make(component);
                    sw.Write(GenerateComponent(tree));
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