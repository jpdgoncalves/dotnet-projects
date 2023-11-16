

namespace LoadOfTemplates {

    public class Template {
        private string _location;
        private TemplateProperties _properties;

        public Template(string location) {
            _location = Path.GetFullPath(location);
            _properties = TemplateProperties.ReadTemplateFile(Path.Combine(location, "template.json"));
        }

        public void CreateInstance(string destination) {
            Dictionary<string, string> paramValues = GetUserInput(_properties);

            Stack<string> remaining = new();
            remaining.Push(_location);
            while (remaining.Count > 0) {
                remaining.Pop();
                var subDirectories = Directory.GetDirectories(_location);
                var files = Directory.GetFiles(_location);
                
                foreach (var file in files) {
                    var destPath = Path.Combine(destination, file.Replace(_location, ""));
                    CreateFile(srcPath: file, destPath, paramValues);
                }

                foreach (var dir in subDirectories) {
                    var destPath = Path.Combine(destination, dir.Replace(_location, ""));
                    CreateDirectory(srcPath: dir, destPath, paramValues);
                    remaining.Push(dir);
                }
            }
        }

        private void CreateFile(string srcPath, string destPath, Dictionary<string, string> paramValues)
        {
            throw new NotImplementedException();
        }

        private void CreateDirectory(string srcPath, string destPath, Dictionary<string, string> paramValues)
        {
            throw new NotImplementedException();
        }

        private Dictionary<string, string> GetUserInput(TemplateProperties properties)
        {
            throw new NotImplementedException();
        }
    }
}