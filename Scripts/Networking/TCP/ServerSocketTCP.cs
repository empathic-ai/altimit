using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;
using System.Threading;

namespace Altimit.Networking
{
    /// <summary>
    ///     Represents a socket, which listen to a port, and to which
    ///     other clients can connect
    /// </summary>
    public class ServerSocketTCP : IServerSocket
    {
        public ILogger Logger {get; set;}
        //private readonly HostTopology _topology;

        private readonly Dictionary<TcpClient, PeerTCP> _connectedPeers;
        private readonly Dictionary<TcpClient, float> _lastMessageTimes;

        private TcpListener tcpListener;
        private TcpClient connectedTcpClient;
        //private readonly byte[] _msgBuffer;
        private Thread tcpThread;

        public Action<ISocketPeer> PeerConnected { get; set; }
        public Action<ISocketPeer> PeerDisconnected { get; set; }

        //private int _socketId = -1;


        public ServerSocketTCP()// : this(BarebonesTopology.Topology)
        {
            _lastMessageTimes = new Dictionary<TcpClient, float>();
            _connectedPeers = new Dictionary<TcpClient, PeerTCP>();
            //_msgBuffer = new byte[NetworkMessage.MaxMessageSize];
        }

        public void Listen(int port)
        {
            tcpListener = new TcpListener(IPAddress.Parse("127.0.0.1"), port);

            /*
            tcpListener.BeginAcceptTcpClient(x => {
                var client = tcpListener.EndAcceptTcpClient(x);
                HandleConnect(client);
            }, tcpListener);
            */

            tcpThread = new Thread(new ThreadStart(Update));
            tcpThread.IsBackground = true;
            tcpThread.Start();
            //BmUpdateRunner.Instance.Add(this);
        }

        public void Update()
        {
            tcpListener.Start();
            Byte[] bytes = new Byte[1024];
            try
            {
                while (true)
                {
                    using (connectedTcpClient = tcpListener.AcceptTcpClient())
                    {
                        // Get a stream object for reading 					
                        using (NetworkStream stream = connectedTcpClient.GetStream())
                        {
                            int length;
                            // Read incomming stream into byte arrary.

                            while ((length = stream.Read(bytes, 0, bytes.Length)) != 0)
                            {
                                var incommingData = new byte[length];
                                Array.Copy(bytes, 0, incommingData, 0, length);
                                // Convert byte array to string message. 							
                                HandleData(connectedTcpClient, incommingData);
                                //Console.WriteLine("client message received as: " + clientMessage);
                            }
                        }
                    }
                }
            }
            catch (SocketException socketException)
            {
                Console.WriteLine("Socket exception: " + socketException);
            }

            /*
            var timedOutConnections = new List<int>();
            foreach (var lastMessageTime in _lastMessageTimes)
            {
                float timeSinceLastMessage = Time.time - lastMessageTime.Value;
                if (timeSinceLastMessage > BarebonesTopology.Topology.DefaultConfig.DisconnectTimeout)
                {
                    timedOutConnections.Add(lastMessageTime.Key);
                }
            }

            foreach (var timedOutconnection in timedOutConnections)
            {
               _connectedPeers[timedOutconnection].Disconnect("");
                HandleDisconnect(timedOutconnection, (byte)NetworkError.Timeout);
            }*/
        }

        private void HandleDisconnect(TcpClient tcpClient, byte error)
        {
            PeerTCP peer;
            _connectedPeers.TryGetValue(tcpClient, out peer);

            if (peer == null)
                return;

            peer.Dispose();

            _connectedPeers.Remove(tcpClient);
            _lastMessageTimes.Remove(tcpClient);

            peer.SetIsConnected(false);
            peer.NotifyDisconnectEvent();

            PeerDisconnected?.Invoke(peer);
        }

        private void HandleData(TcpClient tcpClient, byte[] bytes)
        {
            Console.WriteLine("Received data");
            PeerTCP peer;
            if (!_connectedPeers.TryGetValue(tcpClient, out peer))
            {
               // NetworkTransport.Disconnect(_socketId, connectionId, out error);
               // return;
            }

            peer?.HandleDataReceived(bytes);
        }

        private void HandleConnect(TcpClient socketConnection)
        {
            /*
            if (error != 0)
            {
                Logs.Error(string.Format("Error on ConnectEvent. ConnectionId: {0}, error: {1}", connectionId, error));
                return;
            }*/

            var peer = new PeerTCP(socketConnection);
            peer.SetIsConnected(true);
            _connectedPeers.Add(socketConnection, peer);

            peer.SetIsConnected(true);

            PeerConnected?.Invoke(peer);
        }

        public void Stop()
        {
            tcpListener?.Stop();
            tcpListener = null;
        }

        ~ServerSocketTCP()
        {
            Stop();
        }
    }
}