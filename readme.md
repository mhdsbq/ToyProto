# ToyProto

A simple toy implementation of [protobuf](https://protobuf.dev/) in C# for C#

## Includes 
- a simple code generator (ToyProto.Compiler) that generates c# code from a .proto file
- a simple runtime library for c# (ToyProto.Runtime)
- a simple command line tool (ToyProto.Cli) that can be used to compile .proto files and encode or decode messages to and from json

## Supported features
- TBD

## Usage

### Command line tool
- `toyproto compile <proto file> -o <output directory>`
- `toyproto encode <proto file> -i <input json file> -o <output binary file>`
- `toyproto decode <proto file> -i <input binary file> -o <output json file>`

### C# generated code
```csharp
using ToyProto.Runtime;

var person = new Person
{
    Name = "John Doe",
    Age = 30,
    Email = "john.doe@example.com"
};

var bytes = person.Encode();
var decodedPerson = Person.Decode(bytes);
Console.WriteLine(decodedPerson.Name); // Output: John Doe
```

## Development

### Prerequisites
- .NET 10.0 SDK or later (NET 6.0 and above might work with change in csproj file)
- `Taskfile` for building and running the project - see: https://taskfile.dev/docs/installation

### Install cli 
```bash
task install-cli
```
- now you can run the cli tool with `toyproto` command