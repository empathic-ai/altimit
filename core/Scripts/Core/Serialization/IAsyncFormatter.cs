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
        Task<byte[]> SerializeAsync(T value, CancellationToken cancellationToken = default);

        Task<T> DeserializeAsync(byte[] bytes, CancellationToken cancellationToken = default);

        Task DeserializeAsync(T value, byte[] bytes, CancellationToken cancellationToken = default);
    }
}
