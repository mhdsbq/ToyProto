using ToyProto.Compiler.Lex;

namespace ToyProto.Compiler.Parse;

public class ParserException(string? message) : Exception(message) { }

public class UnexpectedTokenException(TokenType expected, Token found, string? message = null) : ParserException(message)
{
    TokenType Expected { get; } = expected;
    Token Found { get; } = found;
}