using Altimit;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.FindSymbols;
using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Diagnostics;
using System.Linq;
using System.Reflection;
using System.Security.Cryptography;
using System.Threading;

namespace AltimitGenerator
{
    // Generates dereferenced types for all referenced types in Altimit
    // TODO: Possibly expand this generator to also generate referenced types from .proto files
    [Generator]
    public class DereferencedTypeSourceGenerator : ISourceGenerator
    {
        public void Execute(GeneratorExecutionContext context)
        {
            try
            {
                foreach (var (syntaxTree, typeDeclaration) in GetATypes(context))
                {
                    if (typeDeclaration != null)
                    {
                        //var semanticModel = context.Compilation.GetSemanticModel(declaredClass.SyntaxTree);
                        //var typeInfo = semanticModel.GetTypeInfo(declaredClass);
                        TryGenerateDereferencedType(context, syntaxTree, typeDeclaration);
                        TryGenerateProxyType(context, syntaxTree, typeDeclaration);
                    }
                }
            } catch (Exception e)
            {
                // Build up the source code
                string source = "/*"+e.Message + ": " + e.StackTrace+"*/";

                // Add the source code to the compilation
                context.AddSource($"Error.g.cs", source);
            }
        }

        private static IEnumerable<Tuple<SyntaxTree, TypeDeclarationSyntax>> GetATypes(GeneratorExecutionContext context)
        {
            return context.Compilation.SyntaxTrees.SelectMany(syntaxTree=>
                        syntaxTree.GetRoot()
                        .DescendantNodes()
                        .Where(n => n is ClassDeclarationSyntax || n is StructDeclarationSyntax || n is InterfaceDeclarationSyntax)
                        .Select(n => n as TypeDeclarationSyntax)
                        .Where(r => r.AttributeLists
                            .SelectMany(al => al.Attributes)
                            .Any(a => a.Name.GetText().ToString() == "AType")).Select(x => new Tuple<SyntaxTree, TypeDeclarationSyntax>(syntaxTree, x)));
        }

        void TryGenerateProxyType(GeneratorExecutionContext context, SyntaxTree syntaxTree, TypeDeclarationSyntax typeDeclaration)
        {
            if (typeDeclaration is not InterfaceDeclarationSyntax)
                return;

            string interfaceTypeName = typeDeclaration.Identifier.ToString();
            string proxyTypeName = interfaceTypeName.TrimStart('I') + "Proxy";
            var semanticModel = context.Compilation.GetSemanticModel(syntaxTree);
            var classType = semanticModel.GetDeclaredSymbol(typeDeclaration) as ITypeSymbol;
            var typeInfo = semanticModel.GetTypeInfo(typeDeclaration);

            string typeBody = "\n";
            if (classType != null)
            {
                int genericArgumentCount = 0;

                if (classType is INamedTypeSymbol nType) //type is ITypeSymbol
                {
                    genericArgumentCount = nType.TypeArguments.Count();
                }

                // Iterate over properties
                foreach (var property in classType.GetBaseTypesAndThis().SelectMany(x => x.GetMembers()).OfType<IPropertySymbol>())
                {
                    var propertyAttribute = property.GetAttributes().SingleOrDefault(x => x.AttributeClass.Name == nameof(APropertyAttribute));
                    if (propertyAttribute != null)
                    {
                        typeBody +=
$"      [AProperty]\n";
                        typeBody +=
$"      public {property.Type.ToDisplayString(SymbolDisplayFormat.FullyQualifiedFormat)} {property.Name} {{ get; set; }}\n\n";
                    }
                }

                foreach (var method in classType.GetBaseTypesAndThis().SelectMany(x => x.GetMembers()).OfType<IMethodSymbol>())
                {
                    var methodAttribute = method.GetAttributes().SingleOrDefault(x => x.AttributeClass.Name == nameof(AMethodAttribute));
                    if (methodAttribute != null)
                    {
                        var methodBody = "";
                        if (method.ReturnType.ToDisplayString(SymbolDisplayFormat.MinimallyQualifiedFormat) != "void")
                            methodBody = $"return ({method.ReturnType.ToDisplayString(SymbolDisplayFormat.FullyQualifiedFormat)}){nameof(IProxy.ProxyMethodCalled)}?.Invoke(this, {GenerateMethodCallEventArgs(method)});";

                        typeBody +=
$@"        [AMethod]
        {GenerateMethodTitle(method)} {{
            {methodBody} 
        }}

";
                    }
                }

                // Build up the source code
                string source = $@"// <auto-generated/>
using System;
using Altimit;
using System.Threading.Tasks;
using {classType.ContainingNamespace.ToDisplayString(SymbolDisplayFormat.FullyQualifiedFormat)};

namespace {classType.ContainingNamespace.Name}
{{
    [AType]
    public class {proxyTypeName + GenerateGenericArguments(typeDeclaration)} : {interfaceTypeName + GenerateGenericArguments(typeDeclaration)}, {nameof(IProxy)}
    {{

        public Func<object, MethodCalledEventArgs, object> {nameof(IProxy.ProxyMethodCalled)} {{ get; set; }}

        {typeBody}
    }}
}}
";
                // Add the source code to the compilation
                context.AddSource($"{proxyTypeName}.g.cs", source);
            }
        }

