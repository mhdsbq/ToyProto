namespace ToyProto.Compiler.Ast.Declarations;

internal record MessageDeclaration(
    string Name,
    IReadOnlyList<FieldDeclaration> Fields
) : AstNode;