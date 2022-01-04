using Microsoft.CodeAnalysis;

namespace TheInterceptor.SourceGenerator
{
    public static class ITypeSymbolExtensions
    {
        public static string GetFullName(this ITypeSymbol type)
        {
            return $"{type}";
        }
    }
}
