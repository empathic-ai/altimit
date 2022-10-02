using System;
using System.Threading;
using System.Threading.Tasks;

namespace Altimit.Networking
{
    public interface IClientSocket : ISocket
    {
        /// <summary>
        /// Peer, to which we have connected
        /// </summary>
        ISocketPeer ServerPeer { get; }

        bool IsConnected { get; }
        
        /// <summary>
        /// Starts connecting to another socket
        /// </summary>
        /// <param name="ip"></param>
        /// <param name="port"></param>
        /// <returns></returns>
        Task<ISocketPeer> ConnectAsync();

        /// <summary>
        /// Closes socket connection
        /// </summary>
        void Disconnect();
    }
}