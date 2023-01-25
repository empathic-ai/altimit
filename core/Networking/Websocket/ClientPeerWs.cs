using Altimit.Networking;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net.Sockets;
#if GODOT
using Godot;
#else
using WebSocketSharp;
#endif

namespace Altimit.Networking
{
    public class ClientPeerWs : IPeerSocket
#if GODOT
, IUpdateable
#endif
    {
        public ILogger Logger { get; set; }
        public Action<IPeerSocket> Connected { get; set; }
        public Action<IPeerSocket> Disconnected { get; set; }
        public Action<byte[]> OnBytesReceived { get; set; }
#if GODOT
        WebSocketPeer ws;
        bool isConnected = false;

        public ClientPeerWs(WebSocketPeer ws)
        {
            this.ws = ws;
            //ws.ConnectionSucceeded += OnOpen;
            //ws.DataReceived += OnMessage;
            Updater.Instance.AddUpdateable(this);

            
        }

        public void Disconnect()
        {
            ws.Dispose();
        }

        // TODO: pass parameter for writing text vs bytes
        public void SendBytes(byte[] bytes, bool isReliable = true)
        {
            ws.Send(bytes, WebSocketPeer.WriteMode.Text);

            //ws.SendText(System.Text.Encoding.UTF8.GetString(bytes));
            //ws.Send(bytes, WebSocketPeer.WriteMode.Text);
            //var error = ws.PutPacket(bytes);
            //if (error != Error.Ok)
            //    OS.LogError(error);
        }

        public void Update()
        {
            //if (isConnected)
            ws.Poll();

            var state = ws.GetReadyState();
            if (state == WebSocketPeer.State.Open)
            {
                if (!isConnected)
                {
                    isConnected = true;
                    Connected?.Invoke(this);
                }
                while (ws.GetAvailablePacketCount() > 0) {
                    OnBytesReceived?.Invoke(ws.GetPacket());
                }
            }
            else if (state == WebSocketPeer.State.Closing)
            {

            } else if (state == WebSocketPeer.State.Closed)
            {
                isConnected = false;
                //var code = ws.GetCloseCode();
                //var reason = ws.GetCloseReason();
                //OS.LogError($"WebSocket closed with code: {code}, reason {reason}. Clean: {code != -1}");
            }
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

        private void OnError(object sender, WebSocketSharp.ErrorEventArgs e)
        {
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
