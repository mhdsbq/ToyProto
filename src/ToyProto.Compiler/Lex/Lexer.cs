using System.Text;

namespace ToyProto.Compiler.Lex;

public class Lexer : ILexer
{
    private string _input = string.Empty;
    private int _i;

    public List<Token> Tokenize(string input)
    {
        _input = input;
        _i = 0;

        return TokenizeInternal();
    }

    private List<Token> TokenizeInternal()
    {
        var tokens = new List<Token>();

        while (_i < _input.Length)
        {
            SkipWhitespaceAndComments();

            var c = _input[_i];

            if (IsLetter(c))
            {
                tokens.Add(ReadIdentifierOrKeywordToken());
                continue;
            }

            if (IsQuote(c))
            {
                tokens.Add(ReadStringLiteral());
                continue;
            }

            throw new LexerException($"Unexpected character {c} in position {_i}");
        }

        return tokens;
    }

    private Token ReadIdentifierOrKeywordToken()
    {
        // ref: https://protobuf.dev/reference/protobuf/proto3-spec/#identifiers
        // ident = letter { letter | decimalDigit | "_" }

        var startIdx = _i;
        while (++_i < _input.Length)
        {
            var c = _input[_i];
            if (!IsLetter(c) && !IsDecimalDigit(c) && c is not '_')
                break;
        }

        var identifier = _input[startIdx.._i];
        return Keyword.TryGetTokenType(identifier, out var keywordType)
            ? new Token(keywordType)
            : new Token(TokenType.Identifier, identifier);
    }

    private Token ReadStringLiteral()
    {
        // ref: https://protobuf.dev/reference/protobuf/proto3-spec/#string_literals
        // strLit = strLitSingle { strLitSingle }

        var c = _input[_i];

        var sb = new StringBuilder();
        while (_i < _input.Length && IsQuote(c))
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

        char quote = _input[_i];
        _i++;

        var sb = new StringBuilder();
        while (_i < _input.Length && _input[_i] != quote)
        {
            sb.Append(ReadCharValue());
        }

        _i++;

        return sb.ToString();
    }

    private char ReadCharValue()
    {
        // ref: https://protobuf.dev/reference/protobuf/proto3-spec/#string_literals
        // charValue = hexEscape | octEscape | charEscape | unicodeEscape | unicodeLongEscape | /[^\0\n\\]/

        // NOTE: Only supporting non escaped char for now.
        // TODO: Implement reading escaped char
        var c = _input[_i];
        if (c is '\\')
        {
            throw new LexerException("Escape sequences are not supported yet.");
        }

        _i++;
        return c;
    }

    private void SkipWhitespaceAndComments()
    {
        // NOTE: Comments are not supported for now
        // TODO: Implement comments

        while (_i < _input.Length)
        {
            if (IsWhitespace(_input[_i]))
            {
                _i++;
                continue;
            }

            break;
        }
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

    private bool IsQuote(char c)
    {
        return c is '\'' or '"';
    }
}