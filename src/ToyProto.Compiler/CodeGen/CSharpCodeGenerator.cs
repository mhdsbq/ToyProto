using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using ToyProto.Compiler.Ast;
using ToyProto.Compiler.Ast.Declarations;

namespace ToyProto.Compiler.CodeGen;

internal class CSharpCodeGenerator : ICodeGenerator
{
    public string GenerateCode(ProtoFile protoFile)
    {
        var messageClasses = protoFile.Messages.Select(m => GenerateMessageClass(m)).ToArray();
        var root = SyntaxFactory.CompilationUnit().AddMembers(messageClasses);
        var code = root.NormalizeWhitespace().ToFullString();

        return code;
    }

    private ClassDeclarationSyntax GenerateMessageClass(MessageDeclaration message)
    {
        var fieldProperties = message.Fields.Select(f => GenerateFieldProperty(f)).ToArray();
        var encodeMethod = GenerateEncodeMethod();
        var decodeMethod = GenerateDecodeMethod(message);
        return SyntaxFactory.ClassDeclaration(message.Name.ToPascalCase())
            .AddMembers(fieldProperties)
            .AddMembers(encodeMethod, decodeMethod);

    }

    // Generates: public <type> <name> { get; set; }
    private PropertyDeclarationSyntax GenerateFieldProperty(FieldDeclaration field)
    {
        return SyntaxFactory
            .PropertyDeclaration(SyntaxFactory.ParseTypeName(TranslateFieldType(field.Type)), field.Name.ToPascalCase())
            .AddModifiers(SyntaxFactory.Token(SyntaxKind.PublicKeyword))
            .AddAccessorListAccessors(
                SyntaxFactory.AccessorDeclaration(SyntaxKind.GetAccessorDeclaration)
                    .WithSemicolonToken(SyntaxFactory.Token(SyntaxKind.SemicolonToken)),
                SyntaxFactory.AccessorDeclaration(SyntaxKind.SetAccessorDeclaration)
                    .WithSemicolonToken(SyntaxFactory.Token(SyntaxKind.SemicolonToken)));
    }

    private MethodDeclarationSyntax GenerateEncodeMethod()
    {
        return SyntaxFactory.MethodDeclaration(SyntaxFactory.ParseTypeName("byte[]"), "Encode")
            .AddModifiers(
                SyntaxFactory.Token(SyntaxKind.PublicKeyword))
            .WithBody(
                SyntaxFactory.Block(
                    SyntaxFactory.ThrowStatement(SyntaxFactory.ObjectCreationExpression(SyntaxFactory.IdentifierName("NotImplementedException"))
                        .WithArgumentList(SyntaxFactory.ArgumentList()))));
    }

    private MethodDeclarationSyntax GenerateDecodeMethod(MessageDeclaration message)
    {
        return SyntaxFactory.MethodDeclaration(
                SyntaxFactory.IdentifierName(message.Name.ToPascalCase()),
                "Decode")
            .AddModifiers(
                SyntaxFactory.Token(SyntaxKind.PublicKeyword),
                SyntaxFactory.Token(SyntaxKind.StaticKeyword))
            .AddParameterListParameters(
                SyntaxFactory.Parameter(SyntaxFactory.Identifier("bytes"))
                    .WithType(SyntaxFactory.ParseTypeName("byte[]")))
            .WithBody(
                SyntaxFactory.Block(
                    SyntaxFactory.ThrowStatement(
                        SyntaxFactory.ObjectCreationExpression(
                            SyntaxFactory.IdentifierName("NotImplementedException"))
                        .WithArgumentList(SyntaxFactory.ArgumentList()))));
    }

    private string TranslateFieldType(string fieldType)
    {
        return fieldType switch
        {
            "int32" => "int",
            _ => fieldType
        };
    }
}

/*
Proto file:
```
    syntax = "proto3";

    message SearchRequest {
    string query            = 1;
    int32  page_number      = 2;
    int32  results_per_page = 3;
    }
```

Generated Code:
```c#
    class SearchRequest
    {
        public string Query { get; set; }
        public int PageNumber { get; set; }
        public int ResultsPerPage { get; set; }

        public SearchRequest(string query, int pageNumber, int resultsPerPage)
        {
            Query = query;
            PageNumber = pageNumber;
            ResultsPerPage = resultsPerPage;
        }


        public byte[] Encode()
        {
            throw new NotImplementedException();
        }

        public static SearchRequest Decode(byte[] bytes)
        {
            throw new NotImplementedException();
        }
    }
```
*/


