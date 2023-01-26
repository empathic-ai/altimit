
using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
#if UNITY_64
using Cysharp.Threading.Tasks;
using Unity.WebRTC;
using UnityEngine;
#elif GODOT
using Godot;
#else
using WebrtcSharp;
#endif


namespace Altimit.Networking
{
    [AType]
    public enum WebRTCOfferType
    {
        Offer = 0,
        Pranswer = 1,
        Answer = 2,
        Rollback = 3
    }

    [AType]
    public struct WebRTCSessionDescription
    {
        [AProperty]
        public WebRTCOfferType Type { get; set; }
        [AProperty]
        public string SDP { get; set; }

        public WebRTCSessionDescription(WebRTCOfferType offerType, string sdp) {
            Type = offerType;
            SDP = sdp;
        }

        public WebRTCSessionDescription(string offerType, string sdp)
        {
            Type = ConvertOfferType(offerType);
            SDP = sdp;
        }

        public static WebRTCOfferType ConvertOfferType(string offerType)
        {
            switch (offerType.ToLower())
            {
                case "answer":
                    return WebRTCOfferType.Answer;
                case "offer":
                    return WebRTCOfferType.Offer;
                case "pranswer":
                    return WebRTCOfferType.Pranswer;
                default:    // rollback
                    return WebRTCOfferType.Rollback;
            }
        }

#if UNITY_64
        static RTCSdpType ConvertOfferType(WebRTCOfferType offerType) {
            return (RTCSdpType)Enum.ToObject(typeof(RTCSdpType), ((int)offerType));
        }

        static WebRTCOfferType ConvertOfferType(RTCSdpType offerType)
        {
            return (WebRTCOfferType)Enum.ToObject(typeof(WebRTCOfferType), ((int)offerType));
        }

        public static explicit operator WebRTCSessionDescription(RTCSessionDescription b) =>
            new WebRTCSessionDescription(ConvertOfferType(b.type), b.sdp);

        public static implicit operator RTCSessionDescription(WebRTCSessionDescription b) =>
            new RTCSessionDescription() { type = ConvertOfferType(b.Type), sdp = b.SDP };
#endif
    }

    public class WebRTCIceCandidate
    {
        public WebRTCIceCandidate(string candidate, string sdpMid, int? sdpMLineIndex)
        {
            Candidate = candidate;
            SdpMid = sdpMid;
            SdpMLineIndex = sdpMLineIndex;
        }

        public int? SdpMLineIndex { get; }
        public string SdpMid { get; }
        public string Candidate { get; }
    }

    public class WebRTCPeer : IPeerSocket, IUpdateable
    {
        public AID PeerAppID { get; set; }

        Action<WebRTCIceCandidate> onIceCandidate;
        const string reliableDataChannelLabel = "reliable";
        public TaskCompletionSource<bool> ConnectedTaskCompletionSource = new TaskCompletionSource<bool>();
        public Action<IPeerSocket> Connected { get; set; }
        public Action<IPeerSocket> Disconnected { get; set; }
        public Action<byte[]> OnBytesReceived { get; set; }
        public ILogger Logger { get; set; }
        public bool IsConnected = false;

#if UNITY_64
        RTCPeerConnection peerConnection;

        RTCDataChannel unreliableDataChannel;
        RTCDataChannel reliableDataChannel;

        public Action<RTCTrackEvent> onTrackEvent;
        public RTCIceConnectionState ConnectionState;
        public RTCPeerConnectionState PeerConnectionState;

        const string unreliableDataChannelLabel = "unreliable";

        public bool IsPeerHandling = false;
        public bool IsSelfHandling = false;
#elif GODOT
        WebRTCPeerConnection peerConnection;
        WebRTCDataChannel reliableDataChannel;
#else
        PeerConnection peerConnection;
#endif

