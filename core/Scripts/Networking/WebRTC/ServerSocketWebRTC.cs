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
    public class ServerSocketWebRTC : ISocketServer
    {
        //static bool isLogging = false;
        public TaskCompletionSource<bool> OnConnected;
        public Action<IPeerSocket> PeerConnected { get; set; }
        public Action<IPeerSocket> PeerDisconnected { get; set; }
        public APairedDictionary<AID, IPeerSocket> PeersByAppID = new APairedDictionary<AID, IPeerSocket>();
        Action<WebRTCPeer> onNegotiationNeeded;
        Action<AID, WebRTCIceCandidate> onIceCandidate;
        public ILogger Logger { get; set; }
        public AID AppID;

        public ServerSocketWebRTC(AID appID, Action<WebRTCPeer> onNegotiationNeeded, Action<AID, WebRTCIceCandidate> onIceCandidate, ILogger logger)
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

        public WebRTCPeer GetPeer(AID peerAppID)
        {
            return (WebRTCPeer)PeersByAppID[peerAppID];
        }

        public async Task<WebRTCPeer> ConnectToPeer(AID peerAppID)
        {
            WebRTCPeer peer;
            if (!PeersByAppID.ContainsKey(peerAppID))
            {
                peer = new WebRTCPeer(peerAppID, onNegotiationNeeded, (candidate) => onIceCandidate?.Invoke(peerAppID, candidate),
                   Logger.SubLogger(peerAppID.ToAbbreviatedString()));

                PeersByAppID[peerAppID] = peer;

                peer.Connected = OnPeerConnected;
            } else {
                peer = GetPeer(peerAppID);
            }

            if (!peer.IsConnected)
                await peer.ConnectAsync();

            return peer;
        }

        void OnPeerConnected(IPeerSocket peer)
        {
            peer.Disconnected += OnPeerDisconnected;
            PeerConnected?.Invoke(peer);
        }

        void OnPeerDisconnected(IPeerSocket peer)
        {
            PeersByAppID.RemoveBySecond(peer);
            peer.Connected -= OnPeerConnected;
            peer.Disconnected -= OnPeerDisconnected;
            PeerDisconnected?.Invoke(peer);
        }
    }
}