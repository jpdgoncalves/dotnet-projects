// See https://aka.ms/new-console-template for more information

namespace HTMLToJS
{

    public class Program
    {
        public static void Main(string[] args)
        {
            Console.WriteLine(Directory.GetCurrentDirectory());
            string filename = "..\\..\\..\\..\\example.html";
            string filecontent;

            using (var sr = new StreamReader(filename))
            {
                filecontent = sr.ReadToEnd();
            }

            var tokenizer = new Tokenizer(filecontent);

            if (tokenizer.WasSucessful)
            {
                foreach (var token in tokenizer.Tokens)
                {
                    Console.WriteLine(token);
                }
            }
        }
    }
}