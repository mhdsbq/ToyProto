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
        Expect(TokenType.StringLiteral, out var version, "Expected string literal after '=' in 'syntax' declaration");
        Expect(TokenType.Semicolon);

        return version.Value;
    }

    private MessageDeclaration ParseMessage()
    {
        Expect(TokenType.Identifier, out var name, "Expected identifier after 'message' keyword");
        Expect(TokenType.LeftBrace);

        var fields = new List<FieldDeclaration>();
        while (!_cursor.IsAtEnd && !_cursor.Check(TokenType.RightBrace))
            fields.Add(ParseField());

        Expect(TokenType.RightBrace);

        return new MessageDeclaration(name.Value!, fields);
    }

    private FieldDeclaration ParseField()
    {
        Expect(TokenType.Identifier, out var type, "Expected field type");
        Expect(TokenType.Identifier, out var name, "Expected field name");
        Expect(TokenType.Equals);
        Expect(TokenType.IntegerLiteral, out var number, "Expected field number");

        if (!int.TryParse(number.Value, out var fieldNumber))
            throw new UnexpectedTokenException(TokenType.IntegerLiteral, number, "Field number must be a valid integer");

        Expect(TokenType.Semicolon);

        return new FieldDeclaration(type.Value!, name.Value!, fieldNumber);
    }

    private void Expect(TokenType tokenType, string? message = null)
    {
        if (_cursor.Match(tokenType, out var token))
            return;

        throw new UnexpectedTokenException(tokenType, token, message);
    }

    // Just for ease of read :)
    //
    // Earlier:
    //      Expect(TokenType.Equals, "Expected equal");
    //      var version = Expect(TokenType.StringLiteral, "Expected string literal");
    //      Expect(TokenType.Semicolon);
    //
    // Now:
    //      Expect(TokenType.Equals, out var equalsToken, "Expected equal");
    //      Expect(TokenType.StringLiteral, out var versionToken, "Expected string literal");
    //      Expect(TokenType.Semicolon, out var semicolonToken);
    private Token Expect(TokenType tokenType, out Token token, string? message = null)
    {
        if (_cursor.Match(tokenType, out token))
            return token;

        throw new UnexpectedTokenException(tokenType, token, message);
    }
}