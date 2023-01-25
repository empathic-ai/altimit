using Altimit.Networking;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
#if LEGACY_GODOT
using Godot;
#else
using WebSocketSharp;
using WebSocketSharp.Server;
#endif

namespace Altimit.Networking
{
#if LEGACY_GODOT
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
            Godot.
            this.ws = ws;
            this.peerID = peerID;
            // TODO: Add back in
            //ws.DataReceived += OnMessage;
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
    public class ServerPeerWs : WebSocketBehavior, IPeerSocket
    {
        public ILogger Logger { get; set; }
        public Action<IPeerSocket> Connected { get; set; }
        public Action<IPeerSocket> Disconnected { get; set; }
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
