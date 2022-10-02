namespace Altimit.Networking
{
    [AType]
    public enum ServerType
    {
        WebSocketServer,
        P2PServer
    }

    [AType]
    public struct ConnectionSettings
    {
        [AProperty]
        public ServerType Type;
        [AProperty]
        public string IP;
        [AProperty]
        public int Port;

        public ConnectionSettings(ServerType type, string ip, int port)
        {
            Type = type;
            IP = ip;
            Port = port;
        }
    }
}