using ToyProto.Compiler.Ast;
using ToyProto.Compiler.Ast.Declarations;
using ToyProto.Compiler.Lex;

namespace ToyProto.Compiler.Parse;

internal interface IParser
{
    ProtoFile Parse(IReadOnlyList<Token> tokens);
}

internal class Parser : IParser
{
    private ParserCursor _cursor = default!;

    public ProtoFile Parse(IReadOnlyList<Token> tokens)
    {
        _cursor = new ParserCursor(tokens);
        return ParseInternal();
    }

    private ProtoFile ParseInternal()
    {
        var syntax = ParseSyntax();
        var declarations = new List<AstNode>();

        while (!_cursor.IsAtEnd)
        {
            if (_cursor.Match(TokenType.Message))
            {
                declarations.Add(ParseMessage());
                continue;
            }
        }

        return new ProtoFile(syntax, declarations);
    }

    private string? ParseSyntax()
    {
        if (!_cursor.Match(TokenType.Syntax))
            return null;

        Expect(TokenType.Equals, "Expected '=' after 'syntax' keyword");
        var version = Expect(TokenType.StringLiteral, "Expected string literal after '=' in 'syntax' declaration");
        Expect(TokenType.Semicolon);

        return version.Value;
    }

    private MessageDeclaration ParseMessage()
    {
        var name = Expect(TokenType.Identifier, "Expected identifier after 'message' keyword");
        Expect(TokenType.LeftBrace);

        var fields = new List<FieldDeclaration>();
        while (!_cursor.IsAtEnd && !_cursor.Match(TokenType.RightBrace))
            fields.Add(ParseField());

        return new MessageDeclaration(name.Value!, fields);
    }

    private FieldDeclaration ParseField()
    {
        var type = Expect(TokenType.Identifier);
        var name = Expect(TokenType.Identifier);

        Expect(TokenType.Equals);

        var number = Expect(TokenType.IntegerLiteral);
        if (!int.TryParse(number.Value, out var fieldNumber))
            throw new UnexpectedTokenException(TokenType.IntegerLiteral, number, "Field number must be a valid integer");

        Expect(TokenType.Semicolon);

        return new FieldDeclaration(type.Value!, name.Value!, fieldNumber);
    }

    private Token Expect(TokenType tokenType, string? message = null)
    {
        if (_cursor.Match(tokenType, out var token))
            return token;

        throw new UnexpectedTokenException(tokenType, token, message);
    }
}