#if ALTIMIT_GRPC
using Grpc.Core;

namespace Altimit {
    public class AChannel : Grpc.Core.ChannelBase
    {
        public static AChannel ForAddress(string target) {
            return new AChannel(target);
        }

        public AChannel(string target) : base(target)
        {
        }

        public override CallInvoker CreateCallInvoker()
        {
            throw new System.NotImplementedException();
        }
    }
}
#endif