using Altimit;
using System;
using System.Threading.Tasks;

namespace Altimit.Networking
{
    [AType]
    public interface IP2PServerService : IConnectionSingleton
    {
        [AMethod]
        Task<Response<WebRTCSessionDescription>> RelayOffer(AID appID, WebRTCSessionDescription desc);
        [AMethod]
        Task RelayAnswer(AID appID, WebRTCSessionDescription desc);
        [AMethod]
        Task SetAppID(AID appID);
        [AMethod]
        void BroadcastPeerCandidate(AID peerAppID, string candidate, string sdpMid, int sdpMLineIndex);
        [AMethod]
        Task RequestConnectionToPeer(AID peerAppID);
    }
}
