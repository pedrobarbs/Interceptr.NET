using Microsoft.CodeAnalysis;

namespace Interceptr
{
    public static class ITypeSymbolExtensions
    {
        public static string GetFullName(this ITypeSymbol type)
        {
            return $"{type}";
        }
    }
}
