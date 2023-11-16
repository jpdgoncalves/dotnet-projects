
using System.Text.Json;

namespace LoadOfTemplates {

    public class TemplateProperties {
        public required string Name;
        public required string Description;
        public List<TemplateParam> Params = new();

        public static TemplateProperties ReadTemplateFile(string path) {
            var properties = JsonSerializer.Deserialize<TemplateProperties>(File.ReadAllText(path));
            if (properties is null) throw new Exception($"Unable to parse '{path}'!");
            
            // Used to check for repeated parameters
            var temp = new HashSet<string>();
            foreach (var param in properties.Params) {
                if (temp.Contains(param.Name)) throw new Exception($"Param '{param.Name}' appears twice.");
                temp.Add(param.Name);
            }            

            return properties;
        }
    }

    public class TemplateParam {
        public required string Name;
        public required string Description;
        public bool Required = false;
        public string Default = "";
    }
}