namespace ToyProto.Compiler.Ast.Declarations;

internal record FieldDeclaration(
    string Type,
    string Name,
    int Number
) : AstNode;