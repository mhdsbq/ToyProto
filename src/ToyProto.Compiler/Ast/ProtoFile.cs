namespace ToyProto.Compiler.Ast;

internal record ProtoFile(
    string? Syntax,
    IReadOnlyList<AstNode> Declarations
) : AstNode;