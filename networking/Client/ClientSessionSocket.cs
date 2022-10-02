using Altimit.Serialization;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace Altimit.Networking
{
    /*
    public class ClientSessionSocket<TClientSocket, TClientSession, TServerSession> : SessionSocket<TClientSession, TServerSession> where TClientSocket : IClientSocket  where TClientSession : ISelfSession where TServerSession : ISession
    {
        public Action Connected { get; set; }
        public Action Disconnected { get; set; }

        public bool IsConnected = false;
        
        public TClientSession Session;

        bool tryReconnect = true;
        string ip = "";
        int port = 0;

        TClientSocket socket;
        Guid appID;

        public ClientSessionSocket(TClientSocket socket, InstanceDatabase instanceDatabase, Func<TClientSession> selfSessionFactory, ILogger logger) : base(instanceDatabase, selfSessionFactory, logger)
        {
            this.socket = socket;
            //socket.Disconnected += OnDisconnected;
        }

        public async void OnDisconnected()
        {
            IsConnected = false;
            Disconnected?.Invoke();

            RemoveSession(Session);

            if (tryReconnect)
            {
                logger.LogError(new Exception("Disconnected from server! Attempting to reconnect..."));
                await Connect(ip, port);
            }
        }

        public async Task Connect(string ip, int port, CancellationToken cancellationToken = default)
        {
            tryReconnect = true;
            this.ip = ip;
            this.port = port;
            socket.Connect(ip, port);
            int waitTime = 0;
            while (!socket.IsConnected && waitTime < 3000)
            {
                waitTime += 1000;
                await Task.Delay(1000);
                OS.Log("Trying to connect...");
                cancellationToken.ThrowIfCancellationRequested();
            }

            if (socket.IsConnected)
            {
                if (OS.LogSockets)
                    logger.Log("Connected to socket!");
                Session = await AddSession(socket.ServerPeer);
                IsConnected = true;
                Connected?.Invoke();
            }
            else if (tryReconnect)
            {
                if (OS.LogSockets)
                    logger.Log("Failed to connect to server! Attempting to connect again...");
                await Connect(ip, port, cancellationToken);
            } else
            {
                if (OS.LogSockets)
                    logger.Log("Failed to connect to server!");
            }
        }

        public override void Dispose()
        {
            base.Dispose();
            tryReconnect = false;
            socket?.Disconnect();
        }

        public TClientSession GetSession()
        {
            return Session;
        }

        ~ClientSessionSocket()
        {
        }
    }
    */
}