        public WebRTCPeer(AID peerAppID, Action<WebRTCPeer> onNegotiationNeeded, Action<WebRTCIceCandidate> onIceCandidate, ILogger logger)
        {
            Logger = logger;
            PeerAppID = peerAppID;
            this.onIceCandidate = onIceCandidate;

#if UNITY_64
            RTCConfiguration config = default;
            config.iceServers = new RTCIceServer[]
            {
                new RTCIceServer { urls = new string[] { "stun:stun.l.google.com:19302" } }
            };

            // Setup connection handlers
            peerConnection = new RTCPeerConnection(ref config);

            peerConnection.OnConnectionStateChange = x =>
            {
                PeerConnectionState = x;
                if (PeerConnectionState.Equals(RTCPeerConnectionState.Failed) ||
                PeerConnectionState.Equals(RTCPeerConnectionState.Disconnected) || PeerConnectionState.Equals(RTCPeerConnectionState.Closed))
                {
                    reliableDataChannel = null;
                }
            };

            peerConnection.OnTrack += e => onTrackEvent?.Invoke(e);

            peerConnection.OnIceCandidate += x => OnIceCandidate(new WebRTCIceCandidate(x.Candidate, x.SdpMid, x.SdpMLineIndex));
            peerConnection.OnNegotiationNeeded += () => onNegotiationNeeded(this);

            peerConnection.OnIceConnectionChange += x => ConnectionState = x;

            peerConnection.OnDataChannel += ReceiveChannelCallback;

            var transceiver2 = peerConnection.AddTransceiver(TrackKind.Audio);
            transceiver2.Direction = RTCRtpTransceiverDirection.SendRecv;

            var transceiver1 = peerConnection.GetTransceivers().First();
            var error = transceiver1.SetCodecPreferences(RTCRtpSender.GetCapabilities(TrackKind.Audio).codecs);
            if (error != RTCErrorType.None)
                OS.LogError(error);
#elif GODOT


            var stunServerArr = new Godot.Collections.Array(new Variant[] { "stun:stun.l.google.com:19302" });
            var stunServerDict = new Godot.Collections.Dictionary();
            stunServerDict.Add("urls", stunServerArr);
            Godot.Collections.Dictionary RTCInitializer = new Godot.Collections.Dictionary();
            RTCInitializer.Add("iceServers", stunServerDict);

            peerConnection = new WebRTCPeerConnection();
            peerConnection.SessionDescriptionCreated += PeerConnection_SessionDescriptionCreated;
            peerConnection.IceCandidateCreated += (media, index, name) => OnIceCandidate(new WebRTCIceCandidate(name, media, (int?)index));


            peerConnection.Initialize(RTCInitializer);
            peerConnection.DataChannelReceived += ReceiveChannelCallback;
#else
            var factory = new PeerConnectionFactory();
            var configuration = new RTCConfiguration();
            peerConnection = factory.CreatePeerConnection(configuration);
#endif
            Updater.Instance.AddUpdateable(this);
        }

        public void ReceiveCandidate(string candidate, string sdpMid, int sdpMLineIndex)
        {
            if (OS.LogWebRTC)
                Logger.Log($"Adding ICE Candidate: {candidate}, {sdpMid}, {sdpMLineIndex}.");

#if UNITY_64
            var candidateInit = new RTCIceCandidateInit
            {
                candidate = candidate,
                sdpMid = sdpMid,
                sdpMLineIndex = sdpMLineIndex
            };

            var _candidate = new RTCIceCandidate(candidateInit);
            peerConnection.AddIceCandidate(_candidate);
#elif GODOT
            peerConnection.AddIceCandidate(sdpMid, sdpMLineIndex, candidate);
#endif
        }

        public async Task ConnectAsync()
        {

            Connect();
            await ConnectedTaskCompletionSource.Task;
        }

        //bool isBusy = false;
        TaskCompletionSource<WebRTCSessionDescription> offerTaskCompletionSource;
        TaskCompletionSource<WebRTCSessionDescription> answerTaskCompletionSource;

        // Step 1
        public async Task<WebRTCSessionDescription> CreateOffer()
        {

            if (OS.LogWebRTC)
                Logger.Log("Sending WebRTC offer.");

#if UNITY_64
            // Create offer and set description from result of offer
            var offerOptions = new RTCOfferAnswerOptions()
            {
                iceRestart = false
            };
            var createOfferOp = peerConnection.CreateOffer(ref offerOptions);
            await createOfferOp;
            if (createOfferOp.IsError)
            {
                Logger.LogError(createOfferOp.Error.message.ToString());
            }
            var localDescription = createOfferOp.Desc;
            var setLocalDescriptionOp = peerConnection.SetLocalDescription(ref localDescription);
            await setLocalDescriptionOp;
            if (setLocalDescriptionOp.IsError)
            {
                Logger.LogError(setLocalDescriptionOp.Error.message.ToString());
            }
            return (WebRTCSessionDescription)localDescription;
#elif GODOT
            offerTaskCompletionSource = new TaskCompletionSource<WebRTCSessionDescription>();
            var error = peerConnection.CreateOffer();
            if (error != Error.Ok)
            {
                Logger.LogError(error.ToString());
            }
            return await offerTaskCompletionSource.Task;
#else
            // Create offer and set description from result of offer
            var createOfferOp = peerConnection.CreateOffer(false);
            var localDescription = await createOfferOp;
            var setLocalDescriptionOp = peerConnection.SetLocalDescription("offer", localDescription);
            await setLocalDescriptionOp;
            return new WebRTCSessionDescription("offer", localDescription);
#endif
        }

#if GODOT
        private void PeerConnection_SessionDescriptionCreated(string type, string sdp)
        {
            var error = peerConnection.SetLocalDescription(type, sdp);
            if (error != Error.Ok)
            {
                Logger.LogError(error.ToString());
            }

            if (type == "offer")
            {
                offerTaskCompletionSource.SetResult(new WebRTCSessionDescription(type, sdp));
            }
            else if (type == "answer")
            {
                answerTaskCompletionSource.SetResult(new WebRTCSessionDescription(type, sdp));
            }
        }
#endif

