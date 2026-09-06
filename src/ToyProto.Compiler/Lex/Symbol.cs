namespace ToyProto.Compiler.Lex;

public static class Symbol
{
    private static readonly Dictionary<char, TokenType> _symbols = new()
    {
        { '=', TokenType.Equals },
        { ';', TokenType.Semicolon },
        { '{', TokenType.LeftBrace },
        { '}', TokenType.RightBrace },
        { '[', TokenType.LeftBracket },
        { ']', TokenType.RightBracket },
        { '(', TokenType.LeftParen },
        { ')', TokenType.RightParen },
        { '<', TokenType.LeftAngle },
        { '>', TokenType.RightAngle },
        { ',', TokenType.Comma },
        { '.', TokenType.Dot },
        { '+', TokenType.Plus },
        { '-', TokenType.Minus }
    };
    public static bool TryGetTokenType(char symbol, out TokenType tokenType)
    {
        return _symbols.TryGetValue(symbol, out tokenType);
    }
}