using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Altimit;
using Altimit.Networking;

namespace Altimit
{
    //[AType]
    public partial class App
    {
        //public Action OnInitialized;
        public string MasterURL = "";
        /*
    {
        get
        {
            return NetworkType.Equals(NetworkType.Global) ? Settings.MasterURL : "localhost";
        }
    }*/
        // cognitive disonance doesn't mean there's objective reality for an individual

        //public SelfServiceProvider MasterClientSession;
        //ClientSocketWs clientSocket;

        //P2PClientAppModule P2PClientApp;
        //P2PClientModule P2PClientModule;
        //IReplicationServerService replicationServerService;
        IReplicationServer masterServer;

        // replicationAM;
        //public TaskCompletionSource<bool> OnInitialized = new TaskCompletionSource<bool>();
        TaskCompletionDictionary<AID, object> instanceOnboarded = new TaskCompletionDictionary<AID, object>();

        AID serverAppID;

        public App(AID serverAppID, string url, int port)
        {
            this.serverAppID = serverAppID;
            URL = url;
            Port = port;


            //clientSocket = new ClientSessionSocket<ClientSocketWs, UserMasterClientSession, IUserMasterServerSession>(new ClientSocketWs(), InstanceDB, () => new UserMasterClientSession(this), Logger);
            //AddSessionSocket(clientSocket);
        }

        public virtual void OnInstanceAdded(object instance, bool isInstanceGroup)
        {
            //base.OnInstanceAdded(instance, isInstanceGroup);

            // If this app is the owner of the added instance, let the master server know
            //if (App.InstanceDB.IsOwner(instance))
            //    masterServer.SetAppID(App.InstanceDB.GetInstanceID(instance));
        }

        public async Task<AID> ResolveAppID(AID instanceID)
        {
            if (InstanceDB.HasInstanceID(instanceID))
                return GetAppID(instanceID);

            return await masterServer.ResolveAppID(instanceID);
        }

        public virtual void OnInstanceOnboarded(object instance)
        {
            instanceOnboarded.SetResult(instance.GetInstanceID(), instance);
        }

        public async Task<object> GetInstance(AID instanceID)//, Dictionary<Guid, object> resolvedSubInstances)
        {
            // If not already onboarding instance, initiate onboarding and wait for it to finish to return instance
            if (!instanceOnboarded.ContainsKey(instanceID))
            {
                instanceOnboarded.AddOrGetTaskCompletionSource(instanceID);

                if (OS.LogResolver)
                    Logger.Log($"Resolving app ID of instance with an ID of {instanceID}...");

                AID appID = await ResolveAppID(instanceID);

                SetAppID(appID, instanceID);

                if (OS.LogResolver)
                    Logger.Log($"Resolved app ID of instance with an ID of {instanceID} as {appID}.");

                var connection = await GetOrRequestConnection(appID);
                if (connection == null)
                    Logger.LogError($"Failed to find a session with app {appID}!");

                GetConnection(appID).RetrieveInstance(instanceID);
            }

            var instance = await instanceOnboarded.AddOrGetTask(instanceID, TimeSpan.FromSeconds(5));

            if (OS.LogResolver)
                Logger.Log($"Resolved instance with an ID of {instanceID} by onboarding.");

 
            //resolvedSubInstances.Add(instanceID, instance);
            //IsResolving = false;
            return instance;
        }

        public bool HasAuthority(object instance, string propertyName)
        {
            return GetConnection(GetAppID(instance)).HasAuthority(instance, propertyName);
        }

        /*
        public async Task<T> ConnectToAny<T>() where T : ISessionModule
        {
            //var appID = await 
            OS.Log(typeof(T).GetTypeName());
            var appID = await masterServer.GetAppIDWithModule(typeof(T).GetTypeName());

            var response = await ConnectToApp(appID);
            var session = response.Value;
            await session.PeerSession.GetModule<IReplicationSessionModule>().RequestModule(typeof(T).GetTypeName());
            var sessionModule = session.PeerSession.GenerateModule<T>();
            return sessionModule;
        }*/
        /*
        public async Task<SelfSession> ConnectToApp(Guid appID)
        {
            SelfSession session;
            if (networkModule.SessionsByAppID.TryGetValue(appID, out session))
            {
                OS.Log("RETURNING EXISTING SESSION");
                return session;
            }

            var connectionSettings = await masterServer.ConnectToApp(appID);
            if (connectionSettings.Equals(ServerType.WebSocketServer)) {

            }
            return null;// await clientSocket.ConnectAsync();
        }
        */

        /*
        private void OnDisconnected(ISocketPeer peer)
        {
            if (OS.LogApps)
                Logger.Log("Disconnected");
        }
        */

        /*
        public void Dispose()
        {
            //clientSocket?.Disconnect();
            //clientSocket.PeerConnected -= OnConnected;
            //clientSocket.PeerDisconnected -= OnDisconnected;
        }
        */
    }
}
