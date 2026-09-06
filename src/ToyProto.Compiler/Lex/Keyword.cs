namespace ToyProto.Compiler.Lex;

public static class Keyword
{
    private static readonly Dictionary<string, TokenType> _keywords = new()
    {
        { "syntax", TokenType.Syntax },
        { "import", TokenType.Import },
        { "weak",   TokenType.Weak },
        { "public", TokenType.Public },
        { "package",TokenType.Package },
        { "option", TokenType.Option },
        { "message",TokenType.Message },
        { "enum",   TokenType.Enum },
        { "service",TokenType.Service },
        { "rpc",    TokenType.Rpc },
        { "returns",TokenType.Returns },
        { "stream", TokenType.Stream },
        { "repeated", TokenType.Repeated },
        { "optional",TokenType.Optional },
        { "oneof",  TokenType.Oneof },
        { "map",    TokenType.Map },
        { "reserved",TokenType.Reserved },
        { "to",     TokenType.To },
        { "max",    TokenType.Max }
    };

    public static bool TryGetTokenType(string keyword, out TokenType tokenType)
    {
        return _keywords.TryGetValue(keyword, out tokenType);
    }
}