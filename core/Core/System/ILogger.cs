using System;
using System.Collections.Generic;
using Altimit;

namespace Altimit
{
    public enum ALogLevel
    {
        Default,
        Warning,
        Error
    }

    public interface ILogger
    {
        string Name { get; set; }
        void Log(object o, ALogLevel logLevel = ALogLevel.Default);
        void LogWarning(object o);
        void LogError(object o);
        void LogError(Exception e);

        ILogger SubLogger(string _name);
    }
}
