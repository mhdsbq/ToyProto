# ToyProto Compiler
Compiler for a subset of proto 3 language.

- ref: https://protobuf.dev/reference/protobuf/proto3-spec/

## Lexical tokens implemented
- Identifiers
- Keywords (all reserved keywords are implemented)
- String literals
- Boolean literals
- Integer literals
- Floating point literals

## Skipped features
- Comments
- Escaped characters in string literals

## Tech debts
- a cursor abstraction for input