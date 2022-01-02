using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;

namespace TheInterceptor
{
    [Generator]
    public class InterceptedGenerator : ISourceGenerator
    {
        public void Initialize(GeneratorInitializationContext context)
        {
#if DEBUG
            if (!Debugger.IsAttached)
            {
                Debugger.Launch();
            }
#endif
        }

        public void Execute(GeneratorExecutionContext context)
        {
            // TODO: podem existir metodos com nomes parecidos que podem ser pegos erroneamente
            var registrationMethod = ".AddScopedIntercepted";
            var registrations = GetRegistrations(context, registrationMethod).ToList();

            foreach (var (@interface, @class) in registrations)
            {
                var interceptorName = $"{@class.Type.Name}Intercepted";
                var builtClass = BuildClass(interceptorName, @interface, @class);
                context.AddSource($"{interceptorName}.cs", builtClass);
            }
        }

        private static string BuildClass(string interceptorName, TypeInfo @interface, TypeInfo @class)
        {
            var interfaceName = @interface.Type.Name;
            var className = @class.Type.Name;
            var interfaceNamespace = @interface.Type.ContainingNamespace;
            var classNamespace = @class.Type.ContainingNamespace;

            var methods = @interface.Type.GetMembers().OfType<IMethodSymbol>();

            var methodsString = BuildMethodsString(methods);
            var usings = BuildUsings(interfaceNamespace.Name, classNamespace.Name, methods);

            return $@"
{usings}

public class {interceptorName} : {interfaceName} {{ 
    private readonly {className} _service;
    private readonly IInterceptor _interceptor;

    public {interceptorName}({className} service, IInterceptor interceptor) {{
        _service = service;
        _interceptor = interceptor;
}}

{methodsString}

}}";
        }

        private static string BuildUsings(string interfaceNamespace, string classNamespace, IEnumerable<IMethodSymbol> methods)
        {
            List<string> namespaces = new List<string>()
            {
                interfaceNamespace,
                classNamespace,
            };


            foreach (var method in methods)
            {
                foreach (var parameter in method.Parameters)
                {
                    namespaces.Add(parameter.ContainingNamespace.Name);
                }
            }

            return string.Join("", namespaces.Distinct().Select(name => $"using {name};{Environment.NewLine}"));
        }

        private static object BuildMethodsString(IEnumerable<IMethodSymbol> methods)
        {
            var methodsString = "";
            foreach (var method in methods)
            {
                var methodName = method.Name;
                var methodParamsTypes = method.Parameters.Select(p => p.Type.ToString());
                var methodParamsNames = methodParamsTypes.Select((p, index) => $"{GetParamName(p, index)}");
                var methodParamsNamesString = string.Join(",", methodParamsNames);
                var methodParamsDeclaration = methodParamsTypes.Select((p, index) => $"{p} {GetParamName(p, index)}"); // class @class, Carro @carro
                var methodParamsDeclarationString = string.Join(",", methodParamsDeclaration);
                // TODO: verificar se há necessidade
                var methodReturn = method.ReturnsVoid ? "void" : method.ReturnType.ToString();

                methodsString += $@"public {methodReturn} {methodName}({methodParamsDeclarationString}){{
    try
    {{
        _interceptor.Pre();
        {WriteReturn(method.ReturnsVoid is false)} _service.{methodName}({methodParamsNamesString});
    }}
    finally
    {{
        _interceptor.Pos();
    }}
}}";
                
                methodsString += Environment.NewLine;
            }

            return methodsString;
        }

        private static string WriteReturn(bool hasReturn)
        {
            if (hasReturn)
            {
                return "return";
            }

            return "";
        }

        private static string GetParamName(string p, int index)
        {
            return $"@{p.ToLowerInvariant()}{index}";
        }

        private static IEnumerable<(TypeInfo @interface, TypeInfo @class)> GetRegistrations(
            GeneratorExecutionContext context,
            string registrationMethod)
        {
            var compilation = context.Compilation;
            var syntaxTrees = compilation.SyntaxTrees;

            var targetTrees = syntaxTrees.Where(prop => prop.GetText().ToString().Contains(registrationMethod));

            foreach (var tree in targetTrees)
            {
                var semanticModel = compilation.GetSemanticModel(tree);

                var root = tree.GetRoot();

                var descendend = root.DescendantNodes();

                var invocations = descendend
                    .OfType<InvocationExpressionSyntax>();

                var targetInvocations = invocations
                    .Where(
                        prop =>
                            prop.GetText().ToString().Contains(registrationMethod));

                return targetInvocations
                    .Select(invocation =>
                    {
                        var expression = invocation.Expression.DescendantNodes()
                            .OfType<GenericNameSyntax>().FirstOrDefault();

                        var genericsArgs = expression.TypeArgumentList.Arguments;

                        var @interface = semanticModel.GetTypeInfo(genericsArgs[0]);
                        var @class = semanticModel.GetTypeInfo(genericsArgs[1]);

                        return (@interface, @class);
                    })
                    .Distinct();
            }

            return Enumerable.Empty<(TypeInfo, TypeInfo)>();
        }
    }
}