        // Step 2
        public async Task<WebRTCSessionDescription> CreateAnswer(WebRTCSessionDescription desc, CancellationToken token)
        {
            token.ThrowIfCancellationRequested();

            if (OS.LogWebRTC)
                Logger.Log("Received WebRTC offer.");
#if UNITY_64
            var _desc = (RTCSessionDescription)desc;
            var setRemoteDescriptionOp = peerConnection.SetRemoteDescription(ref _desc);
            await setRemoteDescriptionOp;
            token.ThrowIfCancellationRequested();

            if (setRemoteDescriptionOp.IsError)
            {
                Logger.LogError(setRemoteDescriptionOp.Error.message.ToString());
            }


            var answerOptions = new RTCOfferAnswerOptions();
            var createAnswerOp = peerConnection.CreateAnswer(ref answerOptions);
            await createAnswerOp;
            token.ThrowIfCancellationRequested();

            if (createAnswerOp.IsError)
            {
                Logger.LogError(createAnswerOp.Error.message.ToString());
            }

            var localDescription = createAnswerOp.Desc;

            var setLocalDescriptionOp = peerConnection.SetLocalDescription(ref localDescription);
            await setLocalDescriptionOp;
            token.ThrowIfCancellationRequested();

            if (setLocalDescriptionOp.IsError)
            {
                Logger.LogError(setLocalDescriptionOp.Error.message.ToString());
            }

            return (WebRTCSessionDescription)localDescription;
#elif GODOT
            answerTaskCompletionSource = new TaskCompletionSource<WebRTCSessionDescription>();
            var error = peerConnection.SetRemoteDescription(desc.Type.ToString().ToLower(), desc.SDP);
            if (error != Error.Ok)
            {
                Logger.LogError(error.ToString());
            }

            var answer = await answerTaskCompletionSource.Task;
            error = peerConnection.SetLocalDescription(answer.Type.ToString().ToLower(), answer.SDP);
            if (error != Error.Ok)
            {
                Logger.LogError(error.ToString());
            }

            return answer;
#else
            var setRemoteDescriptionOp = peerConnection.SetRemoteDescription("remote", desc.SDP);
            await setRemoteDescriptionOp;
            var localDescription = await peerConnection.CreateAnswer();

            var setLocalDescriptionOp = peerConnection.SetLocalDescription("answer", localDescription);
            await setLocalDescriptionOp;

            return new WebRTCSessionDescription("answer", localDescription);
#endif
        }

        // Step 3
        public async Task ReceiveAnswer(WebRTCSessionDescription desc)
        {
            if (OS.LogWebRTC)
                Logger.Log("Received WebRTC answer.");
#if UNITY_64
            var _desc = (RTCSessionDescription)desc;
            var setRemoteDescriptionOp = peerConnection.SetRemoteDescription(ref _desc);
            await setRemoteDescriptionOp;
            if (setRemoteDescriptionOp.IsError)
            {
                Logger.LogError(setRemoteDescriptionOp.Error.message.ToString());
            }
#elif GODOT
            var error = peerConnection.SetRemoteDescription(desc.Type.ToString().ToLower(), desc.SDP);
            if (error != Error.Ok)
            {
                Logger.LogError(error.ToString());
            }
#endif
        }

        public void Connect()
        {
            //unreliableDataChannel = peerConnection.CreateDataChannel(unreliableDataChannelLabel,
            //    new RTCDataChannelInit() { maxRetransmits = 0 });
#if UNITY_64
            reliableDataChannel = peerConnection.CreateDataChannel(reliableDataChannelLabel,
                new RTCDataChannelInit() { ordered = false });

            reliableDataChannel.OnOpen += OnDataChannelOpen;
            reliableDataChannel.OnClose += OnDataChannelClose;
            reliableDataChannel.OnMessage += OnMessage;
#elif GODOT
            if (OS.LogWebRTC)
                Logger.Log($"Connecting to peer with an ID of {PeerAppID}.");

            var options = new Godot.Collections.Dictionary
            {
                { "ordered", false }
            };

            //peerConnection.Connect(new Callable(this, nameof(this.PeerConnection_SessionDescriptionCreated)));
            reliableDataChannel = peerConnection.CreateDataChannel(reliableDataChannelLabel, options);
#endif
        }

