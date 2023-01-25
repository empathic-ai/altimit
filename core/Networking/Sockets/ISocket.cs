using System;

namespace Altimit.Networking
{
    public interface ISocket
    {
        /// <summary>
        /// Invoked, when a client connects to this socket
        /// </summary>
        Action<IPeerSocket> PeerConnected { get; set; }

        /// <summary>
        /// Invoked, when client disconnects from this socket
        /// </summary>
        Action<IPeerSocket> PeerDisconnected { get; set; }
    }
}

