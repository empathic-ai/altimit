using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Reflection;
using System.Runtime.ExceptionServices;
using Altimit;

namespace Altimit
{

    public class Logger : ILogger
    {
        public string Name { get; set; }
        Action<object> logFunc;
        Action<object> logWarningFunc;
        Action<Exception> logExceptionFunc;

        public Logger(string name, Action<object> logFunc, Action<object> logWarningFunc, Action<Exception> logExceptionFunc)
        {
            this.Name = name;
            this.logFunc = logFunc;
            this.logWarningFunc = logWarningFunc;
            this.logExceptionFunc = logExceptionFunc;
        }

        public void LogFormat(string text, params object[] args)
        {
            Log(string.Format(text, args));
        }

        public ILogger SubLogger(string _name)
        {
            return OS.Settings.GetLogger(Name + "/" + _name);
        }

        [DebuggerHiddenAttribute]
        [StackTraceIgnore]
        public void Log(object o, ALogLevel logLevel = ALogLevel.Default)
        {
            switch (logLevel)
            {
                case ALogLevel.Error:
                    LogError(o);
                    break;
                case ALogLevel.Warning:
                    LogWarning(o);
                    break;
                default:
                    logFunc.Invoke(AppendNameOnObject(o));
                    break;
            }
        }

        string AppendNameOnObject(object message)
        {
            return AppendName(message == null ? null : message.ToString());
        }

        string AppendName(string message = "")
        {
            return Name.ToBracketSring() + ": " + message;
        }

        [DebuggerHidden]
        [StackTraceIgnore]
        public void LogError(object o)
        {
            LogError(new Exception(o.ToString()));
        }


        [DebuggerHidden]
        [StackTraceIgnore]
        public void LogError(Exception e)
        {
            //ExceptionDispatchInfo.Capture(e.InnerException).
            logExceptionFunc?.Invoke(new Exception(AppendName(), e));
        }


        [DebuggerHidden]
        [StackTraceIgnore]
        public void LogWarning(object o)
        {
            logWarningFunc.Invoke(AppendNameOnObject(o));
        }
    }
}
