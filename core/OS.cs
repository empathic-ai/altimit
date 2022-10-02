using System;
using System.Linq;
using System.Collections.Generic;
using System.Reflection;

namespace Altimit
{
    public class OS
    {
        static OS()
        {
            Type settingsType = null;
            foreach (var assembly in AppDomain.CurrentDomain.GetAssemblies())
            {
                try
                {
                    settingsType = assembly.GetTypes().SingleOrDefault(x => typeof(OSSettings).IsAssignableFrom(x) && x != typeof(OSSettings));
                } catch (ReflectionTypeLoadException e)
                {

                }
                if (settingsType != null)
                    break;
            }
            Settings = Activator.CreateInstance(settingsType) as OSSettings;
            Logger = Settings.GetLogger("OS");
        }
        public static void LogError(object text)
        {
            Logger.LogError(text);
        }

        public static void Log(object text)
        {
            Logger.Log(text);
        }

        public static float DeltaTime;
        public static OSSettings Settings;
        public static ILogger Logger;
        public static Action<Guid, string> DeployRoomServer;

        public static bool LogApps = true;
        public static bool LogSerializiation = false;
        public static bool LogWeaving = false;
        public static bool LogBSONFormatting = false;
        public static bool LogFormatting = false;
        public static bool LogObservations = false;
        public static bool LogInstanceDB = false;
        // Logs BaseSM messages
        public static bool LogReplication = false;
        public static bool LogResolver = false;
        // Logs all network method calls--expensive
        public static bool LogMethodCalls = false;
        public static bool LogPropertyChanges = false;
        public static bool LogRecordings = false;
        public static bool LogSockets = false;
        public static bool LogP2P = false;
        public static bool LogWebRTC = false;
        public static Random Random = new Random();
    }
}
