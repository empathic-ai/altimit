using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Altimit;
using Altimit.Networking;

namespace Altimit.Networking
{
    [AType]
    [RequireModule(typeof(P2PClientAM))]
    public class ReplicationClientAM : AppModule<IReplicationClientSM>
    {
        public Action OnInitialized;
        public string MasterURL = "";
        /*
    {
        get
        {
            return NetworkType.Equals(NetworkType.Global) ? Settings.MasterURL : "localhost";
        }
    }*/
        // cognitive disonance doesn't mean there's objective reality for an individual

        public SelfSession MasterClientSession;
        //ClientSocketWs clientSocket;

        //P2PClientAppModule P2PClientApp;
        P2PClientSM P2PClientSession;
        ReplicationClientSM replicationClient;

        ReplicationAM replicationAM;
        //public TaskCompletionSource<bool> OnInitialized = new TaskCompletionSource<bool>();
        TaskCompletionDictionary<AID, object> instanceOnboarded = new TaskCompletionDictionary<AID, object>();

        Guid serverAppID;

        public ReplicationClientAM(Guid serverAppID)
        {
            this.serverAppID = serverAppID;
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

        public async Task<Guid> ResolveAppID(AID instanceID)
        {
            if (replicationAM.InstanceDB.HasInstanceID(instanceID))
                return replicationAM.GetAppID(instanceID);

            return await replicationClient.Peer.ResolveAppID(instanceID);
        }

        public override async Task OnAdded()
        {
            //P2PClientApp = App.AddOrGetModule<P2PClientAppModule>();
            replicationAM = App.Get<ReplicationAM>();
            App.Get<P2PClientAM>().OnInitialized += OnP2PClientInitialized;

            //clientSocket = networkModule.AddSocket(new ClientSocketWs(MasterURL, Settings.MasterPort));

            //await clientSocket.Connect(IP, Settings.UserPort, ctSource.Token);
            //clientSocket.PeerConnected += OnConnected;
            //clientSocket.PeerDisconnected += OnDisconnected;

            //App.InstanceDB.onInstanceAdded += OnInstanceAdded;
        }

        async void OnP2PClientInitialized()
        {
            replicationClient = await App.Get<P2PClientAM>().P2PClientSM.Connect<ReplicationClientSM>(serverAppID);

            replicationAM.InstanceDB.GetInstanceFuncs.Add(GetInstance);

            OnInitialized?.Invoke();
        }

        public virtual void OnInstanceOnboarded(object instance)
        {
            instanceOnboarded.SetResult(instance.GetInstanceID(), instance);
        }


        public async Task<object> GetInstance(AID instanceID)//, Dictionary<AID, object> resolvedSubInstances)
        {

            // If not already onboarding instance, initiate onboarding and wait for it to finish to return instance
            if (!instanceOnboarded.ContainsKey(instanceID))
            {
                instanceOnboarded.AddOrGetTaskCompletionSource(instanceID);

                if (OS.LogResolver)
                    Logger.Log($"Resolving app ID of instance with an ID of {instanceID}...");

                Guid appID = await GetModule<ReplicationClientAM>().ResolveAppID(instanceID);

                replicationAM.SetAppID(appID, instanceID);

                if (OS.LogResolver)
                    Logger.Log($"Resolved app ID of instance with an ID of {instanceID} as {appID}.");

                var session = await GetModule<P2PClientAM>().ConnectOrGetSession(appID);
                if (session == null)
                    Logger.LogError($"Failed to find a session with app {appID}!");

                session.Peer.Get<IReplicationAPI>().RetrieveInstance(instanceID);
            }

            var instance = await instanceOnboarded.AddOrGetTask(instanceID, TimeSpan.FromSeconds(5));

            if (OS.LogResolver)
                Logger.Log($"Resolved instance with an ID of {instanceID} by onboarding.");

 
            //resolvedSubInstances.Add(instanceID, instance);
            //IsResolving = false;
            return instance;
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

        private void OnDisconnected(ISocketPeer peer)
        {
            if (OS.LogApps)
                Logger.Log("Disconnected");
        }

        public override void Dispose()
        {
            //clientSocket?.Disconnect();
            //clientSocket.PeerConnected -= OnConnected;
            //clientSocket.PeerDisconnected -= OnDisconnected;
        }
    }
}
