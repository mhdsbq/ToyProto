using ToyProto.Compiler.Lex;

namespace ToyProto.Compiler.Tests.Lex;

public class LexerTests
{
    [Theory]
    [InlineData("syntax", TokenType.Syntax)]
    [InlineData("import", TokenType.Import)]
    [InlineData("weak", TokenType.Weak)]
    [InlineData("public", TokenType.Public)]
    [InlineData("package", TokenType.Package)]
    [InlineData("option", TokenType.Option)]
    [InlineData("message", TokenType.Message)]
    [InlineData("enum", TokenType.Enum)]
    [InlineData("service", TokenType.Service)]
    [InlineData("rpc", TokenType.Rpc)]
    [InlineData("returns", TokenType.Returns)]
    [InlineData("stream", TokenType.Stream)]
    [InlineData("repeated", TokenType.Repeated)]
    [InlineData("optional", TokenType.Optional)]
    [InlineData("oneof", TokenType.Oneof)]
    [InlineData("map", TokenType.Map)]
    [InlineData("reserved", TokenType.Reserved)]
    [InlineData("to", TokenType.To)]
    [InlineData("max", TokenType.Max)]
    public void KeywordLexingShouldReturnExpectedTokenType(string input, TokenType expectedType)
    {
        var lexer = new Lexer();

        var tokens = lexer.Tokenize(input);

        var token = Assert.Single(tokens);
        Assert.Equal(expectedType, token.Type);
        Assert.Null(token.Value);
    }

    [Theory]
    [InlineData("my_identifier1", "my_identifier1")]
    [InlineData("foo_bar", "foo_bar")]
    [InlineData("alpha", "alpha")]
    public void IdentifierLexingShouldReturnIdentifierToken(string input, string expectedValue)
    {
        var lexer = new Lexer();

        var tokens = lexer.Tokenize(input);

        var token = Assert.Single(tokens);
        Assert.Equal(TokenType.Identifier, token.Type);
        Assert.Equal(expectedValue, token.Value);
    }

    [Theory]
    [InlineData("0", "0")]
    [InlineData("123", "123")]
    [InlineData("-123", "-123")]
    [InlineData("0x10", "0x10")]
    [InlineData("0XFF", "0XFF")]
    [InlineData("-0XFF", "-0XFF")]
    public void IntegerLexingShouldReturnIntegerLiteralToken(string input, string expectedValue)
    {
        var lexer = new Lexer();

        var tokens = lexer.Tokenize(input);

        var token = Assert.Single(tokens);
        Assert.Equal(TokenType.IntegerLiteral, token.Type);
        Assert.Equal(expectedValue, token.Value);
    }

    [Theory]
    [InlineData("3.14", "3.14")]
    [InlineData("1e10", "1e10")]
    [InlineData("3.14e10", "3.14e10")]
    [InlineData("3.14e0", "3.14e0")]
    [InlineData("3.14e-10", "3.14e-10")]
    [InlineData("-3.14e-10", "-3.14e-10")]
    [InlineData("inf", "inf")]
    [InlineData("nan", "nan")]
    public void FloatLexingShouldReturnFloatLiteralToken(string input, string expectedValue)
    {
        var lexer = new Lexer();

        var tokens = lexer.Tokenize(input);

        var token = Assert.Single(tokens);
        Assert.Equal(TokenType.FloatLiteral, token.Type);
        Assert.Equal(expectedValue, token.Value);
    }

    [Theory]
    [InlineData("3.1.")]
    [InlineData("3.1e")]
    [InlineData("3.")]
    [InlineData("3e")]
    [InlineData("1e+")]
    [InlineData("1e-")]
    public void InvalidFloatLexingShouldThrowLexerException(string input)
    {
        var lexer = new Lexer();

        Assert.Throws<LexerException>(() => lexer.Tokenize(input));
    }

    [Theory]
    [InlineData("\"hello\"", "hello")]
    [InlineData("\"\"", "")]
    [InlineData("'world'", "world")]
    [InlineData("''", "")]
    public void StringLexingShouldReturnStringLiteralToken(string input, string expectedValue)
    {
        var lexer = new Lexer();

        var tokens = lexer.Tokenize(input);

        var token = Assert.Single(tokens);
        Assert.Equal(TokenType.StringLiteral, token.Type);
        Assert.Equal(expectedValue, token.Value);
    }

    [Theory]
    [InlineData("true", "true")]
    [InlineData("false", "false")]
    public void BoolLexingShouldReturnBoolLiteralToken(string input, string expectedValue)
    {
        var lexer = new Lexer();

        var tokens = lexer.Tokenize(input);

        var token = Assert.Single(tokens);
        Assert.Equal(TokenType.BoolLiteral, token.Type);
        Assert.Equal(expectedValue, token.Value);
    }

    [Theory]
    [InlineData("=", TokenType.Equals)]
    [InlineData(";", TokenType.Semicolon)]
    [InlineData("{", TokenType.LeftBrace)]
    [InlineData("}", TokenType.RightBrace)]
    [InlineData("[", TokenType.LeftBracket)]
    [InlineData("]", TokenType.RightBracket)]
    [InlineData("(", TokenType.LeftParen)]
    [InlineData(")", TokenType.RightParen)]
    [InlineData("<", TokenType.LeftAngle)]
    [InlineData(">", TokenType.RightAngle)]
    [InlineData(",", TokenType.Comma)]
    [InlineData(".", TokenType.Dot)]
    [InlineData("+", TokenType.Plus)]
    [InlineData("-", TokenType.Minus)]
    public void SymbolLexingShouldReturnExpectedTokenType(string input, TokenType expectedType)
    {
        var lexer = new Lexer();

        var tokens = lexer.Tokenize(input);

        var token = Assert.Single(tokens);
        Assert.Equal(expectedType, token.Type);
        Assert.Null(token.Value);
    }

    [Theory]
    [InlineData("@")]
    [InlineData("$")]
    [InlineData("?")]
    public void UnsupportedCharactersShouldThrowLexerException(string input)
    {
        var lexer = new Lexer();

        Assert.Throws<LexerException>(() => lexer.Tokenize(input));
    }
}