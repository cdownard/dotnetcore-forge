using System;
using Microsoft.CodeAnalysis;
using System.Linq;

namespace Forge
{
    public static class TypeSymbolHelper
    {
        public static bool TypeSymbolMatchesType(ITypeSymbol typeSymbol, Type type, SemanticModel model)
        {
            return GetTypeSymbolForType(type, model).Equals(typeSymbol);
        }

        public static INamedTypeSymbol GetTypeSymbolForType(Type type, SemanticModel model)
        {
            if (!type.IsConstructedGenericType) return model.Compilation.GetTypeByMetadataName(type.FullName);
                
            var typeArgumentInfos = type.GenericTypeArguments
                .Select(typeArgument => GetTypeSymbolForType(typeArgument, model))
                .ToArray();

            var definition = type.GetGenericTypeDefinition();
            var symbol = model.Compilation.GetTypeByMetadataName(type.FullName);

            return symbol.Construct(typeArgumentInfos);
        }
    }
}