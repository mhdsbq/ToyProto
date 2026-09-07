# Parser

> **AI-generated documentation.**

The parser consumes the token list produced by the lexer and builds a
`ProtoFile` abstract syntax tree (AST). It is implemented by `Parser` in
`src/ToyProto.Compiler/Parse/Parser.cs`.

```csharp
var lexer = new Lexer();
var tokens = lexer.Tokenize(input);

var parser = new Parser();
var protoFile = parser.Parse(tokens);
```

The full compilation pipeline is:

```text
.proto source
    |
    v
Lexer.Tokenize
    |
    v
IReadOnlyList<Token>
    |
    v
Parser.Parse
    |
    v
ProtoFile AST
```

`ProtoCompiler.Compile` coordinates both stages.

## Parser design

The parser uses recursive descent. Each supported grammar rule has a parsing
method, and `ParserCursor` handles lookahead, matching, advancement, and EOF
detection.

`Expect` verifies that the next token has the required type. If it does not,
the parser throws `UnexpectedTokenException` with the expected and found token
types and an optional diagnostic message.

## Supported grammar

The current grammar is:

```text
protoFile = [ syntaxDeclaration ] { messageDeclaration }

syntaxDeclaration = "syntax" "=" stringLiteral ";"

messageDeclaration = "message" identifier "{"
                     { fieldDeclaration }
                     "}"

fieldDeclaration = identifier identifier "=" integerLiteral ";"
```

For example:

```proto
syntax = "proto3";

message SearchRequest {
  string query = 1;
  int32 page_number = 2;
  int32 results_per_page = 3;
}
```

The parser reads each field as a type, name, and integer field number:

```text
string query = 1;
------ -----   -
 type   name  number
```

A field number must be representable as a C# `int`. Invalid field numbers raise
`UnexpectedTokenException`.

## AST structure

The example above produces an AST shaped like this:

```text
ProtoFile
|- Syntax: "proto3"
`- Declarations
   `- MessageDeclaration
      |- Name: "SearchRequest"
      `- Fields
         |- FieldDeclaration("string", "query", 1)
         |- FieldDeclaration("int32", "page_number", 2)
         `- FieldDeclaration("int32", "results_per_page", 3)
```

The relevant AST types are:

- `ProtoFile`
- `MessageDeclaration`
- `FieldDeclaration`

They are located under `src/ToyProto.Compiler/Ast`.

## Parser errors

Parser errors are represented by `ParserException`. `UnexpectedTokenException`
records the token type that was expected and the token that was found.

Examples of invalid input include:

```proto
syntax "proto3";
```

```proto
message SearchRequest {
  string query 1;
}
```

```proto
message SearchRequest {
  string query = "one";
}
```

## Current limitations

The parser does not currently support:

- Imports
- Packages
- Options
- Enums
- Services or RPC declarations
- Nested messages
- Repeated, optional, or map fields
- Field options
- Comments
- Source locations in diagnostics
- Code generation
