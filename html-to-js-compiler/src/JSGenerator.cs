
using System.Text;
using static HtmlToJs.HtmlTree;

namespace HtmlToJs
{

    public class JsGenerator
    {
        private string _genFuncName = "";
        private string _genCompName = "";
        private StringBuilder _funcBody = new();
        private StringBuilder _getterFuncs = new();

        private JsGenerator() { }

        private string GenerateCode(ComponentTree root)
        {
            StringBuilder code = new();

            code.AppendLine();
            foreach (var child in root.Children)
            {
                if (
                    child.Type == HTMLNodeType.TEXT
                    || !child.Attributes.ContainsKey("data-component-name")
                    || child.Attributes["data-component-name"].Length == 0
                ) continue;
                
                _genFuncName = child.Attributes["data-component-name"];
                _genCompName = _genFuncName.ToLower();
                _funcBody = new();
                _getterFuncs = new();
                code.Append(GenerateComponent(child));
            }

            return code.ToString();
        }

        private StringBuilder GenerateComponent(ComponentTree node)
        {
            StringBuilder component = new();

            GenerateFuncBody(node);
            GenerateGetterFuncs(node);

            component.AppendLine($"export function base{_genFuncName}() {{");
            component.Append(_funcBody);
            component.AppendLine("}\n");
            component.Append(_getterFuncs);

            return component;
        }

        private void GenerateFuncBody(ComponentTree node, bool isRoot = true)
        {
            var vName = isRoot ? _genCompName : node.Id;

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
                    GenerateFuncBody(child, false);
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

            if (isRoot)
            {
                _funcBody.AppendLine();
                _funcBody.AppendLine($"    return {vName};");
            }
        }

        private void GenerateGetterFuncs(ComponentTree node, bool isRoot = true)
        {
            if (!isRoot && node.Attributes.ContainsKey("data-getter") && node.Attributes["data-getter"].Length != 0)
            {
                var getterName = node.Attributes["data-getter"];
                var path = node.GetPath();
                _getterFuncs.AppendLine($"export function get{getterName}({_genCompName}) {{");
                _getterFuncs.Append($"    return {_genCompName}");
                foreach (var p in path)
                {
                    _getterFuncs.Append($".childNodes[{p}]");
                }
                _getterFuncs.AppendLine(";\n}\n");
            }

            foreach (var child in node.Children) GenerateGetterFuncs(child, false);
        }

        public static string GenerateComponentCode(string filepath, ComponentTree tree)
        {
            var generator = new JsGenerator();
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
}