using System.Threading.Tasks;

#if ALTIMIT_GRPC
using Grpc.Core;

namespace Altimit {
    public class ACAllInvoker : CallInvoker
    {
        public override AsyncClientStreamingCall<TRequest, TResponse> AsyncClientStreamingCall<TRequest, TResponse>(Method<TRequest, TResponse> method, string host, CallOptions options)
        {
            throw new System.NotImplementedException();
        }

        public override AsyncDuplexStreamingCall<TRequest, TResponse> AsyncDuplexStreamingCall<TRequest, TResponse>(Method<TRequest, TResponse> method, string host, CallOptions options)
        {
            throw new System.NotImplementedException();
        }

        public override AsyncServerStreamingCall<TResponse> AsyncServerStreamingCall<TRequest, TResponse>(Method<TRequest, TResponse> method, string host, CallOptions options, TRequest request)
        {
            throw new System.NotImplementedException();
        }

        public override AsyncUnaryCall<TResponse> AsyncUnaryCall<TRequest, TResponse>(Method<TRequest, TResponse> method, string host, CallOptions options, TRequest request)
        {
            return new AsyncUnaryCall<TResponse>(TestCall<TResponse>(), TestMetadata(), null, TrailersFunc, null);
        }

        private async Task<TResponse> TestCall<TResponse>() {
            return default(TResponse);
        }

        private async Task<Metadata> TestMetadata() {
            return default(Metadata);
        }

        private Metadata TrailersFunc() {
            return null;
        }

        public override TResponse BlockingUnaryCall<TRequest, TResponse>(Method<TRequest, TResponse> method, string host, CallOptions options, TRequest request)
        {
            throw new System.NotImplementedException();
        }
    }
}
#endif