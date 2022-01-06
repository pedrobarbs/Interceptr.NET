using Microsoft.CodeAnalysis;

namespace TheInterceptor
{
    public static class ITypeSymbolExtensions
    {
        public static string GetFullName(this ITypeSymbol type)
        {
            return $"{type}";
        }
    }
}
