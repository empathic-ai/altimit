
using System;
using System.Diagnostics;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
#if UNITY_64
using Cysharp.Threading.Tasks;
using Unity.WebRTC;
#elif GODOT
using Godot;
#endif
using UnityEngine;

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

    public class PeerWebRTC : ISocketPeer
    {
        Guid appID;
        public Guid PeerAppID { get; set; }

        Action<WebRTCIceCandidate> onIceCandidate;
        const string reliableDataChannelLabel = "reliable";
        public TaskCompletionSource<bool> CompletionSource = new TaskCompletionSource<bool>();
        public Action<ISocketPeer> Connected { get; set; }
        public Action<ISocketPeer> Disconnected { get; set; }
        public Action<byte[]> OnBytesReceived { get; set; }
        public ILogger Logger { get; set; }

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
#endif

        public PeerWebRTC(Guid appID, Guid peerAppID, Action<PeerWebRTC> onNegotiationNeeded, Action<WebRTCIceCandidate> onIceCandidate, ILogger logger)
        {
            this.appID = appID;
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
            peerConnection = new WebRTCPeerConnection();
            peerConnection.DataChannelReceived += ReceiveChannelCallback;
            peerConnection.IceCandidateCreated += (media, index, name) => OnIceCandidate(new WebRTCIceCandidate(name, media, (int?)index));
#endif
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
#endif
        }

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
#endif
            return new WebRTCSessionDescription() { };
        }

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
#endif
            return new WebRTCSessionDescription();
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
#endif
        }

        public void CreateDataChannel()
        {
            //unreliableDataChannel = peerConnection.CreateDataChannel(unreliableDataChannelLabel,
            //    new RTCDataChannelInit() { maxRetransmits = 0 });
#if UNITY_64
            reliableDataChannel = peerConnection.CreateDataChannel(reliableDataChannelLabel,
                new RTCDataChannelInit() { ordered = false });

            reliableDataChannel.OnOpen += OnDataChannelOpen;
            reliableDataChannel.OnClose += OnDataChannelClose;
            reliableDataChannel.OnMessage += OnMessage;
#endif
        }
        private void OnIceCandidate(WebRTCIceCandidate candidate)
        {
            if (OS.LogWebRTC)
                Logger.Log($"Going to send candidate to peer {PeerAppID}: {candidate.Candidate}, {candidate.SdpMid}, {candidate.SdpMLineIndex}.");
            onIceCandidate(candidate);
        }

        private async void OnDataChannelOpen()
        {
            #if UNITY_64 || GODOT
            if (OS.LogWebRTC)
                Logger.Log("Opened data channel!");

            // Important delay to make sure both parties are 100% ready
            // May be better way of handling this but this fix appears to work reliably
            await Task.Delay(2000);

            if (reliableDataChannel == null || CompletionSource.Task.IsCompleted) //|| !ConnectionState.HasFlag(RTCIceConnectionState.Connected)
                return;

            Connected?.Invoke(this);
            CompletionSource.SetResult(true);
            #endif
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
            throw new NotImplementedException();
        }

        public void SendBytes(byte[] bytes, bool isReliable = true)
        {
            throw new NotImplementedException();
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
#endif
    }
}