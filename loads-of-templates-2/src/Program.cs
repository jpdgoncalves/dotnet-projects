
using System.CommandLine;
using System.Text;

namespace LoadOfTemplates
{
    public static class Program {

        public static int Main(string[] args) {
            RootCommand rootCommand = new RootCommand("Create projects and their components from one of the several templates available");

            RegisterNewCommand(rootCommand);
            RegisterHelpCommand(rootCommand);
            RegisterListCommand(rootCommand);

            // TODO: Use CommandLineBuilder to setup my own custom exception
            // handling routine.
            // Currently we are just printing the exceptions but it would
            // be better to print more helpful messages.

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
        private static readonly string _TEMPLATES_DIRECTORY = Path.Join(AppContext.BaseDirectory, "templates");

        public static void NewCommandHandler(string templateName, string destination) {
            var templates = Template.ReadAllTemplates(_TEMPLATES_DIRECTORY);
            Template targetTemplate;
            try {
                targetTemplate = templates.First((t) => t.Properties.Name.Equals(templateName));
            } catch (Exception e) {
                Console.Error.WriteLine(e);
                return;
            }

            targetTemplate.CreateInstance(destination);
        }

        public static void HelpCommandHandler(string templateName) {
            var templates = Template.ReadAllTemplates(_TEMPLATES_DIRECTORY);
            Template targetTemplate;
            try {
                targetTemplate = templates.First((t) => t.Properties.Name.Equals(templateName));
            } catch (Exception e) {
                Console.Error.WriteLine(e);
                return;
            }

            Console.WriteLine($"\nTemplate: {targetTemplate.Properties.Name}");
            Console.WriteLine($"Description: {targetTemplate.Properties.Description}");
            Console.WriteLine("Parameters:");
            
            foreach (var param in targetTemplate.Properties.Params) {
                Console.WriteLine($"\n- {param.Name} ({(param.Required ? "required" : "optional")})");
                if (!param.Required) Console.WriteLine($"  Default Value: '{param.Default}'");
                Console.WriteLine($"  Description: {param.Description}");
            }
        }

        public static void ListCommandHandler() {
            var templates = Template.ReadAllTemplates(_TEMPLATES_DIRECTORY);

            Console.WriteLine("List of Available Templates");

            foreach (var template in templates) {
                Console.WriteLine($"{template.Properties.Name}: {template.Properties.Description}");
            }
        }
    }
}