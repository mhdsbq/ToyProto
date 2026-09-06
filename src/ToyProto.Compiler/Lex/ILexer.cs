namespace ToyProto.Compiler.Lex;

public interface ILexer
{
    List<Token> Tokenize(string input);
}