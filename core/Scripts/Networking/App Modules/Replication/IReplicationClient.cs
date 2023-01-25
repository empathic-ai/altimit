using Altimit;
using System;
using Altimit.Networking;

namespace Altimit.Networking
{
    [AType]
    public interface IReplicationClient : IPeer<IReplicationServer>
    {
    }
}
