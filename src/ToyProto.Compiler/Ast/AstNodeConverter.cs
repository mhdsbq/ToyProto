using System.Text.Json;
using System.Text.Json.Serialization;

namespace ToyProto.Compiler.Ast;

/// <summary>
/// 
/// FOR DEBUGGING ONLY.
/// 
/// Json converter for serializing AST nodes to JSON.
/// </summary>
internal sealed class AstNodeConverter : JsonConverter<AstNode>
{
    public override AstNode? Read(
        ref Utf8JsonReader reader,
        Type typeToConvert,
        JsonSerializerOptions options)
    {
        throw new NotSupportedException("AST deserialization is not supported.");
    }

    public override void Write(
        Utf8JsonWriter writer,
        AstNode value,
        JsonSerializerOptions options)
    {
        JsonSerializer.Serialize(
            writer,
            value,
            value.GetType(),
            options);
    }
}