        void TryGenerateDereferencedType(GeneratorExecutionContext context, SyntaxTree syntaxTree, TypeDeclarationSyntax typeDeclaration)
        {
            if (typeDeclaration is InterfaceDeclarationSyntax)
                return;

            string referencedTypeName = typeDeclaration.Identifier.ToString();
            string dereferncedTypeName = "Dereferenced" + referencedTypeName;
            var semanticModel = context.Compilation.GetSemanticModel(syntaxTree);
            var classType = semanticModel.GetDeclaredSymbol(typeDeclaration) as ITypeSymbol;
            var typeInfo = semanticModel.GetTypeInfo(typeDeclaration);

            string typeBody = "\n";
            if (classType != null)
            {
                int genericArgumentCount = 0;
                
                if (classType is INamedTypeSymbol nType) //type is ITypeSymbol
                {
                    genericArgumentCount = nType.TypeArguments.Count();
                }
/*
                typeBody += $@" 
        [AProperty]
        [PrimaryKey, AutoIncrement]
        public {nameof(AID)} {"ID"} {{ get; set; }}

";
*/
                // Iterate over properties
                foreach (var property in classType.GetBaseTypesAndThis().SelectMany(x=>x.GetMembers()).OfType<IPropertySymbol>())
                {
                    var propertyAttribute = property.GetAttributes().SingleOrDefault(x => x.AttributeClass.Name == nameof(APropertyAttribute));
                    if (propertyAttribute != null) {
                        if (property.Name != "ID")
                        {
                            typeBody +=
$"          [AProperty]\n";
                            if (property.Type.IsReferenceType && property.Type.ToDisplayString(SymbolDisplayFormat.MinimallyQualifiedFormat).ToLower() != "string" && property.Type.Name.ToLower() != "guid")
                            {
                                typeBody +=
$"          public {nameof(AID)} {property.Name + "ID"} {{ get; set; }}\n\n";
                            }
                            else if (!IsValueType(property.Type) && property.Type.TypeKind == TypeKind.Struct)
                            {
                                typeBody +=
$"          public {"Dereferenced"+property.Type.ToDisplayString(SymbolDisplayFormat.MinimallyQualifiedFormat)} {property.Name} {{ get; set; }}\n\n";
                            } else {
                                typeBody +=
$"          public {property.Type.ToDisplayString(SymbolDisplayFormat.FullyQualifiedFormat)} {property.Name} {{ get; set; }}\n\n";
                            }
                        }
                    }
                }

                // Build up the source code
                string source = $@"// <auto-generated/>
using System;
using Altimit;
using {classType.ContainingNamespace.ToDisplayString(SymbolDisplayFormat.FullyQualifiedFormat)};

namespace {classType.ContainingNamespace.Name}
{{
    [AType]
    public class {dereferncedTypeName+GenerateGenericArguments(typeDeclaration)} : {nameof(IDereferencedObject)+ "<" + referencedTypeName + GenerateGenericArguments(typeDeclaration)}>
    {{
        {typeBody}
    }}
}}
";
                // Add the source code to the compilation
                context.AddSource($"{dereferncedTypeName}.g.cs", source);
            }
        }

