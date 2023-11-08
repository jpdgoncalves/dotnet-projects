
using System.CommandLine;
using System.CommandLine.Parsing;
using LoadsOfTemplates.Core;

namespace LoadsOfTemplates.Templates
{

    public static class Jpdg
    {

        public static void RegisterCommands(RootCommand rootCommand)
        {


            // The Website Template Command
            var websiteTemplateCmd = new Command("jpdg.website", "Create a static website");
            var destinationOption = new Option<DirectoryInfo>("--destination", "The destination directory for the static website")
            {
                IsRequired = true
            };

            destinationOption.AddValidator(result => VerifyDirectoryExists(result, destinationOption));
            websiteTemplateCmd.AddOption(destinationOption);
            websiteTemplateCmd.SetHandler(MakeWebsiteTemplate, destinationOption);


            // The Website Component Template Command
            var websiteComponentCmd = new Command("jpdg.component", "Create a component for the static website");
            var compDirOption = new Option<DirectoryInfo>(
                name: "--component-directory",
                description: "The directory that contains components",
                getDefaultValue: () => new DirectoryInfo(Path.GetFullPath("./component/"))
            );
            var compDestOption = new Option<DirectoryInfo?>(
                name: "--component-destination",
                description: "The directory that will contain the files of the component"
            );
            var compNameOption = new Option<string>(
                name: "--component-name",
                description: "The name of the component"
            )
            {
                IsRequired = true
            };

            compDirOption.AddValidator(result => VerifyDirectoryExists(result, compDirOption));
            compDestOption.AddValidator(result => VerifyDirectoryNotExists(result, compDestOption));
            websiteComponentCmd.AddOption(compDirOption);
            websiteComponentCmd.AddOption(compDestOption);
            websiteComponentCmd.AddOption(compNameOption);
            websiteComponentCmd.SetHandler(MakeWebsiteComponent, compDirOption, compDestOption, compNameOption);


            // Add the commands to the rootCommand
            rootCommand.AddCommand(websiteTemplateCmd);
            rootCommand.AddCommand(websiteComponentCmd);
        }

        public static void MakeWebsiteTemplate(DirectoryInfo destination)
        {
            var websiteTemplate = new TemplateBuilder(destination.FullName);
            websiteTemplate.AddDirectory("css");
            websiteTemplate.AddDirectory("js");
            var indexFile = websiteTemplate.AddFile("index.html");
            indexFile.AddContent("""
            <!DOCTYPE html>
            <html>
                <head>
                    <meta name="viewport" content="width=device-width, initial-scale=1.0">
                </head>
                <body>
                </body>
            </html>
            """);

            websiteTemplate.Build();
        }

        public static void MakeWebsiteComponent(DirectoryInfo compDir, DirectoryInfo? compDest, string compName)
        {
            var componentTemplate = new TemplateBuilder(compDir.FullName);
            IDirectoryBuilder componentDir;
            if (compDest is not null) {
                componentDir = componentTemplate.AddDirectory(compDest.Name);
            } else {
                componentDir = componentTemplate.AddDirectory(compName);
            }
            componentDir.AddDirectory("css");
            componentDir.AddDirectory("js");
            var layoutFile = componentDir.AddFile("layout.html");
            var compFile = componentDir.AddFile("component.html");

            layoutFile.AddContent("""
            <!DOCTYPE html>
            <html>
                <head>
                    <meta name="viewport" content="width=device-width, initial-scale=1.0">
                </head>
                <body>
                </body>
            </html>
            """);

            compFile.AddContent($"""
            <!DOCTYPE html>
            <html>
                <head>
                    <meta name="viewport" content="width=device-width, initial-scale=1.0">
                </head>
                <body>
                    <{compName}></{compName}>
                </body>
            </html>
            """);

            componentTemplate.Build();
        }

        private static void VerifyDirectoryExists(OptionResult result, Option<DirectoryInfo> directoryOption)
        {
            var directoryInfo = result.GetValueForOption(directoryOption);
            if (directoryInfo is null)
            {
                result.ErrorMessage = $"No valid directory provided for options {directoryOption.Name}.";
                return;
            }

            if (!directoryInfo.Exists)
            {
                result.ErrorMessage = $"The directory '{directoryInfo.FullName}' for option {directoryOption.Name} doesn't exist.";
            }
        }

        private static void VerifyDirectoryNotExists(OptionResult result, Option<DirectoryInfo?> directoryOption)
        {
            var directoryInfo = result.GetValueForOption(directoryOption);
            if (directoryInfo is not null && directoryInfo.Exists)
            {
                result.ErrorMessage = $"The directory '{directoryInfo.FullName}' for option {directoryOption} already exists.";
            }
        }
    }
}