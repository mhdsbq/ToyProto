using ToyProto.Compiler.Lex;

namespace ToyProto.Compiler;

public class ProtoCompiler
{
    public CompilationResult Compile(ProtoFile protoFile)
    {
        var lexer = new Lexer();
        // var tokens = Lexer.Tokenize(protoFile.Content);
        // var ast = Parser.Parse(tokens);
        // var csharpCode = CodeGenerator.Generate(ast);
        // return new CompilationResult(true, csharpCode, null);
        throw new NotImplementedException("The Compile method is not implemented yet.");
    }
}

public class CompilationResult
{
    public bool Success { get; set; }
    public string Output { get; set; }
    public string ErrorMessage { get; set; }
}

public class ProtoFile
{
    public string FilePath { get; set; }
    public string Content { get; set; }
}