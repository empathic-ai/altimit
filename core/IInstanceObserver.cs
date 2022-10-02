using System;

namespace Altimit
{
    public interface IInstanceObserver
    {
        Action onInstanceRemoved { get; set; }
        void BindAll(Action<object, string, object> handler);
        void UnbindAll(Action<object, string, object> handler);
        void BindProperty(string propertyName, Action<object, string, object> handler);
        void UnbindProperty(string propertyName, Action<object, string, object> handler);
        void Update();

        // This would be easy to implement... but for what purpose, in God's name!?
        //void BindMethodCall(string methodName, Type[] types, Action<object, object[]> handler);
    }
}