using Altimit;
using System;
using System.Threading.Tasks;

namespace Altimit.Networking
{
    [AType]
    public interface IReplicationServer : IPeer<IReplicationServer>
    {
        [AMethod]
        Task<AID> GetAppID();
        [AMethod]
        Task<string> GetSessionKey();
        [AMethod(ProtocolType.None)]
        Task<AID> ResolveAppID(AID instanceID);
        [AMethod]
        Task<AID> GetAppIDWithModule(string typeName);
        [AMethod]
        Task<Response<ConnectionSettings>> ConnectToApp(AID appID);
        [AMethod(ProtocolType.None)]
        void SetAppID(AID instanceID);
        [AMethod]
        Task<string[]> GetModuleTypeNames();
    }
}
