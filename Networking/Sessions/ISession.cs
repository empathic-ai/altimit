using Altimit.Serialization;
using System;
using System.Threading.Tasks;
using System.Collections.Generic;

namespace Altimit.Networking
{
    public interface ISession : IContainer<ISession, ISessionModule>
    {
        App App { get; set;}
        ISession Peer { get; set; }
    }
}
