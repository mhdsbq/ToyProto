namespace ToyProto.Compiler.Lex;

public enum TokenType
{
    Unknown,

    // Keywords
    Syntax,
    Import,
    Weak,
    Public,
    Package,
    Option,
    Message,
    Enum,
    Service,
    Rpc,
    Returns,
    Stream,
    Repeated,
    Optional,
    Oneof,
    Map,
    Reserved,
    To,
    Max,

    // Literals
    Identifier,
    IntegerLiteral,
    FloatLiteral,
    StringLiteral,
    BoolLiteral,

    // Symbols
    Equals,         // =
    Semicolon,      // ;
    LeftBrace,      // {
    RightBrace,     // }
    LeftBracket,    // [
    RightBracket,   // ]
    LeftParen,      // (
    RightParen,     // )
    LeftAngle,      // <
    RightAngle,     // >
    Comma,          // ,
    Dot,            // .
    Plus,            // +
    Minus,           // -

    // Special
    EndOfFile
}