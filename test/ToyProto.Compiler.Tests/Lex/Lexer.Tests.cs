using ToyProto.Compiler.Lex;

namespace ToyProto.Compiler.Tests.Lex;

public class LexerTests
{
    [Fact]
    public void LexingSyntaxProtoShouldReturnSyntaxToken()
    {
        var lexer = new Lexer();
        var proto = """
        syntax
        """;

        var tokens = lexer.Tokenize(proto);
        Assert.Single(tokens);
        Assert.Equal(TokenType.Syntax, tokens.First().Type);
    }
}