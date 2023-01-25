using Altimit;
using System;
using System.Threading.Tasks;

namespace Altimit.Networking
{
    [AType]
    public interface IServiceRequestService : IConnectionSingleton
    {
        [AMethod]
        Task RequestService(Type serviceType);
    }
}