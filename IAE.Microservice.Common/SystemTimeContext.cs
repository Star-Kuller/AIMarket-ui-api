using System;
using System.Collections.Generic;
using System.Threading;


namespace IAE.Microservice.Common
{
    public class SystemTimeContext : IDisposable
    {
        private static readonly AsyncLocal<Stack<SystemTimeContext>> ScopeStack;

        static SystemTimeContext()
        {
            ScopeStack = new AsyncLocal<Stack<SystemTimeContext>>();
        }

        public DateTime ContextDateTimeUtcNow { get; }

        public SystemTimeContext(DateTime contextDateTimeUtcNow)
        {
            ContextDateTimeUtcNow = new DateTime(
                contextDateTimeUtcNow.Year,
                contextDateTimeUtcNow.Month,
                contextDateTimeUtcNow.Day,
                contextDateTimeUtcNow.Hour,
                contextDateTimeUtcNow.Minute,
                contextDateTimeUtcNow.Second,
                contextDateTimeUtcNow.Millisecond,
                DateTimeKind.Utc);
            ScopeStack.Value ??= new Stack<SystemTimeContext>();
            ScopeStack.Value.Push(this);
        }

        public static SystemTimeContext Current =>
            ScopeStack.Value is null ? null :
            ScopeStack.Value.Count == 0 ? null : 
            ScopeStack.Value.Peek();

        public void Dispose() => ScopeStack.Value.Pop();
    }
}
