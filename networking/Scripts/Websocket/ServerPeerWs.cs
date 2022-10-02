using Altimit.Networking;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
#if UNITY_64
using WebSocketSharp;
using WebSocketSharp.Server;
#else
using Godot;
#endif

namespace Altimit.Networking
{
#if GODOT
    public class ServerPeerWs : ISocketPeer
    {
        public ILogger Logger { get; set; }
        public Action<ISocketPeer> Connected { get; set; }
        public Action<ISocketPeer> Disconnected { get; set; }
        public Action<byte[]> OnBytesReceived { get; set; }
        WebSocketServer ws;
        int peerID;

        public ServerPeerWs(WebSocketServer ws, int peerID)
        {
            this.ws = ws;
            this.peerID = peerID;
            ws.DataReceived += OnMessage;
        }

        public void SendBytes(byte[] data, bool isReliable = true)
        {
            ws.GetPeer(peerID).PutPacket(data);
        }

        void OnMessage(long _peerID)
        {
            if (peerID == (int)_peerID)
            {
                var bytes = ws.GetPacket();
                OnBytesReceived?.Invoke(bytes);
            }
        }

        public void Disconnect()
        {
            ws.DisconnectPeer(peerID);
            Disconnected?.Invoke(this);
        }
    }
#else
    public class ServerPeerWs : WebSocketBehavior, ISocketPeer
    {
        public ILogger Logger { get; set; }
        public Action<ISocketPeer> Connected { get; set; }
        public Action<ISocketPeer> Disconnected { get; set; }
        public Action<byte[]> OnBytesReceived { get; set; }

        public ServerPeerWs()
        {
            IgnoreExtensions = true;
        }

        protected override void OnOpen()
        {
            Updater.Instance.OnNextUpdate(() =>
            {
                Connected?.Invoke(this);
            });
        }

        protected override void OnClose(CloseEventArgs e)
        {
            Updater.Instance.OnNextUpdate(() => Disconnect());
        }

        protected override void OnError(WebSocketSharp.ErrorEventArgs e)
        {
            OS.Logger.LogError(e.Message);
        }

        protected override void OnMessage(MessageEventArgs e)
        {
            Updater.Instance.OnNextUpdate(() => OnBytesReceived?.Invoke(e.RawData));
        }

        public void SendBytes(byte[] data, bool isReliable = true)
        {
            if (Sessions.IDs.Contains(ID))
                Send(data);
        }

        public void Disconnect()
        {
            if (Sessions.IDs.Contains(ID))
            {
                Sessions.CloseSession(ID);
                Disconnected?.Invoke(this);
            }
        }
    }
#endif
}
