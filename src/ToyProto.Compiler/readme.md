# ToyProto Compiler

Compiler for a subset of proto 3 language.

- ref: https://protobuf.dev/reference/protobuf/proto3-spec/

## Lexer

### Lexical tokens implemented

- Identifiers
- Keywords (all reserved keywords are implemented)
- String literals
- Boolean literals
- Integer literals
- Floating point literals

### Skipped features

- Comments
- Escaped characters in string literals

## Parser

### Supported grammar rules

- Syntax: `syntax = "proto3";`
- Message definitions: `message MessageName { ... }`
- Field definitions: `fieldType fieldName = fieldNumber;`
