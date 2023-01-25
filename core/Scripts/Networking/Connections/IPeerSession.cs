using System;
using System.Threading.Tasks;
using System.Collections.Generic;

namespace Altimit.Networking
{
    public interface IPeerSession : IConnectionSingleton, IDisposable
    {
        /*
        ISelfSession GetSelfSession();
        void SetSelfSession(ISelfSession session);
        */
    }
}
