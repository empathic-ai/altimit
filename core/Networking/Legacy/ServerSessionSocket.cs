using Altimit.Serialization;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Altimit.Networking
{
    public class ServerSessionSocket<TServerSocket> where TServerSocket : ISocketServer
    {
        APairedDictionary<IPeerSocket, SelfServiceProvider> SessionsByPeer = new APairedDictionary<IPeerSocket, SelfServiceProvider>();
        //PairedDictionary<IPeer, TClientApp> PeerClients = new PairedDictionary<IPeer, TClientApp>();
        //List<TClientApp> Clients = new List<TClientApp>();

        public TServerSocket Socket;

        public ServerSessionSocket(ILogger logger)
        {
            Socket = Activator.CreateInstance<TServerSocket>();
            //Socket = socket;
            //socket.PeerConnected += OnPeerConnected;
            //socket.PeerDisconnected += OnPeerDisconnected;
        }

        public void Disconnect(SelfServiceProvider session)
        {
            var peer = SessionsByPeer.GetBySecond(session);
            peer.Disconnect();
        }

        public List<SelfServiceProvider> GetSessions()
        {
            return SessionsByPeer.Select(x => x.Value).ToList();
        }

        public async void OnPeerConnected(IPeerSocket peer)
        {
            //var session = await AddSession(peer);
            //SessionsByPeer.Add(peer, session);
        }

        public void OnPeerDisconnected(IPeerSocket peer)
        {
            var session = SessionsByPeer[peer];
            //RemoveSession(session);
            SessionsByPeer.Remove(peer);
        }

        public void Listen(int port)
        {
            Socket.Listen(port);
        }

        public void Dispose()
        {
            Socket.Stop();
        }
    }
}
