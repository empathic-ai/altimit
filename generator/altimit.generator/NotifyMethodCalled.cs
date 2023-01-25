using System;
using System.Threading.Tasks;

//namespace Altimit
//{
    public interface INotifyMethodCalled
    {
        event MethodCalledEventHandler MethodCalled;
    }

    public interface IProxy
    {
        Func<object, MethodCalledEventArgs, object> ProxyMethodCalled { get; set; }
    }

    public delegate void MethodCalledEventHandler(object sender, MethodCalledEventArgs e);

    public class MethodCalledEventArgs : EventArgs
    {
        public virtual string MethodName { get; }
        public virtual Type[] MethodTypes { get; }
        public virtual object[] MethodArgs { get; }

        public MethodCalledEventArgs(string methodName, Type[] types, object[] methodArgs)
        {
            MethodName = methodName;
            MethodTypes = types;
            MethodArgs = methodArgs;
        }
    }

    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Method, AllowMultiple = false, Inherited = true)]
    public class NotifyMethodCalledAttribute : Attribute
    {
    }
//}
