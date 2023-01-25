using Altimit;
using System;
using Altimit.Networking;

namespace Altimit.Networking
{
    [AType]
    public class ReplicationClient : IReplicationClient
    {
        public IReplicationServer Peer { get; set; }
    }
}
