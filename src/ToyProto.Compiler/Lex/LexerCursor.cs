using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace ToyProto.Compiler.Lex;

interface ILexerCursor
{
    int Position {get;}
    bool IsAtEnd {get;}

    char Peek(int offset = 0);
    bool TryPeekAhead(int offset, out char c);

    void Advance();
    void AdvanceWhile(Func<char, bool> advanceCondition);

    bool Match(char expected);
    bool Match(string expected);

    string Slice(int start);
    string Slice(int start, int end);
}

class LexerCursor : ILexerCursor
{
    public int Position => _pos;
    public bool IsAtEnd => _pos >= _len;

    private string _input;
    private int _len;
    private int _pos;

    public LexerCursor(string input)
    {
        _input = input;
        _len = input.Length;
        _pos = 0;
    }
    
    public char Peek(int offset = 0)
    {
        ArgumentOutOfRangeException.ThrowIfNegative(offset);

        var index = _pos + offset;
        if(index >= _len)
            throw new InvalidEnumArgumentException("Cannot peek past end of file");
        
        return _input[index];
    }

    public bool TryPeekAhead(int offset, out char c)
    {
        ArgumentOutOfRangeException.ThrowIfNegative(offset);
        
        c = '\0';

        var index = _pos + offset;
        if(index >= _len)
            return false;
        
        c = _input[index];
        return true;
    }
    
    public void Advance()
    {
        if(IsAtEnd)
            throw new InvalidOperationException("Cannot advance, Eof reached!");
        _pos++;
    }

    public void AdvanceWhile(Func<char, bool> advanceCondition)
    {
        ArgumentNullException.ThrowIfNull(advanceCondition);

        while(!IsAtEnd && advanceCondition(Peek()))
            Advance();
    }

    public bool Match(char expected)
    {
        if(IsAtEnd || Peek() != expected)
            return false;
        
        Advance();
        return true;
    }

    public bool Match(string expected)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(expected);

        if(_pos + expected.Length > _len)
            return false;

        if (!_input.AsSpan(_pos, expected.Length).SequenceEqual(expected.AsSpan()))
            return false;

        _pos += expected.Length;
        return true;
    }

    public string Slice(int start)
    {
        return Slice(start, _pos);
    }

    public string Slice(int start, int end)
    {
        if(start < 0 || start > _len)
            throw new ArgumentOutOfRangeException(nameof(start));

        if(end < start || end > _len)
            throw new ArgumentOutOfRangeException(nameof(end));

        return _input[start..end];
    }
}