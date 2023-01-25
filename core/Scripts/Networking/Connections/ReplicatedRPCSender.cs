using Altimit;
using Altimit.Networking;
using Altimit.Serialization;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.ExceptionServices;
using System.Threading;
using System.Threading.Tasks;
using TaskExtensions = Altimit.TaskExtensions;

namespace Altimit.Networking
{
    /*
    // Routes messages to a given socket, but ensures replication and dereferences when sending
    public class ReplicatedRPCSender : RPCPeer
    {
        RuntimeDatabase instanceDatabase => app.InstanceDB;
        IReplicationService replicationService;

        public ReplicatedRPCSender(IReplicationService replicationService, ISocketPeer peer, ILogger logger) : base(peer, logger)
        {
            this.replicationService = replicationService;
        }

        void Send(string sessionModuleTypeName, string methodName, string[] typeNames, object[] args)
        {

        }


    }
    */
}
