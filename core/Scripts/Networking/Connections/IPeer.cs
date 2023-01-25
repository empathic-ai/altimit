using System;
using System.Threading.Tasks;
using System.Collections.Generic;

namespace Altimit.Networking
{
    [AType]
    public interface IPeer<T>
    {
        [AProperty]
        T Peer { get; set; }
    }
}
