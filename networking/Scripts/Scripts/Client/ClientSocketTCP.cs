using System;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Threading;
using System.Threading.Tasks;

namespace Altimit.Networking
{
    /// <summary>
    ///     Represents a socket (client socket), which can be used to connect
    ///     to another socket (server socket)
    /// </summary>
    /*
    public class ClientSocketTCP : IClientSocket
    {
        public static bool RethrowExceptionsInEditor = true;

        //private readonly HostTopology _topology;
        private int _connectionId;

        private string _ip;
        private int _port;

        private bool _isConnectionPending;
        private readonly byte[] _msgBuffer;

        public ISocketPeer ServerPeer { get => serverPeer; }
        private PeerTCP serverPeer;
        //private int _socketId;

        private ConnectionStatus _status;
        private int _stopConnectingTick;
        private TcpClient _socketConnection;

        public ClientSocketTCP()// : this(BarebonesTopology.Topology)
        {
        }
        
        public ClientSocketTCP(HostTopology topology)
        {
            _msgBuffer = new byte[NetworkMessage.MaxMessageSize];
            _topology = topology;
        }

        public Action<ISocketPeer> PeerConnected { get; set; }
        /// <summary>
        /// Event, which is invoked when we are
        /// disconnected from another socket
        /// </summary>
        public Action<ISocketPeer> PeerDisconnected { get; set; }

        /// <summary>
        /// Event, invoked when connection status changes
        /// </summary>
        public event Action<ConnectionStatus> StatusChanged;

        /// <summary>
        /// Returns true, if we are connected to another socket
        /// </summary>
        public bool IsConnected { get; set; }

        /// <summary>
        /// Returns true, if we're in the process of connecting
        /// </summary>
        public bool IsConnecting
        {
            get
            {
                return Status.Equals(ConnectionStatus.Connecting);
            }
        }//{ get; private set; } 

        /// <summary>
        /// Connection status
        /// </summary>
        public ConnectionStatus Status
        {
            get { return _status; }
             set
            {
                if ((_status != value) && (StatusChanged != null))
                {
                    _status = value;
                    StatusChanged.Invoke(_status);
                    return;
                }
                _status = value;
            }
        }

        public string ConnectionIp { get; set; }

        public int ConnectionPort { get; set; }

        private Thread tcpThread;

        /// <summary>
        /// Starts connecting to another socket
        /// </summary>
        /// <param name="ip"></param>
        /// <param name="port"></param>
        /// <returns></returns>
        public void Connect(string ip, int port)
        {
            Status = ConnectionStatus.Connecting;

            ConnectionIp = ip;
            ConnectionPort = port;
            //NetworkTransport.Init();
            _stopConnectingTick = Environment.TickCount + 5000;
            _ip = ip;
            _port = port;

            if (_ip == "localhost")
                _ip = "127.0.0.1";

            // TODO Finish implementing multiple connects 
            if (!ValidateIPv4(_ip))
            {
                try
                {
                    _ip = Dns.GetHostEntry(_ip).AddressList[0].ToString();
                }
                catch (Exception e)
                {
                    throw new Exception("Invalid IP Address! The following error occurred: " + e);
                }
            }

            //Console.WriteLine(_ip);
            //_socketId = NetworkTransport.AddHost(_topology, 0);

            //BmUpdateRunner.Instance.Add(this);

            _socketConnection = new TcpClient();
            _socketConnection.Connect(_ip, port);

            
            tcpThread = new Thread(new ThreadStart(Update));
            tcpThread.IsBackground = true;
            tcpThread.Start();
        }

        /// <summary>
        /// Disconnects and connects again
        /// </summary>
        public void Reconnect()
        {
            Disconnect();

            Connect(_ip, _port);
        }

        public void Disconnect()
        {
            _socketConnection.Close();
            //NetworkTransport.Disconnect(_socketId, _connectionId, out error);
            //NetworkTransport.RemoveHost(_socketId);

            // When we disconnect ourselves, we dont get NetworkEventType.DisconnectEvent 
            // Not sure if that's the expected behaviour, but oh well...
            // TODO Make sure there's no other way
            HandleDisconnect(_socketConnection);
        }

        public void Update()
        {

            //if (_socketId == -1)
            //    return;

            try
            {
                while (true)
                {
                    if (IsConnecting && !IsConnected)
                    {
                        // Try connecting
                        
                        //UnityEngine.Console.WriteLine(Environment.TickCount > _stopConnectingTick);
                        if (Environment.TickCount > _stopConnectingTick)
                        {
                            // Timeout reached
                            StopConnecting();
                            return;
                        }

                        //Status = ConnectionStatus.Connecting;

                        if (!_isConnectionPending)
                        {
                            _isConnectionPending = true;

                            if (_socketConnection.Connected)
                            {
                                HandleConnect(_socketConnection);
                            }
                            //_connectionId = NetworkTransport.Connect(_socketId, _ip, _port, 0, out error);

                            
                            if (error != (int) NetworkError.Ok)
                            {
                                StopConnecting();
                                return;
                            }
                        }
                    }

                    //NetworkEventType networkEvent;

                    //NetworkStream stream = _socketConnection.GetStream();
                    
                    if (IsConnected)
                    {
                        Byte[] bytes = new Byte[1024];
                        int length;
                        using (NetworkStream stream = _socketConnection.GetStream())
                        {
                            // Read incomming stream into byte arrary. 					
                            while ((length = stream.Read(bytes, 0, bytes.Length)) != 0)
                            {
                                var incommingData = new byte[length];
                                Array.Copy(bytes, 0, incommingData, 0, length);
                                _serverPeer.HandleDataReceived(bytes, 0);
                                // Convert byte array to string message.					
                                //string serverMessage = Encoding.ASCII.GetString(incommingData);
                                //Console.WriteLine("server message received as: " + serverMessage);
                            };// while (networkEvent != NetworkEventType.Nothing);
                        }
                    }
                }
            }
            catch (SocketException socketException)
            {
                Console.WriteLine("Socket exception: " + socketException);
            }
        }

        bool ValidateIPv4(string ipString)
        {
            if (String.IsNullOrWhiteSpace(ipString))
            {
                return false;
            }

            string[] splitValues = ipString.Split('.');
            if (splitValues.Length != 4)
            {
                return false;
            }

            byte tempForParsing;

            return splitValues.All(r => byte.TryParse(r, out tempForParsing));
        }

        public static Task ConnectTaskAsync(TcpClient aClient, string ip, int aPort, AsyncCallback anAsyncCallback)
        {
            Task t = Task.Factory.FromAsync(aClient.BeginConnect(ip, aPort, anAsyncCallback, aClient), aClient.EndConnect);
            return t;
        }

        private void StopConnecting()
        {
            //IsConnecting = false;
            Status = ConnectionStatus.Disconnected;
            //BmUpdateRunner.Instance.Remove(this);
        }

        private void HandleDisconnect(TcpClient socketConnection)
        {
            if (serverPeer != null)
                serverPeer.Dispose();

            if (_socketConnection != socketConnection)
                return;

            _isConnectionPending = false;

            //BmUpdateRunner.Instance.Remove(this);

            Status = ConnectionStatus.Disconnected;
            IsConnected = false;
            _socketConnection = null;
            //_socketId = -1; //EMIL Fix

            if (serverPeer != null)
            {
                serverPeer.SetIsConnected(false);
                serverPeer.NotifyDisconnectEvent();
            }

            PeerDisconnected?.Invoke(serverPeer);
        }

        private void HandleData(int connectionId, int channelId, int receivedSize, byte error)
        {
            if (serverPeer == null)
                return;

            serverPeer.HandleDataReceived(_msgBuffer);
        }

        private void HandleConnect(TcpClient socketConnection)
        {
            if (_socketConnection != socketConnection)
                return;

            _isConnectionPending = false;

            //IsConnecting = false;
            IsConnected = true;

            Status = ConnectionStatus.Connected;

            serverPeer = new PeerTCP(socketConnection);
            serverPeer.SetIsConnected(true);
            PeerConnected?.Invoke(serverPeer);
        }
    }
*/
}