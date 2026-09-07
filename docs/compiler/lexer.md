# Lexer

> **AI-generated documentation.**

The lexer converts `.proto` source text into tokens for the parser. It is
implemented by `Lexer` in `src/ToyProto.Compiler/Lex/Lexer.cs` and scans the
input from left to right with a `LexerCursor`.

```csharp
var lexer = new Lexer();
var tokens = lexer.Tokenize(input);
```

Every successful tokenization ends with an `EndOfFile` token.

## Token categories

Token types are declared in `src/ToyProto.Compiler/Lex/TokenType.cs`. The lexer
recognizes:

- Keywords such as `syntax`, `message`, `enum`, `service`, and `repeated`
- Identifiers
- Integer literals
- Floating-point literals
- String literals
- Boolean literals: `true` and `false`
- Symbols such as `=`, `;`, `{`, `}`, `(`, `)`, and `,`

Keyword and symbol lookup is centralized in `Keyword.cs` and `Symbol.cs`.

Identifiers and literals preserve their source value in the token. Keyword and
symbol tokens do not need a value because their token type identifies them.

```csharp
new Token(TokenType.Identifier, "SearchRequest")
new Token(TokenType.IntegerLiteral, "1")
new Token(TokenType.StringLiteral, "proto3")
new Token(TokenType.Message)
```

## Scanning order

For each input position, the lexer:

1. Skips whitespace.
2. Reads a word when the next character is a letter. The word becomes a
   keyword, boolean literal, floating-point literal (`inf` or `nan`), or
   identifier.
3. Reads a quoted string literal.
4. Reads a number, including an optional leading minus sign.
5. Reads a single-character symbol.
6. Throws `LexerException` when no rule matches the character.

Identifiers follow this form:

```text
letter { letter | decimal digit | "_" }
```

## Numeric literals

The lexer supports:

- Decimal integers: `0`, `123`, `-123`
- Hexadecimal integers: `0x10`, `0XFF`, `-0XFF`
- Decimal floating-point values: `3.14`
- Exponents: `1e10`, `3.14e-10`
- Special floating-point values: `inf`, `nan`

Malformed floating-point values raise `LexerException`, including `3.`,
`3.1.`, `3e`, `1e+`, and `1e-`.

## String literals

Both single and double quotes are accepted:

```proto
"proto3"
'proto3'
```

Adjacent quoted strings are combined into one `StringLiteral` token. Escape
sequences are not implemented yet; a backslash inside a string raises
`LexerException`.

## Cursor behavior

`LexerCursor` provides the primitive operations used by the lexer:

- `Peek` reads the current character without advancing.
- `TryPeekAhead` checks a future character without throwing at EOF.
- `Advance` moves one character forward.
- `AdvanceWhile` consumes characters while a predicate is true.
- `Match` consumes a character or string when it appears at the cursor.
- `Slice` returns the source text between two positions.

## Current limitations

The lexer does not currently support:

- Comments
- Escaped string characters
- Full Protocol Buffers literal validation

The lexer defines token types for several future Protocol Buffers features, but
defining a token does not mean that the parser currently accepts that feature.

## Tests

Lexer behavior is covered in
`test/ToyProto.Compiler.Tests/Lex/Lexer.Tests.cs`, including keyword,
identifier, literal, symbol, invalid-number, and unsupported-character cases.
