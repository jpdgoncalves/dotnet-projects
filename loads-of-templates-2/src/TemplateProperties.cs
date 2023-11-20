
using System.Text.Json;

namespace LoadOfTemplates {

    public class TemplateProperties {
        public required string Name {get; set;}
        public required string Description {get; set;}
        public List<TemplateParam> Params {get; set;} = new();

        public static TemplateProperties ReadTemplateFile(string path) {
            var text = File.ReadAllText(path);
            var properties = JsonSerializer.Deserialize<TemplateProperties>(text);
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
        public required string Name {get; set;}
        public required string Description {get; set;}
        public bool Required {get; set;} = false;
        public string Default {get; set;} = "";
    }
}