using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using Altimit;
using Altimit.Networking;
//using Unity.WebRTC;

namespace Altimit.Networking
{
    // Adds peer to peer functionality to an app
    [AType]
    [RequireModule(typeof(ReplicationAM))]
    public class P2PClientAM : AppModule<P2PClientSM>
    {
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
        public Action<P2PClientSM> onDisconnected;

        public P2PClientSM P2PClientSM;
        //public SelfSession MasterClientSession;
        ClientSocketWs clientSocket;

        public ServerSocketWebRTC ServerSocketWebRTC;

        public IEnumerable<SelfSession> Sessions => SessionsByAppID.Select(x => x.Value);
        public Dictionary<Guid, SelfSession> SessionsByAppID = new Dictionary<Guid, SelfSession>();
        public Action<Guid, SelfSession> onSessionAdded;
        public Action<Guid, SelfSession> onSessionRemoved;
        public TaskCompletionDictionary<Guid, SelfSession> SessionAdded = new TaskCompletionDictionary<Guid, SelfSession>();

        ReplicationAM replicationAM => App.Get<ReplicationAM>();

        public P2PClientAM(string url, int port)
        {
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

        public override async void Init()
        {
            replicationAM.OnInitialized += OnReplicationInitialized;
        }

        async void OnReplicationInitialized()
        {
            ServerSocketWebRTC = new ServerSocketWebRTC(App.Get<ReplicationAM>().AppID, OnNegotiationNeeded, OnIceCandidate, App.Logger);
            ServerSocketWebRTC.PeerConnected += OnPeerConnected;

            if (OS.LogApps)
                Logger.Log("Connecting to P2P server...");

            // Automatically add session with self
            var selfSession = new SelfSession(App);
            selfSession.Peer = selfSession;
            AddSession(replicationAM.AppID, selfSession);

            /*
            if (URL != "localhost")
            {
                URL = Dns.GetHostAddresses(URL)[0].ToString();
            }*/
            clientSocket = new ClientSocketWs(URL, Port);
            clientSocket.PeerDisconnected += OnDisconnected;

            // Establish initial connection to P2P server
            var socketPeer = await clientSocket.ConnectAsync();
            var session = new SelfSession(App);
            session.Peer = new ProxyPeerSession(App, socketPeer, session.Logger);
            session.Peer.Peer = session;

            // Add P2PClientSM and let peer know about this app's ID
            P2PClientSM = await session.Connect<P2PClientSM>();
            await P2PClientSM.Peer.SetAppID(replicationAM.AppID);

            CompletionSource.SetResult(true);
            //var PeerAppID = await P2PServerSession.ExchangeAppID(App.ID);
            //ClientSession.PeerAppID = PeerAppID;
            //App.Replication.AddSession(PeerAppID, session);
        }

        public void OnPeerConnected(ISocketPeer peer)
        {
            var peerWebRTC = peer as PeerWebRTC;
            var peerAppID = peerWebRTC.PeerAppID;

            var session = new SelfSession(App);
            session.Peer = new ProxyPeerSession(App, peerWebRTC, session.Logger);
            session.Peer.Peer = session;

            //peer.Logger.Name = System.IO.Path.Combine(App.Name, peerAppID.ToAbbreviatedString());
            session.Logger.Name = System.IO.Path.Combine(App.Name, peerAppID.ToAbbreviatedString());
            AddSession(peerAppID, session);
        }


        public async void AddSession(Guid peerAppID, SelfSession session)
        {
            session.Get<ReplicationSM>().PeerAppID = peerAppID;

            SessionsByAppID.Add(peerAppID, session);
            onSessionAdded?.Invoke(peerAppID, session);
            SessionAdded.SetResult(peerAppID, session);
        }

        public async Task<SelfSession> ConnectOrGetSession(Guid appID)
        {
            SelfSession session;
            if (!TryGetSession(appID, out session))
            {
                session = await P2PClientSM.ConnectToPeer(appID);
            }
            return session;
        }

        public SelfSession GetSession(Guid appID)
        {
            return SessionsByAppID[appID];
        }

        public bool TryGetSession(Guid appID, out SelfSession session)
        {
            //session = null;
            //return true;
            return SessionsByAppID.TryGetValue(appID, out session);
        }

        public bool HasAuthority(object instance)
        {
            if (replicationAM.IsOwner(instance))
                return true;
            var session = GetSession(replicationAM.GetAppID(instance));
            return session.Get<ReplicationSM>().HasAuthority(instance);
        }

        public bool HasAuthority(object instance, string propertyName)
        {
            if (replicationAM.IsOwner(instance))
                return true;
            var session = GetSession(replicationAM.GetAppID(instance));
            return session.Get<ReplicationSM>().HasAuthority(instance, propertyName);
        }

        public virtual void RemoveSession(SelfSession session)
        {
            SessionsByAppID.Remove(session.Get<ReplicationSM>().PeerAppID);
            session.Dispose();
        }

        /*
        public async Task<string> CreatePeerOffer(Guid peerAppID)
        {
            return await ServerSocketWebRTC.CreateOffer(peerAppID);
        }*/

        public void ReceivePeerCandidate(Guid peerAppID, string candidate, string sdpMid, int sdpMLineIndex)
        {
            ServerSocketWebRTC.GetPeer(peerAppID).ReceiveCandidate(candidate, sdpMid, sdpMLineIndex);
        }

        public async void OnNegotiationNeeded(PeerWebRTC peer)
        {
            Guid peerAppID = peer.PeerAppID;
            var offerSDP = await peer.CreateOffer();

            //if (OS.LogP2P)
            //    Logger.Log($"Sending to peer app {peerAppID}...");

            // After sending an offer to the peer, check if it has also initiated a connection and wants this peer to stop
            // If so, stop handling the connection here and just await the completion of the connection
            var sdps = await P2PClientSM.Peer.RelayOffer(peerAppID, offerSDP);

            //if (OS.LogP2P)
            //    Logger.Log($"Received response from app {peerAppID}...");

            await peer.ReceiveAnswer(sdps.Value);

            //if (OS.LogP2P)
            //    Logger.Log($"Finished connecting to peer app {peerAppID}.");
        }

        public void OnIceCandidate(Guid peerAppID, WebRTCIceCandidate candidate)
        {
            if (candidate.Candidate != null)
                P2PClientSM.Peer.BroadcastPeerCandidate(peerAppID, candidate.Candidate, candidate.SdpMid, (int)candidate.SdpMLineIndex);
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

        public async void OnConnected(ISocketPeer socketPeer)
        {

        }

        private void OnDisconnected(ISocketPeer peer)
        {
            if (OS.LogApps)
                Logger.Log("Disconnected");
        }

        public override void Dispose()
        {
            
            foreach (var session in Sessions.ToList())
            {
                RemoveSession(session);
            }

            clientSocket?.Disconnect();
            //ServerSocketWebRTC.Stop();
            //clientSocket.PeerConnected -= OnConnected;
            clientSocket.PeerDisconnected -= OnDisconnected;
        }
    }
}
