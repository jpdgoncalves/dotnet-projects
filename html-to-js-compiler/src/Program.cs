// See https://aka.ms/new-console-template for more information

namespace HtmlToJs
{

    public class Program
    {
        public static void Main(string[] args)
        {
            if (args.Length == 0 || !Path.GetExtension(args[0]).Equals(".html")) {
                Console.WriteLine("Please provide an html file.");
                Environment.Exit(1);
            }

            string filename = args[0];
            var outfilename = Path.ChangeExtension(filename, ".generated.mjs");
            string filecontent;

            using (var sr = new StreamReader(filename))
            {
                filecontent = sr.ReadToEnd();
            }

            var parser = new HtmlParser();
            var root = parser.Parse(filecontent);
            var component = ComponentTree.Make(root);
            var source = JsGenerator.GenerateComponentCode(filename, component);

            using (var sw = new StreamWriter(outfilename)) {
                sw.Write(source);
            }
        }
    }
}