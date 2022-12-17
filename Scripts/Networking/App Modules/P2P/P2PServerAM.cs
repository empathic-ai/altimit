using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Altimit;
using Altimit.Networking;
//using Unity.WebRTC;
using UnityEngine;

namespace Altimit.Networking
{
    public struct AppPair
    {
        public Guid AppIDA;
        public Guid AppIDB;

        public AppPair(Guid appIDA, Guid appIDB)
        {
            AppIDA = appIDA;
            AppIDB = appIDB;
        }
    }

    [RequireModule(typeof(ReplicationAM))]
    public class P2PServerAM : AppModule<P2PServerSM>
    {
        public Dictionary<Guid, SelfSession> Sessions = new Dictionary<Guid, SelfSession>();
        List<AppPair> appSessionPairs = new List<AppPair>();

        IServerSocket serverSocket;
        int port;

        public P2PServerAM(int port)
        {
            this.port = port;
        }

        public override async Task OnAdded()
        {
            serverSocket = new ServerSocketWs();
            serverSocket.PeerConnected += OnPeerConnected;
            serverSocket.PeerDisconnected += OnPeerDisconnected;
            serverSocket.Listen(port);
            if (OS.LogApps)
                Logger.Log($"P2P server listening on port {port}...");
        }

        List<SelfSession> sessions = new List<SelfSession>();

        async void OnPeerConnected(ISocketPeer peer)
        {
            if (OS.LogApps)
                Logger.Log("Peer connected!");

            var session = new SelfSession(App);
            session.AddModule<ReplicationSM>();
            session.Peer = new ProxyPeerSession(App, peer, session.Logger);
            session.Peer.Peer = session;

            sessions.Add(session);

            //session.AddModule<P2PServerSessionModule>();

            /*
            var P2PClientSession = session.PeerSession.GenerateModule<IP2PClientSessionModule>();
            var PeerAppID = await P2PClientSession.ExchangeAppID();
            App.Replication.AddSession(PeerAppID, session);
            */

            //session.PeerSession.GetModule<IReplicationSessionModule>().ChangeProperty(null, "AsdF", null);
        }

        void OnPeerDisconnected(ISocketPeer peer)
        {
            if (OS.LogApps)
                Logger.Log("Peer disconnected!");

        }

        public void ConnectPeers(Guid peerAppID, Guid otherPeerAppID)
        {
            bool isGreater = (peerAppID.CompareTo(otherPeerAppID) < 0);
            var greaterAppID = isGreater ? peerAppID : otherPeerAppID;
            var lesserAppID = isGreater ? otherPeerAppID : peerAppID;
            var appPair = new AppPair(greaterAppID, lesserAppID);

            if (!appSessionPairs.Contains(appPair))
            {
                appSessionPairs.Add(appPair);
                Sessions[greaterAppID].Peer.Get<IP2PClientSM>().StartConnectionToPeer(lesserAppID);
            }
        }

        public override void Dispose()
        {
            foreach (var session in sessions)
            {
                // TODO: possibly reimplement in smoother way (something like P2PServerAM.Disconnect ?
                session.Dispose();
            }
            serverSocket?.Stop();
        }


        /*
        public async void ConnectSessions(RoomServerSession session, RoomServerSession otherSession)
        {
            Guid appID = session.PeerAppID;
            Guid otherAppID = otherSession.PeerAppID;
            Logger.LogFormat("Connecting Peer {0} to Peer {1}.", appID, otherAppID);
            // Tell the primary peer to introduce itself to the other, returning an offer
            var offer = await session.PeerSession.CreatePeerOffer(otherAppID);
            // Relay the primary peer's offer to the other peer, returning an answer
            var answer = await otherSession.PeerSession.ReceivePeerOffer(appID, offer);
            Logger.LogFormat("Relaying WebRTC answer from {0} to {1}.", otherSession.PeerAppID, session.PeerAppID);
            // Relay the other peer's answer to the primary peer
            session.PeerSession.ReceivePeerAnswer(otherAppID, answer);
        }
        */

        public void BroadcastPeerCandidate(Guid appID, Guid otherAppID, string candidate, string sdpMid, int sdpMLineIndex)
        {
            SelfSession otherSession;
            if (!Sessions.TryGetValue(otherAppID, out otherSession))
                Logger.LogError($"Failed to find app with an ID of {otherAppID}! Attempted to broadcast candidate from app {appID}.");
            //App.Logger.LogFormat("Broadcating WebRTC candidate from Peer {0} to Peer {1}: {2}, {3}, {4}.", appID, otherAppID, candidate, sdpMid, sdpMLineIndex);
            otherSession.Peer.Get<IP2PClientSM>().ReceivePeerCandidate(appID, candidate, sdpMid, sdpMLineIndex);
        }
    }
}
