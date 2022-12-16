using Altimit.Networking;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
#if GODOT
using Godot;
#else
using WebSocketSharp;
#endif

namespace Altimit.Networking
{
    public class ClientPeerWs : ISocketPeer
    {
        public ILogger Logger { get; set; }
        public Action<ISocketPeer> Connected { get; set; }
        public Action<ISocketPeer> Disconnected { get; set; }
        public Action<byte[]> OnBytesReceived { get; set; }
#if GODOT
        WebSocketClient ws;

        public ClientPeerWs(WebSocketClient ws)
        {
            this.ws = ws;

            throw new NotImplementedException();
            //ws.ConnectionSucceeded += OnOpen;
            //ws.DataReceived += OnMessage;
        }

        private void OnOpen()
        {
            Updater.Instance.OnNextUpdate(() => Connected?.Invoke(this));
        }

        private void OnMessage()
        {
            Updater.Instance.OnNextUpdate(() => OnBytesReceived?.Invoke(ws.GetPacket()));
        }
        public void Disconnect()
        {
            ws.DisconnectFromHost();
        }

        public void SendBytes(byte[] bytes, bool isReliable = true)
        {
            ws.PutPacket(bytes);
        }
#else
        WebSocket ws;
        
        public ClientPeerWs(WebSocket ws) {
            this.ws = ws;

            ws.Log.Output += Output;
            ws.OnError += OnError;
            ws.OnOpen += OnOpen;
            ws.OnClose += OnClose;
            ws.OnMessage += OnMessage;
        }

        void Output(LogData logData, string text)
        {
            OS.Log(logData.Message);
            if (logData.Level.Equals(LogLevel.Error) || logData.Level.Equals(LogLevel.Fatal))
            {
                OS.LogError(logData.Message);
            }
        }

        private void OnError(object sender, ErrorEventArgs e)
        {
            OS.Log("ERROR");
            Updater.Instance.OnNextUpdate(() => OS.Logger.LogError(new Exception(e.Message, e.Exception)));
            //OS.Logger.LogError(new Exception(e.Message, e.Exception));
        }

        private void OnMessage(object sender, MessageEventArgs e)
        {
            Updater.Instance.OnNextUpdate(() => OnBytesReceived?.Invoke(e.RawData));
        }

        private void OnClose(object sender, CloseEventArgs e)
        {
            Updater.Instance.OnNextUpdate(() => Disconnected?.Invoke(this));
        }

        private void OnOpen(object sender, EventArgs e)
        {
            Updater.Instance.OnNextUpdate(() => Connected?.Invoke(this));
        }

        public void Disconnect()
        {
            //if (ws.IsAlive)
            ws.Close();
            ws.Log.Output -= Output;
            ws.OnError -= OnError;
            ws.OnOpen -= OnOpen;
            ws.OnClose -= OnClose;
            ws.OnMessage -= OnMessage;
        }
        
        public void SendBytes(byte[] bytes, bool isReliable = true)
        {
            //if (ws.IsAlive)
                ws.Send(bytes);
        }
#endif

    }
}
