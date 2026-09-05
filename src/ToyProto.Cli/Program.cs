using System.CommandLine;
using ToyProto.Cli.Commands;

var rootCommand = new RootCommand("ToyProto cli")
{
    CompileCommand.Build()
};

rootCommand.Parse(args).Invoke();