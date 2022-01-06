using System.Collections.ObjectModel;

namespace TheInterceptor
{
    public class CallContext
    {
        public CallContext(string methodName, ReadOnlyCollection<object> parameters)
        {
            MethodName = methodName;
            Parameters = parameters;
        }

        public string MethodName { get; }
        public ReadOnlyCollection<object> Parameters { get; }
    }
}