        private async void OnDataChannelOpen()
        {
#if UNITY_64 || GODOT
            if (OS.LogWebRTC)
                Logger.Log("Opened data channel!");

            // Important delay to make sure both parties are 100% ready
            // May be better way of handling this but this fix appears to work reliably
            await Task.Delay(2000);

            if (reliableDataChannel == null || ConnectedTaskCompletionSource.Task.IsCompleted) //|| !ConnectionState.HasFlag(RTCIceConnectionState.Connected)
                return;

            IsConnected = true;
            Connected?.Invoke(this);
            ConnectedTaskCompletionSource.SetResult(true);
#endif
        }

        private void OnIceCandidate(WebRTCIceCandidate candidate)
        {
            if (OS.LogWebRTC)
                Logger.Log($"Going to send candidate to peer {PeerAppID}: {candidate.Candidate}, {candidate.SdpMid}, {candidate.SdpMLineIndex}.");
            onIceCandidate(candidate);
        }



#if UNITY_64
        private void ReceiveChannelCallback(RTCDataChannel channel)
        {
            if (channel.Label == reliableDataChannelLabel && reliableDataChannel == null)
            {
                reliableDataChannel = channel;
                reliableDataChannel.OnMessage += OnMessage;
                reliableDataChannel.OnOpen += OnDataChannelOpen;
                reliableDataChannel.OnClose += OnDataChannelClose;

                OnDataChannelOpen();
            }
        }

        public void AddTrack(MediaStreamTrack track, MediaStream stream = null)
        {
            peerConnection.AddTrack(track, stream);
        }

        public void RemoveTrack(RTCRtpSender sender)
        {
            peerConnection.RemoveTrack(sender);
        }

        private void OnDataChannelClose()
        {
            reliableDataChannel = null;
            //if (isLogging)
            //    OS.Log("Closed WebRTC voice channel.");
            Disconnected?.Invoke(this);
        }

        void OnMessage(byte[] bytes)
        {
            OnBytesReceived?.Invoke(bytes);
        }

        public void SendBytes(byte[] bytes, bool isReliable = true)
        {
            try
            {
                if (isReliable)
                {
                    reliableDataChannel?.Send(bytes);
                } else
                {
                    unreliableDataChannel?.Send(bytes);
                }
            } catch (ObjectDisposedException e)
            {
                //OS.Log(ConnectionState);
                //OS.Log(PeerConnectionState);
                //OS.LogError(e);
            }
        }
        
        public void Disconnect()
        {
            Logger.Log("CLOSED");

            reliableDataChannel?.Close();
            reliableDataChannel.OnMessage -= OnMessage;
            reliableDataChannel.OnClose -= OnDataChannelClose;
            reliableDataChannel.OnOpen -= OnDataChannelOpen;
            reliableDataChannel = null;

            peerConnection?.Dispose();
            peerConnection = null;
        }
        
        public void Update()
        {
        }
        /*
        void ThrowIfRemoteConnected()
        {
            if (RemoteConnectionState.Equals(RTCIceConnectionState.Completed))
                throw new P2PConnectedException();
        }*/
#elif GODOT
        private void ReceiveChannelCallback(WebRTCDataChannel channel)
        {
            if (channel.GetLabel() == reliableDataChannelLabel && reliableDataChannel == null)
            {
                reliableDataChannel = channel;
                
                //reliableDataChannel.OnMessage += OnMessage;
                //reliableDataChannel.OnOpen += OnDataChannelOpen;
                //reliableDataChannel.OnClose += OnDataChannelClose;

                OnDataChannelOpen();
            }
        }

        public void Disconnect()
        {
            peerConnection.Dispose();
        }

        public void Update()
        {

            if (reliableDataChannel != null)
            {
                peerConnection.Poll();
                if (reliableDataChannel.GetReadyState() == WebRTCDataChannel.ChannelState.Open && reliableDataChannel.GetAvailablePacketCount() > 0)
                {
                    OnBytesReceived?.Invoke(reliableDataChannel.GetPacket());
                }
            }
        }

        public void SendBytes(byte[] bytes, bool isReliable = true)
        {
            reliableDataChannel.PutPacket(bytes);
        }
#else
        public void Disconnect()
        {
            throw new NotImplementedException();
        }

        public void SendBytes(byte[] bytes, bool isReliable = true)
        {
            throw new NotImplementedException();
        }

        public void Update()
        {
            throw new NotImplementedException();
        }
#endif
    }
}