namespace Altimit.Networking
{
    /*
    public abstract class ServerSocketBase : IServerSocket
    {
        public Action<ISocket, IPeer> PeerConnected { get; set; }
        public Action<ISocket, IPeer> PeerDisconnected { get; set; }

        public Dictionary<int, IPeer> ConnectedPeers { get; } = new Dictionary<int, IPeer>();
        public static bool IsDebugging = false;

        protected bool RethrowExceptionsInEditor = true;

        public abstract void Listen(int port);

        public abstract void Stop();

        protected ServerSocketBase() : base()
        {
  
            PeerConnected += OnPeerConnected;
            PeerDisconnected += OnPeerDisconnected;
        }

        ~ServerSocketBase()
        {
            PeerConnected -= OnPeerConnected;
        }

        public IPeer[] GetPeers()
        {
            return ConnectedPeers.Select(x=>x.Value).ToArray();
        }
        
        public IPeer GetPeer(int peerId)
        {
            IPeer peer;
            ConnectedPeers.TryGetValue(peerId, out peer);
            return peer;
        }

        public virtual void OnPeerConnected(ISocket socket, IPeer peer)
        {
            if (IsDebugging)
                Console.WriteLine(peer.Id + " connected.");
            //ConnectedPeers.Select(x => x.Value).Where(x => x.Equals(peer)).Send(OpCode_Internal.PeerConnect, peer.Id);
        }

        private void OnPeerDisconnected(ISocket socket, IPeer peer)
        {
            // Remove listener to messages
        }

        public void OnPeerAckConnect(IPeer peer, int peerID)
        {
            if (IsDebugging)
                Console.WriteLine(string.Format("Server relaying acknowledgement of peer {0}'s connection from peer {1}", peerID, peer.Id));
            //GetPeer(peerID).Send(OpCode_Internal.PeerAckConnect, peer.Id);
        }

        public void OnPeerAck(IPeer peer, int peerID)
        {
            if (IsDebugging)
                Console.WriteLine(string.Format("Server relaying acknowledgement of peer {0} from peer {1}", peerID, peer.Id));
            //GetPeer(peerID).Send(OpCode_Internal.PeerAck, peer.Id);
        }

        public void OnReceiveRelay(IPeer peer, int peerID, short opCode, byte[] bytes)
        {
            if (IsDebugging)
                Console.WriteLine(string.Format("Server relaying OpCode {0} from peer {1} to peer {2}", opCode, peer.Id, peerID));
            //GetPeer(peerID).Send(OpCode_Internal.Relay, peer.Id, opCode, bytes);
        }
    }
    */
}