using ToyProto.Compiler.Lex;

namespace ToyProto.Compiler.Parse;

internal interface IParserCursor
{
    int Position { get; }
    bool IsAtEnd { get; }

    Token Peek(int offset = 0);
    bool TryPeekAhead(int offset, out Token token);

    void Advance();
    void AdvanceWhile(Func<Token, bool> advanceCondition);

    bool Match(TokenType expected);
    bool Match(TokenType expected, out Token value);
}

internal class ParserCursor : IParserCursor
{
    public int Position => _position;
    public bool IsAtEnd => _position >= _tokens.Count || Peek().Type == TokenType.EndOfFile;

    private readonly IReadOnlyList<Token> _tokens;
    private int _position;

    public ParserCursor(IReadOnlyList<Token> tokens)
    {
        ArgumentNullException.ThrowIfNull(tokens);

        _tokens = tokens;
        _position = 0;
    }

    public Token Peek(int offset = 0)
    {
        ArgumentOutOfRangeException.ThrowIfNegative(offset);

        var index = _position + offset;
        if (index >= _tokens.Count)
            throw new InvalidOperationException("Cannot peek past end of tokens");

        return _tokens[index];
    }

    public bool TryPeekAhead(int offset, out Token token)
    {
        ArgumentOutOfRangeException.ThrowIfNegative(offset);

        var index = _position + offset;
        if (index >= _tokens.Count)
        {
            token = default!;
            return false;
        }

        token = _tokens[index];
        return true;
    }

    public void Advance()
    {
        if (IsAtEnd)
            throw new InvalidOperationException("Cannot advance, end of tokens reached!");

        _position++;
    }

    public void AdvanceWhile(Func<Token, bool> advanceCondition)
    {
        ArgumentNullException.ThrowIfNull(advanceCondition);

        while (!IsAtEnd && advanceCondition(Peek()))
            Advance();
    }

    public bool Match(TokenType expected)
    {
        if (IsAtEnd || Peek().Type != expected)
            return false;

        Advance();
        return true;
    }

    public bool Match(TokenType expected, out Token token)
    {
        if (IsAtEnd || Peek().Type != expected)
        {
            token = Peek();
            return false;
        }

        token = Peek();
        Advance();
        return true;
    }
}