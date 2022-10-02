using Altimit;
using System;
using System.Threading.Tasks;

namespace Altimit.Networking
{
    [AType]
    public interface IP2PServerSM : ISessionModule<IP2PClientSM>
    {
        [AMethod]
        Task<Response<WebRTCSessionDescription>> RelayOffer(Guid appID, WebRTCSessionDescription desc);
        [AMethod]
        Task RelayAnswer(Guid appID, WebRTCSessionDescription desc);
        [AMethod]
        Task SetAppID(Guid appID);
        [AMethod]
        void BroadcastPeerCandidate(Guid peerAppID, string candidate, string sdpMid, int sdpMLineIndex);
        [AMethod]
        void ConnectToPeer(Guid peerAppID);
    }
}
