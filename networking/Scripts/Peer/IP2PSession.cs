using Altimit;
using System.Threading.Tasks;
using Altimit.Networking;

namespace Meridian
{
    [AType]
    public interface IP2PSession : ISession
    {
        [AMethod(ProtocolType.None)]
        void ProcessVoice(byte[] bytes, int length);
    }
}
