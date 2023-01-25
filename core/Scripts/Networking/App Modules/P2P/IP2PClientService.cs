using Altimit;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace Altimit.Networking
{
    //p2p client
    [AType]
    public interface IP2PClientService : IConnectionSingleton
    {
        [AMethod]
        Task ConnectToPeer(AID peerAppID);
        [AMethod]
        Task<Response<WebRTCSessionDescription>> ReceivePeerOffer(AID peerAppID,
            WebRTCSessionDescription desc, CancellationToken token = default);

        [AMethod]
        Task ReceivePeerAnswer(AID peerAppID, WebRTCSessionDescription desc);

        [AMethod]
        void ReceivePeerCandidate(AID peerAppID, string candidate, string sdpMid, int sdpMLineIndex);
    }
}
