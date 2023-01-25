using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using Altimit;
using Altimit.Networking;
//using Unity.WebRTC;

namespace Altimit
{
    // Adds peer to peer functionality to an app
    [AType]
    public partial class App
    {
        public Action OnInitialized;
        //public NetworkType NetworkType;
        public TaskCompletionSource<bool> CompletionSource { get; set; } = new TaskCompletionSource<bool>();

        public string URL = "";
        public int Port;
        /*
        {
            get
            {
                return NetworkType.Equals(NetworkType.Global) ? Settings.MasterURL : "localhost";
            }
        }*/
        //public Action<P2PClientSM> onDisconnected;

        public ServiceProvider ServerApp;
        IP2PServerService p2pServerService;

        // Should peers be connected to using relays with the signaling server?
        bool useRelays = true;

        //public SelfSession MasterClientSession;
        ClientSocketWs masterClientSocket;

        public ServerSocketWebRTC P2PSocket;

        SelfServiceProvider selfP2PServiceProvider;
        public ADictionary<AID, AppConnection> ConnectionsByPeerAppID = new ADictionary<AID, AppConnection>();
        public Action<AID, AppConnection> onConnectionAdded;
        public Action<AID, AppConnection> onConnectionRemoved;
        public TaskCompletionDictionary<AID, AppConnection> ConnectionAdded = new TaskCompletionDictionary<AID, AppConnection>();
        CancellationTokenSource cancellationTokenSource = new CancellationTokenSource();
        CancellationToken cancellationToken => cancellationTokenSource.Token;

        //ReplicationModule replicationAM => App.Get<ReplicationModule>();

        /*
        public P2PClientModule()
        {


            //clientSocket = new ClientSessionSocket<ClientSocketWs, UserMasterClientSession, IUserMasterServerSession>(new ClientSocketWs(), InstanceDB, () => new UserMasterClientSession(this), Logger);
            //AddSessionSocket(clientSocket);
        }
        */

        /*
        public virtual void OnInstanceAdded(object instance, bool isInstanceGroup)
        {
            //base.OnInstanceAdded(instance, isInstanceGroup);

            // If this app is the owner of the added instance, let the master server know
            //if (App.InstanceDB.IsOwner(instance))
            //    masterServer.SetAppID(App.InstanceDB.GetInstanceID(instance));
        }
        */

        /*
        public virtual async Task<T> Connect<T>(AID peerAppID) where T : IService
        {
            var connection = await GetOrRequestConnection(peerAppID);
            await connection.Peer.GetService<IServiceRequestService>().RequestService(typeof(T));
            var replicationRouter = new ReplicationRouter(connection.Self.GetService<IReplicationService>());
            return connection.Peer.AddService(DynamicMethodCallerFactory.CreateOnMethodCalled<T>(replicationRouter));
        }

        
        public Connection CreateConnection(IRPCSender router)
        {
            var connection = new Connection();
            connection.Self.AddService(new ReplicationService(App, connection.Self));

            var replicationService = connection.Self.AddService(new ReplicationService(App, connection.Self));
            var peerReplicationService = DynamicMethodCallerFactory.CreateRoutedService<IReplicationService>(router);
            replicationService.PeerReplicationService = connection.Peer.AddService(peerReplicationService);

            return connection;
        }
        */

        public async Task<T> Connect<T>(ISocketClient socketClient)
        {

            throw new NotImplementedException();
            //var connection = await GetOrRequestConnection(appID);
            //await connection.GetSingleton<T>();
            //return (T)(await Connect(typeof(T)));
        }

        public async Task<T> Connect<T>(AID appID) where T : IConnectionSingleton
        {

            throw new NotImplementedException();
            //var connection = await GetOrRequestConnection(appID);
            //await connection.GetSingleton<T>();
            //return (T)(await Connect(typeof(T)));
        }

        // Locally processes connection attempt.
        // TODO: Rewrite to connec to specified peer service, rather than personal service
        /*
        public virtual async Task<IService> AddService(Connection connection, Type serviceType)
        {
            // If already connected, return initialized module
            if (serviceProvider.HasService(moduleType))
            {
                return serviceProvider.GetService(moduleType);
            }
            else
            {
                // Otherwise grab from peer
                var peerSessionType = RemoteAPIFactoryExtensions.GetNetworkPeerSessionType(moduleType);
                await PeerReplicationService.OnConnection(peerSessionType);

                if (!serviceProvider.HasService(moduleType))
                {
                    if (serviceProvider.Peer is RemoteServiceProvider)
                    {
                        ((RemoteServiceProvider)serviceProvider.Peer).AddRemoteService(peerSessionType);
                    }

                    serviceProvider.AddService(moduleType);
                }
                return serviceProvider.GetService(moduleType);
            }
        }
        */

        public void OnPeerConnected(IPeerSocket peer)
        {
            var peerWebRTC = peer as WebRTCPeer;
            var peerAppID = peerWebRTC.PeerAppID;

            /*
            var serviceProvider = new SelfServiceProvider(App);
            var replicationService = serviceProvider.AddService(new ReplicationService(App, serviceProvider));
            serviceProvider.Peer = new RemoteServiceProvider(App, replicationService, peerWebRTC, App.Logger);
            serviceProvider.Peer.Peer = serviceProvider;

            //peer.Logger.Name = System.IO.Path.Combine(App.Name, peerAppID.ToAbbreviatedString());
            */

            // Add a connection
            throw new NotImplementedException();
            /*
            //var connection = new AppConnection();
            //connection.PeerAppID = peerAppID;
            //connection.Logger.Name = System.IO.Path.Combine(App.Name, peerAppID.ToAbbreviatedString());

            ConnectionsByPeerAppID.Add(peerAppID, connection);
            onConnectionAdded?.Invoke(peerAppID, connection);
            ConnectionAdded.SetResult(peerAppID, connection);
            */
        }

