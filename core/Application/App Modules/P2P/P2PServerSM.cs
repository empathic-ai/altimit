using Altimit.Application.Apps;
using Altimit.Serialization;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Altimit.Networking
{
    public class P2PServerService : IP2PServerService
    {
        public AID PeerAppID;
        MasterServerApp P2PServerApp;

        /*
        public override async Task OnAdded()
        {
            P2PClientSession = peerSession.GenerateModule<IP2PClientSessionModule>();
            PeerAppID = await P2PClientSession.GetAppID();
            replicationApp.AddSession(PeerAppID, Session);
        }
        */

        public P2PServerService(MasterServerApp p2PServerModule)
        {
            P2PServerApp = p2PServerModule;
        }

        public async Task SetAppID(AID appID)
        {
            OS.Log(appID);
            PeerAppID = appID;
            //GetModule<P2PClient>().PeerAppID = appID;
            //Session.AddOrGetModule<P2PClient>().PeerAppID = appID;
            //P2PServerApp.AppSessions.Add(PeerAppID, Session);
            //return app.ID;
        }

        public async Task RequestConnectionToPeer(AID peerAppID)
        {
            await P2PServerApp.ConnectPeers(PeerAppID, peerAppID);
        }

        public void BroadcastPeerCandidate(AID peerAppID, string candidate, string sdpMid, int sdpMLineIndex)
        {
            P2PServerApp.BroadcastPeerCandidate(PeerAppID, peerAppID, candidate, sdpMid, sdpMLineIndex);
        }

        public async Task<Response<WebRTCSessionDescription>> RelayOffer(AID appID, WebRTCSessionDescription desc)
        {
            var result = await P2PServerApp.AppSessions[appID].Peer.GetService<IP2PClientService>().ReceivePeerOffer(PeerAppID, desc);
            //Logger.Log($"Relaying result of offer to peer {PeerAppID}.");
            return result;
        }

        public async Task RelayAnswer(AID appID, WebRTCSessionDescription desc)
        {
            await P2PServerApp.AppSessions[appID].Peer.GetService<IP2PClientService>().ReceivePeerAnswer(PeerAppID, desc);
        }
    }
}

//$28,000