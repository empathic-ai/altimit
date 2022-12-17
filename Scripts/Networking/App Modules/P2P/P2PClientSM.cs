using Altimit;
using Altimit.Networking;
using Altimit.Serialization;
using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using System.Linq;
using System.Threading;

namespace Altimit.Networking
{
    public class P2PClientSM : SessionModule<P2PClientAM, IP2PServerSM>, IP2PClientSM
    {
        public string SessionKey;
        //public Guid PeerAppID;

        P2PClientAM P2PClientApp => App.Get<P2PClientAM>();
        //public IP2PServerSessionModule ServerSession => peerSession.GetModule<IP2PServerSessionModule>();
        public TaskCompletionSource<IP2PServerSM> ConnectedSource;

        public override async Task OnAdded()
        {
        }

        public async void StartConnectionToPeer(Guid peerAppID)
        {
            if (OS.LogP2P)
                Logger.Log($"Starting connection to peer with an app ID of {peerAppID}...");
            var peer = P2PClientApp.ServerSocketWebRTC.GetPeer(peerAppID);
            peer.CreateDataChannel();
            await App.Get<P2PClientAM>().SessionAdded.AddOrGetTask(peerAppID, TimeSpan.FromSeconds(5));
            if (OS.LogP2P)
                Logger.Log($"Connected to peer with an app ID of {peerAppID}!");
        }

        //client 1\fa0f
        //client 0\df

        public virtual async Task<SelfSession> ConnectToPeer(Guid peerAppID)
        {
            if (peerAppID == Guid.Empty)
                OS.LogError("Tried to connect to a peer without providing a peer ID!");

            if (OS.LogP2P)
                Logger.Log($"Connecting to peer with an app ID of {peerAppID}...");

            //OS.Log("PEER SESSION: " + peerSessionType.GenericTypeArguments[0].Name.ToString());
            SelfSession session;
            if (!P2PClientApp.TryGetSession(peerAppID, out session))
            {
                //if (OS.LogP2P)
                //    Logger.Log($"Creating offer for peer app {peerAppID}...");
                    
                Peer.ConnectToPeer(peerAppID);
                session = await App.Get<P2PClientAM>().SessionAdded.AddOrGetTask(peerAppID, TimeSpan.FromSeconds(5));


                // P2PClientApp.ServerSocketWebRTC.GetPeer(peerAppID);
                /*
                                    if (peer.IsPeerHandling || peer.IsSelfHandling)
                                    {
                                        session = await App.Get<P2PClientAM>().SessionAdded.AddOrGetTask(peerAppID);
                                    }
                                    else
                                    {
                */
                // If no one is handling the connection, handle it here
                //P2PClientApp.ServerSocketWebRTC.GetPeer(peerAppID);

                /*.IsSelfHandling = true;

                var offerSDP = await peer.CreateOffer();

                if (OS.LogP2P)
                    Logger.Log($"Sending to peer app {peerAppID}...");

                // After sending an offer to the peer, check if it has also initiated a connection and wants this peer to stop
                // If so, stop handling the connection here and just await the completion of the connection
                var sdps = await Peer.RelayOffer(peerAppID, offerSDP);
                if (sdps.IsError())
                {
                    session = await App.Get<P2PClientAM>().SessionAdded.AddOrGetTask(peerAppID);
                }
                else
                {
                    if (OS.LogP2P)
                        Logger.Log($"Received response from app {peerAppID}...");

                    var answer = await peer.CreateAnswer(sdps.Value.Item2);

                    await Peer.RelayAnswer(peerAppID, answer);

                    if (OS.LogP2P)
                        Logger.Log($"Relayed answer to peer app {peerAppID}...");

                    session = await P2PClientApp.ReceiveAnswer(peerAppID, sdps.Value.Item1);
                    await Task.Delay(1000);

                    if (OS.LogP2P)
                        Logger.Log($"Finished connecting to peer app {peerAppID}.");
                }
                */
                // }
            }
            return session;
        }

        public virtual async Task<T> Connect<T>(Guid peerAppID) where T : ISessionModule
        {
            var session = await ConnectToPeer(peerAppID);
            return await session.Get<ReplicationSM>().Connect<T>();
        }

        public virtual async Task<Response<WebRTCSessionDescription>> ReceivePeerOffer(Guid peerAppID,
            WebRTCSessionDescription desc, CancellationToken token = default)
        {
            /*
            if (P2PClientApp.ServerSocketWebRTC.GetPeer(peerAppID).IsSelfHandling)
            {
                if (App.Get<BaseAM>().AppID.CompareTo(peerAppID) > 0)
                {
                    // Is self handling and disallow peer handling
                    return new Response<(string, string)>((null, null), "Already initiated by peer with higher priority!");
                } else
                {
                    P2PClientApp.ServerSocketWebRTC.GetPeer(peerAppID).IsSelfHandling = false;
                    P2PClientApp.ServerSocketWebRTC.GetPeer(peerAppID).IsPeerHandling = true;
                }
            }
            */

            //if (OS.LogP2P)
            //    Logger.Log($"Received peer offer from app {peerAppID}. Creating answer...");
            var answerSDP = await P2PClientApp.ServerSocketWebRTC.GetPeer(peerAppID).CreateAnswer(desc, token);

            //if (OS.LogP2P)
            //    Logger.Log($"Creating offer for app {peerAppID}...");
            //var offerSDP = await P2PClientApp.ServerSocketWebRTC.GetPeer(peerAppID).CreateOffer();
            //if (OS.LogP2P)
            //    Logger.Log($"Sending answer and offer to app {peerAppID}.");
            return new Response<WebRTCSessionDescription>(answerSDP);
        }

        public virtual void ReceivePeerCandidate(Guid peerAppID, string candidate, string sdpMid, int sdpMLineIndex)
        {
            P2PClientApp.ReceivePeerCandidate(peerAppID, candidate, sdpMid, sdpMLineIndex);
        }

        public async Task<string[]> GetModuleTypeNames()
        {
            var moduleTypeNames = App.GetSessionModuleInterfaceNames();
            moduleTypeNames.Where(x=>x != typeof(IP2PClientSM).GetNativeTypeName());
            return moduleTypeNames.ToArray();
        }

        public async void GetSessionKey()
        {
        //    SessionKey = await PeerSession.GetSessionKey();
        }

        public async Task ReceivePeerAnswer(Guid peerAppID, WebRTCSessionDescription desc)
        {
            await P2PClientApp.ServerSocketWebRTC.GetPeer(peerAppID).ReceiveAnswer(desc);
        }
    }
}
