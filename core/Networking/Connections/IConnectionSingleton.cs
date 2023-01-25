using Altimit.Serialization;
using System;
using System.Threading.Tasks;
using System.Collections.Generic;
using Altimit.Networking;
using Altimit.UI;

namespace Altimit
{
    public interface IConnectionSingleton
    {

    }

    /*
    public interface IConnectionSingleton
    {
        App App { get; set;}
        IConnectionSingleton Peer { get; set; }
        IConnectionSingleton GetService(Type type);
        T GetService<T>() where T : IConnectionSingleton;

    }
    */
}
