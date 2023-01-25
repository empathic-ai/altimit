using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Security.Cryptography.X509Certificates;
using System.Threading.Tasks;
#if GODOT
using Godot;
using WebSocketSharp.Server;
#else
using WebSocketSharp;
using WebSocketSharp.Server;
#endif

namespace Altimit.Networking
{
    /// <summary>
    /// Server socket, which accepts websocket connections
    /// </summary>
    public class ServerSocketWs : ISocketServer
    {
        public Action<IPeerSocket> PeerConnected { get; set; }
        public Action<IPeerSocket> PeerDisconnected { get; set; }
#if GODOT
        TCPServer tcpServer;
#else
        WebSocketServer wsServer;
#endif
        public ILogger Logger { get; set; }

        protected bool isSecure = true;

        public ServerSocketWs(bool isSecure = true)
        {
            this.isSecure = isSecure;
        }

        ~ServerSocketWs()
        {
            Stop();
        }

        public async void Listen(int port = 0)
        {
            Stop();
#if GODOT
            // TODO: add full support--refer to this repo:
            // https://github.com/godotengine/godot-demo-projects/blob/4.0-dev/networking/websocket_chat/websocket/WebSocketServer.gd
            throw new NotImplementedException();
            tcpServer = new Godot.TCPServer();
            var cert = new Godot.X509Certificate();
            cert.Load(System.IO.Path.Combine(System.Environment.CurrentDirectory, "certificate.crt"));

            tcpServer.Listen((ushort)port);

            //wsServer.SslCertificate = cert;
            //wsServer.Listen(port, new string[] { "tls12", "tls11", "tls" });

            // TODO: Add back in
            //wsServer.PeerConnected += OnPeerConnected;
#else
            wsServer = new WebSocketServer(IPAddress.Any, port, isSecure);

            wsServer.Log.Output = (logData, text) => {
                if (logData.Level.Equals(LogLevel.Error) || logData.Level.Equals(LogLevel.Fatal) || logData.Level.Equals(LogLevel.Warn))
                {
                    //if (!logData.Message.Contains("An existing connection was forcibly closed by the remote host") &&
                    //!logData.Message.Contains("The header of a frame cannot be read from the stream"))
                    OS.LogError(logData.Message);
                    //OS.Log(text);
                    //throw new Exception(logData.Message + ": " + text);
                }
            };
            
            //ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12 | SecurityProtocolType.Tls11 | SecurityProtocolType.Tls;
            //wsServer.SslConfiguration.EnabledSslProtocols = System.Security.Authentication.SslProtocols.Tls12 | System.Security.Authentication.SslProtocols.Tls11 | System.Security.Authentication.SslProtocols.Tls;


            try
            {
                //wsServer.SslConfiguration.ServerCertificate = //new X509Certificate2(Path.Combine(Environment.CurrentDirectory, "certificate.crt"));
                //new X509Certificate2(Path.Combine(Environment.CurrentDirectory, "certificate.pfx"), "raindrops", X509KeyStorageFlags.DefaultKeySet);
            }
            catch (Exception e)
            {
                throw new Exception("A certificate.pfx file for this WebSocket server has not been added to the directory of this application! Please add it.", e);
            }

            
            wsServer.AddWebSocketService<ServerPeerWs>("/w", peer =>
            {
                Updater.Instance.OnNextUpdate(() =>
                {
                    PeerConnected?.Invoke(peer);
                    peer.Disconnected += PeerDisconnected;
                });
            });

            /*
            wsServer.AddWebSocketService("/w", () =>
            {
                var peer = new ServerPeerWs();
                Updater.Instance.OnNextUpdate(() =>
                {
                    PeerConnected?.Invoke(peer);
                    peer.Disconnected += PeerDisconnected;
                });
                return peer;
            });
            */

            wsServer.Start();
#endif
        }

#if LEGACY_GODOT
        private void OnPeerConnected(long id)
        {
            Updater.Instance.OnNextUpdate(() =>
            {

                var peer = new ServerPeerWs(wsServer, (int)id);
        PeerConnected?.Invoke(peer);
                peer.Disconnected += PeerDisconnected;
            });
        }
#endif

        public void Stop()
        {
#if GODOT
#else
            wsServer?.Stop();
#endif
        }
    }
}