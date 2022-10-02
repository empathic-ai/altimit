using System;
using System.Collections.Generic;
#if UNITY_64
using Unity.WebRTC;
#endif
using System.Threading.Tasks;

namespace Altimit.Networking
{
    /// <summary>
    /// Client for connecting to websocket server.
    /// </summary>
    public class ServerSocketWebRTC : IServerSocket
    {

        //static bool isLogging = false;
        public TaskCompletionSource<bool> OnConnected;
        public Action<ISocketPeer> PeerConnected { get; set; }
        public Action<ISocketPeer> PeerDisconnected { get; set; }
        public PairedDictionary<Guid, ISocketPeer> PeersByAppID = new PairedDictionary<Guid, ISocketPeer>();
        Action<PeerWebRTC> onNegotiationNeeded;
        Action<Guid, WebRTCIceCandidate> onIceCandidate;
        public ILogger Logger { get; set; }
        public Guid AppID;

        public ServerSocketWebRTC(Guid appID, Action<PeerWebRTC> onNegotiationNeeded, Action<Guid, WebRTCIceCandidate> onIceCandidate, ILogger logger)
        {
            AppID = appID;
            Logger = logger;

            this.onNegotiationNeeded = onNegotiationNeeded;
            this.onIceCandidate = onIceCandidate;
        }

        public void Listen(int port = default)
        {
        }

        public void Stop()
        {
            foreach (var peerByID in PeersByAppID)
            {
                peerByID.Value.Disconnect();
            }
        }

        public PeerWebRTC GetPeer(Guid peerAppID)
        {
            ISocketPeer peer;
            if (!PeersByAppID.TryGetValue(peerAppID, out peer))
            {
                peer = new PeerWebRTC(AppID, peerAppID, onNegotiationNeeded, (candidate) => onIceCandidate?.Invoke(peerAppID, candidate),
                   Logger.SubLogger(peerAppID.ToAbbreviatedString()));
                peer.Connected = OnPeerConnected;
                PeersByAppID[peerAppID] = peer;
            }
            return (PeerWebRTC)peer;
        }

        void OnPeerConnected(ISocketPeer peer)
        {
            peer.Disconnected += OnPeerDisconnected;
            PeerConnected?.Invoke(peer);
        }

        void OnPeerDisconnected(ISocketPeer peer)
        {
            PeersByAppID.RemoveBySecond(peer);
            peer.Connected -= OnPeerConnected;
            peer.Disconnected -= OnPeerDisconnected;
            PeerDisconnected?.Invoke(peer);
        }
    }

    public class P2PConnectedException : Exception
    {
        public P2PConnectedException()
        {
        }
    }
}