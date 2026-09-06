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

    [Fact]
    public void LexingIntegerLiteralShouldReturnIntegerLiteralToken()
    {
        var lexer = new Lexer();
        var proto = """
        123
        """;

        var tokens = lexer.Tokenize(proto);
        Assert.Single(tokens);
        Assert.Equal(TokenType.IntegerLiteral, tokens.First().Type);
        Assert.Equal("123", tokens.First().Value);
    }

    [Theory]
    [InlineData("3.14")]
    [InlineData("1e10")]
    [InlineData("3.14e10")]
    [InlineData("3.14e0")]
    [InlineData("3.14e-10")]
    public void LexingFloatLiteralShouldReturnFloatLiteralToken(string floatString)
    {
        var lexer = new Lexer();

        var tokens = lexer.Tokenize(floatString);

        Assert.Single(tokens);
        Assert.Equal(TokenType.FloatLiteral, tokens.First().Type);
        Assert.Equal(floatString, tokens.First().Value);
    }

    [Theory]
    [InlineData("3.1.")]
    [InlineData("3.1e")]
    [InlineData("3.")]
    [InlineData("3e")]
    public void LexingInvalidFloatingLiteralShouldThrowLexerException(string floatString)
    {
        var lexer = new Lexer();
        Assert.Throws<LexerException>(() => lexer.Tokenize(floatString));
    }

    [Fact]
    public void LexingTrueShouldReturnBoolLiteralToken()
    {
        var lexer = new Lexer();
        var proto = """
        true
        """;

        var tokens = lexer.Tokenize(proto);
        Assert.Single(tokens);
        Assert.Equal(TokenType.BoolLiteral, tokens.First().Type);
        Assert.Equal("true", tokens.First().Value);
    }

    [Fact]
    public void LexingFalseShouldReturnBoolLiteralToken()
    {
        var lexer = new Lexer();
        var proto = """
        false
        """;

        var tokens = lexer.Tokenize(proto);
        Assert.Single(tokens);
        Assert.Equal(TokenType.BoolLiteral, tokens.First().Type);
        Assert.Equal("false", tokens.First().Value);
    }
}