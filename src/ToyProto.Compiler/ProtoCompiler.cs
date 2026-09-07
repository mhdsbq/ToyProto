using System.Text.Json;
using System.Text.Json.Serialization;
using ToyProto.Compiler.Ast;
using ToyProto.Compiler.Lex;
using ToyProto.Compiler.Parse;

namespace ToyProto.Compiler;

public class ProtoCompiler
{
    public CompilationResult Compile(string input)
    {
        var lexer = new Lexer();
        var parser = new Parser();

        var tokens = lexer.Tokenize(input);

        // DEBUG LOG
        Console.WriteLine("\n[DEBUG] ## Tokens: ");
        foreach (var token in tokens)
            Console.WriteLine($"[DEBUG] {token.Type,-15} - {token.Value}");

        var ast = parser.Parse(tokens);

        // DEBUG LOG
        Console.WriteLine("\n[DEBUG] ## AST: ");
        var options = new JsonSerializerOptions
        {
            WriteIndented = true,
            DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull,
            Converters = { new AstNodeConverter() }
        };

        Console.WriteLine(JsonSerializer.Serialize(ast, options));

        // var csharpCode = CodeGenerator.Generate(ast);
        return new CompilationResult(true, string.Empty, null);
    }
}

public record CompilationResult(bool Success, string Output, string? Error);