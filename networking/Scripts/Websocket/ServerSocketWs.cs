using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Security.Cryptography.X509Certificates;
using System.Threading.Tasks;
#if GODOT
using Godot;
#else
using WebSocketSharp;
using WebSocketSharp.Server;
#endif

namespace Altimit.Networking
{
    /// <summary>
    /// Server socket, which accepts websocket connections
    /// </summary>
    public class ServerSocketWs : IServerSocket
    {
        public Action<ISocketPeer> PeerConnected { get; set; }
        public Action<ISocketPeer> PeerDisconnected { get; set; }
        WebSocketServer wsServer;
        public ILogger Logger { get; set; }

        public ServerSocketWs()
        {
        }

        ~ServerSocketWs()
        {
            Stop();
        }

        public async void Listen(int port = 0)
        {
            Stop();
#if GODOT
            wsServer = new WebSocketServer();
            var cert = new Godot.X509Certificate();
            cert.Load(Path.Combine(System.Environment.CurrentDirectory, "certificate.crt"));
            wsServer.TlsCertificate = cert;
            wsServer.Listen(port, new string[] { "tls12", "tls11", "tls" });
            wsServer.PeerConnected += OnPeerConnected;
#else
            wsServer = new WebSocketServer(IPAddress.Any, port, true);

            wsServer.Log.Output = (logData, text) => {
                if (logData.Level.Equals(LogLevel.Error) || logData.Level.Equals(LogLevel.Fatal) || logData.Level.Equals(LogLevel.Warn))
                {
                    //if (!logData.Message.Contains("An existing connection was forcibly closed by the remote host") &&
                    //!logData.Message.Contains("The header of a frame cannot be read from the stream"))
                    throw new Exception(logData.Message);
                }
            };
            
            ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12 | SecurityProtocolType.Tls11 | SecurityProtocolType.Tls;
            wsServer.SslConfiguration.EnabledSslProtocols = System.Security.Authentication.SslProtocols.Tls12 | System.Security.Authentication.SslProtocols.Tls11 | System.Security.Authentication.SslProtocols.Tls;

            try
            {
                wsServer.SslConfiguration.ServerCertificate = new X509Certificate2(Path.Combine(Environment.CurrentDirectory, "certificate.pfx"), "raindrops", X509KeyStorageFlags.DefaultKeySet);
            }
            catch (Exception e)
            {
                throw new Exception("A certificate.pfx file for this WebSocket server has not been added to the directory of this application! Please add it.", e);
            }
            
            wsServer.AddWebSocketService<ServerPeerWs>("/w", peer => {
                Updater.Instance.OnNextUpdate(() =>
                {
                    PeerConnected?.Invoke(peer);
                    peer.Disconnected += PeerDisconnected;
                });
            });

            wsServer.Start();
#endif
        }

#if GODOT
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
            wsServer?.Stop();
        }
    }
}