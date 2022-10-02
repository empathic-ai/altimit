using System;

namespace Altimit.Networking
{
    public delegate void PeerActionHandler(ISocketPeer peer);

    public interface IServerSocket : ISocket
    {
        ILogger Logger { get; set; }
        /// <summary>
        /// Opens the socket and starts listening to a given port
        /// </summary>
        /// <param name="port"></param>
        void Listen(int port = default);
        /// <summary>
        /// Stops listening
        /// </summary>
        void Stop();
    }
}