using System.CommandLine;

namespace ToyProto.Cli.Commands;

internal static class CompileCommand
{
    const string DefaultOutputPath = "./";
    public static Command Build()
    {
        var compileCommand = new Command("compile", "Compile a proto file")
        {
            new Argument<FileInfo>("proto")
            {
                Description = "File path of .proto file to be compiled"
            },
            new Option<DirectoryInfo>("--output", ["-o"])
            {
                Description = "Path to write compiled files",
            }
        };

        compileCommand.SetAction(Execute);
        return compileCommand;
    }

    public static void Execute(ParseResult parseResult)
    {
        var protoFile = parseResult.GetRequiredValue<FileInfo>("proto");
        var outputDir = parseResult.GetValue<DirectoryInfo>("--output")
            ?? new DirectoryInfo(DefaultOutputPath);

        if (!protoFile.Exists)
        {
            Console.Error.WriteLine($"Error: Proto file '{protoFile.FullName}' does not exist.");
            return;
        }

        Console.WriteLine($"[DEBUG] Compiling proto file: {protoFile.FullName} to output directory: {outputDir.FullName}");
    }
}