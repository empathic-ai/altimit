using System;

namespace Altimit.Networking
{

    /// <summary>
    ///     Represents connection peer
    /// </summary>
    public interface ISocketPeer
    {
        ILogger Logger { get; set; }

        Action<ISocketPeer> Connected { get; set; }

        Action<ISocketPeer> Disconnected { get; set; }

        Action<byte[]> OnBytesReceived { get; set; }

        void Disconnect();

        void SendBytes(byte[] bytes, bool isReliable = true);
    }
}