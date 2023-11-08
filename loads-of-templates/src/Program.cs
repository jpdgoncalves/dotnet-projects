
using System.CommandLine;

using LoadsOfTemplates.Templates;

namespace LoadsOfTemplates {
    public static class Program {
        
        public static int Main(string[] args) {

            var rootCommand = new RootCommand(
                "Command to create projects and project parts (like components in a website) from templates"
            );

            Jpdg.RegisterCommands(rootCommand);

            return rootCommand.Invoke(args);
        }
    }
}