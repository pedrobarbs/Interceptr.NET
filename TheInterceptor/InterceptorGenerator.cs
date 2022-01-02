using Microsoft.CodeAnalysis;

namespace TheInterceptor
{
    [Generator]
    public class InterceptorGenerator : ISourceGenerator
    {
        public void Execute(GeneratorExecutionContext context)
        {
            var syntaxTrees = context.Compilation.SyntaxTrees;

            //var handlers = syntaxTrees.
        }
        public void Initialize(GeneratorInitializationContext context)
        {
            throw new NotImplementedException();
        }
    }
}
