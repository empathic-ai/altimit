using System;

namespace Altimit.Networking
{

    /// <summary>
    ///     Represents connection peer
    /// </summary>
    public interface IPeerSocket
    {
        ILogger Logger { get; set; }

        Action<IPeerSocket> Connected { get; set; }

        Action<IPeerSocket> Disconnected { get; set; }

        Action<byte[]> OnBytesReceived { get; set; }

        void Disconnect();

        void SendBytes(byte[] bytes, bool isReliable = true);
    }
}