        internal static bool IsValueType(ITypeSymbol type)
        {
            Debug.Assert(type != null);

            if (type.ToDisplayString(SymbolDisplayFormat.MinimallyQualifiedFormat) == nameof(Guid))
                return true;

            if (type.ToDisplayString(SymbolDisplayFormat.MinimallyQualifiedFormat) == nameof(DateTime))
                return true;

            switch (type.SpecialType)
            {
                case SpecialType.System_Boolean:
                case SpecialType.System_SByte:
                case SpecialType.System_Int16:
                case SpecialType.System_Int32:
                case SpecialType.System_Int64:
                case SpecialType.System_Byte:
                case SpecialType.System_UInt16:
                case SpecialType.System_UInt32:
                case SpecialType.System_UInt64:
                case SpecialType.System_Single:
                case SpecialType.System_Double:
                case SpecialType.System_Char:
                case SpecialType.System_String:
                case SpecialType.System_Object:
                    return true;
                default:
                    return false;
            }
        }

        private string GenerateMethodCallEventArgs(IMethodSymbol methodSymbol)
        {
            var source = "new  " + nameof(MethodCalledEventArgs);
            source += "(";

            source += $"nameof({methodSymbol.Name}), ";

            source += "new Type[] { ";
            bool isFirst = true;
            foreach (var parameter in methodSymbol.Parameters)
            {
                if (!isFirst)
                    source += ", ";
                source += "typeof(" + parameter.Type.ToDisplayString(SymbolDisplayFormat.FullyQualifiedFormat) + ")";
                isFirst = false;
            }
            source += "}, ";

            source += "new object[] { ";
            isFirst = true;
            foreach (var parameter in methodSymbol.Parameters)
            {
                if (!isFirst)
                    source += ", ";
                source += parameter.Name;
                isFirst = false;
            }
            source += "}";

            source += ")";
            return source;
        }

        private string GenerateMethodTitle(IMethodSymbol methodSymbol)
        {
            var source = "public " + methodSymbol.ReturnType.ToDisplayString(SymbolDisplayFormat.FullyQualifiedFormat);
            source += " " + methodSymbol.Name + "(";

            bool isFirst = true;
            foreach (var parameter in methodSymbol.Parameters) {
                if (!isFirst)
                    source += ", ";
                source += parameter.Type.ToDisplayString(SymbolDisplayFormat.FullyQualifiedFormat);
                source += " " + parameter.Name;
                isFirst = false;
            }
            source += ")";
            return source;
        }

        // This method generates source code for a single property
        private string GenerateGenericArguments(TypeDeclarationSyntax typeDeclarationSyntax)
        {
            if (typeDeclarationSyntax.TypeParameterList == null)
                return "";
            
            var source = "";
            bool isFirst = true;
            foreach (var argument in typeDeclarationSyntax.TypeParameterList.Parameters)
            {
                if (isFirst)
                {
                    source += "<";
                } else
                {
                    source += ", ";
                }
                source += argument.Identifier.Text;

                isFirst = false;
            }

            if (!isFirst)
                source += ">";

            return source;
        }

        public void Initialize(GeneratorInitializationContext context)
        {
            // No initialization required for this one
        }
    }

    public static class AltimitGeneratorExtensions
    {
        public static IEnumerable<ITypeSymbol> GetBaseTypesAndThis(this ITypeSymbol type)
        {
            var current = type;
            foreach (var interfaceType in current.Interfaces)
            {
                yield return interfaceType;
            }
            while (current != null)
            {
                yield return current;
                current = current.BaseType;
            }
        }
    }
}