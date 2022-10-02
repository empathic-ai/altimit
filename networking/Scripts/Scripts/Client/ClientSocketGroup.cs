namespace Altimit.Networking
{
    /*
    public class ClientSocketGroup : IClientSocket
    {
        static bool IsLogging = false;
        Type defaultSocketType;
        public override event Action<IClientSocket> Connected;
        public override event Action<IClientSocket> Disconnected;
        IClientSocket[] clientSockets;
        IClientSocket defaultSocket;

        public ClientSocketGroup(params IClientSocket[] _clientSockets) : base()
        {
            clientSockets = _clientSockets;
            ServerPeer = new PeerGroup();

            defaultSocketType = clientSockets.First().GetType();
            foreach (var clientSocket in clientSockets)
            {
                clientSocket.Connected += SocketConnected;
                clientSocket.Disconnected += SocketDisconnected;
            }
            defaultSocket = Get(defaultSocketType);
           // defaultSocket.On(OpCode_Internal.GroupConnect, new Delegate<int>(OnGroupConnect));
            //defaultSocket.On(OpCode_Internal.GroupAck, new Delegate(OnGroupAck));
        }

        private void SocketConnected(IClientSocket clientSocket)
        {
            bool isConnected = clientSockets.All(x => x.IsConnected);
            if (IsLogging)
                Console.WriteLine(string.Format("{0] connected.", clientSocket.GetType()));

            if (isConnected && IsConnecting)
            {
                //IsConnecting = false;
                if (IsLogging)
                    Console.WriteLine("All client sockets connected.");
                //defaultSocket.ServerPeer.Send((int)OpCode_Internal.GroupPending);
            }
        }

        private void SocketDisconnected(IClientSocket clientSocket)
        {
            if (IsConnecting || IsConnected)
            {
                Disconnected?.Invoke(clientSocket);
                IsConnecting = false;
                IsConnected = false;
            }
        }

        public void OnGroupConnect(int socketGroupID)
        {
            ID = socketGroupID;
            foreach (var clientSocket in clientSockets)
            {
                //clientSocket.ServerPeer.Send((int)OpCode_Internal.GroupAckConnect, socketGroupID);
            }
        }

        public void OnGroupAck()
        {
            IsConnecting = false;
            IsConnected = true;

            var peerGroup = new PeerGroup();
            foreach (var clientSocket in clientSockets)
                peerGroup.AddPeer(clientSocket.ServerPeer);
            peerGroup.MessageReceived += HandleMessage;
            ServerPeer = peerGroup;

            Connected?.Invoke(this);
        }

        public override void Connect(string ip, int port, int timeoutMillis)
        {
            IsConnecting = true;
            foreach (var clientSocket in clientSockets)
                clientSocket.Connect(ip, port, timeoutMillis);
        }

        public override void Disconnect()
        {
            foreach (var clientSocket in clientSockets)
                clientSocket.Disconnect();
        }

        public IClientSocket Get<T>() where T : IClientSocket
        {
            return Get(typeof(T));
        }

        public IClientSocket Get(Type socketType)
        {
            return clientSockets.SingleOrDefault(x => x.GetType() == socketType);
        }

        public override void Reconnect()
        {
            throw new System.NotImplementedException();
        }
    }*/
}
