using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Linq.Expressions;

namespace TheInterceptor
{
    [Generator]
    public class InterceptorGenerator : ISourceGenerator
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
            var registrationMethod = ".AddScopedIntercepted";
            var registrations = GetRegistrations(context, registrationMethod).ToList();

            foreach (var (@interface, @class) in registrations)
            {
                var interceptorName = $"{@class}Interceptor";
                context.AddSource($"{interceptorName}.cs", BuildClass(interceptorName));
            }

            //var handlers = syntaxTrees.
        }

        private static string BuildClass(string interceptorName)
        {
            return $@"class {interceptorName} {{ 
            

}}";
        }

        private static IEnumerable<(string @interface, string @class)> GetRegistrations(
            GeneratorExecutionContext context,
            string registrationMethod)
        {
            var compilation = context.Compilation;
            var syntaxTrees = compilation.SyntaxTrees;

            var targetTrees = syntaxTrees.Where(prop => prop.GetText().ToString().Contains(registrationMethod));

            foreach (var tree in targetTrees)
            {
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
                        var text = invocation.GetText().ToString();
                        var openIndex = text.IndexOf('<');
                        var closeIndex = text.IndexOf('>');

                        var types = text.Substring(openIndex + 1, closeIndex - openIndex - 1);

                        var array = types.Split(',');
                        var @interface = array[0].Trim();
                        var @class = array[1].Trim();

                        var expression = invocation.Expression.DescendantNodes();
                        
                        //.OfType<GenericNameSyntax>().FirstOrDefault();

                        //var generics = expression.TypeArgumentList.Arguments.Select(p => p.Identifier);
                        var generics = expression;
                        return (@interface, @class);
                    })
                    .Distinct();

            }

            return Enumerable.Empty<(string, string)>();
        }
    }
}