        public AppConnection GetConnection(AID appID)
        {
            return ConnectionsByPeerAppID[appID];
        }

        public bool TryGetConnection(AID appID, out AppConnection connection)
        {
            //session = null;
            //return true;
            return ConnectionsByPeerAppID.TryGetValue(appID, out connection);
        }

        /*
        public bool HasAuthority(object instance, string propertyName)
        {
            if (IsOwner(instance))
                return true;
            var connection = GetConnection(GetAppID(instance));
            return connection.HasAuthority(instance, propertyName);
        }
        */

        public virtual void RemoveSession(AppConnection connection)
        {
            onConnectionRemoved?.Invoke(connection.PeerAppID, connection);
            ConnectionsByPeerAppID.Remove(connection.PeerAppID);
            connection.Dispose();
        }

        /*
        public async Task<string> CreatePeerOffer(Guid peerAppID)
        {
            return await ServerSocketWebRTC.CreateOffer(peerAppID);
        }*/

        public void ReceivePeerCandidate(AID peerAppID, string candidate, string sdpMid, int sdpMLineIndex)
        {
            P2PSocket.GetPeer(peerAppID).ReceiveCandidate(candidate, sdpMid, sdpMLineIndex);
        }

        public async void OnNegotiationNeeded(WebRTCPeer peer)
        {
            AID peerAppID = peer.PeerAppID;
            var offerSDP = await peer.CreateOffer();

            //if (OS.LogP2P)
            //    Logger.Log($"Sending to peer app {peerAppID}...");

            // After sending an offer to the peer, check if it has also initiated a connection and wants this peer to stop
            // If so, stop handling the connection here and just await the completion of the connection
            var sdps = await selfP2PServiceProvider.Peer.GetService<IP2PServerService>().RelayOffer(peerAppID, offerSDP);

            //if (OS.LogP2P)
            //    Logger.Log($"Received response from app {peerAppID}...");

            await peer.ReceiveAnswer(sdps.Value);

            //if (OS.LogP2P)
            //    Logger.Log($"Finished connecting to peer app {peerAppID}.");
        }

        public void OnIceCandidate(AID peerAppID, WebRTCIceCandidate candidate)
        {
            if (candidate.Candidate != null)
                p2pServerService.BroadcastPeerCandidate(peerAppID, candidate.Candidate, candidate.SdpMid, (int)candidate.SdpMLineIndex);
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

        public async void OnConnected(IPeerSocket socketPeer)
        {
            if (OS.LogApps)
                Logger.Log("P2P client connected to signaling server.");
        }

        /*
        private void OnDisconnected(ISocketPeer peer)
        {
            if (OS.LogApps)
                Logger.Log("Disconnected from signaling server");
        }
        */

        /*
        public void Dispose()
        {
            cancellationTokenSource.Cancel();

            if (P2PSocket != null)
                P2PSocket.PeerConnected -= OnPeerConnected;

            foreach (var connection in ConnectionsByPeerAppID.Select(x=>x.Value))
            {
                RemoveSession(connection);
            }

            //ServerSocketWebRTC.Stop();
            //clientSocket.PeerConnected -= OnConnected;
            if (p2pClientSocket != null)
            {
                p2pClientSocket.Disconnect();
                throw new NotImplementedException();
                //p2pClientSocket.PeerDisconnected -= OnDisconnected;
            }
        }
        */

        public async Task ConnectToPeer(AID peerAppID)
        {
            if (OS.LogP2P)
                Logger.Log($"Starting connection to peer with an app ID of {peerAppID}...");
            var peer = await P2PSocket.ConnectToPeer(peerAppID);
            peer.Connect();

            if (OS.LogP2P)
                Logger.Log($"Connected to peer with an app ID of {peerAppID}!");
        }

        public virtual async Task<AppConnection> GetOrRequestConnection(AID peerAppID)
        {
            if (peerAppID.IsEmpty())
                OS.LogError("Tried to connect to a peer without providing a peer ID!");

            AppConnection connection;
            if (!TryGetConnection(peerAppID, out connection))
            {
                if (OS.LogP2P)
                    Logger.Log($"Requesting connection to peer with an app ID of {peerAppID}...");

                await p2pServerService.RequestConnectionToPeer(peerAppID);
                connection = GetConnection(peerAppID);
            }
            return connection;
        }

        public virtual async Task<Response<WebRTCSessionDescription>> ReceivePeerOffer(AID peerAppID,
            WebRTCSessionDescription desc, CancellationToken token = default)
        {
            var answerSDP = await P2PSocket.GetPeer(peerAppID).CreateAnswer(desc, token);

            return new Response<WebRTCSessionDescription>(answerSDP);
        }

        public async Task<string[]> GetModuleTypeNames()
        {
            throw new NotImplementedException();
            /*
            var moduleTypeNames = App.GetSessionModuleInterfaceNames();
            moduleTypeNames.Where(x => x != typeof(IP2PClientService).GetNativeTypeName());
            return moduleTypeNames.ToArray();
            */
        }

        public async Task ReceivePeerAnswer(AID peerAppID, WebRTCSessionDescription desc)
        {
            await P2PSocket.GetPeer(peerAppID).ReceiveAnswer(desc);
        }
    }
}
