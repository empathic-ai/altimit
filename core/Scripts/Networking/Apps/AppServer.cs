using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Altimit.Networking;
//using Unity.WebRTC;
using UnityEngine;

namespace Altimit
{
    public struct AppPair
    {
        public AID AppIDA;
        public AID AppIDB;

        public AppPair(AID appIDA, AID appIDB)
        {
            AppIDA = appIDA;
            AppIDB = appIDB;
        }
    }

    //[RequireModule(typeof(ReplicationModule))]
    public class MasterServerApp : App // : AppModule
    {
        public Dictionary<AID, SelfServiceProvider> AppSessions = new Dictionary<AID, SelfServiceProvider>();
        List<AppPair> appSessionPairs = new List<AppPair>();

        ISocketServer serverSocket;

        public MasterServerApp(AID appID) : base(appID)
        {
        }

        public override async Task Init()
        {
            await base.Init();
            serverSocket = new ServerSocketWs();
            serverSocket.PeerConnected += OnPeerConnected;
            serverSocket.PeerDisconnected += OnPeerDisconnected;
            serverSocket.Listen(OS.Settings.MasterPort);
            if (OS.LogApps)
                Logger.Log($"Master server listening on port {OS.Settings.MasterPort}...");
        }

        List<SelfServiceProvider> sessions = new List<SelfServiceProvider>();

        async void OnPeerConnected(IPeerSocket peer)
        {
            if (OS.LogApps)
                Logger.Log("Peer connected!");


            // TODO: Reimplement
            /*
            var selfServiceProvider = new SelfServiceProvider(App);
            var replicationService = selfServiceProvider.AddService(new AppConnection(App, selfServiceProvider));

            //selfServiceProvider.Peer = new RemoteServiceProvider(App, replicationService, peer, selfServiceProvider.Logger);
            replicationService.PeerReplicationService = selfServiceProvider.Peer.AddService(replicationService);

            selfServiceProvider.Peer.Peer = selfServiceProvider;

            sessions.Add(selfServiceProvider);
            */


            //session.AddModule<P2PServerSessionModule>();

            /*
            var P2PClientSession = session.PeerSession.GenerateModule<IP2PClientSessionModule>();
            var PeerAppID = await P2PClientSession.ExchangeAppID();
            App.Replication.AddSession(PeerAppID, session);
            */

            //session.PeerSession.GetModule<IReplicationSessionModule>().ChangeProperty(null, "AsdF", null);
        }

        void OnPeerDisconnected(IPeerSocket peer)
        {
            if (OS.LogApps)
                Logger.Log("Peer disconnected!");

        }

        public async Task ConnectPeers(AID peerAppID, AID otherPeerAppID)
        {
            bool isGreater = peerAppID.CompareTo(otherPeerAppID) < 0;
            var greaterAppID = isGreater ? peerAppID : otherPeerAppID;
            var lesserAppID = isGreater ? otherPeerAppID : peerAppID;
            var appPair = new AppPair(greaterAppID, lesserAppID);

            if (!appSessionPairs.Contains(appPair))
            {
                appSessionPairs.Add(appPair);
                await AppSessions[greaterAppID].GetService<IP2PClientService>().ConnectToPeer(lesserAppID);
            }
        }

        public void Dispose()
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

        public void BroadcastPeerCandidate(AID appID, AID otherAppID, string candidate, string sdpMid, int sdpMLineIndex)
        {
            SelfServiceProvider otherSession;
            if (!AppSessions.TryGetValue(otherAppID, out otherSession))
                Logger.LogError($"Failed to find app with an ID of {otherAppID}! Attempted to broadcast candidate from app {appID}.");
            //App.Logger.LogFormat("Broadcating WebRTC candidate from Peer {0} to Peer {1}: {2}, {3}, {4}.", appID, otherAppID, candidate, sdpMid, sdpMLineIndex);
            otherSession.Peer.GetService<IP2PClientService>().ReceivePeerCandidate(appID, candidate, sdpMid, sdpMLineIndex);
        }
    }
}
