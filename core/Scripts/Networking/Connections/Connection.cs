using Altimit.Networking;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Altimit
{
    /*
    public class Connection<TSelf, TPeer> where TSelf : IConnectionSingleton where TPeer : IConnectionSingleton
    {
        public TSelf Self;
        public TPeer Peer;

        public Connection(ISocketPeer socketPeer)
        {
            Self = Activator.CreateInstance<TSelf>();
            Peer = DynamicMethodCallerFactory.CreateNotifyMethodCalledOfType<TPeer>();

            ((INotifyMethodCalled)Peer).MethodCalled += OnPeerMethodCalled;
        }

        public Connection(TSelf self, ISocketPeer socketPeer)
        {
            Self = self;
            Peer = DynamicMethodCallerFactory.CreateNotifyMethodCalledOfType<TPeer>();

            ((INotifyMethodCalled)Peer).MethodCalled += OnPeerMethodCalled;
        }

        private void OnPeerMethodCalled(object sender, MethodCalledEventArgs e)
        {
            
        }
    }
    */
}
