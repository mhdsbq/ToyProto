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

            throw new LexerException($"Unexpected character {c} in position {_i}");
        }

        return tokens;
    }

    private Token ReadWord()
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

        var value = _input[startIdx.._i];

        return value switch
        {
            "true" or "false"
                => new Token(TokenType.BoolLiteral, value),

            _ when Keyword.TryGetTokenType(value, out var keywordType)
                => new Token(keywordType),

            _ => new Token(TokenType.Identifier, value)
        };
    }

    private Token ReadNumber()
    {
        // ref: https://protobuf.dev/reference/protobuf/proto3-spec/#integer_literals
        // ref: https://protobuf.dev/reference/protobuf/proto3-spec/#floating-point-literals

        var startIdx = _i;

        if (_input[_i] is '-')
            _i++;

        // NOTE: Hex starts with 0(x|X) and doesn't allow decimals
        if (_input[_i] is '0' && _i + 1 < _input.Length && _input[_i + 1] is 'x' or 'X')
        {
            _i++;
            _i++;

            while (_i < _input.Length)
            {
                var c = _input[_i];
                if (!IsHexDigit(c))
                    break;

                _i++;
            }

            return new Token(TokenType.IntegerLiteral, _input[startIdx.._i]);
        }

        while (_i < _input.Length)
        {
            var c = _input[_i];

            if (IsDecimalDigit(c) || IsOctalDigit(c))
            {
                _i++;
                continue;
            }

            break;
        }

        if (_i >= _input.Length || _input[_i] is not ('.' or 'e' or 'E'))
        {
            return new Token(TokenType.IntegerLiteral, _input[startIdx.._i]);
        }

        // Continue with float literal parsing. ie; decimals and exponential
        if (_input[_i] is '.')
        {
            _i++;

            while (_i < _input.Length)
            {
                var c = _input[_i];
                if (IsDecimalDigit(c))
                {
                    _i++;
                    continue;
                }

                break;
            }
        }


        if (_i >= _input.Length)
        {
            return new Token(TokenType.FloatLiteral, _input[startIdx.._i]);
        }

        if (_input[_i] is 'e' or 'E')
        {
            _i++;

            if (_i < _input.Length && _input[_i] is '+' or '-')
                _i++;

            // TODO: Enforce at least one decimal after exponential start => (e|E)([+] | [-]) {decimal digit} 
            while (_i < _input.Length)
            {
                var c = _input[_i];
                if (IsDecimalDigit(c))
                {
                    _i++;
                    continue;
                }

                break;
            }
        }

        return new Token(TokenType.FloatLiteral, _input[startIdx.._i]);
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
        // NOTE: tokens starting with - is either integer or floating point literal
        return IsDecimalDigit(c) || c is '-';
    }
}