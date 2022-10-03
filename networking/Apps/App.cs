using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Altimit;
using Altimit.Serialization;

namespace Altimit.Networking
{
    public partial class App : ModuleContainer<App, IAppModule>
    {
        protected override App container => this;
        public ILogger Logger { get; private set; }
        public Action<App> onAppClosed { get; set; }

        protected CancellationTokenSource ctSource = new CancellationTokenSource();
        public bool IsInitialized { get; private set; } = false;
        [AProperty]
        public string Name;

        List<ISessionSocket> sessionSockets = new List<ISessionSocket>();

        [AConstructor(nameof(Name))]
        protected App(string name = "")
        {
            Name = name;
            this.Logger = OS.Settings.GetLogger(name);
        }

        public virtual void AddSessionSocket(ISessionSocket sessionSocket)
        {
            sessionSockets.Add(sessionSocket);
            //sessionSocket.Sessions.OnAdded += OnSessionAdded;
            //sessionSocket.Sessions.OnRemoved += OnSessionRemoved;
        }

        public virtual void RemoveSessionSocket(ISessionSocket sessionSocket)
        {
            sessionSockets.Remove(sessionSocket);
            sessionSocket.Dispose();
            //sessionSocket.Sessions.OnAdded -= OnSessionAdded;
            //sessionSocket.Sessions.OnRemoved -= OnSessionRemoved;
        }

        public void Init(string name)
        {
            Name = name;

            /*
                        foreach (var module in Modules)
                        {
                            module.Init();
                        }
            */

            //TODO: reimplement
            //Updater.Instance.AddUpdateable(this);
        }

        /*
        public string GetName()
        {
            return SpacesFromCamel(GetType().Name.Replace("App", null));
        }*/

        public IEnumerable<string> GetSessionModuleInterfaceNames()
        {
            return GetSessionModuleTypes().Select(x => GetSessionModuleInterface(x).GetNativeTypeName());
        }

        public static Type GetSessionModuleInterface(Type sessionModuleType)
        {
            return sessionModuleType.GetInterfaces().Where(x => typeof(ISessionModule).IsAssignableFrom(x) && !x.Equals(typeof(ISessionModule)) && (!x.IsGenericType || !x.GetGenericTypeDefinition().Equals(typeof(ISessionModule<>)))).OrderByDescending(x=>x.GetMethods().Length).FirstOrDefault();
        }

        public IEnumerable<Type> GetSessionModuleTypes()
        {
            foreach (var module in Modules)
            {
                var appModuleType = module.GetType().BaseType;
                if (appModuleType.GenericTypeArguments.Length > 0)
                {
                    var sessionModuleType = appModuleType.GenericTypeArguments[0];
                    yield return sessionModuleType;
                }
            }
        }

        public static string SpacesFromCamel(string value)
        {
            if (value.Length > 0)
            {
                var result = new List<char>();
                char[] array = value.ToCharArray();
                for (int i = 0; i < array.Length; i++)
                {
                    var item = array[i];
                    if (i > 0 && char.IsUpper(item) && !char.IsUpper(array[i - 1]))
                    {
                        result.Add(' ');
                    }

                    result.Add(item);
                }

                return new string(result.ToArray());
            }

            return "";
        }

        public virtual void Update()
        {
        }

        public override void Dispose()
        {
            if (OS.LogApps)
                Logger.Log("Shutting down.");
            base.Dispose();

            //TODO: reimplement
            /*
            saveTimer.Dispose();
            ctSource.Cancel();

            while (sessionSockets.Count > 0)
            {
                RemoveSessionSocket(sessionSockets.First());
            }
            InstanceDB.onInstanceRemoved -= OnInstanceRemoved;
            InstanceDB.onPropertyChanged -= OnPropertyChanged;
            InstanceDB.onInstanceAdded -= OnInstanceAdded;
            InstanceDB.onAssetAdded -= OnAssetAdded;
            InstanceDB.ClearInstances();
            LocalDB.Dispose();

            onAppClosed?.Invoke(this);
            */
        }




        /*
        public bool HasAuthority(object instance, string propertyName)
        {
            if (InstanceDB.IsOwner(instance))
                return true;
            var session = Sessions.SingleOrDefault(x => x.PeerAppID == InstanceDB.GetAppID(instance));
            return session.GetModule<ReplicationSessionModule>().HasAuthority(instance, propertyName);
        }*/
    }
}
