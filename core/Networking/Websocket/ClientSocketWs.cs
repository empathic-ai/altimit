using System;
using System.IO;
using System.Net;
using System.Threading.Tasks;
using UnityEngine;
using System.Security.Policy;
using System.ComponentModel.DataAnnotations;
using System.Security.Cryptography.X509Certificates;
#if GODOT
using Godot;
#else
using WebSocketSharp;
#endif
namespace Altimit.Networking
{
    public class ClientSocketWs : ISocketClient
    {
        public IPeerSocket ServerPeer { get; private set; }
        public bool IsConnected { get; private set; } = false;

        public Action<IPeerSocket> PeerConnected { get; set; }
        public Action<IPeerSocket> PeerDisconnected { get; set; }
#if GODOT
        WebSocketPeer ws;
        string url;
#else
        WebSocket ws;
#endif

        TaskCompletionSource<IPeerSocket> completionSource = new TaskCompletionSource<IPeerSocket>();

        public ClientSocketWs(string url)
        {
            Init(url);
        }
        
        public ClientSocketWs(string ip, int port, bool isSecure = true)
        {
            Init(ip, port, isSecure);
        }

        void Init(string ip, int port, bool isSecure = true)
        {
            
            var startUrl = isSecure ? "wss" : "ws";
            var url = startUrl + string.Format("://{0}:{1}/", ip, port);

            Init(url);
        }

        void Init(string url)
        {
            System.Net.ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls | SecurityProtocolType.Tls11 | SecurityProtocolType.Tls12;

#if GODOT
            ws = new Godot.WebSocketPeer();
            ws.EncodeBufferMaxSize = 268435456;
            ws.InboundBufferSize = 268435456;
            ws.OutboundBufferSize = 268435456;

            //ws.SupportedProtocols = new string[] { "tls12", "tls11", "tls" };
            this.url = url;
#else
            ws = new WebSocket(url);
            ws.WaitTime = TimeSpan.FromSeconds(3);
            ws.EnableRedirection = true;
#endif
            ServerPeer = new ClientPeerWs(ws);
            ServerPeer.Connected += OnPeerConnected;
            ServerPeer.Disconnected += OnPeerDisconnected;
        }

        public async Task<IPeerSocket> ConnectAsync()
        {
            //TODO: make async again if doesn't fix connection problems
#if GODOT

#elif GODOT
            ws.Connect();
#else
            ws.ConnectAsync();
            
            while (!ws.IsAlive)
            {
                await Task.Delay(TimeSpan.FromSeconds(3));
            }
#endif
            return await completionSource.Task;
        }

        public void Connect()
        {
#if GODOT
            //var certificate = new Godot.X509Certificate();
            //certificate.Load(Path.Combine(System.Environment.CurrentDirectory, "certificate.crt"));
            var error = ws.ConnectToUrl(url);//, certificate);
            if (error != Error.Ok)
            {
                OS.LogError(error);
            }
#else
            ws.Connect();
#endif
        }

        void OnPeerConnected(IPeerSocket peer)
        {
            IsConnected = true;
            PeerConnected?.Invoke(peer);
            completionSource.SetResult(peer);
        }

        void OnPeerDisconnected(IPeerSocket peer)
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