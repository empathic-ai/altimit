using System;
using System.IO;
using System.Net;
using System.Threading.Tasks;
using UnityEngine;
using System.Security.Policy;
#if GODOT
using Godot;
#else
using WebSocketSharp;
#endif
namespace Altimit.Networking
{
    public class ClientSocketWs : IClientSocket
    {
        public ISocketPeer ServerPeer { get; private set; }

        public bool IsConnected { get; private set; } = false;

        public Action<ISocketPeer> PeerConnected { get; set; }
        public Action<ISocketPeer> PeerDisconnected { get; set; }
#if GODOT
        WebSocketClient ws;
        string url;
#else
        WebSocket ws;
#endif

        TaskCompletionSource<ISocketPeer> completionSource = new TaskCompletionSource<ISocketPeer>();

        public ClientSocketWs(string ip, int port)
        {
            System.Net.ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls | SecurityProtocolType.Tls11 | SecurityProtocolType.Tls12;

            var url = string.Format("wss://{0}:{1}/w", ip, port);
#if GODOT
            ws = new WebSocketClient();
            this.url = url;
#else
            ws = new WebSocket(url);
#endif
            ServerPeer = new ClientPeerWs(ws);
            ServerPeer.Connected += OnPeerConnected;
            ServerPeer.Disconnected += OnPeerDisconnected;
        }

        public async Task<ISocketPeer> ConnectAsync()
        {
            //TODO: make async again if doesn't fix connection problems
#if GODOT
            ws.ConnectToUrl(url);
#else
            ws.ConnectAsync();
#endif
            return await completionSource.Task;
        }

        void OnPeerConnected(ISocketPeer peer)
        {
            IsConnected = true;
            PeerConnected?.Invoke(peer);
            completionSource.SetResult(peer);
        }

        void OnPeerDisconnected(ISocketPeer peer)
        {
            IsConnected = false;
            ServerPeer.Connected -= OnPeerConnected;
            ServerPeer.Disconnected -= OnPeerDisconnected;
            PeerDisconnected?.Invoke(ServerPeer);
        }

        public void Disconnect()
        {
            //if (IsConnected)
            ServerPeer?.Disconnect();
        }
    }
}