using Altimit.Networking;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Altimit
{
    public class PeerSessionModule<T> :  Module<ISession, ISessionModule>, ISessionModule<T> where T : ISessionModule
    {
        public T Peer => peerSession.Peer.Get<T>();
        protected ProxyPeerSession peerSession => (ProxyPeerSession)container;
    }
}
