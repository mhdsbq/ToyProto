using System.Text;

namespace ToyProto.Compiler.Lex;

public interface ILexer
{
    List<Token> Tokenize(string input);
}

public class Lexer : ILexer
{
    private LexerCursor _cursor = default!;

    public List<Token> Tokenize(string input)
    {
        _cursor = new LexerCursor(input);
        return TokenizeInternal();
    }

    private List<Token> TokenizeInternal()
    {
        var tokens = new List<Token>();

        while (!_cursor.IsAtEnd)
        {
            SkipWhitespaceAndComments();

            if (_cursor.IsAtEnd)
                break;

            var c = _cursor.Peek();

            if (IsLetter(c))
            {
                tokens.Add(ReadWord());
                continue;
            }

            if (IsQuote(c))
            {
                tokens.Add(ReadStringLiteral());
                continue;
            }

            if (IsNumberStart(c))
            {
                tokens.Add(ReadNumber());
                continue;
            }

            if(Symbol.TryGetTokenType(c, out var tokenType))
            {
                tokens.Add(new Token(tokenType));
                _cursor.Advance();
                continue;
            }

            throw new LexerException($"Unexpected character {c} in position {_cursor.Position}");
        }

        tokens.Add(new Token(TokenType.EndOfFile));
        return tokens;
    }

    private Token ReadWord()
    {
        // ref: https://protobuf.dev/reference/protobuf/proto3-spec/#identifiers
        // ident = letter { letter | decimalDigit | "_" }

        var start = _cursor.Position;
        _cursor.AdvanceWhile((c) => IsLetter(c) || IsDecimalDigit(c) || c is '_');

        var value = _cursor.Slice(start);

        return value switch
        {
            "true" or "false"
                => new Token(TokenType.BoolLiteral, value),

            "inf" or "nan"
                => new Token(TokenType.FloatLiteral, value),

            _ when Keyword.TryGetTokenType(value, out var keywordType)
                => new Token(keywordType),

            _ => new Token(TokenType.Identifier, value)
        };
    }

    private Token ReadNumber()
    {
        // ref: https://protobuf.dev/reference/protobuf/proto3-spec/#integer_literals
        // ref: https://protobuf.dev/reference/protobuf/proto3-spec/#floating-point-literals

        var start = _cursor.Position;

        // negative symbol is accepted
        _cursor.Match('-');

        // NOTE: Hex starts with 0(x|X) and doesn't allow decimals
        if (_cursor.Match("0x") || _cursor.Match("0X"))
        {
            _cursor.AdvanceWhile(IsHexDigit);
            return new Token(TokenType.IntegerLiteral, _cursor.Slice(start));
        }

        _cursor.AdvanceWhile(IsDecimalDigit);
        if (_cursor.IsAtEnd || _cursor.Peek() is not ('.' or 'e' or 'E'))
            return new Token(TokenType.IntegerLiteral, _cursor.Slice(start));

        // Continue with float literal parsing

        // Decimal parsing
        if (_cursor.Match('.'))
        {
            // At least one decimal digit is required after a decimal (.)
            if (_cursor.IsAtEnd || !IsDecimalDigit(_cursor.Peek()))
                throw new LexerException("At least one decimal digit is required after decimal symbol.");

            _cursor.AdvanceWhile(IsDecimalDigit);
        }

        if (_cursor.IsAtEnd)
            return new Token(TokenType.FloatLiteral, _cursor.Slice(start));

        // Ensure there isn't a trailing decimal point
        if (_cursor.Peek() == '.')
            throw new LexerException("Unexpected trailing decimal point in float literal.");

        // Exponential parsing
        if (_cursor.Match('e') || _cursor.Match('E'))
        {
            _cursor.Match('+');
            _cursor.Match('-');

            // There should be at least one decimal digit after Exponential symbol
            if (_cursor.IsAtEnd || !IsDecimalDigit(_cursor.Peek()))
                throw new LexerException($"Expected at least one decimal after exponential at position: {_cursor.Position}");

            _cursor.AdvanceWhile(IsDecimalDigit);
        }

        return new Token(TokenType.FloatLiteral, _cursor.Slice(start));
    }


    private Token ReadStringLiteral()
    {
        // ref: https://protobuf.dev/reference/protobuf/proto3-spec/#string_literals
        // strLit = strLitSingle { strLitSingle }

        var sb = new StringBuilder();
        while (!_cursor.IsAtEnd && IsQuote(_cursor.Peek()))
        {
            sb.Append(ReadStringLiteralSingle());
            SkipWhitespaceAndComments();
        }

        return new Token(TokenType.StringLiteral, sb.ToString());
    }

    private string ReadStringLiteralSingle()
    {
        // ref: https://protobuf.dev/reference/protobuf/proto3-spec/#string_literals
        // strLitSingle = ( "'" { charValue } "'" ) |  ( '"' { charValue } '"' )

        char quote = _cursor.Peek();
        _cursor.Advance();

        var sb = new StringBuilder();
        while (!_cursor.IsAtEnd && !_cursor.Match(quote))
        {
            sb.Append(ReadCharValue());
        }

        return sb.ToString();
    }

    private char ReadCharValue()
    {
        // ref: https://protobuf.dev/reference/protobuf/proto3-spec/#string_literals
        // charValue = hexEscape | octEscape | charEscape | unicodeEscape | unicodeLongEscape | /[^\0\n\\]/

        // NOTE: Only supporting non escaped char for now.
        // TODO: Implement reading escaped char
        var c = _cursor.Peek();
        if (c is '\\')
        {
            throw new LexerException("Escape sequences are not supported yet.");
        }

        _cursor.Advance();
        return c;
    }

    private void SkipWhitespaceAndComments()
    {
        // NOTE: Comments are not supported for now
        // TODO: Implement comments

        _cursor.AdvanceWhile(IsWhitespace);
    }

    private bool IsWhitespace(char c)
    {
        return c is ' ' or '\t' or '\n' or '\r';
    }

    private bool IsLetter(char c)
    {
        return ('a' <= c && c <= 'z') || ('A' <= c && c <= 'Z');
    }

    private bool IsDecimalDigit(char c)
    {
        return '0' <= c && c <= '9';
    }

    private bool IsOctalDigit(char c)
    {
        return '0' <= c && c <= '7';
    }

    private bool IsHexDigit(char c)
    {
        return IsDecimalDigit(c)
            || 'a' <= c && c <= 'f'
            || 'A' <= c && c <= 'F';
    }

    private bool IsQuote(char c)
    {
        return c is '\'' or '"';
    }

    private bool IsNumberStart(char c)
    {
        if (IsDecimalDigit(c))
            return true;

        if (c is '-' && _cursor.TryPeekAhead(1, out var next))
            return IsDecimalDigit(next);

        return false;
    }
}