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
            var c = _input[_i];

            if (IsWhitespace(c))
            {
                _i++;
                continue;
            }

            if (IsLetter(c))
            {
                tokens.Add(ReadIdentifierOrKeywordToken());
                continue;
            }
        }

        return tokens;
    }

    private Token ReadIdentifierOrKeywordToken()
    {
        //ident = letter { letter | decimalDigit | "_" }

        var startIdx = _i;
        while(++_i < _input.Length)
        {
            var c = _input[_i];
            if(!IsLetter(c) && !IsDecimalDigit(c) && c is not '_')
                break;
        }

        var identifier = _input[startIdx .. _i];
        return Keyword.TryGetTokenType(identifier, out var keywordType) 
            ? new Token(keywordType)
            : new Token(TokenType.Identifier, identifier);
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
}