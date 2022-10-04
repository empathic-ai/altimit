namespace UnityEngine.Networking
{
    /// <summary>
    ///   <para>The details of a network message received by a client or server on a network connection.</para>
    /// </summary>
    public class NetworkMessage
    {
        /// <summary>
        ///   <para>The size of the largest message in bytes that can be sent on a NetworkConnection.</para>
        /// </summary>
        public const int MaxMessageSize = 65535;
    }
}