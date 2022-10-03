using Altimit;
using System;
using System.Threading.Tasks;

namespace Altimit.Networking
{
    [AType]
    public interface IReplicationServerSM : ISessionModule<IReplicationClientSM>
    {
        [AMethod]
        Task<Guid> GetAppID();
        [AMethod]
        Task<string> GetSessionKey();
        [AMethod(ProtocolType.None)]
        Task<Guid> ResolveAppID(AID instanceID);
        [AMethod]
        Task<Guid> GetAppIDWithModule(string typeName);
        [AMethod]
        Task<Response<ConnectionSettings>> ConnectToApp(Guid appID);
        [AMethod(ProtocolType.None)]
        void SetAppID(AID instanceID);
        [AMethod]
        Task<string[]> GetModuleTypeNames();
    }
}
