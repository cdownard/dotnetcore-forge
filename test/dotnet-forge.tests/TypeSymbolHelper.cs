using System;
using Microsoft.CodeAnalysis;
using System.Linq;

namespace Forge
{
    public static class TypeSymbolHelper
    {
        public static bool TypeSymbolMatchesType(ITypeSymbol typeSymbol, Type type, SemanticModel model)
        {
            var compiledTypeSymbol = GetTypeSymbolForType(type, model);
            return compiledTypeSymbol.Equals(typeSymbol);
        }

        public static INamedTypeSymbol GetTypeSymbolForType(Type type, SemanticModel model)
        {
            if (!type.IsConstructedGenericType)
            {
                var temp = model.Compilation.GetTypeByMetadataName(type.FullName);
                return temp;
            }

            var typeArgumentInfos = type.GenericTypeArguments
                .Select(typeArgument => GetTypeSymbolForType(typeArgument, model))
                .ToArray();

            var definition = type.GetGenericTypeDefinition();
            var symbol = model.Compilation.GetTypeByMetadataName(type.FullName);

            return symbol.Construct(typeArgumentInfos);
        }
    }
}