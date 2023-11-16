
using System.CommandLine;

namespace LoadOfTemplates
{
    public static class Program {
        public static int Main(string[] args) {
            RootCommand rootCommand = new RootCommand("Create projects and their components from one of the several templates available");

            RegisterNewCommand(rootCommand);
            RegisterHelpCommand(rootCommand);
            RegisterListCommand(rootCommand);

            return rootCommand.Invoke(args);
        }

        public static void RegisterNewCommand(RootCommand rootCommand) {
            Command newCommand = new Command("new", "Initialize a new project or file structure from a template");
            Argument<string> templateArgument = new(
                name: "Template Name",
                description: "Name of the template to use"
            );
            Argument<string> destArgument = new(
                name: "Destination",
                description: "Destination of the instance"
            );

            newCommand.AddArgument(templateArgument);
            newCommand.AddArgument(destArgument);
            newCommand.SetHandler(CommandHandlers.NewCommandHandler, templateArgument, destArgument);
            rootCommand.AddCommand(newCommand);
        }

        public static void RegisterHelpCommand(RootCommand rootCommand) {
            Command helpCommand = new Command("help", "Display help information about a template");
            Argument<string> templateArgument = new(
                name: "Template Name",
                description: "Name of the template"
            );

            helpCommand.AddArgument(templateArgument);
            helpCommand.SetHandler(CommandHandlers.HelpCommandHandler, templateArgument);
            rootCommand.AddCommand(helpCommand);
        }

        public static void RegisterListCommand(RootCommand rootCommand) {
            Command helpCommand = new Command("list", "List templates available in the template folder.");
            
            helpCommand.SetHandler(CommandHandlers.ListCommandHandler);
            rootCommand.AddCommand(helpCommand);
        }
    }

    public static class CommandHandlers {

        public static void NewCommandHandler(string templateName, string destination) {
            Console.WriteLine("new is not implemented yet!");
        }

        public static void HelpCommandHandler(string templateName) {
            Console.WriteLine("help is not implemented yet!");
        }

        public static void ListCommandHandler() {
            Console.WriteLine("list is not implemented yet!");
        }
    }
}