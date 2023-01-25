using System;
using System.Threading;
using System.Threading.Tasks;

namespace Altimit.Networking
{
    public interface ISocketClient : ISocket
    {
        /// <summary>
        /// Peer, to which we have connected
        /// </summary>
        IPeerSocket ServerPeer { get; }

        bool IsConnected { get; }

        void Connect();

        /// <summary>
        /// Starts connecting to another socket
        /// </summary>
        /// <param name="ip"></param>
        /// <param name="port"></param>
        /// <returns></returns>
        Task<IPeerSocket> ConnectAsync();

        /// <summary>
        /// Closes socket connection
        /// </summary>
        void Disconnect();
    }
}