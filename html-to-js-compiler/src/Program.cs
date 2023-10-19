// See https://aka.ms/new-console-template for more information

namespace HtmlToJs
{

    public class Program
    {
        public static void Main(string[] args)
        {
            if (args.Length == 0 || !Path.GetExtension(args[args.Length - 1]).Equals(".html"))
            {
                Console.WriteLine("Please provide an html file.");
                Environment.Exit(1);
            }

            bool domMode = findFlag(args, "dom-mode");
            string filename = args[args.Length - 1];
            var outfilename = Path.ChangeExtension(filename, ".generated.mjs");

            if (domMode)
            {
                JsGenerator.GenerateComponentsFromDom(filename, outfilename);
            }
            else
            {
                JsGenerator.GenerateComponents(filename, outfilename);
            }
        }

        public static bool findFlag(string[] args, string optionName)
        {
            foreach (var arg in args)
            {
                if (arg.StartsWith("--") && arg.Substring(2).Equals(optionName)) return true;
            }

            return false;
        }
    }
}