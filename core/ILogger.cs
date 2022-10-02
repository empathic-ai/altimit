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
        public string Name { get; set; }
        public void Log(object o, ALogLevel logLevel = ALogLevel.Default);
        public void LogWarning(object o);
        public void LogError(object o);
        public void LogError(Exception e);

        public ILogger SubLogger(string _name);
    }
}
