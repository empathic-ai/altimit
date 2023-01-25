using System.Net.Sockets;

namespace Altimit.Networking
{
    /// <summary>
    ///     Unet low level api based peer implementation
    /// </summary>
    public class TCPPeer : PeerBase
    {
        private readonly int _connectionId;
        private readonly int _socketId;
        private bool _isConnected;
        private TcpClient _socketConnection;

        public TCPPeer(TcpClient socketConnection)
        {
            _socketConnection = socketConnection;
        }

        /// <summary>
        ///     True, if connection is stil valid
        /// </summary>
        public override bool IsConnected
        {
            get { return _isConnected; }
        }

        /// <summary>
        ///     Sends a message to peer
        /// </summary>
        /// <param name="message">Message to send</param>
        /// <param name="deliveryMethod">Delivery method</param>
        /// <returns></returns>
        public override void SendBytes(byte[] bytes, bool isReliable = true)
        {
            if (!IsConnected)
                return;

            // TODO update this monstrosity
            //int channelId = BarebonesTopology.GetChannel(deliveryMethod);
            var stream = _socketConnection.GetStream();

            stream.Write(bytes, 0, bytes.Length);
            /*
            byte error;  
            NetworkTransport.Send(_socketId, _connectionId, channelId, bytes, bytes.Length, out error);

            var networkError = (NetworkError)error;
            if (networkError != NetworkError.Ok)
            {
                Logs.Debug(deliveryMethod.ToString());
                Logs.Error("A network error occured: " + networkError);
            }*/
        }

        /// <summary>
        ///     Force disconnect
        /// </summary>
        /// <param name="reason"></param>
        public override void Disconnect()
        {
            //byte error;
            _socketConnection.Close();
            //NetworkTransport.Disconnect(_socketId, _connectionId, out error);
        }

        public void SetIsConnected(bool status)
        {
            _isConnected = status;
        }
    }
}