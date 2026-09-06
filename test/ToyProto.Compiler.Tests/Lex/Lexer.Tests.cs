using ToyProto.Compiler.Lex;

namespace ToyProto.Compiler.Tests.Lex;

public class LexerTests
{
    [Fact]
    public void LexingSyntaxShouldReturnSyntaxToken()
    {
        var lexer = new Lexer();
        var proto = """
        syntax
        """;

        var tokens = lexer.Tokenize(proto);
        Assert.Single(tokens);
        Assert.Equal(TokenType.Syntax, tokens.First().Type);
    }

    [Fact]
    public void LexingStringLiteralShouldReturnStringLiteralToken()
    {
        var lexer = new Lexer();
        var proto = """
        "hello"
        """;

        var tokens = lexer.Tokenize(proto);
        Assert.Single(tokens);
        Assert.Equal(TokenType.StringLiteral, tokens.First().Type);
        Assert.Equal("hello", tokens.First().Value);
    }

    [Fact]
    public void LexingIdentifierShouldReturnIdentifierToken()
    {
        var lexer = new Lexer();
        var proto = """
        my_identifier1
        """;

        var tokens = lexer.Tokenize(proto);
        Assert.Single(tokens);
        Assert.Equal(TokenType.Identifier, tokens.First().Type);
        Assert.Equal("my_identifier1", tokens.First().Value);
    }
}