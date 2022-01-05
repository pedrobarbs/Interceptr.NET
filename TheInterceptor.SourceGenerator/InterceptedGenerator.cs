using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using System;
using System.Collections.Generic;
using System.Linq;

namespace TheInterceptor
{
    [Generator]
    public class InterceptedGenerator : ISourceGenerator
    {
        public void Initialize(GeneratorInitializationContext context)
        {
//#if DEBUG
//            if (!Debugger.IsAttached)
//            {
//                Debugger.Launch();
//            }
//#endif
        }

        public void Execute(GeneratorExecutionContext context)
        {
            // TODO: podem existir metodos com nomes parecidos que podem ser pegos erroneamente
            var registrationMethod = ".AddScopedIntercepted";
            var registrations = GetRegistrations(context, registrationMethod).ToList();

            GenerateLibraryClasses(context);

            foreach (var (@interface, @class) in registrations)
            {
                var interceptorName = $"{@class.Type.Name}Intercepted";
                var builtClass = BuildClass(interceptorName, @interface, @class);
                context.AddSource($"{interceptorName}.cs", builtClass);
            }
        }

        private void GenerateLibraryClasses(GeneratorExecutionContext context)
        {
            var iinterceptor = $@"
namespace TheInterceptor
{{
    public interface IInterceptor
    {{
        public void ExecuteBefore(CallContext context);
        public void ExecuteAfter(CallContext context, object result);
    }}
}}
";

            context.AddSource($"IInterceptor.cs", iinterceptor);

            var callContext = $@"
using System.Collections.ObjectModel;

namespace TheInterceptor
{{
    public class CallContext
    {{
        public CallContext(string methodName, ReadOnlyCollection<object> parameters)
        {{
            MethodName = methodName;
            Parameters = parameters;
        }}

        public string MethodName {{ get; }}
        public ReadOnlyCollection<object> Parameters {{ get; }}
    }}
}}
";
            context.AddSource($"CallContext.cs", callContext);

            var serviceCollection = $@"

using Microsoft.Extensions.DependencyInjection;
using System;

namespace TheInterceptor
{{
    public static class IServiceCollectionExtensions
    {{
        /// <summary>
        /// Add scoped services and intercepts them. The first provided interceptor will execute closer to the service's method. So the last provided interceptor will execute more distant to the service's method.
        /// </summary>
        /// <typeparam name=""Interface""></typeparam>
        /// <typeparam name=""Class"" ></typeparam>
        /// <param name=""services"" ></param>
        /// <param name=""interceptors"" ></param>
        /// <exception cref=""InvalidOperationException"" ></exception>
            public static void AddScopedIntercepted<Interface, Class>(this IServiceCollection services, params IInterceptor[] interceptors) where Class : class
        {{
            services.AddScoped<Class>();

            var classname = typeof(Class).Name;
            var intercepted = Type.GetType($""{{classname}}Intercepted"", true);

            if (intercepted is null)
                throw new InvalidOperationException($""{{classname}} has no dynamically generated intercepted service"");

            services.AddScoped(typeof(Interface), provider =>
            {{
                var service = provider.GetService<Class>();

                return Activator.CreateInstance(intercepted, service, interceptors);
            }});
        }}
    }}
}}

";

            context.AddSource($"IServiceCollectionExtensions.cs", serviceCollection);
        }

        private static string BuildClass(string interceptorName, TypeInfo @interface, TypeInfo @class)
        {
            var interfaceFullName = @interface.Type.GetFullName();
            var className = @class.Type.GetFullName();

            var methods = @interface.Type.GetMembers().OfType<IMethodSymbol>();

            var methodsString = BuildMethodsString(methods);
            var usings = BuildUsings();

            return $@"
{usings}

public class {interceptorName} : {interfaceFullName} {{ 

    private readonly {className} _service;
    private readonly TheInterceptor.IInterceptor[] _interceptors;

    public {interceptorName}({className} service, params TheInterceptor.IInterceptor[] interceptors) 
    {{
        _service = service;
        _interceptors = interceptors;
    }}

    {methodsString}

}}";
        }

        private static string BuildUsings()
        {
            List<string> namespaces = new List<string>()
            {
                "System",
                "TheInterceptor"
            };

            return string.Join("", 
                namespaces
                    .Distinct()
                    .OrderBy(@namespace => @namespace)
                    .Select(name => $"using {name};{Environment.NewLine}"));
        }

