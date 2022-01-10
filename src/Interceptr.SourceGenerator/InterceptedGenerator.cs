// Copyright 2022 - Interceptr.NET - https://github.com/pedrobarbs/Interceptr.NET
//
// Licensed under the Apache License, Version 2.0 (the "License");
// you may not use this file except in compliance with the License.
// You may obtain a copy of the License at
//
// http://www.apache.org/licenses/LICENSE-2.0
//
// Unless required by applicable law or agreed to in writing, software
// distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and
// limitations under the License.

using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Interceptr
{
    [Generator]
    public class InterceptedGenerator : ISourceGenerator
    {
        private const string InterceptedSuffix = "Intercepted";

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
            var registrations = GetServiceToIntercept(context);

            GenerateInterceptedServices(context, registrations);
        }

        private static void GenerateInterceptedServices(GeneratorExecutionContext context, IEnumerable<(TypeInfo @interface, TypeInfo @class)> registrations)
        {
            foreach (var (@interface, @class) in registrations)
            {
                var interceptedName = $"{@class.Type.Name}{InterceptedSuffix}";

                var intercepted = CreateInterceptedService(interceptedName, @interface, @class);

                context.AddSource($"{interceptedName}.cs", intercepted);
            }
        }

        // TODO: need to improve because methods with similar names could already exist
        private static IEnumerable<(TypeInfo @interface, TypeInfo @class)> GetServiceToIntercept(GeneratorExecutionContext context)
        {
            var registrationMethods = new[]
            {
                ".AddTransientIntercepted",
                ".AddScopedIntercepted",
                ".AddSingletonIntercepted"
            };

            return GetInterceptedServices(context, registrationMethods);
        }

        private static string CreateInterceptedService(string interceptorName, TypeInfo @interface, TypeInfo @class)
        {
            var interfaceFullName = @interface.Type.GetFullName();
            var className = @class.Type.GetFullName();

            var methodsInfo = @interface.Type.GetMembers().OfType<IMethodSymbol>();

            return WriteInterceptedService(interceptorName, interfaceFullName, className, methodsInfo);
        }

        private static string WriteInterceptedService(
            string interceptorName,
            string interfaceFullName,
            string className,
            IEnumerable<IMethodSymbol> methodsInfo)
        {
            var interceptedMethods = WriteInterceptedMethods(methodsInfo);
            var usings = WriteUsings();

            return $@"
{usings}

public class {interceptorName} : {interfaceFullName} {{ 

    private readonly {className} _service;
    private readonly Interceptr.IInterceptr[] _interceptors;

    public {interceptorName}({className} service, params Interceptr.IInterceptr[] interceptors) 
    {{
        _service = service;
        _interceptors = interceptors;
    }}

    {interceptedMethods}

}}";
        }

        private static string WriteUsings()
        {
            List<string> namespaces = new List<string>()
            {
                "Interceptr",
                "System",
                "System.Collections.Generic",
                "System.IO",
                "System.Linq",
                "System.Net.Http",
                "System.Threading",
                "System.Threading.Tasks"
            };

            return string.Join("",
                namespaces
                    .Distinct()
                    .OrderBy(@namespace => @namespace)
                    .Select(name => $"using {name};{Environment.NewLine}"));
        }

        private static string WriteInterceptedMethods(IEnumerable<IMethodSymbol> methods)
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

            return $"new Interceptr.CallContext(\"{method.Name}\", new System.Collections.ObjectModel.ReadOnlyCollection<object>(new List<object>(){{{methodParamsNamesString}}}))";
        }

        private static string WriteAwait(IMethodSymbol method)
        {
            if (CanBeAwaited(method))
                return "await ";

            return "";
        }

        private static bool CanBeAwaited(IMethodSymbol method)
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
            return $@"public {WriteAsyncKeyword(method)}{WriteMethodReturn(method)} {method.Name}({WriteMethodArguments(method)})";
        }

        private static object WriteAsyncKeyword(IMethodSymbol method)
        {
            if (CanBeAwaited(method))
                return "async ";

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

        private static IEnumerable<(TypeInfo @interface, TypeInfo @class)> GetInterceptedServices(
            GeneratorExecutionContext context,
            string[] registrationMethods)
        {
            var compilation = context.Compilation;
            var syntaxTrees = compilation.SyntaxTrees;

            var targetTrees = syntaxTrees.Where(prop => registrationMethods.Any(method => prop.GetText().ToString().Contains(method)));

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
                            registrationMethods.Any(method => prop.GetText().ToString().Contains(method)));

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
