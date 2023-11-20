
using System.Text.RegularExpressions;

namespace LoadOfTemplates {

    public class Template {
        private static Regex _TEMPLATE_REGEX = new Regex("{{(.*?)}}");
        private static string _TEMPLATE_JSON_FILE = "template.json";

        private string _location;
        public readonly TemplateProperties Properties;

        public Template(string location) {
            _location = Path.GetFullPath(location);
            Properties = TemplateProperties.ReadTemplateFile(Path.Combine(location, _TEMPLATE_JSON_FILE));
        }

        public static IEnumerable<Template> ReadAllTemplates(string templateDir) {
            return Directory.EnumerateDirectories(templateDir).Select((path) => new Template(path));
        }

        public void CreateInstance(string destination) {
            Dictionary<string, string> paramValues = GetUserInput(Properties);

            Stack<string> remaining = new();
            remaining.Push(_location);
            while (remaining.Count > 0) {
                var curDir = remaining.Pop();
                var subDirectories = Directory.GetDirectories(curDir);
                var files = Directory.GetFiles(curDir).ToList().FindAll(
                    f => !Path.GetFileName(f).Equals(_TEMPLATE_JSON_FILE)
                );
                
                foreach (var file in files) {
                    var destPath = Path.Combine(destination, file.Replace(_location + "\\", ""));
                    CreateFile(srcPath: file, destPath, paramValues);
                }

                foreach (var dir in subDirectories) {
                    var destPath = Path.Combine(destination, dir.Replace(_location + "\\", ""));
                    CreateDirectory(destPath, paramValues);
                    remaining.Push(dir);
                }
            }
        }

        private void CreateFile(string srcPath, string destPath, Dictionary<string, string> paramValues)
        {
            string template = File.ReadAllText(srcPath);
            destPath = EvaluateTemplateString(destPath, paramValues);
            using (var sw = new StreamWriter(destPath)) {
                sw.Write(EvaluateTemplateString(template, paramValues));
            }
        }

        private void CreateDirectory(string destPath, Dictionary<string, string> paramValues)
        {
            Directory.CreateDirectory(EvaluateTemplateString(destPath, paramValues));
        }

        private Dictionary<string, string> GetUserInput(TemplateProperties properties)
        {
            Dictionary<string, string> paramValues = new();

            var i = 0;
            var paramList = properties.Params;
            while (i < paramList.Count) {
                var param = paramList[i];

                // Ask the user for the input of the specified parameter.
                Console.Write($"\n {param.Name}");
                if (param.Required) Console.Write($" (required): ");
                else Console.Write($" (default: '{param.Default}'): ");

                // Read the input
                var value = Console.ReadLine();
                // If the parameter is required but no value was read or an empty value was read
                // then warn the user and ask for the parameter again.
                if (param.Required && (value is null || value.Length == 0)) {
                    Console.WriteLine($"Parameter {param.Name} is required. It can't be an empty value");
                    continue;
                }

                // At this point the only way the parameter doesn't have a value
                // is if it wasn't required or the user provide a value.
                // We test if we have a value and if we don't, we use
                // the default one.
                if (value is null || value.Length == 0) {
                    value = param.Default;
                }

                paramValues.Add(param.Name, value);
                i++;
            }

            return paramValues;
        }

        private string EvaluateTemplateString(string template, Dictionary<string, string> paramValues) {
            return _TEMPLATE_REGEX.Replace(template, (match) => {
                var prop = match.Groups[1].Value.Trim();
                return paramValues.ContainsKey(prop) ? paramValues[prop] : "<ERROR>";
            });
        }
    }
}