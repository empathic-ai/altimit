using System;

namespace Altimit.Networking
{
    public interface ISocket
    {
        /// <summary>
        /// Invoked, when a client connects to this socket
        /// </summary>
        Action<ISocketPeer> PeerConnected { get; set; }

        /// <summary>
        /// Invoked, when client disconnects from this socket
        /// </summary>
        Action<ISocketPeer> PeerDisconnected { get; set; }
    }
}

