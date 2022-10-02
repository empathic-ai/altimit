using Altimit;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace Altimit.Networking
{
    //p2p client
    [AType]
    public interface IP2PClientSM : ISessionModule<IP2PServerSM>
    {
        [AMethod]
        void StartConnectionToPeer(Guid peerAppID);
        [AMethod]
        Task<Response<WebRTCSessionDescription>> ReceivePeerOffer(Guid peerAppID,
            WebRTCSessionDescription desc, CancellationToken token = default);

        [AMethod]
        Task ReceivePeerAnswer(Guid peerAppID, WebRTCSessionDescription desc);

        [AMethod]
        void ReceivePeerCandidate(Guid peerAppID, string candidate, string sdpMid, int sdpMLineIndex);
    }
}
