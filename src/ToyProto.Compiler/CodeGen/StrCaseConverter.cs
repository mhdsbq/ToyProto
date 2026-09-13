using System.Text;

namespace ToyProto.Compiler.CodeGen;

internal static class StrCaseConverter
{

    public static string ToPascalCase(this string str)
    {
        if (string.IsNullOrWhiteSpace(str))
            return string.Empty;

        var sb = new StringBuilder(str.Length);
        var capitalizeNext = true;
        foreach (var c in str)
        {
            if (c is '_')
            {
                capitalizeNext = true;
                continue;
            }

            sb.Append(capitalizeNext ? char.ToUpperInvariant(c) : c);
            capitalizeNext = false;
        }

        return sb.ToString();
    }
}
