using MessagePack;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Altimit.Serialization
{
    public interface IAsyncFormatter<T>
    {
        public Task<byte[]> SerializeAsync(T value, CancellationToken cancellationToken = default);

        public Task<T> DeserializeAsync(byte[] bytes, CancellationToken cancellationToken = default);

        public Task DeserializeAsync(T value, byte[] bytes, CancellationToken cancellationToken = default);
    }
}
