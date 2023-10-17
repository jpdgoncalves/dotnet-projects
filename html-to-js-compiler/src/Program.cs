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
            
            JsGenerator.GenerateComponentCode(filename, outfilename);
        }
    }
}