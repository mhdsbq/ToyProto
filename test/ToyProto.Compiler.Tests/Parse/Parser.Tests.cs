using ToyProto.Compiler.Ast;
using ToyProto.Compiler.Ast.Declarations;
using ToyProto.Compiler.Lex;
using ToyProto.Compiler.Parse;

namespace ToyProto.Compiler.Tests.Parse;

public class ParserTests
{
    [Fact]
    public void ParseShouldReturnSyntaxAndMessageDeclarations()
    {
        var source = "syntax = \"proto3\"; message User { string name = 1; int32 age = 2; }";

        var result = Parse(source);

        Assert.Equal("proto3", result.Syntax);
        var message = Assert.IsType<MessageDeclaration>(Assert.Single(result.Declarations));
        Assert.Equal("User", message.Name);
        Assert.Collection(
            message.Fields,
            field => Assert.Equal(new FieldDeclaration("string", "name", 1), field),
            field => Assert.Equal(new FieldDeclaration("int32", "age", 2), field));
    }

    [Fact]
    public void ParseShouldAllowAFileWithoutSyntaxDeclaration()
    {
        var result = Parse("message Empty {}");

        Assert.Null(result.Syntax);
        var message = Assert.IsType<MessageDeclaration>(Assert.Single(result.Declarations));
        Assert.Equal("Empty", message.Name);
        Assert.Empty(message.Fields);
    }

    [Theory]
    [InlineData("syntax \"proto3\";")]
    [InlineData("message { }")]
    [InlineData("message User string name = 1; }")]
    [InlineData("message User { string name 1; }")]
    [InlineData("message User { string name = 1 }")]
    public void ParseShouldThrowForMissingRequiredTokens(string source)
    {
        Assert.Throws<UnexpectedTokenException>(() => Parse(source));
    }

    [Fact]
    public void ParseShouldThrowWhenMessageClosingBraceIsMissing()
    {
        Assert.Throws<UnexpectedTokenException>(() => Parse("message User { string name = 1;"));
    }

    private static ProtoFile Parse(string source)
    {
        var lexer = new Lexer();
        var parser = new Parser();

        return parser.Parse(lexer.Tokenize(source));
    }
}