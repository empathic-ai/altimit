using Altimit.Networking;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Altimit;
using Altimit.Serialization;

namespace Altimit.Networking
{
    [AType]
    public class ReplicationClientSM : SessionModule<ReplicationClientAM, IReplicationServerSM>, IReplicationClientSM
    {
        public ReplicationClientSM()
        {
        }

        public override async Task OnAdded()
        {
            App.Get<ReplicationAM>().InstanceDB.onInstanceAdded += OnInstanceAdded;
        }

        public virtual void OnInstanceAdded(object instance)
        {
            //Logger.Log(App.Get<ReplicationAM>().GetAppID(instance));

            if (App.Get<ReplicationAM>().IsOwner(instance)) {
                if (OS.LogReplication)
                {
                    Logger.Log($"Setting app ID for {instance.GetInstanceID()} on replication server.");
                }
                Peer.SetAppID(instance.GetInstanceID());
            }
        }
    }
}
