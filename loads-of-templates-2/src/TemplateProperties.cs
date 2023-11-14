
using System.Text.Json;

namespace LoadOfTemplates {

    public class TemplateProperties {
        public required string Name;
        public required string Description;
        public IList<TemplateParams>? Params;

        public static TemplateProperties ReadTemplateFile(string path) {
            var properties = JsonSerializer.Deserialize<TemplateProperties>(File.ReadAllText(path));
            if (properties is null) throw new Exception($"Unable to parse '{path}'!");
            return properties;
        }
    }

    public class TemplateParams {
        public required string Name;
        public required string Description;
        public bool Required = false;
        public string Default = "";
    }
}