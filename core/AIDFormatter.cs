using MessagePack;
using MessagePack.Formatters;
using System;

namespace Altimit.Serialization
{
    public sealed class AIDFormatter: IMessagePackFormatter<AID>
    {
        public void Serialize(ref MessagePackWriter writer, AID id, MessagePackSerializerOptions options)
        {
            options.Resolver.GetFormatter<Guid>().Serialize(ref writer, id.Value, options);
        }

        public AID Deserialize(ref MessagePackReader reader, MessagePackSerializerOptions options)
        {
            return new AID(options.Resolver.GetFormatter<Guid>().Deserialize(ref reader, options));
        }
    }
}
