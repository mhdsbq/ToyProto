using ToyProto.Compiler.Ast;

namespace ToyProto.Compiler.CodeGen;

internal interface ICodeGenerator
{
    string GenerateCode(ProtoFile protoFile);
}
