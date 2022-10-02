using Altimit.Serialization;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Altimit.Networking
{
    public class ServerSessionSocket<TServerSocket> where TServerSocket : IServerSocket
    {
        PairedDictionary<ISocketPeer, SelfSession> SessionsByPeer = new PairedDictionary<ISocketPeer, SelfSession>();
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

        public void Disconnect(SelfSession session)
        {
            var peer = SessionsByPeer.GetBySecond(session);
            peer.Disconnect();
        }

        public List<SelfSession> GetSessions()
        {
            return SessionsByPeer.Select(x => x.Value).ToList();
        }

        public async void OnPeerConnected(ISocketPeer peer)
        {
            //var session = await AddSession(peer);
            //SessionsByPeer.Add(peer, session);
        }

        public void OnPeerDisconnected(ISocketPeer peer)
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
