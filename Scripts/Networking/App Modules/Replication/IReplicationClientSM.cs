using Altimit;
using System;
using Altimit.Networking;

namespace Altimit.Networking
{
    [AType]
    public interface IReplicationClientSM : ISessionModule<IReplicationServerSM>
    {
    }
}