        private static string BuildMethodsString(IEnumerable<IMethodSymbol> methods)
        {
            var methodsStrings = new List<string>();
            foreach (var method in methods)
            {

                methodsStrings.Add(
$@"{WriteMethodSignature(method)}
{{
    var callContext = {WriteCallContext(method)};
    {WriteObjectResultDeclaration(method)}
    try
    {{
        foreach(var interceptor in _interceptors.Reverse())
        {{
            interceptor.ExecuteBefore(callContext);
        }}
        
        {WriteServiceCall(method)}
        {WriteToObjectResult(method)}
        {WriteReturn(method)}
    }}
    finally
    {{
        foreach(var interceptor in _interceptors)
        {{
            interceptor.ExecuteAfter(callContext, objectResult);
        }}
    }}
}}");
            }

            return string.Join($"{Environment.NewLine}{Environment.NewLine}", methodsStrings);
        }

        private static object WriteToObjectResult(IMethodSymbol method)
        {
            if (method.ReturnsVoid || ReturnsTask(method))
            {
                return "";
            }

            return "objectResult = result;";
        }

        private static object WriteObjectResultDeclaration(IMethodSymbol method)
        {
            return $"object objectResult = null;";
        }
    

        private static string WriteCallContext(IMethodSymbol method)
        {
            var methodParamsNames = GetParamsNames(method);
            var methodParamsNamesString = string.Join(",", methodParamsNames);

            return $"new TheInterceptor.CallContext(\"{method.Name}\", new System.Collections.ObjectModel.ReadOnlyCollection<object>(new List<object>(){{{methodParamsNamesString}}}))";
        }

        private static string WriteAwait(IMethodSymbol method)
        {
            if (ReturnsTaskOrTaskT(method))
                return "await ";

            return "";
        }

        private static bool ReturnsTaskOrTaskT(IMethodSymbol method)
        {
            return ReturnsTask(method) || ReturnsTaskT(method);
        }

        private static bool ReturnsTaskT(IMethodSymbol method)
        {
            return GetReturnTypeName(method).StartsWith("Task<");
        }

        private static string GetReturnTypeName(IMethodSymbol method)
        {
            return method.ReturnType.ToString().Split('.').Last();
        }

        private static bool ReturnsTask(IMethodSymbol method)
        {
            return GetReturnTypeName(method) == "Task";
        }

        private static string WriteServiceCall(IMethodSymbol method)
        {
            var methodParamsNames = GetParamsNames(method);
            var methodParamsNamesString = string.Join(",", methodParamsNames);

            return $"{WriteToResult(method)}{WriteAwait(method)}_service.{method.Name}({methodParamsNamesString});";
        }

        private static string WriteToResult(IMethodSymbol method)
        {
            if (method.ReturnsVoid || ReturnsTask(method))
            {
                return "";
            }

            return "var result = ";
        }

        private static IEnumerable<string> GetParamsNames(IMethodSymbol method)
        {
            var methodParamsTypes = method.Parameters.Select(p => p.Type.ToString());
            var methodParamsNames = methodParamsTypes.Select((p, index) => $"{GetParamName(p, index)}");
            return methodParamsNames;
        }

        private static string WriteMethodSignature(IMethodSymbol method)
        {
            return $@"public {WriteAsyncKeyword(method)} {WriteMethodReturn(method)} {method.Name}({WriteMethodArguments(method)})";
        }

        private static object WriteAsyncKeyword(IMethodSymbol method)
        {
            if (ReturnsTaskOrTaskT(method))
                return "async";

            return "";
        }

        private static string WriteMethodArguments(IMethodSymbol method)
        {
            var methodParamsTypes = method.Parameters.Select(p => p.Type.GetFullName());
            var methodParamsDeclaration = methodParamsTypes.Select((p, index) => $"{p} {GetParamName(p, index)}"); // class @class, Carro @carro
            return string.Join(",", methodParamsDeclaration);
        }

        private static string WriteMethodReturn(IMethodSymbol method)
        {
            return method.ReturnType.GetFullName();
        }

        private static string WriteReturn(IMethodSymbol method)
        {
            if (method.ReturnsVoid || ReturnsTask(method))
            {
                return "return;";
            }

            return "return result;";
        }

        private static string GetParamName(string p, int index)
        {
            return $"@{p.ToLowerInvariant().Split('.').Last()}{index}";
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
