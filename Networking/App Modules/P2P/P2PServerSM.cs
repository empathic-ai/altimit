using Altimit;
using Altimit.Networking;
using Altimit.Serialization;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Altimit.Networking
{
    public class P2PServerSM : SessionModule<P2PServerAM, IP2PClientSM>, IP2PServerSM
    {
        public Guid PeerAppID;
        P2PServerAM P2PServerApp => App.Get<P2PServerAM>();

        public override async void Init()
        {
            /*
            P2PClientSession = peerSession.GenerateModule<IP2PClientSessionModule>();
            PeerAppID = await P2PClientSession.GetAppID();
            replicationApp.AddSession(PeerAppID, Session);
            */
        }

        public async Task SetAppID(Guid appID)
        {
            PeerAppID = appID;
            //GetModule<P2PClient>().PeerAppID = appID;
            //Session.AddOrGetModule<P2PClient>().PeerAppID = appID;
            P2PServerApp.Sessions.Add(PeerAppID, Session);
            //return app.ID;
        }

        public void ConnectToPeer(Guid peerAppID)
        {
            P2PServerApp.ConnectPeers(PeerAppID, peerAppID);
        }

        public void BroadcastPeerCandidate(Guid peerAppID, string candidate, string sdpMid, int sdpMLineIndex)
        {
            P2PServerApp.BroadcastPeerCandidate(PeerAppID, peerAppID, candidate, sdpMid, sdpMLineIndex);
        }

        public async Task<Response<WebRTCSessionDescription>> RelayOffer(Guid appID, WebRTCSessionDescription desc)
        {
            var result = await P2PServerApp.Sessions[appID].Get<P2PServerSM>().Peer.ReceivePeerOffer(PeerAppID, desc);
            //Logger.Log($"Relaying result of offer to peer {PeerAppID}.");
            return result;
        }

        public async Task RelayAnswer(Guid appID, WebRTCSessionDescription desc)
        {
            await P2PServerApp.Sessions[appID].Get<P2PServerSM>().Peer.ReceivePeerAnswer(PeerAppID, desc);
        }
    }
}

//$28,000