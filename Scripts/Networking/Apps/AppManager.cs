using Altimit;
using Altimit.Networking;
using Altimit.Serialization;

using System;
//using Meridian.Core;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;


namespace Altimit
{
    // TODO: Create more basic version of AppManager that doesn't depend on Unity's engine
    // Create custom init per OSSettings type and have AppManager call custom Init()
    // Manages the simulation of Altimit applications within Unity's editor
    public class AppManager
    {
        static AppManager()
        {
        }

        public static AppManager Instance
        {
            get
            {
                if (instance == null)
                    instance = new AppManager();

                return instance;
            }
        }

        static AppManager instance;

        public static NetworkType NetworkType = NetworkType.Global;

        //public Action OnControllerLoaded;
        /*
        [HideInInspector]
        public List<UserClientAM> ClientRoomApps = new List<UserClientAM>();
        [NonSerialized]
        public UserClientAM ActiveClient;
        */
        public AList<App> Apps = new AList<App>();
        public Action<App> onAppOpened { get; set; }
        public Action<App> onAppClosed { get; set; }
        //public bool InitOnAwake = false;

        static bool isWebRTCInitialized = false;

        public T OpenApp<T>(T app) where T : App
        {
            Apps.Add(app);
            
            /*
            if (app.HasModule<RoomAM>())
            {
                //var roomAppBehaviour = .AddComponent<RoomAppBehaviour>();
                //roomAppBehaviour.SetApp(app);

                if (app.HasModule<UserClientAM>())
                {
                    ClientRoomApps.Add(app.Get<UserClientAM>());
                    if (ClientRoomApps.Count == 1)
                        SetActiveClient(app.Get<UserClientAM>());
                    if (ClientRoomApps.Count > 1)
                        SetClientInactive(app.Get<UserClientAM>());
                }
            }
            */

            onAppOpened?.Invoke(app);
            app.onAppClosed += OnAppClosed;

            return app;
        }

        public void OnAppClosed(App app)
        {
            Apps.Remove(app);
            onAppClosed?.Invoke(app);
            app.onAppClosed -= OnAppClosed;
        }

        // TODO: Uncomment
        /*
        public async void Awake()
        {
            // Define OS properties
            // TODO: Refactor
           
        }

        public UnityUserApp DeployClient(Guid appID)
        {
            return Instance.OpenApp(UnityUserApp.Create(GetNextRoomPosition(), IsDesktop, OSSettings.MasterURL, Settings.P2PServerPort, appID, $"Client {Instance.ClientRoomApps.Count}"));
        }
        */

        public static string SpacesFromCamel(string value)
        {
            if (value.Length > 0)
            {
                var result = new List<char>();
                char[] array = value.ToCharArray();
                for (int i = 0; i < array.Length; i++)
                {
                    var item = array[i];
                    if (i > 0 && char.IsUpper(item) && !char.IsUpper(array[i-1]))
                    {
                        result.Add(' ');
                    }

                    result.Add(item);
                }

                return new string(result.ToArray());
            }

            return "";
        }
    }
}