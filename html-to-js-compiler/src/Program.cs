// See https://aka.ms/new-console-template for more information

namespace HtmlToJs
{

    public class Program
    {
        public static void Main(string[] args)
        {
            string filename = "testfiles/example2.html";
            string filecontent;

            using (var sr = new StreamReader(filename))
            {
                filecontent = sr.ReadToEnd();
            }

            var parser = new HtmlParser();
            var generator = new JsGenerator();
            var root = parser.Parse(filecontent);
            var component = ComponentTree.Make(root);
            var source = generator.GenerateComponent("Example2", component);

            using (var sw = new StreamWriter("example2.js")) {
                sw.Write(source);
            }
        }
    }
}