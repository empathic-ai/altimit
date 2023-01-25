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
                    settingsType = assembly.GetTypes().SingleOrDefault(x => typeof(PlatformSettings).IsAssignableFrom(x) && x != typeof(PlatformSettings));
                } catch (ReflectionTypeLoadException e)
                {

                }
                if (settingsType != null)
                    break;
            }
            Settings = Activator.CreateInstance(settingsType) as PlatformSettings;
            Logger = Settings.GetLogger("OS");
            Settings.Init();
        }

        public static void LogError(object text)
        {
            Logger.LogError(text);
        }

        public static void Log(object text)
        {
            Logger.Log(text);
        }

        static Random random = new Random(DateTime.Now.Second);

        public static int RandomRange(int min, int max)
        {
            return random.Next(min, max);
        }

        public static float DeltaTime;
        public static float FixedDeltaTime;
        public static PlatformSettings Settings;
        public static ILogger Logger;
       
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
        public static bool LogP2P = true;
        public static bool LogWebRTC = true;
        public static Random Random = new Random();
    }
}
