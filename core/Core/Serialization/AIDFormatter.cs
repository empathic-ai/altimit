using MessagePack;
using MessagePack.Formatters;
using System;

namespace Altimit.Serialization
{
    /*
    public sealed class GuidFormatter: IMessagePackFormatter<Guid>
    {
        public void Serialize(ref MessagePackWriter writer, Guid id, MessagePackSerializerOptions options)
        {
            options.Resolver.GetFormatter<Guid>().Serialize(ref writer, id.Value, options);
        }

        public Guid Deserialize(ref MessagePackReader reader, MessagePackSerializerOptions options)
        {
            return new Guid(options.Resolver.GetFormatter<Guid>().Deserialize(ref reader, options));
        }
    }
    */
}