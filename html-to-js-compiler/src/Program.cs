// See https://aka.ms/new-console-template for more information

namespace HTMLToJS
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

            var parser = new HTMLParser();
            var root = parser.Parse(filecontent);
            Console.WriteLine(root);
        }
    }
}