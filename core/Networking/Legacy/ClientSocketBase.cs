//using Byn.Net;
//using Byn.Net.Native;
//using WebRtcCSharp;

//using SimplePeerConnectionM;
/*
using WebRtc;
using WebRtc.Native;
using WebRtc.Webrtc;
*/
//using SimplePeerConnectionM;

namespace Altimit.Networking
{
    /*
    public class RTCCreateSessionDescriptionObserver : CreateSessionDescriptionObserver
    {
        public override int AddRef()
        {
            throw new NotImplementedException();
        }

        public override int Release()
        {
            throw new NotImplementedException();
        }

        public override void OnSuccess(SessionDescriptionInterface desc)
        {
            throw new NotImplementedException();
        }

        public override void OnFailure(string error)
        {
            throw new NotImplementedException();
        }
    }

    public class RTCConductor : Conductor
    {
        public override void OnSuccess()
        {
            throw new NotImplementedException();
        }

        public override void OnSuccess(SessionDescriptionInterface desc)
        {
            throw new NotImplementedException();
        }
    }

    public class RTCPeerConnectionFactory : PeerConnectionFactory
    {
        public override int AddRef()
        {
            throw new NotImplementedException();
        }

        public override int Release()
        {
            throw new NotImplementedException();
        }
    }

    public unsafe class RTCPeerConnection : PeerConnection {

        public RTCPeerConnection(void* native, bool skipVTables = false) : base(native, skipVTables)
        {
        }

        public RTCPeerConnection(PeerConnectionFactory factory) : base(factory)
        {
        }

        public RTCPeerConnection(PeerConnection _0) : base(_0)
        {
        }

        public override int AddRef()
        {
            throw new NotImplementedException();
        }

        public override int Release()
        {
            throw new NotImplementedException();
        }
    }
    */
    /*
    public abstract class ClientSocketBase : BaseSocket, IClientSocket, IUpdateable
    {
        public const bool IsDebugging = false;

        public virtual IPeer ServerPeer { get; set; }
        public virtual Dictionary<int, IPeer> Peers { get; set; }
        public virtual ConnectionStatus Status { get; set; }
        public virtual bool IsConnected { get; set; }
        public virtual bool IsConnecting { get; set; }
        public virtual string ConnectionIp { get; set; }
        public virtual int ConnectionPort { get; set; }
        public virtual event Action<IClientSocket> Connected;
        public virtual event Action<IClientSocket> Disconnected;
        public virtual event Action<ConnectionStatus> StatusChanged;

        public int ID { get; set; }

        Dictionary<int, IPeer> relayedPeers { get; }


        public static bool RethrowExceptionsInEditor = true;

   
        
        public virtual void HandleMessage(int methodID, byte[] bytes, IPeer peer)
        {
            IncommingMessageHandler handler;
            _handlers.TryGetValue(methodID, out handler);

            if (handler != null)
                handler.Invoke(methodID, bytes, peer);
            else
            {
                OS.Log("Connection is missing a handler. OpCode: " + methodID);
                //message.Respond(ResponseStatus.Error);
            }
            
            try
            {

            }
            catch (Exception e)
            {
#if UNITY_64_EDITOR
                if (RethrowExceptionsInEditor)
                    throw;
#endif

                //Logs.Error("Failed to handle a message. OpCode: " + message.MethodID);
                OS.Log(e.ToString());

                if (!message.IsExpectingResponse)
                    return;

                try
                {
                    message.Respond(ResponseStatus.Error);
                }
                catch (Exception exception)
                {
                    OS.Log(exception.ToString());
                }
            }
            
        }

        protected ClientSocketBase()
        {
            //EditorApplication.playModeStateChanged += OnPlayModeStateChanged;
            //_handlers = new Dictionary<int, IPacketHandler>();

            //this.On(OpCode_Internal.ServerAck, API.Delegate<int>(OnServerAck));
        }

        #region abstract ClientSocket methods
        public abstract void Connect(string ip, int port, int timeoutMillis);

        public void Connect(string ip, int port)
        {
            Connect(ip, port, 5000);
        }

        public abstract void Reconnect();

        public abstract void Disconnect();
        #endregion

        #region Serializable Messages
        public void SendBytes(byte[] bytes, ResponseCallback responseCallback, DeliveryMethod deliveryMethod)
        {
            ServerPeer.SendBytes(bytes, responseCallback, deliveryMethod);
        }
        #endregion
        
        public virtual void Update()
        {
            //while(dispatchedActions.Count > 0)
              //  dispatchedActions.Dequeue()();
        }

    }
    */
}