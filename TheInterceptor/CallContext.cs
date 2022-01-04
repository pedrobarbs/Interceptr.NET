using System.Collections.ObjectModel;

namespace TheInterceptor
{
    public class CallContext
    {
        public string MethodName { get; init; }
        public ReadOnlyCollection<object> Parameters { get; init; }
    }
}