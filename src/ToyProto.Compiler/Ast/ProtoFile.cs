using ToyProto.Compiler.Ast.Declarations;

namespace ToyProto.Compiler.Ast;

internal record ProtoFile(
    string? Syntax,
    IReadOnlyList<MessageDeclaration> Messages
) : AstNode;